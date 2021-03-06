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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    public partial class AddressEditorControl : ApplicationComponentUserControl
    {
        private AddressEditorComponent _component;

        public AddressEditorControl(AddressEditorComponent component)
			:base(component)
        {
            InitializeComponent();
            _component = component;

            _type.DataSource = _component.AddressTypeChoices;
            _type.DataBindings.Add("Value", _component, "AddressType", true, DataSourceUpdateMode.OnPropertyChanged);
            _type.DataBindings.Add("Enabled", _component, "AddressTypeEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _province.DataSource = _component.ProvinceChoices;
            _province.DataBindings.Add("Value", _component, "Province", true, DataSourceUpdateMode.OnPropertyChanged);

            _country.DataSource = _component.CountryChoices;
            _country.DataBindings.Add("Value", _component, "Country", true, DataSourceUpdateMode.OnPropertyChanged);

            _street.DataBindings.Add("Value", _component, "Street", true, DataSourceUpdateMode.OnPropertyChanged);
            _unit.DataBindings.Add("Value", _component, "Unit", true, DataSourceUpdateMode.OnPropertyChanged);
            _city.DataBindings.Add("Value", _component, "City", true, DataSourceUpdateMode.OnPropertyChanged);
            _postalCode.DataBindings.Add("Value", _component, "PostalCode", true, DataSourceUpdateMode.OnPropertyChanged);
            _validFrom.DataBindings.Add("Value", _component, "ValidFrom", true, DataSourceUpdateMode.OnPropertyChanged);
            _validUntil.DataBindings.Add("Value", _component, "ValidUntil", true, DataSourceUpdateMode.OnPropertyChanged);

            _acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
