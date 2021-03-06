#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    public class StorageFolder<TItem> : Folder
    {
        private string _folderName;

        private Table<TItem> _itemsTable;

        public StorageFolder(string folderName, Table<TItem> itemsTable)
        {
            _folderName = folderName;
            _itemsTable = itemsTable;

            _itemsTable.Items.ItemsChanged += delegate
                {
                	this.TotalItemCount = _itemsTable.Items.Count;
                };
        }

        protected ItemCollection<TItem> Items
        {
            get { return _itemsTable.Items; }
        }

		protected override bool IsItemCountKnown
		{
			get { return true; }
		}

		protected override bool UpdateCore()
		{
			// do nothing
			return false;
		}

		protected override void InvalidateCore()
		{
			// do nothing
		}

		public override ITable ItemsTable
        {
            get { return _itemsTable; }
        }

        public override DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
        {
            // return the requested kind if all items are of type TItem, otherwise None
            return CollectionUtils.TrueForAll(items, delegate(object obj) { return obj is TItem; }) ? kind : DragDropKind.None;
        }

        public override DragDropKind AcceptDrop(object[] items, DragDropKind kind)
        {
            if (kind != DragDropKind.None)
            {
                // store any items that are not already in this folder
                foreach (TItem item in items)
                {
                    if (!CollectionUtils.Contains<TItem>(this.Items, delegate(TItem x) { return x.Equals(item); }))
                    {
                        this.Items.Add(item);
                    }
                }
            }

            return kind;
        }

        public override void DragComplete(object[] items, DragDropKind kind)
        {
            // if the operation was a Move, then we should remove the items from this folder
            if (kind == DragDropKind.Move)
            {
                foreach (TItem item in items)
                {
                    this.Items.Remove(item);
                }
            }
        }
    }
}
