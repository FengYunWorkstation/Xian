#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Abstract base class for application component hosts.
    /// </summary>
    public abstract class ApplicationComponentHost : IApplicationComponentHost
    {
        private readonly IApplicationComponent _component;
        private IApplicationComponentView _componentView;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="component">The component to be hosted.</param>
        protected ApplicationComponentHost(IApplicationComponent component)
        {
            _component = component;
            _component.SetHost(this);
        }

        /// <summary>
        /// Starts the hosted component.
        /// </summary>
        public virtual void StartComponent()
        {
            if (_component.IsStarted)
				throw new InvalidOperationException(SR.ExceptionComponentAlreadyStarted);

            _component.Start();
        }

        /// <summary>
        /// Stops the hosted component.
        /// </summary>
        public virtual void StopComponent()
        {
            if (!_component.IsStarted)
				throw new InvalidOperationException(SR.ExceptionComponentNeverStarted);

            _component.Stop();
        }

        /// <summary>
        /// Gets a value indicating whether the hosted component has been started.
        /// </summary>
        public bool IsStarted
        {
            get { return _component.IsStarted; }
        }

        /// <summary>
        /// Gets the hosted component.
        /// </summary>
        public IApplicationComponent Component
        {
            get { return _component; }
        }

        /// <summary>
        /// Gets the view for the hosted component, creating it if it has not yet been created.
        /// </summary>
        public IApplicationComponentView ComponentView
        {
            get
            {
                if (_componentView == null)
                {
                    _componentView = (IApplicationComponentView)ViewFactory.CreateAssociatedView(_component.GetType());
                    _componentView.SetComponent(_component);
                }
                return _componentView;
            }
        }

        #region IApplicationComponentHost Members

        /// <summary>
        /// Asks the host to exit.
        /// </summary>
        /// <exception cref="NotSupportedException">The host does not support exit requests.</exception>
        public virtual void Exit()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets the associated command history object.
        /// </summary>
        /// <exception cref="NotSupportedException">The host does not support command history.</exception>
        public virtual CommandHistory CommandHistory
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Shows a message box in the associated desktop window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public virtual DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
        {
            return this.DesktopWindow.ShowMessageBox(message, this.Title, buttons);
        }

        /// <summary>
        /// Asks the host to set the title in the user-interface.
        /// </summary>
        /// <exception cref="NotSupportedException">The host does not support titles.</exception>
        public void SetTitle(string title)
        {
            this.Title = title;
        }

        /// <summary>
        /// Gets or sets the title displayed in the user-interface.
        /// </summary>
        /// <exception cref="NotSupportedException">The host does not support titles.</exception>
        public virtual string Title
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the associated desktop window.
        /// </summary>
        public abstract DesktopWindow DesktopWindow { get; }

        #endregion
    }
}
