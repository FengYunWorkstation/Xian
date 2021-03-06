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
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public class TelephoneNumberTable : Table<TelephoneDetail>
    {
        public TelephoneNumberTable()
        {
            this.Columns.Add(new TableColumn<TelephoneDetail, string>(SR.ColumnType,
                delegate(TelephoneDetail t) { return t.Type.Value; }, 
                1.1f));
            this.Columns.Add(new TableColumn<TelephoneDetail, string>(SR.ColumnNumber,
                delegate(TelephoneDetail pn) { return TelephoneFormat.Format(pn); },
                2.2f));
            this.Columns.Add(new DateTableColumn<TelephoneDetail>(SR.ColumnExpiryDate,
                delegate(TelephoneDetail pn) { return pn.ValidRangeUntil; }, 
                0.9f));
        }
    }
}
