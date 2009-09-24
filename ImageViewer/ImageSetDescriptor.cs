using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	public interface IDicomImageSetDescriptor : IImageSetDescriptor
	{
		IStudyRootStudyIdentifier SourceStudy { get; }
	}

	public class DicomImageSetDescriptor : ImageSetDescriptor, IDicomImageSetDescriptor
	{
		private readonly IStudyRootStudyIdentifier _sourceStudy;

		private string _name;
		private string _patientInfo;
		private string _uid;

		public DicomImageSetDescriptor(IStudyRootStudyIdentifier sourceStudy)
		{
			Platform.CheckForNullReference(sourceStudy, "sourceStudy");
			_sourceStudy = sourceStudy;
		}

		public IStudyRootStudyIdentifier SourceStudy
		{
			get { return _sourceStudy; }
		}

		public override string Name
		{
			get
			{
				if (_name == null)
					_name = GetName() ?? "";
				return _name;
			}
			set { throw new InvalidOperationException("The Name property cannot be set publicly."); }
		}

		public override string PatientInfo
		{
			get
			{
				if (_patientInfo == null)
					_patientInfo = GetPatientInfo() ?? "";
				return _patientInfo;
			}
			set { throw new InvalidOperationException("The PatientInfo property cannot be set publicly."); }
		}

		public override string Uid
		{
			get
			{
				if (_uid == null)
					_uid = GetUid() ?? "";
				return _uid;
			}
			set { throw new InvalidOperationException("The Uid property cannot be set publicly."); }
		}

		protected virtual string GetName()
		{
			DateTime studyDate;
			DateParser.Parse(_sourceStudy.StudyDate, out studyDate);
			DateTime studyTime;
			TimeParser.Parse(_sourceStudy.StudyTime, out studyTime);

			string modalitiesInStudy = StringUtilities.Combine(_sourceStudy.ModalitiesInStudy, ", ");

			return String.Format("{0} {1} [{2}] {3}",
										  studyDate.ToString(Format.DateFormat),
										  studyTime.ToString(Format.TimeFormat),
										  modalitiesInStudy ?? "",
										  _sourceStudy.StudyDescription);
		}

		protected virtual string GetPatientInfo()
		{
			return String.Format("{0} � {1}", new PersonName(_sourceStudy.PatientsName).FormattedName, _sourceStudy.PatientId);
		}

		protected virtual string GetUid()
		{
			return _sourceStudy.StudyInstanceUid;
		}
	}

	public class BasicImageSetDescriptor : ImageSetDescriptor
	{
		private string _name;
		private string _patientInfo;
		private string _uid;

		public BasicImageSetDescriptor()
		{}

		public override string Name
		{
			get { return _name ?? ""; }
			set { _name = value; }
		}

		public override string PatientInfo
		{
			get { return _patientInfo ?? ""; }
			set { _patientInfo = value; }
		}

		public override string Uid
		{
			get { return _uid ?? ""; }
			set { _uid = value; }
		}
	}

	public interface IImageSetDescriptor
	{
		IImageSet ImageSet { get; }

		string Name { get; }
		string PatientInfo { get; }
		string Uid { get; }
	}

	public abstract class ImageSetDescriptor : IImageSetDescriptor
	{
		private ImageSet _imageSet;

		protected ImageSetDescriptor()
		{
		}

		#region IImageSetDescriptor Members

		IImageSet IImageSetDescriptor.ImageSet
		{
			get { return _imageSet; }	
		}

		public virtual ImageSet ImageSet
		{
			get { return _imageSet; }
			internal set { _imageSet = value; }
		}

		public abstract string Name { get; set; }

		public abstract string PatientInfo { get; set; }

		public abstract string Uid { get; set; }

		#endregion

		public override string ToString()
		{
			return StringUtilities.Combine(new string[] { PatientInfo, Name, Uid }, " | ", true);
		}
	}
}