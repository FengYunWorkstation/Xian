#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	internal class SliceLocationAnnotationItem : AnnotationItem
	{
		public SliceLocationAnnotationItem()
			: base("Dicom.ImagePlane.SliceLocation", new AnnotationResourceResolver(typeof(SliceLocationAnnotationItem).Assembly))
		{
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			if (presentationImage is IImageSopProvider)
			{
				Frame frame = ((IImageSopProvider) presentationImage).Frame;
				Vector3D normal = frame.ImagePlaneHelper.GetNormalVector();
				Vector3D positionCenterOfImage = frame.ImagePlaneHelper.ConvertToPatient(new PointF((frame.Columns - 1) / 2F, (frame.Rows - 1) / 2F));

				if (normal != null && positionCenterOfImage != null)
				{
					// Try to be a bit more specific when we have spatial information
					// by showing directional information (L, R, H, F, A, P) as well as
					// the slice location.
					float absX = Math.Abs(normal.X);
					float absY = Math.Abs(normal.Y);
					float absZ = Math.Abs(normal.Z);

					// Get the primary direction based on the largest component of the normal.
					if (absZ >= absY && absZ >= absX)
					{
						//mostly axial because Z >= X and Y
						string directionString = (positionCenterOfImage.Z >= 0F) ? SR.ValueDirectionalMarkersHead : SR.ValueDirectionalMarkersFoot;
						return string.Format("{0}{1:F1}", directionString, Math.Abs(positionCenterOfImage.Z));
					}
					else if (absY >= absX && absY >= absZ)
					{
						//mostly coronal because Y >= X and Z
						string directionString = (positionCenterOfImage.Y >= 0F) ? SR.ValueDirectionalMarkersPosterior : SR.ValueDirectionalMarkersAnterior;
						return string.Format("{0}{1:F1}", directionString, Math.Abs(positionCenterOfImage.Y));
					}
					else
					{
						//mostly sagittal because X >= Y and Z
						string directionString = (positionCenterOfImage.X >= 0F) ? SR.ValueDirectionalMarkersLeft : SR.ValueDirectionalMarkersRight;
						return string.Format("{0}{1:F1}", directionString, Math.Abs(positionCenterOfImage.X));
					}
				}
			}

			return "";
		}
	}
}
