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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise.SqlServer;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model.SqlServer.EntityBrokers
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class WorkQueueProcessDuplicateSopBroker : EntityBroker<WorkQueueProcessDuplicateSop, WorkQueueProcessDuplicateSopSelectCriteria, WorkQueueProcessDuplicateSopUpdateColumns>, IWorkQueueProcessDuplicateSopBroker
    {
        public WorkQueueProcessDuplicateSopBroker()
            : base("WorkQueue")
        { }
    }
}
