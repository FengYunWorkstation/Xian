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

// This file is auto-generated by the ClearCanvas.Model.SqlServer2005.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model
{
    using System;
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;
    using ClearCanvas.ImageServer.Model.EntityBrokers;

    [Serializable]
    public partial class Filesystem: ServerEntity
    {
        #region Constructors
        public Filesystem():base("Filesystem")
        {}
        #endregion

        #region Private Members
        private System.String _description;
        private System.Boolean _enabled;
        private System.String _filesystemPath;
        private FilesystemTierEnum _filesystemTierEnum;
        private System.Decimal _highWatermark;
        private System.Decimal _lowWatermark;
        private System.Boolean _readOnly;
        private System.Boolean _writeOnly;
        #endregion

        #region Public Properties
        public System.String Description
        {
        get { return _description; }
        set { _description = value; }
        }
        public System.Boolean Enabled
        {
        get { return _enabled; }
        set { _enabled = value; }
        }
        public System.String FilesystemPath
        {
        get { return _filesystemPath; }
        set { _filesystemPath = value; }
        }
        public FilesystemTierEnum FilesystemTierEnum
        {
        get { return _filesystemTierEnum; }
        set { _filesystemTierEnum = value; }
        }
        public System.Decimal HighWatermark
        {
        get { return _highWatermark; }
        set { _highWatermark = value; }
        }
        public System.Decimal LowWatermark
        {
        get { return _lowWatermark; }
        set { _lowWatermark = value; }
        }
        public System.Boolean ReadOnly
        {
        get { return _readOnly; }
        set { _readOnly = value; }
        }
        public System.Boolean WriteOnly
        {
        get { return _writeOnly; }
        set { _writeOnly = value; }
        }
        #endregion

        #region Static Methods
        static public Filesystem Load(ServerEntityKey key)
        {
            using (IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                return Load(read, key);
            }
        }
        static public Filesystem Load(IReadContext read, ServerEntityKey key)
        {
            IFilesystemEntityBroker broker = read.GetBroker<IFilesystemEntityBroker>();
            Filesystem theObject = broker.Load(key);
            return theObject;
        }
        #endregion
    }
}
