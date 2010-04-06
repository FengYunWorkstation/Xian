﻿#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuMatchScale", "Activate")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarMatchScale", "Activate", KeyStroke = XKeys.M)]
	[IconSet("activate", IconScheme.Colour, "Icons.MatchScaleToolSmall.png", "Icons.MatchScaleToolMedium.png", "Icons.MatchScaleToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.MatchScale")]
	[Tooltip("activate", "TooltipMatchScale")]
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class MatchScaleTool : ImageViewerTool, IUndoableOperation<IPresentationImage>
	{
		#region Private Fields

		private const float _oneDegreeInRadians = (float)(Math.PI / 180);

		private bool _matchScaleNonParallelImages = true;
		private float _referenceDisplayedWidth;
		private RectangleF _referenceDisplayRectangle;

		#endregion

		public MatchScaleTool()
		{
			_matchScaleNonParallelImages = ToolSettings.Default.MatchScaleForNonParallelImages;
		}

		#region Private Properties

		private IPresentationImage ReferenceImage
		{
			get { return base.ImageViewer.SelectedPresentationImage; }
		}

		#endregion

		#region Public Methods

		public void Activate()
		{
			if (!AppliesTo(ReferenceImage))
				return;

			CalculateReferenceDisplayValues();
			if (Math.Abs(_referenceDisplayRectangle.Width) < 50)
			{
				Context.DesktopWindow.ShowMessageBox(SR.MessageNotEnoughImageVisible, MessageBoxActions.Ok);
				return;
			}

			var historyCommand = new DrawableUndoableOperationCommand<IPresentationImage>(this, GetAllImages());
			historyCommand.Execute();
			if (historyCommand.Count > 0)
			{
				historyCommand.Name = SR.CommandMatchScale;
				base.ImageViewer.CommandHistory.AddCommand(historyCommand);
			}
		}

		#endregion

		#region IUndoableOperation<IPresentationImage> Members

		public IMemorable GetOriginator(IPresentationImage image)
		{
			return GetImageTransform(image);
		}

		public bool AppliesTo(IPresentationImage image)
		{
			ImageSpatialTransform transform = GetImageTransform(image);
			if (transform == null)
				return false;

			//mustn't be rotated at a non-right angle to the viewport.
			if (transform.RotationXY % 90 != 0)
				return false;

			Frame frame = GetFrame(image);
			if (frame == null || frame.NormalizedPixelSpacing.IsNull)
				return false;
			
			if (!_matchScaleNonParallelImages && image != this.ReferenceImage)
			{
				Frame referenceFrame = GetFrame(this.ReferenceImage);
				if (!IsInSameFrameOfReference(referenceFrame, frame))
					return false;

				Vector3D referenceNormal = referenceFrame.ImagePlaneHelper.GetNormalVector();
				Vector3D normal = frame.ImagePlaneHelper.GetNormalVector();

				//If the reference image has no spatial info, then match scale with other such images
				if (referenceNormal == null && normal == null)
					return true;

				//if only one is null, don't do it
				if (referenceNormal == null || normal == null)
					return false;

				//if they're not parallel, don't do it
				if (!referenceNormal.IsParallelTo(normal, _oneDegreeInRadians))
					return false;
			}

			return true;
		}

		public void Apply(IPresentationImage image)
		{
			if (image == ReferenceImage)
				return;

			//Turn off scale to fit and start with scale=1, then adjust it.
			//We do this because images that have been "scaled to fit", but have not been shown yet,
			//have no client rectangle and their scale is often very small.  This is safer
			//and could produce a more accurate result.
			ImageSpatialTransform matchTransform = GetImageTransform(image);
			matchTransform.ScaleToFit = false;
			matchTransform.Scale = 1;

			//get the displayed width (in mm) for the same size display rectangle in the image to be matched.
			float matchDisplayedWidth = GetDisplayedWidth(image, _referenceDisplayRectangle);
			float rescaleAmount = matchDisplayedWidth / _referenceDisplayedWidth;

			matchTransform.Scale *= rescaleAmount;
		}

		#endregion

		#region Private Methods

		private void CalculateReferenceDisplayValues()
		{
			ImageSpatialTransform transform = GetImageTransform(ReferenceImage);
			Frame frame = GetFrame(ReferenceImage);

			//calculate the width (in mm) of the portion of the image that is visible on the display,
			//as well as the display rectangle it occupies.

			RectangleF sourceRectangle = new RectangleF(0, 0, frame.Columns, frame.Rows);
			_referenceDisplayRectangle = transform.ConvertToDestination(sourceRectangle);
			_referenceDisplayRectangle = RectangleUtilities.Intersect(_referenceDisplayRectangle, ReferenceImage.ClientRectangle);

			_referenceDisplayedWidth = GetDisplayedWidth(ReferenceImage, _referenceDisplayRectangle);
		}

		#region Private Helper Methods

		private static bool IsInSameFrameOfReference(Frame thisFrame, Frame otherFrame)
		{
			if (thisFrame.ParentImageSop.StudyInstanceUid != otherFrame.ParentImageSop.StudyInstanceUid)
				return false;

			return thisFrame.FrameOfReferenceUid == otherFrame.FrameOfReferenceUid;
		}

		private static float GetDisplayedWidth(IPresentationImage presentationImage, RectangleF referenceDisplayedRectangle)
		{
			ImageSpatialTransform transform = GetImageTransform(presentationImage);
			Frame frame = GetFrame(presentationImage);

			//Convert the displayed width to source dimensions
			SizeF sourceSize = transform.ConvertToSource(new SizeF(referenceDisplayedRectangle.Width, 0));
			float x = Math.Abs(sourceSize.Width);
			float y = Math.Abs(sourceSize.Height);

			//The displayed width is the magnitude of the line in source coordinates,
			//but one of xLength or yLength will always be zero, so we can optimize.
			if (x > y)
				return x * (float)frame.NormalizedPixelSpacing.Column;
			
			return y * (float)frame.NormalizedPixelSpacing.Row;
		}

		private static ImageSpatialTransform GetImageTransform(IPresentationImage image)
		{
			if (image != null && image is ISpatialTransformProvider)
				return ((ISpatialTransformProvider)image).SpatialTransform as ImageSpatialTransform;

			return null;
		}

		private static Frame GetFrame(IPresentationImage image)
		{
			if (image != null && image is IImageSopProvider)
				return ((IImageSopProvider)image).Frame;

			return null;
		}

		private IEnumerable<IPresentationImage> GetAllImages()
		{
			foreach (IImageBox imageBox in base.ImageViewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox.DisplaySet == null)
					continue;
				
				foreach (IPresentationImage image in imageBox.DisplaySet.PresentationImages)
					yield return image;
			}
		}

		#endregion
		#endregion
	}
}
