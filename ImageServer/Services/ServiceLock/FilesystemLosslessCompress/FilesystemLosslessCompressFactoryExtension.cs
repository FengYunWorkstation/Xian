#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.ServiceLock.FilesystemLosslessCompress
{
	[ExtensionOf(typeof(ServiceLockFactoryExtensionPoint))]
	class FilesystemLosslessCompressFactoryExtension : IServiceLockProcessorFactory
	{
		#region IServiceLockProcessorFactory Members

		public ServiceLockTypeEnum GetServiceLockType()
		{
			return ServiceLockTypeEnum.FilesystemLosslessCompress;
		}

		public IServiceLockItemProcessor GetItemProcessor()
		{
			return new FilesystemLosslessCompressItemProcessor();
		}

		#endregion
	}
}
