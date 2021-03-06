#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Common.ServiceModel
{
    [ImageServerService]
    [ServiceContract]
    [Authentication(false)]
    public interface IProductVerificationService
    {
        [OperationContract]
        ProductVerificationResponse Verify(ProductVerificationRequest request);
    }
}