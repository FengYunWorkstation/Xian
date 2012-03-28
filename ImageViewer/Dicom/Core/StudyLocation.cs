﻿#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Dicom.Core
{
    /// <summary>
    /// Class and utilities for finding the directory where a study is stored.
    /// </summary>
    public class StudyLocation
    {
        #region Constructors

        public StudyLocation(string studyInstanceUid)
        {
            Study = new StudyIdentifier
            {
                StudyInstanceUid = studyInstanceUid
            };

            StudyFolder = Path.Combine(GetFileStoreDirectory(), studyInstanceUid);
        }

        public StudyLocation(DicomMessageBase message)
        {
            Study = new StudyIdentifier(message.DataSet);

            StudyFolder = Path.Combine(GetFileStoreDirectory(), Study.StudyInstanceUid);
        }

        #endregion

        #region Public Properties

        public string StudyFolder { get; private set; }

        public StudyIdentifier Study { get; set; }

        #endregion

        #region Public Methods

        public string GetSopInstancePath(string seriesInstanceUid, string sopInstanceUid)
        {
            return Path.Combine(StudyFolder, 
                string.Format("{0}.{1}", sopInstanceUid, "dcm"));
        }

        #endregion

        private static string GetFileStoreDirectory()
        {
            string directory = null;
            Platform.GetService<IDicomServerConfiguration>(
                s => directory = s.GetConfiguration(new GetDicomServerConfigurationRequest()).Configuration.FileStoreDirectory);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }
    }
}