#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations
{
	[AllowMultiplePresetVoiLutOperations]
	internal sealed class LinearPresetVoiLutOperationFactory : PresetVoiLutOperationFactory<LinearPresetVoiLutOperationComponent>
	{
		internal static readonly string FactoryName = "Linear Preset";

		public LinearPresetVoiLutOperationFactory()
		{
		}

		public override string Name
		{
			get { return FactoryName; }
		}

		public override string Description
		{
			get { return SR.LinearPresetVoiLutOperationFactoryDescription; }
		}
	}
}