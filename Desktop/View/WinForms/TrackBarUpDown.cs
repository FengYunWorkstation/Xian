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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
	public partial class TrackBarUpDown : UserControl
	{
		private bool _trackBarValueChanging;
		private bool _upDownValueChanging;

		private event EventHandler _valueChangedEvent;
		private event EventHandler _minimumChangedEvent;
		private event EventHandler _maximumChangedEvent;

		private DelayedEventDispatcher _valueChangedDispatcher;

		public TrackBarUpDown()
		{
			InitializeComponent();

			_valueChangedDispatcher = new DelayedEventDispatcher(new EventHandler(NotifyValueChanged), 500, true);
			_trackBar.ValueChanged += new EventHandler(OnTrackBarValueChanged);
			_numericUpDown.ValueChanged += new EventHandler(OnNumericUpDownValueChanged);
		}

		public int TrackBarIncrements
		{
			get { return _trackBar.Maximum; }
			set 
			{
				_trackBar.Maximum = value;
				_trackBar.TickFrequency = 10;
			}
		}

		public decimal Minimum
		{
			get { return _numericUpDown.Minimum; }
			set 
			{
				if (value > this.Maximum)
					throw new ArgumentException(SR.ExceptionMinimumTooLarge);

				if (_numericUpDown.Minimum != value)
				{
					_numericUpDown.Minimum = value;
					EventsHelper.Fire(_minimumChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public decimal Maximum
		{
			get { return _numericUpDown.Maximum; }
			set
			{
				if (value < this.Minimum)
					throw new ArgumentException(SR.ExceptionMaximumTooSmall);

				if (_numericUpDown.Maximum != value)
				{
					_numericUpDown.Maximum = value;
					EventsHelper.Fire(_maximumChangedEvent, this, EventArgs.Empty);
				}
			}
		}

		public int DecimalPlaces
		{
			get { return _numericUpDown.DecimalPlaces; }
			set 
			{ 
				_numericUpDown.DecimalPlaces = value;
				_numericUpDown.Increment = (decimal) Math.Pow(10, -value);
			}
		}

		public decimal Value
		{
			get { return _numericUpDown.Value; }
			set 
			{
				if (_numericUpDown.Value != value)
					_numericUpDown.Value = value;
			}
		}

		public event EventHandler ValueChanged
		{
			add { _valueChangedEvent += value; }
			remove { _valueChangedEvent -= value; }
		}

		public event EventHandler MinimumChanged
		{
			add { _minimumChangedEvent += value; }
			remove { _minimumChangedEvent -= value; }
		}

		public event EventHandler MaximumChanged
		{
			add { _maximumChangedEvent += value; }
			remove { _maximumChangedEvent -= value; }
		}

		private decimal Range
		{
			get { return this.Maximum - this.Minimum; }
		}

		private void NotifyValueChanged(object sender, EventArgs e)
		{
			EventsHelper.Fire(_valueChangedEvent, sender, e);
		}

		void OnTrackBarValueChanged(object sender, EventArgs e)
		{
			if (!_trackBarValueChanging)
			{
				_trackBarValueChanging = true;

				decimal ratio = (decimal)_trackBar.Value / (decimal)this.TrackBarIncrements;
				decimal numericUpDownValue = ratio * this.Range + this.Minimum;
				this.Value = numericUpDownValue; // Math.Round(numericUpDownValue, this.DecimalPlaces);
				
				_trackBarValueChanging = false;
			}
		}

		void OnNumericUpDownValueChanged(object sender, EventArgs e)
		{
			if (!_upDownValueChanging)
			{
				_upDownValueChanging = true;

				decimal value = this.Value - this.Minimum;
				decimal ratio = value / this.Range;
				decimal trackBarValue = ratio * this.TrackBarIncrements;
				_trackBar.Value = (int)Math.Round(trackBarValue, 0);

				_upDownValueChanging = false;
				_valueChangedDispatcher.RegisterAuthenticEvent(this, EventArgs.Empty);
			}
		}

	}
}
