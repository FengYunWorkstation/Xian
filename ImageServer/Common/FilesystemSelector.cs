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
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Class used for incoming studies to select which filesystem the study should be 
    /// stored to.
    /// </summary>
    public class FilesystemSelector
    {
        private readonly FilesystemMonitor _monitor;

        public FilesystemSelector(FilesystemMonitor monitor)
        {
            _monitor = monitor;    
        }

        public Filesystem SelectFilesystem(DicomMessageBase msg)
        {
            ServerFilesystemInfo selectedFilesystem = null;
            float selectedFreeBytes = 0;

            List<ServerFilesystemInfo> list = new List<ServerFilesystemInfo>();
            list.AddRange(_monitor.GetFilesystems());

            list.Sort(delegate(ServerFilesystemInfo fs1, ServerFilesystemInfo fs2)
                           {
                               if (fs1.Filesystem.FilesystemTierEnum.Enum.Equals(fs2.Filesystem.FilesystemTierEnum.Enum))
                               {
                                   return fs1.FreeBytes.CompareTo(fs2.FreeBytes);
                               }
                               else
                               {
                                   return fs1.Filesystem.FilesystemTierEnum.Enum.CompareTo(fs2.Filesystem.FilesystemTierEnum.Enum);
                               }

                           }
            );


            // find the first filesystem that can be written to
            foreach (ServerFilesystemInfo info in list)
            {
                if (info.Online && info.Filesystem.Enabled && !info.Filesystem.ReadOnly)
                {
                    selectedFilesystem = info;
                    break;
                }
            }

            if (selectedFilesystem == null)
                return null;

            if (selectedFilesystem.FreeBytes < 1024.0 * 1024.0 * 1024.0)
                return null;

            return selectedFilesystem.Filesystem;
        }
    }
}
