﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Common
{
    /// <summary>
    /// Abstract base for a generic collection of object which can be indexed based on a key.
    /// </summary>
    public abstract class KeyedCollectionBase<TData, TKey> : Dictionary<TKey, TData>
    {
        public KeyedCollectionBase()
        {
        }


        public KeyedCollectionBase(IList<TData> list)
        {
            foreach (TData item in list)
            {
                Add(GetKey(item), item);
            }
        }


        protected abstract TKey GetKey(TData item);

        public TData this[int index]
        {
            get
            {
                List<TData> list = new List<TData>(this.Values);
                return list[index];
            }
        }

        public void Add(IList<TData> items)
        {
            foreach (TData item in items)
                Add(item);
        }

        public void Add(TData item)
        {
            TKey key = GetKey(item);

            if (IndexOf(key) > 0)
            {
                Exception e = new Exception(string.Format("Key {0} already exists in list.\n\nItem: {1}\n\nList: {2}", key, item, this));
                Platform.Log(LogLevel.Error, e.Message);
                throw e;
            }
            else
            {
                Add(key, item);
            }
        }

        public int IndexOf(TKey key)
        {
            List<TKey> list = new List<TKey>(Keys);
            return list.IndexOf(key);
        }

        public int IndexOf(TData item)
        {
            List<TData> list = new List<TData>(this.Values);
            return list.IndexOf(item);
        }

        /// <summary>
        /// Return the row index of the item if rendered in the specified grid
        /// </summary>
        /// <param name="key"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public int RowIndexOf(TKey key, GridView grid)
        {
            int index = IndexOf(key);

            if (!grid.AllowPaging)
            {
                return index;
            }
            else
            {
                int curPageMinIndex = grid.PageSize * grid.PageIndex;
                int curPageMaxIndex = curPageMinIndex + grid.PageSize;
                if (index < curPageMinIndex || index > curPageMaxIndex)
                    return -1;
                else
                    return index % grid.PageSize;
            }

        }

        /// <summary>
        /// Return the row index of the item if rendered in the specified grid
        /// </summary>
        /// <param name="item"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        public int RowIndexOf(TData item, GridView grid)
        {
            return RowIndexOf(GetKey(item), grid);
        }
    }
}