#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Imex
{
    /// <summary>
    /// Defines the class of data that an extension of <see cref="XmlDataImexExtensionPoint"/> is
    /// responsible for.
    /// </summary>
    /// <remarks>
    /// The data-class is a logical class name and need not refer to an actual entity class.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class ImexDataClassAttribute : Attribute
    {
        private readonly string _dataClass;
        private int _itemsPerFile;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="dataClass"></param>
        public ImexDataClassAttribute(string dataClass)
        {
            _dataClass = dataClass;
        }

        /// <summary>
        /// Gets the name of the data class.
        /// </summary>
        public string DataClass
        {
            get { return _dataClass; }
        }

        /// <summary>
        /// Gets or sets the default number of items to export per file.
        /// </summary>
        public int ItemsPerFile
        {
            get { return _itemsPerFile; }
            set { _itemsPerFile = value; }
        }
    }
}
