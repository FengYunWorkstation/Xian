#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare.Workflow.Transcription
{
	public class TranscriptionOperations
	{
		public abstract class TranscriptionOperation
		{
			public virtual bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				return false;
			}
		}

		public class StartTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff performingStaff)
			{
				step.Start(performingStaff);
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (!step.IsInitial
					|| step.State == ActivityStatus.IP && !Equals(step.PerformingStaff, currentUserStaff))
					return false;

				if (step.AssignedStaff != null && !Equals(step.AssignedStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class DiscardTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff performingStaff)
			{
				step.Discontinue();
				var transcriptionStep = new TranscriptionStep(step);
				transcriptionStep.Schedule(Platform.Time);
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.IP)
					return false;

				if (!Equals(step.PerformingStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class SaveTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Dictionary<string, string> reportPartExtendedProperties)
			{
				ExtendedPropertyUtils.Update(step.ReportPart.ExtendedProperties, reportPartExtendedProperties);
			}

			public void Execute(TranscriptionStep step, Dictionary<string, string> reportPartExtendedProperties, Staff supervisor)
			{
				this.Execute(step, reportPartExtendedProperties);

				step.ReportPart.TranscriptionSupervisor = supervisor;
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.IP)
					return false;

				if (!Equals(step.PerformingStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class SubmitTranscriptionForReview : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff performingStaff, Staff supervisor)
			{
				step.Complete();

				var transcriptionStep = new TranscriptionStep(step);
				transcriptionStep.Assign(supervisor);
				transcriptionStep.Schedule(Platform.Time);
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.IP)
					return false;

				if (!Equals(step.PerformingStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class CompleteTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff performingStaff)
			{
				if(step.State == ActivityStatus.SC)
					step.Complete(performingStaff);
				else
					step.Complete();

				TranscriptionReviewStep transcriptionReviewStep = new TranscriptionReviewStep(step);
				step.Procedure.AddProcedureStep(transcriptionReviewStep);
				transcriptionReviewStep.Assign(step.ReportPart.Interpreter);
				transcriptionReviewStep.Schedule(Platform.Time);
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.SC && step.State != ActivityStatus.IP)
					return false;

				if (step.State == ActivityStatus.IP && !Equals(step.PerformingStaff, currentUserStaff))
					return false;

				// scheduled items should should look like items submitted for review
				if (step.State == ActivityStatus.SC && (step.ReportPart.Transcriber == null || step.AssignedStaff == null))
					return false;

				// if it looks like a transcription submitted for review, but not to the current user.
				if (step.State == ActivityStatus.SC && step.ReportPart.Transcriber != null && !Equals(step.AssignedStaff, currentUserStaff))
					return false;

				return true;
			}
		}

		public class RejectTranscription : TranscriptionOperation
		{
			public void Execute(TranscriptionStep step, Staff rejectedBy, TranscriptionRejectReasonEnum reason)
			{
				if (step.State == ActivityStatus.SC)
					step.Complete(rejectedBy);
				else
					step.Complete();

				step.ReportPart.TranscriptionRejectReason = reason;

				TranscriptionReviewStep transcriptionReviewStep = new TranscriptionReviewStep(step);
				step.Procedure.AddProcedureStep(transcriptionReviewStep);
				transcriptionReviewStep.Assign(step.ReportPart.Interpreter);
				transcriptionReviewStep.Schedule(Platform.Time);
				transcriptionReviewStep.HasErrors = true;
			}

			public override bool CanExecute(TranscriptionStep step, Staff currentUserStaff)
			{
				if (step.State != ActivityStatus.SC && step.State != ActivityStatus.IP)
					return false;

				if (step.State == ActivityStatus.IP && !Equals(step.PerformingStaff, currentUserStaff))
					return false;

				// scheduled items should should look like items submitted for review
				if (step.State == ActivityStatus.SC && (step.ReportPart.Transcriber == null || step.AssignedStaff == null))
					return false;

				// if it looks like an transcription submitted for review, but not to the current user.
				if (step.State == ActivityStatus.SC && step.ReportPart.Transcriber != null && !Equals(step.AssignedStaff, currentUserStaff))
					return false;

				return true;
			}
		}
	}
}
