#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Data
{
    /// <summary>
    /// Represents a DICOM UID map 
    /// </summary>
    public class Map
    {
        /// <summary>
        /// The original DICOM UID
        /// </summary>
        [XmlAttribute]
        public string Source;


        /// <summary>
        /// The new DICOM UID
        /// </summary>
        [XmlAttribute]
        public string Target;
    }

    public class StudyUidMap
    {
        /// <summary>
        /// The original DICOM UID
        /// </summary>
        [XmlAttribute]
        public string Source;


        /// <summary>
        /// The new DICOM UID
        /// </summary>
        [XmlAttribute]
        public string Target;


        public List<Map> Series { get; set; }
        public List<Map> Instances { get; set; }
    }

    /// <summary>
    /// Represents the Uid Map Xml used for mapping series/instance from one study to another
    /// during reconciliation.
    /// </summary>
    public class UidMapXml
    {
        [XmlArray(ElementName = "StudyUidMaps")]
        [XmlArrayItem(ElementName="Study")]
        public List<StudyUidMap> StudyUidMaps { get; set; }

        public UidMapXml()
        {
            StudyUidMaps = new List<StudyUidMap>();
        }

        /// <summary>
        /// Loads the <see cref="Series"/> and instance mappings for the specified study.
        /// </summary>
        /// <param name="location"></param>
        public void Load(StudyStorageLocation location)
        {
            Load(Path.Combine(location.GetStudyPath(), "UidMap.xml"));
        }

        /// <summary>
        /// Loads the <see cref="Series"/> and instance mappings from the specified file.
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            if (File.Exists(path))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                UidMapXml copy = XmlUtils.Deserialize<UidMapXml>(doc);
                StudyUidMaps = copy.StudyUidMaps;
            }
        }
    }
}