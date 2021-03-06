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
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[RisApplicationService]
	[ServiceContract]
	[ServiceKnownType(typeof(ReportingWorklistItemSummary))]
	public interface ITranscriptionWorkflowService : IWorklistService<ReportingWorklistItemSummary>, IWorkflowService
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		GetRejectReasonChoicesResponse GetRejectReasonChoices(GetRejectReasonChoicesRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		StartTranscriptionResponse StartTranscription(StartTranscriptionRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		DiscardTranscriptionResponse DiscardTranscription(DiscardTranscriptionRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof (RequestValidationException))]
		[FaultContract(typeof (ConcurrentModificationException))]
		SaveTranscriptionResponse SaveTranscription(SaveTranscriptionRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		SubmitTranscriptionForReviewResponse SubmitTranscriptionForReview(SubmitTranscriptionForReviewRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof (RequestValidationException))]
		[FaultContract(typeof (ConcurrentModificationException))]
		CompleteTranscriptionResponse CompleteTranscription(CompleteTranscriptionRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof (RequestValidationException))]
		[FaultContract(typeof (ConcurrentModificationException))]
		RejectTranscriptionResponse RejectTranscription(RejectTranscriptionRequest request);

		/// <summary>
		/// Load the report of a given reporting step
		/// </summary>
		/// <param name="request"><see cref="LoadTranscriptionForEditRequest"/></param>
		/// <returns><see cref="LoadTranscriptionForEditResponse"/></returns>
		[OperationContract]
		LoadTranscriptionForEditResponse LoadTranscriptionForEdit(LoadTranscriptionForEditRequest request);
	}
}
