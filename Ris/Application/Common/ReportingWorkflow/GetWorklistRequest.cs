using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class GetWorklistRequest : DataContractBase
    {
        public GetWorklistRequest(string worklistClassName)
        {
            this.WorklistClassName = worklistClassName;
        }

        [DataMember]
        public string WorklistClassName;

        //[DataMember]
        //public Type StepClass;

        //[DataMember]
        //public ReportingWorklistSearchCriteria SearchCriteria;
    }
}
