#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

using System;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Extension point for views onto <see cref="TestComponent"/>
    /// </summary>
    [ExtensionPoint]
	public sealed class TestComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
	/// A test component not intended for production use.
    /// </summary>
    [AssociateView(typeof(TestComponentViewExtensionPoint))]
    public class TestComponent : ApplicationComponent
    {
        private string _name;
        private string _text;

        /// <summary>
        /// Constructor
        /// </summary>
        public TestComponent(string name)
        {
            _name = name;
        }


        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }


        public void ShowMessageBox()
        {
            this.Host.ShowMessageBox("Message from " + _name, MessageBoxActions.Ok);
        }

        public void ShowDialogBox()
        {
            ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, new TestComponent("Dialog from " + _name), "Dialog from " + _name);
        }

		public void ShowWorkspaceDialogBox()
		{
			ApplicationComponent.LaunchAsWorkspaceDialog(this.Host.DesktopWindow, new TestComponent("WorkspaceDialog from " + _name), "WorkspaceDialog from " + _name);
		}

		public void AlertError()
		{
			ShowAlert(AlertLevel.Error);
		}

		public void AlertWarning()
		{
			ShowAlert(AlertLevel.Warning);
		}

		public void AlertInfo()
		{
			ShowAlert(AlertLevel.Info);
		}

        public void SetTitle()
        {
            this.Host.Title = _text;
        }

        public void Modify()
        {
            this.Modified = true;
        }

        public void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        public void Accept()
        {
            this.Exit(ApplicationComponentExitCode.Accepted);
        }

		private void ShowAlert(AlertLevel level)
		{
			switch (level)
			{
				case AlertLevel.Info:
					this.Host.DesktopWindow.ShowAlert(level, "Wherever you go, there you are.",
								  "Go there", window => HandleLink(window, "there you are"), true);
					break;
				case AlertLevel.Warning:
					this.Host.DesktopWindow.ShowAlert(level, "Power corrupts; absolute power corrupts absolutely.");
					break;
				case AlertLevel.Error:
					this.Host.DesktopWindow.ShowAlert(level, "Disco inferno!",
								  "Go to the disco", window => HandleLink(window, "disco"), false);
					break;
				default:
					throw new ArgumentOutOfRangeException("level");
			}
		}

		private void HandleLink(DesktopWindow window, string message)
		{
			window.ShowMessageBox(message, MessageBoxActions.Ok);
		}
    }
}
