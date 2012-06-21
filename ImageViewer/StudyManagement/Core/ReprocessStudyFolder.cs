﻿#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core
{
    /// <summary>
    /// Class for reprocessing a Study on disk.  Primarily used by <see cref="ReindexUtility"/>.
    /// </summary>
    internal class ReprocessStudyFolder
    {
        #region Private Members

        // TODO (CR Jun 2012 - Med): Should be volatile because it's modified on a different thread than it's read on.
        private bool _cancelRequested;
        
        #endregion

        #region Public Properties

        public StudyLocation Location { get; private set; }
        public bool StudyStoredInDatabase { get; private set; }
        public bool Failed { get; private set; }
        public string FailureMessage { get; private set; }
        #endregion

        #region Constructors

        public ReprocessStudyFolder(StudyLocation location )
        {
            Location = location;
            StudyStoredInDatabase = CheckIfStudyExists();
        }

        #endregion

        #region Public Methods

        public void Process()
        {
            if (!StudyStoredInDatabase)
                ReprocessFolder();
            else
                RebuildStudyXml();
        }

        public void Cancel()
        {
            _cancelRequested = true;
        }

        #endregion

        #region Private Methods

        private void ReprocessFolder()
        {
            try
            {                        
                var studyXml = Location.LoadStudyXml();
                var fileList = new List<ProcessStudyUtility.ProcessorFile>();

                // This code will cleanup a folder and move images around to the proper location.
                // It in essence allows you to just copy a bunch of files into the filestore, and reindex will clean them up and organize them.
                FileProcessor.Process(Location.StudyFolder, "*.dcm", delegate(string file, out bool cancel)
                                                           {
                                                               cancel = _cancelRequested;
                                                               try
                                                               {
                                                                   var dicomFile = new DicomFile(file);

                                                                   dicomFile.Load(DicomReadOptions.StorePixelDataReferences | DicomReadOptions.Default);
                                                                   String studyInstanceUid = dicomFile.DataSet[DicomTags.StudyInstanceUid].GetString(0, string.Empty);

                                                                   if (!Location.Study.StudyInstanceUid.Equals(studyInstanceUid))
                                                                   {
                                                                       Platform.Log(LogLevel.Warn,
                                                                                    "Importing file that was in the wrong study folder: {0}",
                                                                                    file);
                                                                       var context =
                                                                           new ImportStudyContext(
                                                                               dicomFile.SourceApplicationEntityTitle, StudyStore.GetConfiguration());
                                                                       var importer = new ImportFilesUtility(context);
                                                                       var result = importer.Import(dicomFile, BadFileBehaviourEnum.Delete, FileImportBehaviourEnum.Move);
                                                                       if (!result.DicomStatus.Equals(DicomStatuses.Success))
                                                                       {
                                                                           try
                                                                           {
                                                                               Platform.Log(LogLevel.Error, "Unable to import file: {0}, deleting: {1}", result.ErrorMessage, file);
                                                                               FileUtils.Delete(file);
                                                                           }
                                                                           catch (Exception x)
                                                                           {
                                                                               Platform.Log(LogLevel.Warn, x, "Unexpected exception deleting file: {0}", file);
                                                                               Failed = true;
                                                                               FailureMessage = x.Message;
                                                                           }
                                                                       }
                                                                   }
                                                                   else
                                                                   {
                                                                       fileList.Add(new ProcessStudyUtility.ProcessorFile(dicomFile, null));

                                                                       if (fileList.Count > 19)
                                                                       {
                                                                           var p = new ProcessStudyUtility(Location){IsReprocess = true};

                                                                           p.ProcessBatch(fileList, studyXml);

                                                                           fileList.Clear();
                                                                       }
                                                                   }
                                                               }
                                                               catch (Exception x)
                                                               {
                                                                   Platform.Log(LogLevel.Error, "Exception when reindexing {0} files, last file: {1}: {2}", fileList.Count, file, x.Message);
                                                                   fileList.Clear(); // Clear out the failed entries
                                                                   Failed = true;
                                                                   FailureMessage = x.Message;
                                                               }
                                                           }, true);
                if (fileList.Count > 0)
                {
                    var p = new ProcessStudyUtility(Location) {IsReprocess = true};

                    p.ProcessBatch(fileList, studyXml);

                    // Now apply Deletion rules              

                    var ruleContext = new RulesEngineOptions
                                          {
                                              ApplyDeleteActions = true,
                                              ApplyRouteActions = false
                                          };
					RulesEngine.Create().ApplyStudyRules(p.StudyLocation.Study.ToStoreEntry(), ruleContext);
                }
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x, "Unexpected exception reindexing folder: {0}", Location.StudyFolder);
                Failed = true;
                FailureMessage = x.Message;
            }

            if (_cancelRequested)
                Platform.Log(LogLevel.Info, "Cancel requested while reprocessing folder: {0}", Location.StudyFolder);
            else
                Platform.Log(LogLevel.Info, "Completed reprocessing study folder: {0}", Location.Study.StudyInstanceUid);
        }    

        private void RebuildStudyXml()
        {         
            try
            {                        
                var studyXml = new StudyXml(Location.Study.StudyInstanceUid);

                DicomFile lastFile = null;
                FileProcessor.Process(Location.StudyFolder, "*.dcm", delegate(string file, out bool cancel)
                                                           {
                                                               cancel = _cancelRequested;
                                                               try
                                                               {
                                                                   lastFile = new DicomFile(file);
                                                                   lastFile.Load(DicomReadOptions.Default |
                                                                                  DicomReadOptions.StorePixelDataReferences);
                                                                   if (Path.GetFileNameWithoutExtension(file) == lastFile.MediaStorageSopInstanceUid)
                                                                   {
                                                                       if (!studyXml.AddFile(lastFile))
                                                                       {
                                                                           Platform.Log(LogLevel.Warn,
                                                                                        "Importing file that was in the wrong study folder: {0}",
                                                                                        file);
                                                                           var context =
                                                                               new ImportStudyContext(
                                                                                   lastFile.SourceApplicationEntityTitle,
                                                                                   StudyStore.GetConfiguration());
                                                                           var importer = new ImportFilesUtility(context);
                                                                           var result = importer.Import(lastFile,
                                                                                                        BadFileBehaviourEnum.Delete,
                                                                                                        FileImportBehaviourEnum.Move);
                                                                           if (result.DicomStatus != DicomStatuses.Success)
                                                                           {
                                                                               Platform.Log(LogLevel.Error,
                                                                                            "Unable to import file: {0}",
                                                                                            result.ErrorMessage);
                                                                               Failed = true;
                                                                               FailureMessage = result.ErrorMessage;
                                                                           }
                                                                       }
                                                                   }
                                                                   else
                                                                   {
                                                                       Platform.Log(LogLevel.Info, "Ignoring duplicate file: {0}", file);
                                                                   }
                                                               }
                                                               catch (Exception x)
                                                               {
                                                                   Platform.Log(LogLevel.Error, x,
                                                                                "Failed to load file for reprocessing: {0}", file);
                                                                   Failed = true;
                                                                   FailureMessage = x.Message;
                                                               }

                                                           }, false);

                // This saves the study Xml to disk, and ensures the database is updated and the study is not marked as "deleted".
                // If a cancel was requested, don't save the file, and it will remain "as is"
                if (lastFile !=null && !_cancelRequested)
                {
                    var p = new ProcessStudyUtility(Location) { IsReprocess = true };

                    p.ProcessFile(lastFile, studyXml, null);
                }
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x, "Unexpected exception reindexing folder: {0}", Location.StudyFolder);
                Failed = true;
                FailureMessage = x.Message;
            }

            if (_cancelRequested)
                Platform.Log(LogLevel.Info, "Cancel requested while rebuilding Study XML in folder: {0}", Location.StudyFolder);
            else
                Platform.Log(LogLevel.Info, "Rebuilt Study XML for study: {0}", Location.Study.StudyInstanceUid);
        }

        private bool CheckIfStudyExists()
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetStudyBroker();
                var study = broker.GetStudy(Location.Study.StudyInstanceUid);

                if (study != null)
                {
                    Location.Study = study;
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}
