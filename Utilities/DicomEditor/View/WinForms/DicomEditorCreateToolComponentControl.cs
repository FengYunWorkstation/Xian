#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Utilities.DicomEditor.Tools;

namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomEditorCreateToolComponent"/>
    /// </summary>
    public partial class DicomEditorCreateToolComponentControl : CustomUserControl
    {
        private DicomEditorCreateToolComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomEditorCreateToolComponentControl(DicomEditorCreateToolComponent component)
        {
            InitializeComponent();

            _component = component;

            _group.DataBindings.Add("Text", _component, "Group", true, DataSourceUpdateMode.OnPropertyChanged);
            _element.DataBindings.Add("Text", _component, "Element", true, DataSourceUpdateMode.OnPropertyChanged);
            _tagName.DataBindings.Add("Value", _component, "TagName", true, DataSourceUpdateMode.OnPropertyChanged);
            _vr.DataBindings.Add("Value", _component, "Vr", true, DataSourceUpdateMode.OnPropertyChanged);
            _vr.DataBindings.Add("Enabled", _component, "VrEnabled", true, DataSourceUpdateMode.Never);
            _value.DataBindings.Add("Value", _component, "Value", true, DataSourceUpdateMode.OnPropertyChanged);
            _accept.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.Never);

			base.AcceptButton = _accept;
			base.CancelButton = _cancel;
        }

        private void _accept_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancel_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
