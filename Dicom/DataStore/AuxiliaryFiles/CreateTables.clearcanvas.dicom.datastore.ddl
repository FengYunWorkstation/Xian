create table InternalSopInstances_ (ParcelOid_ UNIQUEIDENTIFIER not null, ReferencedSopInstance_ UNIQUEIDENTIFIER not null, Index_ INT not null, primary key (ParcelOid_, Index_));
create table RetrieveParcel_ (ParcelOid_ UNIQUEIDENTIFIER not null, RetrieveObjectUid_ NVARCHAR(255) null, primary key (ParcelOid_));
create table Parcel_ (ParcelOid_ UNIQUEIDENTIFIER not null, ParcelTransferState_ SMALLINT null, Description_ NVARCHAR(255) null, CurrentStep_ INT null, TotalProgessSteps_ INT null, DestinationAEHost_ NVARCHAR(255) null, DestinationAEAE_ NVARCHAR(255) null, DestinationAEPort_ INT null, DestinationAEConnectionTimeout_ INT null, DestinationAEOperationTimeout_ INT null, SourceAEHost_ NVARCHAR(255) null, SourceAEAE_ NVARCHAR(255) null, SourceAEPort_ INT null, SourceAEConnectionTimeout_ INT null, SourceAEOperationTimeout_ INT null, primary key (ParcelOid_));
create table DicomDictionaryContainer_ (EntryOid_ UNIQUEIDENTIFIER not null, primary key (EntryOid_));
create table SopInstance_ (Oid_ UNIQUEIDENTIFIER not null, TransferSyntaxUid_ NVARCHAR(64) null, InstanceNumber_ INT null, SopInstanceUid_ NVARCHAR(64) null, SopClassUid_ NVARCHAR(64) null, SpecificCharacterSet_ NVARCHAR(64) null, LocationUri_ NVARCHAR(1024) null, SeriesOid_ UNIQUEIDENTIFIER null, primary key (Oid_));
create table TransferSyntaxes_ (ParcelOid_ UNIQUEIDENTIFIER not null, TransferSyntax_ NVARCHAR(255) null, Index_ INT not null, primary key (ParcelOid_, Index_));
create table WindowValues_ (Oid_ UNIQUEIDENTIFIER not null, Width_ FLOAT null, Center_ FLOAT null, VM_ INT not null, primary key (Oid_, VM_));
create table SendParcel_ (ParcelOid_ UNIQUEIDENTIFIER not null, primary key (ParcelOid_));
create table ImageSopInstance_ (Oid_ UNIQUEIDENTIFIER not null, SamplesPerPixel_ SMALLINT null, BitsStored_ SMALLINT null, RescaleSlope_ FLOAT null, Rows_ INT null, Columns_ INT null, PlanarConfiguration_ SMALLINT null, RescaleIntercept_ FLOAT null, PixelRepresentation_ SMALLINT null, BitsAllocated_ SMALLINT null, HighBit_ SMALLINT null, PhotometricInterpretation_ INT null, PixelSpacingRow_ FLOAT null, PixelSpacingColumn_ FLOAT null, PixelAspectRatioRow_ FLOAT null, PixelAspectRatioColumn_ FLOAT null, primary key (Oid_));
create table Series_ (SeriesOid_ UNIQUEIDENTIFIER not null, SeriesInstanceUid_ NVARCHAR(64) null, Modality_ NVARCHAR(16) null, SeriesNumber_ INT null, Laterality_ NVARCHAR(16) null, SeriesDescription_ NVARCHAR(64) null, SpecificCharacterSet_ NVARCHAR(64) null, StudyOid_ UNIQUEIDENTIFIER null, primary key (SeriesOid_));
create table DictionaryEntries_ (EntryOid_ UNIQUEIDENTIFIER not null, TagName_ NVARCHAR(64) null, Path_ NVARCHAR(256) null, IsComputed_ BIT null, ValueRepresentation_ NVARCHAR(16) null, Index_ INT not null, primary key (EntryOid_, Index_));
create table SopClasses_ (ParcelOid_ UNIQUEIDENTIFIER not null, SopClass_ NVARCHAR(255) null, Index_ INT not null, primary key (ParcelOid_, Index_));
create table Study_ (StudyOid_ UNIQUEIDENTIFIER not null, ProcedureCodeSequenceCodingSchemeDesignator_ NVARCHAR(16) null, StudyId_ NVARCHAR(16) null, StudyTime_ NVARCHAR(16) null, StudyDate_ NVARCHAR(26) null, AccessionNumber_ NVARCHAR(16) null, StudyInstanceUid_ NVARCHAR(64) null, StudyDescription_ NVARCHAR(64) null, SpecificCharacterSet_ NVARCHAR(64) null, ProcedureCodeSequenceCodeValue_ NVARCHAR(16) null, StoreTime_ DATETIME null, PatientsNameRaw_ NVARCHAR(255) null, PatientsName_ NVARCHAR(256) null, PatientId_ NVARCHAR(64) null, PatientsSex_ NVARCHAR(16) null, PatientsBirthDate_ NVARCHAR(16) null, primary key (StudyOid_));
alter table InternalSopInstances_  add constraint FK21F4B0F5CAA8D405 foreign key (ParcelOid_) references SendParcel_ ;
alter table InternalSopInstances_  add constraint FK21F4B0F5CB62A0FB foreign key (ReferencedSopInstance_) references SopInstance_ ;
alter table RetrieveParcel_  add constraint FK1AD4D92ECAA8D405 foreign key (ParcelOid_) references Parcel_ ;
alter table SopInstance_  add constraint FK4E7B8DA9F8D3BFCC foreign key (SeriesOid_) references Series_ ;
alter table TransferSyntaxes_  add constraint FKF4882296CAA8D405 foreign key (ParcelOid_) references SendParcel_ ;
alter table WindowValues_  add constraint FK33BD3FB7DDA74CD7 foreign key (Oid_) references ImageSopInstance_ ;
alter table SendParcel_  add constraint FK94EF88DBCAA8D405 foreign key (ParcelOid_) references Parcel_ ;
alter table ImageSopInstance_  add constraint FK50D8FA8EDDA74CD7 foreign key (Oid_) references SopInstance_ ;
alter table Series_  add constraint FKC47BF4932152F823 foreign key (StudyOid_) references Study_ ;
alter table DictionaryEntries_  add constraint FK2EEA2795EFE4C811 foreign key (EntryOid_) references DicomDictionaryContainer_ ;
alter table SopClasses_  add constraint FKE7F58195CAA8D405 foreign key (ParcelOid_) references SendParcel_ ;
