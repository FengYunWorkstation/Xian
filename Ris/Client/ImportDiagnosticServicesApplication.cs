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
using System.IO;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class ImportDiagnosticServicesApplication : IApplicationRoot
    {
        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            if (args.Length < 1)
                throw new Exception("Import file path required");

            string fileName = args[0];

            List<string[]> rows = new List<string[]>();
            using (StreamReader reader = File.OpenText(fileName))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] row = line.Split(new string[] { "," }, StringSplitOptions.None);
                    rows.Add(row);
                }
            }

            IDiagnosticServiceAdminService service = ApplicationContext.GetService<IDiagnosticServiceAdminService>();
            service.BatchImport(rows);
        }

        #endregion
    }
}
