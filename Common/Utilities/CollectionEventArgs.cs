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

using System;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Event used to notify observers of a change in a collection.
	/// </summary>
	/// <remarks>
	/// This class is used internally by the <see cref="ObservableList{TItem}"/>, but can be used
	/// for any collection-related event.
	/// </remarks>
	/// <typeparam name="TItem">The type of item in the collection.</typeparam>
	public class CollectionEventArgs<TItem> : EventArgs
	{
		private TItem _item;
		private int _index;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal protected CollectionEventArgs()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item">The item that has changed.</param>
		/// <param name="index">The index of the item that has changed.</param>
		public CollectionEventArgs(TItem item, int index)
		{
			_item = item;
			_index = index;
		}

		/// <summary>
		/// Gets the item that has somehow changed in the related collection.
		/// </summary>
		public TItem Item
		{
			get { return _item; }
			internal protected set { _item = value; }
		}

		/// <summary>
		/// Gets the index of the item that has somehow changed in the related collection.
		/// </summary>
		public int Index
		{
			get { return _index; }
			internal protected set { _index = value; }
		}
	}
}
