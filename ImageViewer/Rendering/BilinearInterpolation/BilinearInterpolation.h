#pragma region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#pragma endregion

// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the BILINEARINTERPOLATION_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// BILINEARINTERPOLATION_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.

#ifdef BILINEARINTERPOLATION_EXPORTS
#define BILINEARINTERPOLATION_API __declspec(dllexport)
#else
#define BILINEARINTERPOLATION_API __declspec(dllimport)
#endif

struct LUTDATA
{
	int *LutData;
	int FirstMappedPixelValue;
	int Length;
};

extern "C"
{
	BILINEARINTERPOLATION_API BOOL InterpolateBilinear
	(
            BYTE* pSrcPixelData,

			unsigned int srcWidth,
            unsigned int srcHeight,
            unsigned int srcBytesPerPixel,
			unsigned int srcBitsStored,

			BOOL isSigned,
			BOOL isRGB,
			BOOL isPlanar,

			float srcRegionRectLeft,
            float srcRegionRectTop,
            float srcRegionRectRight,
            float srcRegionRectBottom,
			
            BYTE* pDstPixelData,
            unsigned int dstWidth,
            unsigned int dstBytesPerPixel,

			int dstRegionRectLeft,
            int dstRegionRectTop,
            int dstRegionRectRight,
            int dstRegionRectBottom,

			BOOL swapXY,
			LUTDATA* pLutData
	);
}
