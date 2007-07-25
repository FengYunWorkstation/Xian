using System.Collections;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// ReportingToBeReportedWorklist entity
    /// </summary>
    public partial class ReportingToBeReportedWorklist : ClearCanvas.Healthcare.Worklist
    {
        /// <summary>
        /// This method is called from the constructor.  Use this method to implement any custom
        /// object initialization.
        /// </summary>
        private void CustomInitialize()
        {
        }

        #region Worklist overrides

        public override IList GetWorklist(Staff currentUserStaff, IPersistenceContext context)
        {
            return (IList)GetBroker<IReportingWorklistBroker>(context).GetToBeReportedWorklist(this);
        }

        public override int GetWorklistCount(Staff currentUserStaff, IPersistenceContext context)
        {
            return GetBroker<IReportingWorklistBroker>(context).GetToBeReportedWorklistCount(this);
        }

        public override string NameSuffix
        {
            get { return " - To Be Reported"; }
        }

        #endregion

        #region Object overrides

        public override bool Equals(object that)
        {
            // TODO: implement a test for business-key equality
            return base.Equals(that);
        }

        public override int GetHashCode()
        {
            // TODO: implement a hash-code based on the business-key used in the Equals() method
            return base.GetHashCode();
        }

        #endregion
    }
}