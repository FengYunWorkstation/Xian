﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    //
    //  Used to display the list of Restore Queue Items.
    //
    public partial class StudyIntegrityQueueGridView : GridViewPanel
    {
        private const string HighlightCssClass = "ConflictField";

        #region Delegates
		public delegate void StudyIntegrityQueueDataSourceCreated(StudyIntegrityQueueDataSource theSource);
		public event StudyIntegrityQueueDataSourceCreated DataSourceCreated;
		#endregion

        #region Private members
        // list of studies to display
        private IList<StudyIntegrityQueueSummary> _queueItems;
        private Unit _height;
    	private StudyIntegrityQueueDataSource _dataSource;
        private StudySummary _study;
        #endregion Private members

        #region Public properties

		public int ResultCount
		{
			get
			{
				if (_dataSource == null)
				{
					_dataSource = new StudyIntegrityQueueDataSource();

                    _dataSource.StudyIntegrityQueueFoundSet += delegate(IList<StudyIntegrityQueueSummary> newlist)
											{
												Items = newlist;
											};
					if (DataSourceCreated != null)
						DataSourceCreated(_dataSource);
					_dataSource.SelectCount();
				}
				if (_dataSource.ResultCount == 0)
				{
					if (DataSourceCreated != null)
						DataSourceCreated(_dataSource);

					_dataSource.SelectCount();
				}
				return _dataSource.ResultCount;
			}
		}

        public StudySummary Study
        {
            get { return _study; }
            set { _study = value; }
        }

        /// <summary>
        /// Retrieve reference to the grid control being used to display the devices.
        /// </summary>
        public Web.Common.WebControls.UI.GridView StudyIntegrityQueueGrid
        {
            get { return StudyIntegrityQueueList; }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public IList<StudyIntegrityQueueSummary> SelectedItems
        {
            get
            {
                if(!StudyIntegrityQueueList.IsDataBound) StudyIntegrityQueueList.DataBind();

                if (Items==null || Items.Count == 0)
                    return null;

                int[] rows = StudyIntegrityQueueList.SelectedIndices;
                if (rows == null || rows.Length == 0)
                    return null;

                IList<StudyIntegrityQueueSummary> queueItems = new List<StudyIntegrityQueueSummary>();
                for(int i=0; i<rows.Length; i++)
                {
                    if (rows[i] < Items.Count)
                    {
                        queueItems.Add(Items[rows[i]]);
                    }
                }

                return queueItems;
            }
        }

        /// <summary>
        /// Gets/Sets the list of devices rendered on the screen.
        /// </summary>
        public IList<StudyIntegrityQueueSummary> Items
        {
            get
            {
                return _queueItems;
            }
            set
            {
                _queueItems = value;
            }
        }

        /// <summary>
        /// Gets/Sets the height of the study list panel
        /// </summary>
        public Unit Height
        {
            get {
                if (ContainerTable != null)
                    return ContainerTable.Height;
                else
                    return _height; 
            }
            set
            {
                _height = value;
                if (ContainerTable != null)
                    ContainerTable.Height = value;
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// Defines the handler for <seealso cref="OnQueueItemSelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedItems"></param>
        public delegate void QueueItemSelectedEventHandler(object sender, IList<StudyIntegrityQueueSummary> selectedItems);

        /// <summary>
        /// Occurs when the selected device in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected device can change programmatically or by users selecting the device in the list.
        /// </remarks>
        public event QueueItemSelectedEventHandler OnQueueItemSelectionChanged;

        #endregion // Events
        
        #region protected methods
       
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TheGrid = StudyIntegrityQueueList;

            // Set up the grid
            if (Height != Unit.Empty)
                ContainerTable.Height = _height;

            // The embeded grid control will show pager control if "allow paging" is set to true
            // We want to use our own pager control instead so let's hide it.
            TheGrid.SelectedIndexChanged += StudyIntegrityQueueGridView_SelectedIndexChanged;

            TheGrid.DataSource = StudyIntegrityQueueDataSourceObject;

            this.DataSourceCreated += delegate(StudyIntegrityQueueDataSource source)
                                        {
                                            source.Partition = Study.ThePartition;
                                            source.PatientName = "*" + Study.PatientsName + "*";
                                            source.PatientId = "*" + Study.PatientId + "*";
                                            source.AccessionNumber = "*" + Study.AccessionNumber + "*";
                                        };
        }

        protected void StudyIntegrityQueueGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            IList<StudyIntegrityQueueSummary> queueItems = SelectedItems;
            if (queueItems != null)
                if (OnQueueItemSelectionChanged != null)
                    OnQueueItemSelectionChanged(this, queueItems);            
        }

        protected void StudyIntegrityQueueGridView_PageIndexChanged(object sender, EventArgs e)
        {
            StudyIntegrityQueueList.DataBind();
        }

        protected void StudyIntegrityQueueGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            StudyIntegrityQueueList.PageIndex = e.NewPageIndex;
            StudyIntegrityQueueList.DataBind();
        }

		protected void GetStudyIntegrityQueueDataSource(object sender, ObjectDataSourceEventArgs e)
		{
			if (_dataSource == null)
			{
                _dataSource = new StudyIntegrityQueueDataSource();

                _dataSource.StudyIntegrityQueueFoundSet += delegate(IList<StudyIntegrityQueueSummary> newlist)
										{
											Items = newlist;
										};
			}

			e.ObjectInstance = _dataSource;

			if (DataSourceCreated != null)
				DataSourceCreated(_dataSource);

		}

        protected void StudyIntegrityQueueItemList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.EmptyDataRow)
            {
                EmptySearchResultsMessage message =
                                        (EmptySearchResultsMessage)e.Row.FindControl("EmptySearchResultsMessage");
                if (message != null)
                {
                    message.Message = "No reconcile items for this study.";
                }

            }
            else
            {
                GridViewRow row = e.Row;

                if (StudyIntegrityQueueList.EditIndex != e.Row.RowIndex)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        StudyIntegrityQueueSummary item = row.DataItem as StudyIntegrityQueueSummary;
                        row.FindControl("ExistingStudyTable").Visible = item.StudyExists;
                        row.FindControl("StudyNotAvailableLabel").Visible = !item.StudyExists;
                        if (item.StudyExists)
                        {
                            CustomizeRowAttribute(e.Row);
                            HighlightDifference(e.Row);
                        }
                    }
                }
            }
        }

        private static void HighlightDifference(GridViewRow row)
        {
            StudyIntegrityQueueSummary item = row.DataItem as StudyIntegrityQueueSummary;
            if (item!=null)
            {
                Label existingAccessionNumber = row.FindControl("ExistingAccessionNumber") as Label;
                Label conflictingAccessionNumber = row.FindControl("ConflictingAccessionNumber") as Label;
                Label existingPatientId = row.FindControl("ExistingPatientId") as Label;
                Label conflictingPatientId = row.FindControl("ConflictingPatientId") as Label;
                Label existingPatientName = row.FindControl("ExistingPatientName") as Label;
                Label conflictingPatientName = row.FindControl("ConflictingPatientName") as Label;

                Highlight(item.ExistingPatientId, item.ConflictingPatientId, existingPatientId, conflictingPatientId);
                Highlight(item.ExistingPatientName, item.ConflictingPatientName, existingPatientName, conflictingPatientName);
                Highlight(item.ExistingAccessionNumber, item.ConflictingAccessionNumber, existingAccessionNumber, conflictingAccessionNumber);
                
            }
        }

        private static void Highlight(String value1, String value2, WebControl existingFieldControl, WebControl conflictingFieldControl)
        {
            if (existingFieldControl != null && conflictingFieldControl != null)
            {
                if (!StringUtils.AreEqual(value1, value2, StringComparison.InvariantCultureIgnoreCase))
                {
                    existingFieldControl.CssClass += " " + HighlightCssClass;
                    conflictingFieldControl.CssClass += " " + HighlightCssClass;
                }
            }
        }

        private static void CustomizeRowAttribute(GridViewRow row)
        {
            StudyIntegrityQueueSummary item = row.DataItem as StudyIntegrityQueueSummary;
            if (item!=null)
            {
                row.Attributes["canreconcile"] = item.CanReconcile ? "true" : "false";    
            }
            
        }

        #endregion
    }

}