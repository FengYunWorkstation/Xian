#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	internal class ChangeNotifier<T> where T : class
	{
		public delegate void ValueChangeEventHandler(T oldValue, T newValue);

		public event ValueChangeEventHandler BeforeValueChange;
		public event ValueChangeEventHandler AfterValueChange;

		public ChangeNotifier() {}

		public ChangeNotifier(ValueChangeEventHandler beforeValueChangeEventHandler, ValueChangeEventHandler afterValueChangeEventHandler)
		{
			this.BeforeValueChange += beforeValueChangeEventHandler;
			this.AfterValueChange += afterValueChangeEventHandler;
		}

		private T _value;

		public T Value
		{
			get { return _value; }
			set
			{
				if (_value != value)
				{
					T oldValue = _value;
					T newValue = value;

					if (this.BeforeValueChange != null)
						this.BeforeValueChange(oldValue, newValue);

					_value = value;

					if (this.AfterValueChange != null)
						this.AfterValueChange(oldValue, newValue);
				}
			}
		}
	}
}