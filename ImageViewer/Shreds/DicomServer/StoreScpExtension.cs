#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	[ExtensionOf(typeof(DicomScpExtensionPoint<IDicomServerContext>))]
	public class ImageStorageScpExtension : StoreScpExtension
	{
		public ImageStorageScpExtension()
			: base(GetSupportedSops())
		{}

		private static IEnumerable<SupportedSop> GetSupportedSops()
		{
			foreach (SopClass sopClass in GetSopClasses(DicomServerSettings.Instance.ImageStorageSopClasses))
			{
			    var supportedSop = new SupportedSop
			                           {
			                               SopClass = sopClass
			                           };

			    supportedSop.AddSyntax(TransferSyntax.ExplicitVrLittleEndian);
				supportedSop.AddSyntax(TransferSyntax.ImplicitVrLittleEndian);

				foreach (TransferSyntax transferSyntax in GetTransferSyntaxes(DicomServerSettings.Instance.StorageTransferSyntaxes))
				{
					if (transferSyntax.DicomUid.UID != TransferSyntax.ExplicitVrLittleEndianUid &&
						transferSyntax.DicomUid.UID != TransferSyntax.ImplicitVrLittleEndianUid)
					{
						supportedSop.AddSyntax(transferSyntax);
					}
				}

				yield return supportedSop;
			}
		}
	}

	[ExtensionOf(typeof(DicomScpExtensionPoint<IDicomServerContext>))]
	public class NonImageStorageScpExtension : StoreScpExtension
	{
		public NonImageStorageScpExtension()
			: base(GetSupportedSops())
		{ }

		private static IEnumerable<SupportedSop> GetSupportedSops()
		{
			foreach (SopClass sopClass in GetSopClasses(DicomServerSettings.Instance.NonImageStorageSopClasses))
			{
			    var supportedSop = new SupportedSop
			                           {
			                               SopClass = sopClass
			                           };
			    supportedSop.AddSyntax(TransferSyntax.ExplicitVrLittleEndian);
				supportedSop.AddSyntax(TransferSyntax.ImplicitVrLittleEndian);
				yield return supportedSop;
			}
		}
	}

	public abstract class StoreScpExtension : ScpExtension
	{
	    private DicomReceiveImportContext _importContext;

		protected StoreScpExtension(IEnumerable<SupportedSop> supportedSops)
			: base(supportedSops)
		{
		}

		protected static IEnumerable<SopClass> GetSopClasses(SopClassConfigurationElementCollection config)
		{
			foreach (SopClassConfigurationElement element in config)
			{
				if (!String.IsNullOrEmpty(element.Uid))
				{
					SopClass sopClass = SopClass.GetSopClass(element.Uid);
					if (sopClass != null)
						yield return sopClass;
				}
			}
		}

		protected static IEnumerable<TransferSyntax> GetTransferSyntaxes(TransferSyntaxConfigurationElementCollection config)
		{
			foreach (TransferSyntaxConfigurationElement element in config)
			{
				if (!String.IsNullOrEmpty(element.Uid))
				{
					TransferSyntax syntax = TransferSyntax.GetTransferSyntax(element.Uid);
					if (syntax != null)
					{
						//at least for now, restrict to available codecs for compressed syntaxes.
						if (!syntax.Encapsulated || ClearCanvas.Dicom.Codec.DicomCodecRegistry.GetCodec(syntax) != null)
							yield return syntax;
					}
				}
			}
		}

		public override bool OnReceiveRequest(ClearCanvas.Dicom.Network.DicomServer server, 
			ServerAssociationParameters association, byte presentationID, DicomMessage message)
		{
			string studyInstanceUid;
			string seriesInstanceUid;
			DicomUid sopInstanceUid;

			bool ok = message.DataSet[DicomTags.SopInstanceUid].TryGetUid(0, out sopInstanceUid);
			if (ok) ok = message.DataSet[DicomTags.SeriesInstanceUid].TryGetString(0, out seriesInstanceUid);
			if (ok) ok = message.DataSet[DicomTags.StudyInstanceUid].TryGetString(0, out studyInstanceUid);

			if (!ok)
			{
				Platform.Log(LogLevel.Error, "Unable to retrieve UIDs from request message, sending failure status.");

				server.SendCStoreResponse(presentationID, message.MessageId, sopInstanceUid.UID,
					DicomStatuses.ProcessingFailure);

				return true;
			}

            if (_importContext == null)
            {
                _importContext = new DicomReceiveImportContext(association.CallingAE, GetRemoteHostName(association), StudyStore.GetConfiguration());

                // Publish new WorkItems as they're added to the context
                lock (_importContext.StudyWorkItemsSyncLock)
                {
                    _importContext.StudyWorkItems.ItemAdded += (sender, args) => Platform.GetService(
                        (IWorkItemActivityMonitorService service) =>
                        service.Publish(new WorkItemPublishRequest {Item = WorkItemDataHelper.FromWorkItem(args.Item)}));

                    _importContext.StudyWorkItems.ItemChanged += (sender, args) => Platform.GetService(
                        (IWorkItemActivityMonitorService service) =>
                        service.Publish(new WorkItemPublishRequest {Item = WorkItemDataHelper.FromWorkItem(args.Item)}));
                }
            }

		    // TODO (CR Jun 2012 - High): Doesn't look like received files are audited?
		    var importer = new ImportFilesUtility(_importContext);

		    var result = importer.Import(message,BadFileBehaviourEnum.Ignore, FileImportBehaviourEnum.Save);
            if (result.Successful)
            {
                if (!String.IsNullOrEmpty(result.AccessionNumber))
                    Platform.Log(LogLevel.Info, "Received SOP Instance {0} from {1} to {2} (A#:{3} StudyUid:{4})",
                                 result.SopInstanceUid, association.CallingAE, association.CalledAE, result.AccessionNumber,
                                 result.StudyInstanceUid);
                else
                    Platform.Log(LogLevel.Info, "Received SOP Instance {0} from {1} to {2} (StudyUid:{3})",
                                 result.SopInstanceUid, association.CallingAE, association.CalledAE,
                                 result.StudyInstanceUid);
                server.SendCStoreResponse(presentationID, message.MessageId, message.AffectedSopInstanceUid, result.DicomStatus);
            }
            else
            {
                if (result.DicomStatus==DicomStatuses.ProcessingFailure)
                    Platform.Log(LogLevel.Error, "Failure importing sop: {0}", result.ErrorMessage);

                //OnReceiveError(message, result.ErrorMessage, association.CallingAE);
                server.SendCStoreResponse(presentationID, message.MessageId, message.AffectedSopInstanceUid,
                                          result.DicomStatus, result.ErrorMessage);
            }		    
               
			return true;
		}

        private static string GetRemoteHostName(AssociationParameters association)
        {
            string remoteHostName = null;
            try
            {
                if (association.RemoteEndPoint != null)
                {
                    try
                    {
                        IPHostEntry entry = Dns.GetHostEntry(association.RemoteEndPoint.Address);
                        remoteHostName = entry.HostName;
                    }
                    catch
                    {
                        remoteHostName = association.RemoteEndPoint.Address.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                remoteHostName = null;
                Platform.Log(LogLevel.Warn, e, "Unable to resolve remote host name.");
            }

            return remoteHostName;
        }
	}
}
