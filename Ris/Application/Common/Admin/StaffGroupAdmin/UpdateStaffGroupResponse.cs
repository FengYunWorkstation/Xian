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
using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin
{
    [DataContract]
    public class UpdateStaffGroupResponse : DataContractBase
    {
        public UpdateStaffGroupResponse(StaffGroupSummary staffGroup)
        {
            this.StaffGroup = staffGroup;
        }

        [DataMember]
        public StaffGroupSummary StaffGroup;
   }
}
