#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="ExternalPractitionerMergeSelectedDuplicateComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class ExternalPractitionerMergeSelectedDuplicateComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ExternalPractitionerMergeSelectedDuplicateComponent class.
	/// </summary>
	[AssociateView(typeof(ExternalPractitionerMergeSelectedDuplicateComponentViewExtensionPoint))]
	public class ExternalPractitionerMergeSelectedDuplicateComponent : ApplicationComponent
	{
		private class ExternalPractitionerTable : Table<ExternalPractitionerSummary>
		{
			private readonly ExternalPractitionerMergeSelectedDuplicateComponent _owner;

			public ExternalPractitionerTable(ExternalPractitionerMergeSelectedDuplicateComponent owner)
			{
				_owner = owner;

				var nameColumn = new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnName,
					prac => PersonNameFormat.Format(prac.Name), 0.5f)
					{ ClickLinkDelegate = _owner.LaunchSelectedPractitionerPreview };

				var licenseColumn = new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnLicenseNumber,
					prac => prac.LicenseNumber, 0.25f);

				var billingColumn = new TableColumn<ExternalPractitionerSummary, string>(SR.ColumnBillingNumber,
					prac => prac.BillingNumber, 0.25f);

				this.Columns.Add(nameColumn);
				this.Columns.Add(licenseColumn);
				this.Columns.Add(billingColumn);
			}
		}

		private readonly ExternalPractitionerTable _table;
		private ExternalPractitionerDetail _originalPractitioner;
		private ExternalPractitionerSummary _selectedItem;

		public ExternalPractitionerMergeSelectedDuplicateComponent()
		{
			_table = new ExternalPractitionerTable(this);
		}

		public override void Start()
		{
			// Must select at least one practitioner
			this.Validation.Add(new ValidationRule("SummarySelection",
				component => new ValidationResult(_selectedItem != null, SR.MessageValidationSelectDuplicate)));

			base.Start();
		}

		public ExternalPractitionerDetail OriginalPractitioner
		{
			get { return _originalPractitioner; }
			set
			{
				if (_originalPractitioner != null && _originalPractitioner.Equals(value))
					return;

				_originalPractitioner = value;

				_table.Items.Clear();
				if (_originalPractitioner == null)
					return;

				var duplicates = LoadDuplicates(_originalPractitioner.PractitionerRef);
				_table.Items.AddRange(duplicates);
			}
		}
		public ExternalPractitionerSummary SelectedPractitioner
		{
			get { return _selectedItem; }
		}

		#region Presentation Models

		public string Name
		{
			get { return _originalPractitioner == null ? null : PersonNameFormat.Format(_originalPractitioner.Name); }
		}

		public string LicenseNumber
		{
			get { return _originalPractitioner == null ? null : _originalPractitioner.LicenseNumber; }
		}

		public string BillingNumber
		{
			get { return _originalPractitioner == null ? null : _originalPractitioner.BillingNumber; }
		}

		public string Instruction
		{
			get { return SR.MessageInstructionSelectDuplicate; }
		}

		public ITable PractitionerTable
		{
			get { return _table; }
		}

		public ISelection SummarySelection
		{
			get { return new Selection(_selectedItem); }
			set
			{
				var previousSelection = new Selection(_selectedItem);
				if (previousSelection.Equals(value)) 
					return;

				_selectedItem = (ExternalPractitionerSummary) value.Item;
				NotifyPropertyChanged("SummarySelection");
			}
		}

		#endregion

		private static List<ExternalPractitionerSummary> LoadDuplicates(EntityRef practitionerRef)
		{
			var duplicates = new List<ExternalPractitionerSummary>();

			if (practitionerRef != null)
			{
				Platform.GetService(
					delegate(IExternalPractitionerAdminService service)
					{
						var request = new LoadMergeExternalPractitionerFormDataRequest { PractitionerRef = practitionerRef };
						var response = service.LoadMergeExternalPractitionerFormData(request);

						duplicates = response.Duplicates;
					});
			}

			return duplicates;
		}

		private void LaunchSelectedPractitionerPreview(object practitioner)
		{
			var component = new ExternalPractitionerOverviewComponent { PractitionerSummary = (ExternalPractitionerSummary) practitioner };
			LaunchAsDialog(this.Host.DesktopWindow, component, SR.TitlePractitioner);
		}
	}
}
