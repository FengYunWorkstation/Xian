﻿#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Dicom.Network.Scu;

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// Data Export Audit Log Helper
	/// </summary>
	/// <remarks>
	/// This message describes the event of exporting data from a system, implying that the data is leaving
	/// control of the system�s security domain. Examples of exporting include printing to paper, recording on film,
	/// creation of a .pdf or HTML file, conversion to another format for storage in an EHR, writing to removable
	/// media, or sending via e-mail. Multiple patients may be described in one event message.
	/// </remarks>
	public class DataExportAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="auditSource">The source of the audit.</param>
		/// <param name="outcome">The outcome (success or failure)</param>
		/// <param name="exportDestination">Any machine readable identifications on the media, such as media serial number, volume label, 
		/// DICOMDIR SOP Instance UID.</param>
		public DataExportAuditHelper(DicomAuditSource auditSource, EventIdentificationContentsEventOutcomeIndicator outcome, string exportDestination)
			: base("DataExport")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents();
			AuditMessage.EventIdentification.EventID = EventID.Export;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationContentsEventActionCode.E;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddAuditSource(auditSource);

			// Add the Destination
			_participantList.Add(
				new ActiveParticipantContents(RoleIDCode.DestinationMedia, ProcessName, exportDestination, null, null, null,false));
		}

		/// <summary>
		/// Add an exporter.
		/// </summary>
		/// <param name="userId">The identity of the local user or process exporting the data. If both
		/// are known, then two active participants shall be included (both the
		/// person and the process).</param>
		/// <param name="userName">The name of the user</param>
		/// <param name="userIsRequestor">Flag telling if the exporter is a user (as opposed to a process)</param>
		public void AddExporter(AuditActiveParticipant participant)
		{
            participant.RoleIdCode = RoleIDCode.SourceMedia;
			participant.UserIsRequestor = true;
			InternalAddActiveParticipant(participant);
		}

		/// <summary>
		/// Add details of a Patient.
		/// </summary>
		/// <param name="study"></param>
		public void AddPatientParticipantObject(AuditPatientParticipantObject patient)
		{
			InternalAddParticipantObject(patient.PatientId + patient.PatientsName, patient);
		}

		/// <summary>
		/// Add details of a study.
		/// </summary>
		/// <param name="study"></param>
		public void AddStudyParticipantObject(AuditStudyParticipantObject study)
		{
			InternalAddParticipantObject(study.StudyInstanceUid, study);
		}

		/// <summary>
		/// Add details of images within a study.  SOP Class information is automatically updated.
		/// </summary>
		/// <param name="instance">Descriptive object being audited</param>
		public void AddStorageInstance(StorageInstance instance)
		{
			InternalAddStorageInstance(instance);
		}
	}
}
