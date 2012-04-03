#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Configuration.ServerTree;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint]
	public sealed class StudyBrowserToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public sealed class StudyBrowserComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	public interface IStudyBrowserToolContext : IToolContext
	{
		StudyItem SelectedStudy { get; }

		ReadOnlyCollection<StudyItem> SelectedStudies { get; }

		AEServerGroup SelectedServerGroup { get; }

		event EventHandler SelectedStudyChanged;

		event EventHandler SelectedServerChanged;

		ClickHandlerDelegate DefaultActionHandler { get; set; }

		IDesktopWindow DesktopWindow { get; }

		void RefreshStudyTable();
	}

	[AssociateView(typeof(StudyBrowserComponentViewExtensionPoint))]
	public class StudyBrowserComponent : ApplicationComponent, IStudyBrowserComponent
	{
		#region Tool Context

		private class StudyBrowserToolContext : ToolContext, IStudyBrowserToolContext
		{
			private readonly StudyBrowserComponent _component;

			public StudyBrowserToolContext(StudyBrowserComponent component)
			{
				Platform.CheckForNullReference(component, "component");
				_component = component;
			}

			#region IStudyBrowserToolContext Members

			public StudyItem SelectedStudy
			{
				get
				{
					return _component.SelectedStudy;
				}
			}

			public ReadOnlyCollection<StudyItem> SelectedStudies
			{
				get
				{
					return _component.SelectedStudies;
				}
			}

			public AEServerGroup SelectedServerGroup
			{
				get { return _component._selectedServerGroup; }
			}

			public event EventHandler SelectedStudyChanged
			{
				add { _component.SelectedStudyChanged += value; }
				remove { _component.SelectedStudyChanged -= value; }
			}

			public event EventHandler SelectedServerChanged
			{
				add { _component.SelectedServerChanged += value; }
				remove { _component.SelectedServerChanged -= value; }
			}

			public ClickHandlerDelegate DefaultActionHandler
			{
				get { return _component._defaultActionHandler; }
				set { _component._defaultActionHandler = value; }
			}

			public IDesktopWindow DesktopWindow
			{
				get { return _component.Host.DesktopWindow; }
			}

			public void RefreshStudyTable()
			{
				try
				{
					_component.Search(_component._lastQueryParametersList);
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, _component.Host.DesktopWindow);
				}
			}

			#endregion
		}

		#endregion

		#region Fields

		private List<QueryParameters> _lastQueryParametersList;
		private readonly Dictionary<string, SearchResult> _searchResults;
		private readonly Table<StudyItem> _dummyStudyTable;
		private event EventHandler _studyTableChanged;
		private bool _filterDuplicateStudies = true;

		private ISelection _currentSelection;
		private event EventHandler _selectedStudyChangedEvent;

		private AEServerGroup _selectedServerGroup = new AEServerGroup();
		private event EventHandler _selectedServerChangedEvent;

		private ToolSet _toolSet;
		private ClickHandlerDelegate _defaultActionHandler;

		private ActionModelRoot _toolbarModel;
		private ActionModelRoot _contextMenuModel;

		private readonly Dictionary<string, string> _setChangedStudies;

		private SearchResultColumnOptionCollection _searchResultColumnOptions;

		private IWorkItemActivityMonitor _activityMonitor;
        private DelayedEventPublisher _processStudiesEventPublisher;

        private bool _notificationTextVisible;
        private string _notificationText;
        private DelayedEventPublisher _notificationEventPublisher;

		private bool _isEnabled = true;
		private bool _searchInProgress;

	    #endregion

		public StudyBrowserComponent()
		{
			_dummyStudyTable = new Table<StudyItem>();
			_searchResults = new Dictionary<string, SearchResult>();
			_lastQueryParametersList = new List<QueryParameters> { CreateOpenSearchQueryParams() };

			_setChangedStudies = new Dictionary<string, string>();
		}

		#region Properties/Events

		public AEServerGroup SelectedServerGroup
		{
			get { return _selectedServerGroup; }
			set
			{
				_selectedServerGroup = value;

				if (!_searchResults.ContainsKey(_selectedServerGroup.GroupID))
				{
					SearchResult searchResult = CreateSearchResult();
					searchResult.Initialize();
					_searchResults.Add(_selectedServerGroup.GroupID, searchResult);
				}

				if (_searchResultColumnOptions != null) _searchResultColumnOptions.ApplyColumnSettings(CurrentSearchResult);
				ProcessChangedStudies();
				OnSelectedServerChanged();
			}
		}

		internal bool FilterDuplicateStudies
		{
			get { return _filterDuplicateStudies; }
			set
			{
				if (_filterDuplicateStudies != value)
				{
					_filterDuplicateStudies = value;
					if (CurrentSearchResult != null)
					{
						CurrentSearchResult.FilterDuplicates = _filterDuplicateStudies;
						UpdateResultsTitle();
					}
				}
			}
		}

		internal SearchResult CurrentSearchResult
		{
			get
			{
				if (_selectedServerGroup == null || _selectedServerGroup.GroupID == null || !_searchResults.ContainsKey(_selectedServerGroup.GroupID))
					return null;

				return _searchResults[_selectedServerGroup.GroupID];
			}
		}

		private event EventHandler SelectedStudyChanged
		{
			add { _selectedStudyChangedEvent += value; }
			remove { _selectedStudyChangedEvent -= value; }
		}

		private event EventHandler SelectedServerChanged
		{
			add { _selectedServerChangedEvent += value; }
			remove { _selectedServerChangedEvent -= value; }
		}

		#endregion

		#region Presentation Model

		public bool IsEnabled
		{
			get { return _isEnabled; }
			private set
			{
				if(value != _isEnabled)
				{
					_isEnabled = value;
					NotifyPropertyChanged("IsEnabled");
				}
			}
		}

		public Table<StudyItem> StudyTable
		{
			get
			{
				if (CurrentSearchResult == null)
					return _dummyStudyTable;
				else
					return CurrentSearchResult.StudyTable;
			}
		}

		public string ResultsTitle
		{
			get
			{
				if (CurrentSearchResult == null)
					return "";
				else
					return CurrentSearchResult.ResultsTitle;
			}
		}

        public string NotificationText
        {
            get { return _notificationTextVisible ? _notificationText : String.Empty; }
            private set
            {
                if (Equals(_notificationText, value))
                    return;

                _notificationText = value;
                NotifyPropertyChanged("NotificationText");
            }
        }

		public event EventHandler StudyTableChanged
		{
			add { _studyTableChanged += value; }
			remove { _studyTableChanged -= value; }
		}

		public StudyItem SelectedStudy
		{
			get
			{
				if (_currentSelection == null)
					return null;
				else
					return _currentSelection.Item as StudyItem;
			}
		}

		public ReadOnlyCollection<StudyItem> SelectedStudies
		{
			get
			{
				List<StudyItem> selectedStudies = new List<StudyItem>();

				if (_currentSelection != null)
				{
					foreach (StudyItem item in _currentSelection.Items)
						selectedStudies.Add(item);
				}

				return selectedStudies.AsReadOnly();
			}
		}

		public ActionModelRoot ToolbarModel
		{
			get { return _toolbarModel; }
		}

		public ActionModelNode ContextMenuModel
		{
			get { return _contextMenuModel; }
		}

		public void SetSelection(ISelection selection)
		{
			if (_currentSelection != selection)
			{
				_currentSelection = selection;
				EventsHelper.Fire(_selectedStudyChangedEvent, this, EventArgs.Empty);
			}
		}


		public void ItemDoubleClick()
		{
			if (_defaultActionHandler != null)
				_defaultActionHandler();
		}

		#endregion

		#region IStudyBrowserComponent implementation

		public virtual SearchResult CreateSearchResult()
		{
			return new SearchResult();
		}

		public virtual QueryParameters CreateOpenSearchQueryParams()
		{
			var queryParams = new QueryParameters
			                  	{
			                  		{"PatientsName", ""},
			                  		{"ReferringPhysiciansName", ""},
			                  		{"PatientId", ""},
			                  		{"AccessionNumber", ""},
			                  		{"StudyDescription", ""},
			                  		{"ModalitiesInStudy", ""},
			                  		{"StudyDate", ""},
			                  		{"StudyInstanceUid", ""},
			                  	};

			return queryParams;
		}


		public virtual void Search(List<QueryParameters> queryParametersList)
		{
			// cancel any pending searches
			Async.CancelPending(this);

			if (_selectedServerGroup != null && _selectedServerGroup.IsLocalDatastore)
				_setChangedStudies.Clear();

			var isOpenSearchQuery = CollectionUtils.TrueForAll(queryParametersList,
				q => CollectionUtils.TrueForAll(q.Values,
					v => string.IsNullOrEmpty(v)));

			if (!_selectedServerGroup.IsLocalDatastore && isOpenSearchQuery)
			{
				if (Host.DesktopWindow.ShowMessageBox(SR.MessageConfirmContinueOpenSearch, MessageBoxActions.YesNo) == DialogBoxAction.No)
					return;
			}

			// disable the study browser while the search is executing
			this.IsEnabled = false;
			_searchInProgress = true;

			EventsHelper.Fire(this.SearchStarted, this, EventArgs.Empty);

			_lastQueryParametersList = new List<QueryParameters>(queryParametersList);
			var failedServerInfo = new List<KeyValuePair<string, Exception>>();
			var aggregateStudyItemList = new StudyItemList();

			Async.Invoke(this,
						 () => aggregateStudyItemList = Query(queryParametersList, failedServerInfo),
						 () => OnSearchCompleted(aggregateStudyItemList, failedServerInfo));
		}

		public virtual void CancelSearch()
		{
			if(!_searchInProgress)
				return;

			Async.CancelPending(this);
			_searchInProgress = false;

			// re-enable the study browser
			this.IsEnabled = true;

			EventsHelper.Fire(this.SearchEnded, this, EventArgs.Empty);
		}

		public event EventHandler SearchStarted;
		public event EventHandler SearchEnded;

		#endregion

		#region IApplicationComponent overrides

		public override void Start()
		{
			base.Start();

			_processStudiesEventPublisher = new DelayedEventPublisher(DelayProcessReceivedAndRemoved);
		    const int tenSeconds = 10000;
            _notificationEventPublisher = new DelayedEventPublisher(DelayedClearNotificationText, tenSeconds);
			ArrayList tools = new ArrayList(new StudyBrowserToolExtensionPoint().CreateExtensions());
			tools.Add(new FilterDuplicateStudiesTool(this));
			_toolSet = new ToolSet(tools, new StudyBrowserToolContext(this));

			_toolbarModel = ActionModelRoot.CreateModel(GetType().FullName, "dicomstudybrowser-toolbar", _toolSet.Actions);
			_contextMenuModel = ActionModelRoot.CreateModel(GetType().FullName, "dicomstudybrowser-contextmenu", _toolSet.Actions);

		    _activityMonitor = WorkItemActivityMonitor.Create();
            _activityMonitor.WorkItemChanged += OnWorkItemChanged;

			_searchResultColumnOptions = SearchResult.ColumnOptions;

			DicomExplorerConfigurationSettings.Default.PropertyChanged += OnConfigurationSettingsChanged;
		}

	    public override void Stop()
		{
			Async.CancelPending(this);

			_processStudiesEventPublisher.Dispose();
            _notificationEventPublisher.Dispose();

			_toolSet.Dispose();
			_toolSet = null;

            _activityMonitor.WorkItemChanged -= OnWorkItemChanged;
            _activityMonitor.Dispose();

			DicomExplorerConfigurationSettings.Default.PropertyChanged -= OnConfigurationSettingsChanged;

			base.Stop();
		}

		#endregion

		#region Private Helpers

		private void OnSelectedServerChanged()
		{
			CurrentSearchResult.ServerGroupName = _selectedServerGroup.Name;
			CurrentSearchResult.IsLocalDataStore = _selectedServerGroup.IsLocalDatastore;
			CurrentSearchResult.NumberOfChildServers = _selectedServerGroup.Servers.Count;

			CurrentSearchResult.UpdateColumnVisibility();
			CurrentSearchResult.FilterDuplicates = _filterDuplicateStudies;

			EventsHelper.Fire(_selectedServerChangedEvent, this, EventArgs.Empty);
			EventsHelper.Fire(_studyTableChanged, this, EventArgs.Empty);
			UpdateResultsTitle();

            if (SelectedServerGroup.IsLocalDatastore)
            {
                _notificationTextVisible = true;
                NotifyPropertyChanged("NotificationText");
            }
            else
            {
                _notificationTextVisible = false;
                NotifyPropertyChanged("NotificationText");
            }
		}

		private void UpdateResultsTitle()
		{
			NotifyPropertyChanged("ResultsTitle");
		}

		private StudyItemList Query(List<QueryParameters> queryParamsList, ICollection<KeyValuePair<string, Exception>> failedServerInfo)
		{
			StudyItemList aggregateStudyItemList = new StudyItemList();

			foreach (IServerTreeNode serverNode in _selectedServerGroup.Servers)
			{
				var serverStudyItemList = new StudyItemList();
				var serverHasError = false;

				try
				{
					foreach (var q in queryParamsList)
					{
						// Make sure the query parameters sent contains all user specified parameters, plus any keys defined in 
						// OpenSearchQueryParams but not in user specified parameters.
						var queryParams = MergeQueryParams(q, this.CreateOpenSearchQueryParams());

						if (serverNode.IsLocalDataStore)
						{
							var studyItemList = ImageViewerComponent.FindStudy(queryParams, null, "DICOM_LOCAL");
							serverStudyItemList.AddRange(studyItemList);
						}
						else if (serverNode.IsServer)
						{
                            var server = (IServerTreeDicomServer)serverNode;
                            IApplicationEntity ae = server.ToDataContract();

							var studyItemList = ImageViewerComponent.FindStudy(queryParams, ae, "DICOM_REMOTE");
							serverStudyItemList.AddRange(studyItemList);
						}
						else
						{
							throw new Exception("The specified server object is not queryable.");
						}
					}
				}
				catch (Exception e)
				{
					// keep track of the failed server names and exceptions
					failedServerInfo.Add(new KeyValuePair<string, Exception>(serverNode.Name, e));
					serverHasError = true;
				}

				if (!serverHasError)
					aggregateStudyItemList.AddRange(serverStudyItemList);
			}

			return aggregateStudyItemList;
		}

		private static QueryParameters MergeQueryParams(QueryParameters primary, QueryParameters secondary)
		{
			// Merge the primary with secondary query parameters.  If the key exist in both, keep the value in the primary
			// Otherwise, add the value in the secondary to the merged query parameters.
			var merged = new QueryParameters(primary);
			foreach (var k in secondary.Keys)
			{
				if (!merged.ContainsKey(k))
					merged.Add(k, secondary[k]);
			}

			return merged;
		}

		private bool StudyExists(string studyInstanceUid)
		{
			return GetStudyIndex(studyInstanceUid) >= 0;
		}

		private int GetStudyIndex(string studyInstanceUid)
		{
			return StudyTable.Items.FindIndex(
				delegate(StudyItem test)
				{
					return test.StudyInstanceUid == studyInstanceUid;
				});
		}

		private void ProcessChangedStudies()
		{
			if (_selectedServerGroup == null || !_selectedServerGroup.IsLocalDatastore || _setChangedStudies.Count == 0)
                return;

			Table<StudyItem> studyTable = StudyTable;

		    var changed = _setChangedStudies.Keys.ToList();
		    var confirmedDeleted = new List<string>();

            string studyUids = DicomStringHelper.GetDicomStringArray(changed);
			if (String.IsNullOrEmpty(studyUids))
                return;

			var queryParams = new QueryParameters(this.CreateOpenSearchQueryParams());
			queryParams["StudyInstanceUid"] = studyUids;

			try
			{
			    //TODO (Marmot): use service node?
				StudyItemList matchingStudies = ImageViewerComponent.FindStudy(queryParams, null, "DICOM_LOCAL");
				foreach (StudyItem item in matchingStudies)
				{
                    //What's left over in this list may have been deleted.
                    changed.Remove(item.StudyInstanceUid);
                    
                    //don't need to check this again, it's just paranoia
					if (!StudyExists(item.StudyInstanceUid))
					{
						studyTable.Items.Add(item);
					}
					else
					{
						int index = GetStudyIndex(item.StudyInstanceUid);
						//just update this since the rest won't change.
						UpdateItem(studyTable.Items[index], item);
						studyTable.Items.NotifyItemUpdated(index);
					}
				}

                string possiblyDeletedUids = DicomStringHelper.GetDicomStringArray(changed);
                if (String.IsNullOrEmpty(possiblyDeletedUids))
                    return;

                queryParams = new QueryParameters();
                queryParams["StudyInstanceUid"] = possiblyDeletedUids;
			    StudyItemList notDeletedStudies = ImageViewerComponent.FindStudy(queryParams, null, "DICOM_LOCAL");
                if (notDeletedStudies != null && notDeletedStudies.Count > 0)
                {
                    var notDeletedUids = notDeletedStudies.Select(s => s.StudyInstanceUid);
                    confirmedDeleted = changed.Where(uid => !notDeletedUids.Contains(uid)).ToList();
                }
                else
                {
                    //They've all been deleted.
                    confirmedDeleted = changed;
                }

			}
			catch (StudyFinderNotFoundException e)
			{
				//should never get here, really.
				Platform.Log(LogLevel.Error, e);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			foreach (string deletedUid in confirmedDeleted)
			{
                var uid = deletedUid;
				int foundIndex = studyTable.Items.FindIndex(test => test.StudyInstanceUid == uid);
				if (foundIndex >= 0)
					studyTable.Items.RemoveAt(foundIndex);
			}

			_setChangedStudies.Clear();
		}

		private void DelayProcessReceivedAndRemoved(object sender, EventArgs e)
		{
			ProcessChangedStudies();
			UpdateResultsTitle();
		}

		private void OnSearchCompleted(StudyItemList aggregateStudyItemList, List<KeyValuePair<string, Exception>> failedServerInfo)
		{
			CurrentSearchResult.Refresh(aggregateStudyItemList, _filterDuplicateStudies);

			// Re-throw the last exception with a list of failed server name, if any
			if (failedServerInfo.Count > 0)
			{
				var aggregateExceptionMessage = new StringBuilder();
				var count = 0;
				foreach (var pair in failedServerInfo)
				{
					if (count++ > 0)
						aggregateExceptionMessage.Append("\n\n");

					aggregateExceptionMessage.AppendFormat(SR.FormatUnableToQueryServer, pair.Key, pair.Value.Message);
				}

				// this isn't ideal, but since we can operate on multiple entities, we need to aggregate all the
				// exception messages. We should at least attempt to get at the first inner exception, and that's
				// what we do here, to aid in debugging

				//NOTE: must use Application.ActiveDesktopWindow instead of Host.DesktopWindow b/c this
				//method is called on startup before the component is started.
				Application.ActiveDesktopWindow.ShowMessageBox(aggregateExceptionMessage.ToString(), MessageBoxActions.Ok);
			}

			UpdateResultsTitle();

			_searchInProgress = false;

			// re-enable the study browser
			this.IsEnabled = true;

			EventsHelper.Fire(this.SearchEnded, this, EventArgs.Empty);
		}

        private void OnWorkItemChanged(object sender, WorkItemChangedEventArgs args)
        {
            if (args.ItemData == null)
                return;

            switch (args.ItemData.Type)
            {
                case WorkItemTypeEnum.ReIndex:
                    UpdateNotificationText(args.ItemData);
                    break;
                case WorkItemTypeEnum.DicomSend:
                case WorkItemTypeEnum.ReapplyRules:
                    return;
                default:
                    if (!String.IsNullOrEmpty(args.ItemData.StudyInstanceUid))
                        _setChangedStudies[args.ItemData.StudyInstanceUid] = args.ItemData.StudyInstanceUid;
                    break;
            }

            _processStudiesEventPublisher.Publish(this, EventArgs.Empty);
        }

        private void UpdateNotificationText(WorkItemData workItemData)
        {
            switch (workItemData.Status)
            {
                case WorkItemStatusEnum.Pending:
                    _notificationEventPublisher.Cancel();
                    NotificationText = SR.NotificationReindexScheduled;
                    break;
                case WorkItemStatusEnum.InProgress:
                    NotificationText = SR.NotificationReindexRunning;
                    break;
                case WorkItemStatusEnum.Failed:
                    NotificationText = SR.NotificationReindexFailed;
                    _notificationEventPublisher.Publish(this, EventArgs.Empty);
                    break;
                case WorkItemStatusEnum.Idle:
                    //NotificationText = SR.NotificationReindexComplete;
                    //_notificationEventPublisher.Publish(this, EventArgs.Empty);
                    break;
                case WorkItemStatusEnum.Complete:
                    NotificationText = SR.NotificationReindexComplete;
                    _notificationEventPublisher.Publish(this, EventArgs.Empty);
                    break;
                case WorkItemStatusEnum.Canceled:
                    NotificationText = SR.NotificationReindexCancelled;
                    _notificationEventPublisher.Publish(this, EventArgs.Empty);
                    break;
                default:
                    NotificationText = String.Empty;
                    break;
            }
        }

        private void DelayedClearNotificationText(object sender, EventArgs e)
        {
            NotificationText = String.Empty;
        }
        
        private void OnConfigurationSettingsChanged(object sender, PropertyChangedEventArgs e)
		{
			_searchResultColumnOptions = SearchResult.ColumnOptions;
			_searchResultColumnOptions.ApplyColumnSettings(CurrentSearchResult);

			if (CurrentSearchResult != null)
				CurrentSearchResult.UpdateColumnVisibility();
		}

		private static void UpdateItem(StudyItem existingItem, StudyItem sourceItem)
		{
			//TODO: later, make each item have a 'changed' event for the properties instead of doing this
			existingItem.AccessionNumber = sourceItem.AccessionNumber;
			existingItem.ReferringPhysiciansName = sourceItem.ReferringPhysiciansName;
			existingItem.ModalitiesInStudy = sourceItem.ModalitiesInStudy;
			existingItem.NumberOfStudyRelatedInstances = sourceItem.NumberOfStudyRelatedInstances;
			existingItem.PatientId = sourceItem.PatientId;
			existingItem.PatientsName = sourceItem.PatientsName;
			existingItem.PatientsBirthDate = sourceItem.PatientsBirthDate;
			existingItem.SpecificCharacterSet = sourceItem.SpecificCharacterSet;
			existingItem.StudyDate = sourceItem.StudyDate;
			existingItem.StudyDescription = sourceItem.StudyDescription;

			existingItem.PatientSpeciesDescription = sourceItem.PatientSpeciesDescription;
			existingItem.PatientSpeciesCodeSequenceCodingSchemeDesignator = sourceItem.PatientSpeciesCodeSequenceCodingSchemeDesignator;
			existingItem.PatientSpeciesCodeSequenceCodeValue = sourceItem.PatientSpeciesCodeSequenceCodeValue;
			existingItem.PatientSpeciesCodeSequenceCodeMeaning = sourceItem.PatientSpeciesCodeSequenceCodeMeaning;
			existingItem.PatientBreedDescription = sourceItem.PatientBreedDescription;
			existingItem.PatientBreedCodeSequenceCodingSchemeDesignator = sourceItem.PatientBreedCodeSequenceCodingSchemeDesignator;
			existingItem.PatientBreedCodeSequenceCodeValue = sourceItem.PatientBreedCodeSequenceCodeValue;
			existingItem.PatientBreedCodeSequenceCodeMeaning = sourceItem.PatientBreedCodeSequenceCodeMeaning;
			existingItem.ResponsibleOrganization = sourceItem.ResponsibleOrganization;
			existingItem.ResponsiblePersonRole = sourceItem.ResponsiblePersonRole;
			existingItem.ResponsiblePerson = sourceItem.ResponsiblePerson;
		}

		#endregion
	}
}
