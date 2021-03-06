#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IImageMimeTypeConverter
    {
        string OutputMimeType { get; }

        ImageConverterOutput Convert(ImageStreamingContext context);
    }

}