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
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	[ExtensionPoint]
	public sealed class MprViewerToolExtensionPoint : ExtensionPoint<ITool> {}

	public interface IMprViewerToolContext : IImageViewerToolContext
	{
		new MprViewerComponent Viewer { get; }
	}

	public abstract class MprViewerTool : MouseImageViewerTool
	{
		protected MprViewerTool() {}

		public new MprViewerComponent ImageViewer
		{
			get { return (MprViewerComponent) base.ImageViewer; }
		}

		public new IMprViewerToolContext Context
		{
			get { return (IMprViewerToolContext) base.Context; }
		}

		[Obsolete("JY")]
		public bool IsMprImage(IPresentationImage image)
		{
			return image.ParentDisplaySet is MprDisplaySet;
		}

		[Obsolete("JY")]
		public bool IsIdentityImage(IPresentationImage image)
		{
			return IsMprImage(image, MprDisplaySetIdentifier.Identity);
		}

		[Obsolete("JY")]
		public bool IsOrthoXImage(IPresentationImage image)
		{
			return IsMprImage(image, MprDisplaySetIdentifier.OrthoX);
		}

		[Obsolete("JY")]
		public bool IsOrthoYImage(IPresentationImage image)
		{
			return IsMprImage(image, MprDisplaySetIdentifier.OrthoY);
		}

		[Obsolete("JY")]
		public bool IsObliqueImage(IPresentationImage image)
		{
			return IsMprImage(image, MprDisplaySetIdentifier.Oblique);
		}

		[Obsolete("JY")]
		public MprDisplaySet GetIdentityDisplaySet()
		{
			return FindMprDisplaySet(MprDisplaySetIdentifier.Identity);
		}

		[Obsolete("JY")]
		public MprDisplaySet GetOrthoYDisplaySet()
		{
			return FindMprDisplaySet(MprDisplaySetIdentifier.OrthoY);
		}

		[Obsolete("JY")]
		public MprDisplaySet GetOrthoXDisplaySet()
		{
			return FindMprDisplaySet(MprDisplaySetIdentifier.OrthoX);
		}

		[Obsolete("JY")]
		public MprDisplaySet GetObliqueDisplaySet()
		{
			return FindMprDisplaySet(MprDisplaySetIdentifier.Oblique);
		}

		[Obsolete("JY")]
		private static bool IsMprImage(IPresentationImage image, MprDisplaySetIdentifier identifier)
		{
			if (image == null || image.ParentDisplaySet == null)
				return false;

			return image.ParentDisplaySet is MprDisplaySet && ((MprDisplaySet) image.ParentDisplaySet).Identifier == identifier;
		}

		[Obsolete("JY")]
		private MprDisplaySet FindMprDisplaySet(MprDisplaySetIdentifier identifier)
		{
			IPhysicalWorkspace workspace = this.ImageViewer.PhysicalWorkspace;
			foreach (IImageBox imageBox in workspace.ImageBoxes)
			{
				MprDisplaySet displaySet = imageBox.DisplaySet as MprDisplaySet;
				if (displaySet != null && displaySet.Identifier == identifier)
					return displaySet;
			}

			return null;
		}
	}
}