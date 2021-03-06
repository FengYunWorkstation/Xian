#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    /// <summary>
    /// Helper class used in rendering the information encoded in the ChangeDescription column
    /// of a StudyHistory record.
    /// </summary>
    internal class DefaultStudyHistoryRendererFactory : IStudyHistoryColumnControlFactory
    {
        public Control GetChangeDescColumnControl(Control parent, StudyHistory historyRecord)
        {
            Label lb = new Label();
            lb.Text = XmlUtils.GetXmlDocumentAsString(historyRecord.ChangeDescription, true);
            return lb;
        }
    }
}