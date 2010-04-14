﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
	public class ReportingWorklistItem : WorklistItem
	{

		private static readonly Dictionary<WorklistItemField, WorklistItemFieldSetterDelegate> _fieldSetters
			= new Dictionary<WorklistItemField, WorklistItemFieldSetterDelegate>();

		static ReportingWorklistItem()
		{
			// need to careful about checking for null values here, because a ReportingWorklistItem may not 
			// have any report information (if it represents a scheduled interpretation step)
			_fieldSetters.Add(WorklistItemField.Report,
				(item, value) => ((ReportingWorklistItem)item).ReportRef = (EntityRef)value);

			_fieldSetters.Add(WorklistItemField.ReportPartIndex,
				(item, value) => ((ReportingWorklistItem)item).ReportPartIndex = value == null ? -1 : (int)value);

			_fieldSetters.Add(WorklistItemField.ReportPartHasErrors,
				(item, value) => ((ReportingWorklistItem)item).HasErrors = value == null ? false : (bool)value);

			_fieldSetters.Add(WorklistItemField.ReportPartPreliminaryTime,
				(item, value) => item.Time = (DateTime?)value);

			_fieldSetters.Add(WorklistItemField.ReportPartCompletedTime,
				(item, value) => item.Time = (DateTime?)value);
		}



		/// <summary>
		/// Default constructor required for dyanmic instantiation.
		/// </summary>
		public ReportingWorklistItem()
		{
			ReportPartIndex = -1;
		}

		/// <summary>
		/// Initialize this worklist item from the specified procedure step and related entities.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="timeField"></param>
		/// <remarks>
		/// This method is not efficient for generating a large number of worklist items from a large set of procedure steps,
		/// because it causes a large number of secondary references and collections to be initiliazed.
		/// Use <see cref="WorklistItem.InitializeFromTuple"/> instead.
		/// </remarks>
		public override void InitializeFromProcedureStep(ProcedureStep step, WorklistItemField timeField)
		{
			var reportingStep = step.As<ReportingProcedureStep>();
			if (reportingStep != null && reportingStep.ReportPart != null)
			{
				this.ReportRef = reportingStep.ReportPart.Report.GetRef();
				this.ReportPartIndex = reportingStep.ReportPart.Index;
				this.HasErrors = reportingStep.Is<TranscriptionReviewStep>()
					? reportingStep.As<TranscriptionReviewStep>().HasErrors : false;
			}

			base.InitializeFromProcedureStep(step, timeField);
		}

		#region Public Properties

		/// <summary>
		/// Gets the report ref.
		/// </summary>
		public EntityRef ReportRef { get; internal set; }

		/// <summary>
		/// Gets the report part index, or -1 if there is no report part.
		/// </summary>
		public int ReportPartIndex { get; internal set; }

		/// <summary>
		/// Gets a value indicating if transcription has flagged report for errors.
		/// </summary>
		public bool HasErrors { get; internal set; }

		#endregion


		#region Protected API

		/// <summary>
		/// Gets the setter for the specified field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		protected override WorklistItemFieldSetterDelegate GetFieldSetter(WorklistItemField field)
		{
			WorklistItemFieldSetterDelegate updater;
			return _fieldSetters.TryGetValue(field, out updater) ? updater : base.GetFieldSetter(field);
		}

		/// <summary>
		/// Gets the value for the specified time field from the specified procedure step or its associated entities.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="timeField"></param>
		/// <returns></returns>
		protected override DateTime? GetTimeValue(ProcedureStep step, WorklistItemField timeField)
		{
			var reportingStep = step.As<ReportingProcedureStep>();
			if(reportingStep != null)
			{
				if(timeField == WorklistItemField.ReportPartPreliminaryTime)
					return reportingStep.ReportPart.PreliminaryTime;

				if(timeField == WorklistItemField.ReportPartCompletedTime)
					return reportingStep.ReportPart.CompletedTime;
			}

			return base.GetTimeValue(step, timeField);
		}

		#endregion
	}
}