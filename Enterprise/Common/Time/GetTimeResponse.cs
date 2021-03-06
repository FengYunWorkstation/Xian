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
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common.Time
{
	[DataContract]
	public class GetTimeResponse : DataContractBase
	{
		public GetTimeResponse(DateTime time)
		{
			Time = time;
		}

		[DataMember]
		public DateTime Time;
	}
}
