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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	/// <summary>
	/// An interactive graphic that adds statefulness to the subject graphic.
	/// </summary>
	[Cloneable]
	public abstract class StatefulCompositeGraphic : ControlGraphic, IStatefulGraphic, IMouseButtonHandler
	{
		private event EventHandler<GraphicStateChangedEventArgs> _stateChangedEvent;

		[CloneIgnore]
		private GraphicState _state;

		[CloneIgnore]
		private IMouseInformation _mouseInformation;

		/// <summary>
		/// Initializes a new instance of <see cref="StatefulCompositeGraphic"/>.
		/// </summary>
		protected StatefulCompositeGraphic() : base(new CompositeGraphic()) {}

		/// <summary>
		/// Initializes a new instance of <see cref="StatefulCompositeGraphic"/>.
		/// </summary>
		protected StatefulCompositeGraphic(IGraphic subject) : base(subject) {}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected StatefulCompositeGraphic(StatefulCompositeGraphic source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets or sets the current <see cref="GraphicState"/>.
		/// </summary>
		public GraphicState State
		{
			get { return _state; }
			set
			{
				Platform.CheckForNullReference(value, "State");

				// If it's the same state, then don't do anything
				if (_state != null)
					if (_state.GetType() == value.GetType())
						return;

				GraphicStateChangedEventArgs args = new GraphicStateChangedEventArgs();

				// Old state *can* be null, i.e., we're assigning state for the first time,
				// so there isn't an old state.
				args.OldState = _state;

				_state = value;

				args.NewState = _state;
				args.MouseInformation = _mouseInformation;
				args.StatefulGraphic = _state.StatefulGraphic;

				// If the old state is null, we're really just initializing the state variable,
				// so don't tell anyone about it
				if (args.OldState != null)
				{
					OnStateChanged(args);
					EventsHelper.Fire(_stateChangedEvent, this, args);
				} 
				else
				{
					OnStateInitialized();
				}
			}
		}

		/// <summary>
		/// Occurs when the value of <see cref="State"/> has changed.
		/// </summary>
		public event EventHandler<GraphicStateChangedEventArgs> StateChanged
		{
			add { _stateChangedEvent += value; }
			remove { _stateChangedEvent -= value; }
		}

		/// <summary>
		/// Called when the value of <see cref="State"/> is initialized for the first time.
		/// </summary>
		protected virtual void OnStateInitialized() {}

		/// <summary>
		/// Called when the value of <see cref="State"/> changes.
		/// </summary>
		/// <param name="e">An object containing data describing the specific state change.</param>
		protected virtual void OnStateChanged(GraphicStateChangedEventArgs e) {}

		#region IMouseButtonHandler Members

		/// <summary>
		/// Called by the framework each time a mouse button is pressed.
		/// </summary>
		/// <remarks>
		/// As a general rule, if the <see cref="IMouseButtonHandler"/> object did anything as a result of this call, it must 
		/// return true.  If false is returned, <see cref="IMouseButtonHandler.Start"/> is called on other <see cref="IMouseButtonHandler"/>s
		/// until one returns true.
		/// </remarks>
		/// <returns>
		/// True if the <see cref="IMouseButtonHandler"/> did something as a result of the call, 
		/// and hence would like to receive capture.  Otherwise, false.
		/// </returns>
		protected override bool Start(IMouseInformation mouseInformation)
		{
			Platform.CheckMemberIsSet(this.State, "State");
			_mouseInformation = mouseInformation;
			this.State.Start(mouseInformation);
			return false;
		}

		/// <summary>
		/// Called by the framework when the mouse has moved.
		/// </summary>
		/// <remarks>
		/// A button does not necessarily have to be down for this message to be called.  The framework can
		/// call it any time the mouse moves.
		/// </remarks>
		/// <returns>True if the message was handled, otherwise false.</returns>
		protected override bool Track(IMouseInformation mouseInformation)
		{
			Platform.CheckMemberIsSet(this.State, "State");
			_mouseInformation = mouseInformation;
			this.State.Track(mouseInformation);
			return false;
		}

		/// <summary>
		/// Called by the framework when the mouse button is released.
		/// </summary>
		/// <returns>
		/// True if the framework should <b>not</b> release capture, otherwise false.
		/// </returns>
		protected override bool Stop(IMouseInformation mouseInformation)
		{
			Platform.CheckMemberIsSet(this.State, "State");
			_mouseInformation = mouseInformation;
			this.State.Stop(mouseInformation);
			return false;
		}

		/// <summary>
		/// Called by the framework to let <see cref="IMouseButtonHandler"/> perform any necessary cleanup 
		/// when capture is going to be forcibly released.
		/// </summary>
		/// <remarks>
		/// It is important that this method is implemented correctly and doesn't simply do nothing when it is inappropriate
		/// to do so, otherwise odd behaviour may be experienced.
		/// </remarks>
		protected override void Cancel()
		{
			this.State.Cancel();
		}

		/// <summary>
		/// Allows the <see cref="IMouseButtonHandler"/> to override certain default framework behaviour.
		/// </summary>
		public override MouseButtonHandlerBehaviour Behaviour
		{
			get { return this.State.Behaviour; }
		}

		#endregion
	}
}