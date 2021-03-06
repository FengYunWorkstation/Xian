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
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace $rootnamespace$
{
    [DataContract]
    public class List$fileinputname$sResponse : DataContractBase
    {
        public List$fileinputname$sResponse(List<$fileinputname$Summary> $fileinputname$s)
        {
            this.$fileinputname$s = $fileinputname$s;
        }

        [DataMember]
        public List<$fileinputname$Summary> $fileinputname$s;
    }
}
