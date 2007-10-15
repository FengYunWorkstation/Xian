#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model.Brokers;

namespace ClearCanvas.ImageServer.Model
{
    public class StatusEnum : ServerEnum
    {
        private static Dictionary<short, StatusEnum> _dict = new Dictionary<short, StatusEnum>();

        /// <summary>
        /// One-time load of status values from the database.
        /// </summary>
        static StatusEnum()
        {
            IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
            IEnumBroker<StatusEnum> broker = read.GetBroker<IStatusEnum>();
            IList<StatusEnum> list = broker.Execute();
            read.Dispose();

            foreach (StatusEnum type in list)
            {
                _dict.Add(type.Enum, type);
            }
        }

        #region Constructors
        public StatusEnum()
            : base("StatusEnum")
        {
        }
        #endregion

        public override void SetEnum(short val)
        {
            StatusEnum statusEnum;
            if (false == _dict.TryGetValue(val, out statusEnum))
                throw new PersistenceException("Unknown TypeEnum value: " + val, null);

            Enum = statusEnum.Enum;
            Lookup = statusEnum.Lookup;
            Description = statusEnum.Description;
            LongDescription = statusEnum.LongDescription;
        }

        public static StatusEnum GetEnum(string lookup)
        {
            foreach (StatusEnum status in _dict.Values)
            {
                if (status.Lookup.Equals(lookup))
                    return status;
            }
            throw new PersistenceException("Unknown StatusEnum: " + lookup, null);
        }
    }
}
