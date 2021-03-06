#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ValidationManagementComponent"/>
    /// </summary>
    public partial class ValidationManagementComponentControl : ApplicationComponentUserControl
    {
        private ValidationManagementComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationManagementComponentControl(ValidationManagementComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _appComponentTableView.Table = _component.ApplicationComponents;
            _appComponentTableView.DataBindings.Add("Selection", _component, "SelectedComponent", true, DataSourceUpdateMode.OnPropertyChanged);

        }

        private void _appComponentTableView_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.ApplicationComponentDoubleClicked();
        }
    }
}
