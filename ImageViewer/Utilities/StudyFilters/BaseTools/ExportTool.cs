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
using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Export;
using Path=System.IO.Path;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.BaseTools
{
	[DropDownAction("export", DefaultToolbarActionSite + "/ToolbarExport", "DropDownActionModel")]
	[IconSet("export", IconScheme.Colour, "Icons.SaveToolSmall.png", "Icons.SaveToolMedium.png", "Icons.SaveToolLarge.png")]
	[EnabledStateObserver("export", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[MenuAction("exportAnonymized", DropDownMenuActionSite + "/MenuExportAnonymized", "ExportAnonymized")]
	[MenuAction("exportAnonymized", DefaultContextMenuActionSite + "/MenuExportAnonymized", "ExportAnonymized")]
	[VisibleStateObserver("exportAnonymized", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[MenuAction("exportCopy", DropDownMenuActionSite + "/MenuExportCopy", "ExportCopy")]
	[MenuAction("exportCopy", DefaultContextMenuActionSite + "/MenuExportCopy", "ExportCopy")]
	[VisibleStateObserver("exportCopy", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class ExportTool : StudyFilterTool
	{
		public const string DropDownMenuActionSite = "studyfilters-exportdropdown";

		private string _lastExportCopyFolder = string.Empty;
		private string _lastExportAnonymizedFolder = string.Empty;

		public ActionModelNode DropDownActionModel
		{
			get { return ActionModelRoot.CreateModel(this.GetType().FullName, DropDownMenuActionSite, this.Actions); }
		}

		public void ExportAnonymized()
		{
			try
			{
				if (base.SelectedItems.Count > 0)
				{
					ExportComponent component = new ExportComponent();
					component.OutputPath = _lastExportAnonymizedFolder;

					foreach (StudyItem item in base.SelectedItems)
					{
						FileInfo file = item.File;
						if (file.Exists)
						{
							DicomFile dcf = new DicomFile(file.FullName);
							dcf.Load();
							component.Files.Add(dcf);
						}
					}

					if (DialogBoxAction.Ok == base.DesktopWindow.ShowDialogBox(component, SR.Export))
						_lastExportAnonymizedFolder = component.OutputPath;
				}
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, base.DesktopWindow);
			}
		}

		public void ExportCopy()
		{
			try
			{
				if (base.SelectedItems.Count > 0)
				{
					SelectFolderDialogCreationArgs args = new SelectFolderDialogCreationArgs();
					args.Prompt = SR.MessageSelectOutputLocation;
					args.Path = _lastExportCopyFolder;

					FileDialogResult result = base.DesktopWindow.ShowSelectFolderDialogBox(args);
					if (result.Action != DialogBoxAction.Ok)
						return;

					_lastExportCopyFolder = result.FileName;

					string outputDir = result.FileName;
					int count = 0;

					foreach (StudyItem item in base.SelectedItems)
					{
						FileInfo file = item.File;
						if (file.Exists)
						{
							string newpath = Path.Combine(outputDir, file.Name);
							if (!File.Exists(newpath))
							{
								file.CopyTo(newpath);
								count++;
							}
						}
					}

					if (count > 0)
					{
						Process p = Process.Start(outputDir);
					}
				}
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, base.DesktopWindow);
			}
		}
	}
}