#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Network;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using LocalDicomServer = ClearCanvas.ImageViewer.Common.DicomServer.DicomServer;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal static class AssociationVerifier
	{
		public static bool VerifyAssociation(IDicomServerContext context, AssociationParameters assocParms, out DicomRejectResult result, out DicomRejectReason reason)
		{
			string calledTitle = (assocParms.CalledAE ?? "").Trim();
			string callingAE = (assocParms.CallingAE ?? "").Trim();

			result = DicomRejectResult.Permanent;
			reason = DicomRejectReason.NoReasonGiven;

		    var extendedConfiguration = LocalDicomServer.GetExtendedConfiguration();
            if (!extendedConfiguration.AllowUnknownCaller && ServerDirectory.GetRemoteServersByAETitle(callingAE).Count == 0)
			{
				reason = DicomRejectReason.CallingAENotRecognized;
			}
			else if (calledTitle != context.AETitle)
			{
				reason = DicomRejectReason.CalledAENotRecognized;
			}
			else
			{
				return true;
			}

			return false;
		}
	}
}
