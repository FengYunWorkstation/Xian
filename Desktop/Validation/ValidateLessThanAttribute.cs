#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop.Validation
{
	/// <summary>
	/// Validates that a property value is less than reference value.
	/// </summary>
	public class ValidateLessThanAttribute : ValidateCompareAttribute
	{
		/// <summary>
		/// Constructor that accepts the name of a reference property.
		/// </summary>
		/// <param name="referenceProperty">The name of a property on the component that provides a reference value.</param>
		public ValidateLessThanAttribute(string referenceProperty)
			: base(referenceProperty)
		{
		}

		/// <summary>
		/// Constructor that accepts a constant reference value.
		/// </summary>
		public ValidateLessThanAttribute(int referenceValue)
			: base(referenceValue)
		{
		}

		/// <summary>
		/// Constructor that accepts a constant reference value.
		/// </summary>
		public ValidateLessThanAttribute(float referenceValue)
			: base(referenceValue)
		{
		}

		/// <summary>
		/// Constructor that accepts a constant reference value.
		/// </summary>
		public ValidateLessThanAttribute(double referenceValue)
			: base(referenceValue)
		{
		}

		/// <summary>
		/// Returns -1.
		/// </summary>
		protected override int GetCompareSign()
		{
			return -1;
		}
	}
}
