#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Trees;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="FolderExplorerComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class FolderExplorerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// WorklistExplorerComponent class
    /// </summary>
    [AssociateView(typeof(FolderExplorerComponentViewExtensionPoint))]
    public class FolderExplorerComponent : ApplicationComponent
    {
		private readonly FolderTreeRoot _folderTreeRoot;
		private FolderTreeNode _selectedTreeNode;
        private event EventHandler _selectedFolderChanged;

        private readonly IFolderSystem _folderSystem;
    	private Timer _folderInvalidateTimer;

        /// <summary>
        /// Constructor
        /// </summary>
        public FolderExplorerComponent(IFolderSystem folderSystem)
        {
			_folderTreeRoot = new FolderTreeRoot(this);
            _folderSystem = folderSystem;
        }

		/// <summary>
		/// Gets or sets the currently selected folder.
		/// </summary>
    	public IFolder SelectedFolder
    	{
			get { return _selectedTreeNode == null ? null : _selectedTreeNode.Folder; }
			set
			{
				this.SelectedFolderTreeNode = new Selection(_folderTreeRoot.FindNode(value));
			}
    	}

		/// <summary>
		/// Invalidates all folders.
		/// </summary>
		internal void InvalidateFolders()
		{
			// invalidate all folders, and update starting at the root
			_folderSystem.InvalidateFolders();
		}

		internal void ExecuteSearch(SearchParams searchParams)
		{
			if (_folderSystem.SearchEnabled)
				_folderSystem.ExecuteSearch(searchParams);
		}

		#region Application Component overrides

        public override void Start()
        {
			// initialize the folder system on a background task
			// in case it takes a long time
			BackgroundTask task = new BackgroundTask(
				delegate
				{
					_folderSystem.Initialize();
				}, false);
        	task.Terminated += 
				delegate(object sender, BackgroundTaskTerminatedEventArgs args)
				{
					if (args.Reason == BackgroundTaskTerminatedReason.Exception)
					{
						Platform.Log(LogLevel.Error, args.Exception);
						return;
					}

					// subscribe to events
					_folderSystem.Folders.ItemAdded += FolderAddedEventHandler;
					_folderSystem.Folders.ItemRemoved += FolderRemovedEventHandler;
					_folderSystem.FoldersChanged += FoldersChangedEventHandler;
					_folderSystem.FoldersInvalidated += FoldersInvalidatedEventHandler;

					// build the initial folder tree
					BuildFolderTree();

					// invalidate all folders and update the entire tree
					InvalidateFolders();

					// this timer is responsible for monitoring the auto-invalidation of all folders
					// in the folder system, and performing the appropriate invalidations
					_folderInvalidateTimer = new Timer(delegate { AutoInvalidateFolders(); });
					_folderInvalidateTimer.IntervalMilliseconds = 1000; // resolution of 1 second
					_folderInvalidateTimer.Start();
				};

			task.Run();

			base.Start();
		}

		public override void Stop()
		{
			_folderInvalidateTimer.Stop();
			_folderInvalidateTimer.Dispose();

			base.Stop();
		}

        public override IActionSet ExportedActions
        {
            get 
            { 
                return _folderSystem.FolderTools == null
                    ? new ActionSet()
                    : _folderSystem.FolderTools.Actions; 
            }
        }

        #endregion

        #region Presentation Model

        public ITree FolderTree
        {
			get { return _folderTreeRoot.GetSubTree(); }
        }

        public ISelection SelectedFolderTreeNode
        {
            get { return new Selection(_selectedTreeNode); }
            set
            {
				FolderTreeNode nodeToSelect = (FolderTreeNode)value.Item;
                SelectFolder(nodeToSelect);
            }
        }

        public ITable FolderContentsTable
        {
            get { return _selectedTreeNode == null ? null : _selectedTreeNode.Folder.ItemsTable; }
        }

        public event EventHandler SelectedFolderChanged
        {
            add { _selectedFolderChanged += value; }
            remove { _selectedFolderChanged -= value; }
        }

        public ActionModelRoot FoldersContextMenuModel
        {
            get
            {
                return ActionModelRoot.CreateModel(this.GetType().FullName, "folderexplorer-folders-contextmenu", _folderSystem.FolderTools.Actions);
            }
        }

        public ActionModelNode FoldersToolbarModel
        {
            get
            {
				return ActionModelRoot.CreateModel(this.GetType().FullName, "folderexplorer-folders-toolbar", _folderSystem.FolderTools.Actions);
            }
        }

        public IFolderSystem FolderSystem
        {
            get { return _folderSystem; }
        }

        #endregion

        #region Private methods

		private void AutoInvalidateFolders()
		{
			int count = 0;
			foreach (IFolder folder in _folderSystem.Folders)
			{
				if(folder.AutoInvalidateInterval > TimeSpan.Zero
					&& (Platform.Time - folder.LastUpdateTime) > folder.AutoInvalidateInterval)
				{
					_folderSystem.InvalidateFolder(folder);
					count++;
				}
			}

			if(count > 0)
			{
				// update folder tree in case any folders were invalidated
				// this is done regardless of whether this folder explorer is currently visible, because
				// we need to keep the title bars of the folder explorers updated
				_folderTreeRoot.Update();
			}
		}

    	private void SelectFolder(FolderTreeNode node)
        {
            if (_selectedTreeNode != node)
            {
                if (_selectedTreeNode != null)
                {
					_selectedTreeNode.Folder.CloseFolder();
                }

				if (node != null)
                {
					node.Folder.OpenFolder();
					
					// ensure the content of this nodes folder is up to date
					node.Update();
                }

				_selectedTreeNode = node;
				EventsHelper.Fire(_selectedFolderChanged, this, EventArgs.Empty);
			}
		}

        internal DragDropKind CanFolderAcceptDrop(FolderTreeNode treeNode, object dropData, DragDropKind kind)
        {
            if (treeNode.Folder != _selectedTreeNode && dropData is ISelection)
            {
                return treeNode.Folder.CanAcceptDrop((dropData as ISelection).Items, kind);
            }
            return DragDropKind.None;
        }

		internal DragDropKind FolderAcceptDrop(FolderTreeNode treeNode, object dropData, DragDropKind kind)
        {
			if (treeNode.Folder != _selectedTreeNode && dropData is ISelection)
            {
                // inform the target folder to accept the drop
				DragDropKind result = treeNode.Folder.AcceptDrop((dropData as ISelection).Items, kind);

                // inform the source folder that a drag was completed
                _selectedTreeNode.Folder.DragComplete((dropData as ISelection).Items, result);
            }
            return DragDropKind.None;
        }

		private void FolderAddedEventHandler(object sender, ListEventArgs<IFolder> e)
		{
			// folder was added to the folder system, so add it to the tree
			_folderTreeRoot.InsertFolder(e.Item, false);
		}

		private void FolderRemovedEventHandler(object sender, ListEventArgs<IFolder> e)
		{
			// bug: noticed that if the folder being removed or one of its parents is currently selected,
			// the UI may exhibit strange behaviour
			// to be safe, just remove the current selection
			this.SelectedFolder = null;

			// folder was removed from the folder system, so remove it from the tree
			_folderTreeRoot.RemoveFolder(e.Item);
		}

		private void FoldersChangedEventHandler(object sender, EventArgs e)
		{
			BuildFolderTree();
		}

		private void FoldersInvalidatedEventHandler(object sender, EventArgs e)
		{
			//TODO: only do update if this explorer is active
			_folderTreeRoot.Update();
		}

		private void BuildFolderTree()
		{
			// clear existing
			_folderTreeRoot.GetSubTree().Items.Clear();

			// put folders in correct insertion order from XML
			List<IFolder> orderedFolders;
			List<IFolder> remainderFolders;
			FolderExplorerComponentSettings.Default.OrderFolders(_folderSystem, out orderedFolders, out remainderFolders);

			_folderTreeRoot.InsertFolders(orderedFolders, false);	// insert the ordered folders as ordered
			_folderTreeRoot.InsertFolders(remainderFolders, true);	// insert the remainder sorting alphabetically
		}

		#endregion

	}
}
