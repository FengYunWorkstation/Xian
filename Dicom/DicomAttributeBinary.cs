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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ClearCanvas.Dicom.IO;

namespace ClearCanvas.Dicom
{

    #region DicomAttributeBinary<T>
    /// <summary>
    /// <see cref="DicomAttribute"/> derived class used to represent tags with binary values.
    /// </summary>
    /// <typeparam name="T">The type that the attribute is storing.</typeparam>

    public abstract class DicomAttributeBinary<T> : DicomAttribute
    {
        protected T[] _values = new T[0];
        protected NumberStyles _numberStyle = NumberStyles.Any;

        #region Constructors
        internal DicomAttributeBinary(uint tag)
            : base(tag)
        {
        }

        internal DicomAttributeBinary(DicomTag tag)
            : base(tag)
        {
        }

        internal DicomAttributeBinary(DicomTag tag, ByteBuffer item)
            : base(tag)
        {
            if (ByteBuffer.LocalMachineEndian != item.Endian)
                item.Swap(tag.VR.UnitSize);
            _values = new T[item.Length / tag.VR.UnitSize];
            Buffer.BlockCopy(item.ToBytes(), 0, _values, 0, _values.Length * tag.VR.UnitSize);

            SetStreamLength();
        }

        internal DicomAttributeBinary(DicomAttributeBinary<T> attrib)
            : base(attrib)
        {
            T[] values = (T[])attrib.Values;

            _values = new T[values.Length];

            values.CopyTo(_values, 0);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The number style for the attribute.
        /// </summary>
        public NumberStyles NumberStyle
        {
            get { return _numberStyle; }
            set { _numberStyle = value; }
        }

        public T this[int index]
        {
            get
            {
                return _values[index];
            }
        }

        #endregion

        #region private Methods

        
        protected virtual void SetStreamLength()
        {
            if (_values == null)
            {
                Count = 0;
                StreamLength = 0;
            }
            else
            {
                Count = _values.Length;
                StreamLength = (uint) (_values.Length*Tag.VR.UnitSize);
            }
        }

        protected void AppendValue(T val)
        {
            T[] temp = new T[Count + 1];
            if (_values != null && _values.Length > 0)
                Array.Copy(_values, temp, Count);

            temp[Count++] = val;
            _values = temp;
            SetStreamLength();
        }

        #endregion

        #region Abstract Methods

        protected abstract T ParseNumber(string val);


        public override void SetNullValue()
        {
            _values = new T[0];
            SetStreamLength();
        }

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet)
        {
            int len = _values.Length * Tag.VR.UnitSize;
            byte[] byteVal = new byte[len];

            Buffer.BlockCopy(_values, 0, byteVal, 0, len);

            ByteBuffer bb = new ByteBuffer(byteVal, syntax.Endian);
            if (syntax.Endian != ByteBuffer.LocalMachineEndian)
            {
                bb.Swap(Tag.VR.UnitSize);
            }

            return bb;
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            DicomAttributeBinary<T> a = (DicomAttributeBinary<T>)obj;
            T[] array = (T[])a.Values;

            if (Count != a.Count)
                return false;
            if (Count == 0 && a.Count == 0)
                return true;
            if (array.Length != _values.Length)
                return false;

            for (int index = 0; index < a.Count; index++)
                if (!array[index].Equals(_values[index]))
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            if (_values == null)
                return 0; // TODO
            else
            {
                int hash = 0;
                for (int index = 0; index < _values.Length; index++)
                {
                    hash += (index + 1) * _values[index].GetHashCode();
                }
                return hash;
            }
        }

        /// <summary>
        /// The type that the attribute stores.
        /// </summary>
        /// <returns></returns>
        public override Type GetValueType()
        {
            return typeof(T);
        }


        /// <summary>
        /// Retrieves a value as a string.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGetString(int index, out String value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = "";
                return false;
            }

            value = _values[index].ToString();

            return true;
        }

        /// <summary>
        /// Sets the tag value(s) from a string
        /// If the string cannot be converted into tag's VR, DicomDataException will be thrown
        /// </summary>
        /// <param name="stringValue"></param>
        public override void SetStringValue(String stringValue)
        {
            if (stringValue == null || stringValue=="")
            {
                _values = new T[0];
                SetStreamLength();
                return;
            }

            String[] stringValues = stringValue.Split(new char[] { '\\' });
            _values = new T[stringValues.Length];
            for (int index = 0; index < stringValues.Length; index++)
            {
                _values[index] = ParseNumber(stringValues[index]);
            }

            SetStreamLength();
        }

        
        /// <summary>
        /// Sets the value from a string
        /// If the string cannot be converted into tag's VR, DicomDataException will be thrown
        /// If <paramref name="index"/> equals to <seealso cref="Count"/>, this method behaves the same as <seealso cref="AppendString"/>.
        /// If <paramref name="index"/> is less than 0 or greater than <see cref="Count"/>, IndexOutofBoundException will be thrown.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public override void SetString(int index, string value)
        {
            if (index == Count)
                AppendString(value);
            else
            {
                _values[index] = ParseNumber(value);
                SetStreamLength();
            }

        }

        /// <summary>
        /// Appends an element from a string
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be converted into tag VR</exception>
        public override void AppendString(string value)
        {
            T v = ParseNumber(value);

            AppendValue(v);
        }

        /// <summary>
        /// Sets an attribute value
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> exceeds the range of the VR or cannot convert into the VR</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public void SetValue(int index, T value)
        {
            if (index == Count)
                AppendValue(value);
            else
            {
                _values[index] = value;
                SetStreamLength();
            }
            
        }


        public override string ToString()
        {
            if (_values == null)
                return "";

            StringBuilder val = null;
            foreach (T index in _values)
            {
                if (val == null)
                    val = new StringBuilder(index.ToString());
                else
                    val.AppendFormat("\\{0}", index);
            }

            if (val == null)
                return "";

            return val.ToString();
        }

        public override bool IsNull
        {
            get
            {
                if ((_values != null) && (_values.Length == 0))
                    return true;
                return false;
            }
        }
        public override bool IsEmpty
        {
            get
            {
                if ((Count == 0) && (_values == null))
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Abstract property for setting or getting the values associated with the attribute.
        /// </summary>
        public abstract override Object Values { get; set; }
        public abstract override DicomAttribute Copy();
        internal abstract override DicomAttribute Copy(bool copyBinary);

        #endregion

    }
    #endregion

    #region DicomAttributeAT
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing AT value representation tags.
    /// </summary>
    /// 
    public class DicomAttributeAT : DicomAttributeBinary<uint>
    {

        #region Constructors

        public DicomAttributeAT(uint tag)
            : base(tag)
        {
        }

        public DicomAttributeAT(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.ATvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeAT(DicomTag tag, ByteBuffer item)
            : base(tag)
        {
            if (ByteBuffer.LocalMachineEndian != item.Endian)
                item.Swap(tag.VR.UnitSize/2);

         
            _values = new uint[item.Length / tag.VR.UnitSize];
            for (int i = 0; i < _values.Length; i++)
            {
                Buffer.BlockCopy(item.ToBytes(), i * tag.VR.UnitSize, _values, i * tag.VR.UnitSize+2, 2);
                Buffer.BlockCopy(item.ToBytes(), i * tag.VR.UnitSize + 2, _values, i * tag.VR.UnitSize, 2);
            }

            SetStreamLength();
        }

        internal DicomAttributeAT(DicomAttributeAT attrib)
            : base(attrib)
        {
        }


        #endregion


        #region Operators

        

        #endregion


        #region Abstract Method Implementation

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet)
        {
            int len = _values.Length * Tag.VR.UnitSize;
            byte[] byteVal = new byte[len];

            for (int i = 0; i < _values.Length; i++)
            {
                Buffer.BlockCopy(_values, i * Tag.VR.UnitSize + 2, byteVal, i * Tag.VR.UnitSize, 2);
                Buffer.BlockCopy(_values, i * Tag.VR.UnitSize, byteVal, i * Tag.VR.UnitSize + 2, 2);
            }

            ByteBuffer bb = new ByteBuffer(byteVal, syntax.Endian);
            if (syntax.Endian != ByteBuffer.LocalMachineEndian)
            {
                bb.Swap(Tag.VR.UnitSize);
            }

            return bb;
        }

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is uint[])
                {
                    _values = (uint[])value;
                    SetStreamLength();
                }
                else if (value is String)
                {
                    SetStringValue((String)value);
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeAT(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeAT(this);
        }

        protected override uint ParseNumber(string val)
        {
            if (val == null)
                throw new DicomDataException("Null values invalid for AT VR");

            uint parseVal;
            if (false == uint.TryParse(val.Trim(), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out parseVal))
                throw new DicomDataException(
                    String.Format("Invalid uint format value for tag {0}: {1}", Tag, val));
            return parseVal;
        }

        public override string ToString()
        {
            if (_values == null)
                return "";

            StringBuilder val = null;
            foreach (uint index in _values)
            {
                if (val == null)
                    val = new StringBuilder(String.Format("{0:X}", index));
                else
                    val.AppendFormat("\\{0:X}", index);
            }

            if (val == null)
                return "";

            return val.ToString();
        }

        /// <summary>
        /// Retrieves an Int16 value from an AT attribute.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>If the value doesn't exist</item>
        /// <item>The value exceeds Int16 range.</item>
        /// </list>
        ///     
        /// If the method returns false, the returned <paramref name="value"/> is not reliable..
        /// </remarks>
        /// 
        public override bool TryGetInt16(int index, out Int16 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }


            value = (Int16)_values[index];

            // If the value cannot fit into the destination, return false
            if (_values[index] > Int16.MaxValue)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Retrieves an Int32 value from an AT attribute.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item>If the value doesn't exist</item>
        /// <item>The value cannot be converted into Int32</item>
        /// <item>The value is an integer but too big or too small to fit into an Int32</item>
        /// </list>
        /// If the method returns false, the returned <paramref name="index"/> is not reliable..
        /// </remarks>
        public override bool TryGetInt32(int index, out Int32 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }


            value = (Int32)_values[index];

            // If the value cannot fit into the destination, return false
            if (_values[index] > Int32.MaxValue)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Retrieves an Int64 value from an AT attribute.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>If the value doesn't exist</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable..
        /// </remarks>
        public override bool TryGetInt64(int index, out Int64 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = _values[index];
            return true;
        }

        /// <summary>
        /// Retrieves an UInt16 value from an AT attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist</item>
        /// <item>The value exceeds UInt16 range.</item>
        /// </list>
        ///     
        /// If the method returns false, the returned <paramref name="value"/> is not reliable..
        /// </remarks>
        public override bool TryGetUInt16(int index, out UInt16 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = (UInt16)_values[index];

            // If the value cannot fit into the destination, return false
            if (_values[index] < UInt16.MinValue || _values[index] > UInt16.MaxValue)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Retrieves an UInt32 value from an AT attribute.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list>
        /// <item>If the value doesn't exist.</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable..
        /// </remarks>
        public override bool TryGetUInt32(int index, out UInt32 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = _values[index];
            return true;
        }
        /// <summary>
        /// Retrieves an UInt64 value from an AT attribute.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list>
        /// <item>If the value doesn't exist.</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable..
        /// </remarks>
        /// 
        public override bool TryGetUInt64(int index, out UInt64 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = _values[index];
            return true;
        }

        /// <summary>
        /// Retrieves the string representation of an AT value in hexadecimal format.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TryGetString(int index, out string value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = String.Empty;
                return false;
            }

            value = _values[index].ToString("X8"); // Convert to HEX
            return true;
        }

        /// <summary>
        /// Sets an AT value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        public override void SetUInt16(int index, UInt16 value)
        {
            // If the source value cannot fit into the destination throw exception
            if (value < UInt32.MinValue)
                throw new DicomDataException(String.Format("Invalid AT value {0} for tag {1}.", value, Tag));

            SetValue(index, value);
        }

        /// <summary>
        /// Sets an AT value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        public override void SetUInt32(int index, UInt32 value)
        {
            SetValue(index, value);
        }

        /// <summary>
        /// Sets an AT value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        public override void SetUInt64(int index, UInt64 value)
        {
            // If the source value cannot fit into the destination throw exception
            if (value < UInt32.MinValue || value > UInt32.MaxValue)
                throw new DicomDataException(String.Format("Invalid AT value {0} for tag {1}.", value, Tag));

            SetValue(index,(uint) value);
        }

        /// <summary>
        /// Sets an AT value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        public override void SetInt16(int index, Int16 value)
        {
            // If the source value cannot fit into the destination throw exception
            if (value < UInt32.MinValue)
                throw new DicomDataException(String.Format("Invalid AT value {0} for tag {1}.", value, Tag));

            SetValue(index, (uint) value);
        }

        /// <summary>
        /// Sets an AT value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        public override void SetInt32(int index, Int32 value)
        {
            // If the source value cannot fit into the destination throw exception
            if (value < UInt32.MinValue)
                throw new DicomDataException(String.Format("Invalid AT value {0} for tag {1}.", value, Tag));

            SetValue(index,  (uint) value);
        }
        /// <summary>
        /// Sets an AT value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        public override void SetInt64(int index, Int64 value)
        {
            // If the source value cannot fit into the destination throw exception
            if (value < UInt32.MinValue || value > UInt32.MaxValue)
                throw new DicomDataException(String.Format("Invalid AT value {0} for tag {1}.", value, Tag));

            SetValue(index, (uint) value);
        }

        /// <summary>
        /// Appends an AT value.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        public override void AppendUInt16(UInt16 value)
        {
            AppendValue(value);
        }
        /// <summary>
        /// Appends an AT value.
        /// </summary>
        /// <param name="value"></param>
        public override void AppendUInt32(UInt32 value)
        {
            // If the source value cannot fit into the destination throw exception
            if (value < uint.MinValue || value > uint.MaxValue)
                throw new DicomDataException(String.Format("Invalid AT value {0} for tag {1}.", value, Tag));

            AppendValue(value);
        }

        /// <summary>
        /// Appends an AT value.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        public override void AppendUInt64(UInt64 value)
        {
            // If the source value cannot fit into the destination throw exception
            if (value < UInt32.MinValue || value > UInt32.MaxValue)
                throw new DicomDataException(String.Format("Invalid AT value {0} for tag {1}.", value, Tag));

            AppendValue((UInt32)value);
        }

        /// <summary>
        /// Appends an AT value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        public override void AppendInt16(Int16 value)
        {
            if (value < UInt32.MinValue)
                throw new DicomDataException(String.Format("Invalid AT value {0} for tag {1}.", value, Tag));
 
            AppendValue((UInt32)value);
        }

        /// <summary>
        /// Appends an AT value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        public override void AppendInt32(Int32 value)
        {
            // If the source value cannot fit into the destination throw exception
            if (value < UInt32.MinValue)
                throw new DicomDataException(String.Format("Invalid value {0} for tag {1}.", value, Tag));

            AppendValue((UInt32)value);
        }

        /// <summary>
        /// Appends an AT value.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        public override void AppendInt64(Int64 value)
        {
            // If the source value cannot fit into the destination throw exception
            if (value < UInt32.MinValue || value > UInt32.MaxValue)
                throw new DicomDataException(String.Format("Invalid value {0} for tag {1}.", value, Tag));
            AppendValue((UInt32)value);
        }

        #endregion

    }
    #endregion

    #region DicomAttributeFD
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing FD value representation tags.
    /// </summary>
    public class DicomAttributeFD : DicomAttributeBinary<double>
    {
        #region Constructors

        public DicomAttributeFD(uint tag)
            : base(tag)
        {
        }

        public DicomAttributeFD(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.FDvr)
                && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeFD(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeFD(DicomAttributeFD attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Operators


        #endregion

        #region Abstract Method Implementation
        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is double[])
                {
                    _values = (double[])value;
                    SetStreamLength();
                }
                else if (value is String)
                {
                    SetStringValue((String)value);
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeFD(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeFD(this);
        }

        protected override double ParseNumber(string val)
        {
            if (val == null)
                throw new DicomDataException("Null values invalid for FD VR");

            double parseVal;
            if (false == double.TryParse(val.Trim(), NumberStyle, null, out parseVal))
                throw new DicomDataException(
                    String.Format("Invalid double format value for tag {0}: {1}", Tag, val));
            return parseVal;
        }

        

        /// <summary>
        /// Sets an FD value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetFloat32(int index, float value)
        {
            SetValue(index, value);

        }

        /// <summary>
        /// Sets an FD value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> is null or  cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetFloat64(int index, double value)
        {
            SetValue(index, value);
        }
        /// <summary>
        /// Sets an FD value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> is null or  cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void AppendFloat32(float value)
        {
            AppendValue(value);
        }
        /// <summary>
        /// Sets an FD value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> is null or  cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void AppendFloat64(double value)
        {
            AppendValue(value);
        }


        #endregion

        /// <summary>
        /// Retrieves a float value from an FD attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>If the value doesn't exist</item>
        /// <item>The value is too big or too small to fit into a float (eg, 1E+100)</item>
        /// </list>
        /// If the method returns <b>false</b>, the returned <paramref name="value"/> is not reliable.
        /// </remarks>
        /// 
        public override bool TryGetFloat32(int index,  out float value)
        {
            if (index < 0 || index >= Count)
            {
                value = 0.0f;
                return false;
            }

            value = (float)_values[index];

            if (_values[index] < float.MinValue || _values[index] > float.MaxValue)
            {
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// Retrieves a double value from an FD attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>If the value doesn't exist</item>
        /// </list>
        ///  
        /// If the method returns <b>false</b>, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetFloat64(int index,  out double value)
        {
            if (index < 0 || index >= Count)
            {
                value = 0.0f;
                return false;
            }

            value = _values[index];

            if (_values[index] < double.MinValue || _values[index] > double.MaxValue)
            {
                return false;
            }
            else
                return true;
        }
    }
    #endregion

    #region DicomAttributeFL
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing FL value representation tags.
    /// </summary>
    public class DicomAttributeFL : DicomAttributeBinary<float>
    {
        #region Constructors

        public DicomAttributeFL(uint tag)
            : base(tag)
        {
        }

        public DicomAttributeFL(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.FLvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeFL(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeFL(DicomAttributeFL attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Operators

        #endregion

        #region Abstract Method Implementation


        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is float[])
                {
                    _values = (float[])value;
                    SetStreamLength();
                }
                else if (value is String)
                {
                    SetStringValue((String)value);
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeFL(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeFL(this);
        }

        protected override float ParseNumber(string val)
        {
            if (val == null)
                throw new DicomDataException("Null values invalid for FL VR");
            float parseVal;
            if (false == float.TryParse(val.Trim(), NumberStyle, null, out parseVal))
                throw new DicomDataException(
                    String.Format("Invalid float format value for tag {0}: {1}", Tag, val));
            return parseVal;
        }



        /// <summary>
        /// Retrieves a float value from an FL attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list>
        /// <item>If the value doesn't exist</item>
        /// <item>The value is infinite</item>
        /// </list>
        ///     
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetFloat32(int index,  out float value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0.0f;
                return false;
            }

            value = _values[index];

            if (float.IsInfinity(value))
                return false;
            else
                return true;
        }
        /// <summary>
        /// Retrieves a double value from an FL attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// <list>
        /// <item>If the value doesn't exist</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetFloat64(int index,  out double value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0.0f;
                return false;
            }

            value = (double)(decimal)_values[index]; // casting to decimal then double seems to prevent precision loss
            if (double.IsInfinity(value))
                return false;
            else
                return true;
        }


        /// <summary>
        /// Sets an FL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetFloat32(int index, float value)
        {
            SetValue(index, value);
        }

        /// <summary>
        /// Sets an FL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetFloat64(int index, double value)
        {
            if (value <float.MinValue || value > float.MaxValue)
                throw new DicomDataException(String.Format("Invalid FL value {0} for tag {1}", value, Tag));

            SetValue(index, (float) value);
        }
        /// <summary>
        /// Appendss an FL value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void AppendFloat32(float value)
        {
            AppendValue(value);
        }

        /// <summary>
        /// Appends an FL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> is null or  cannot be fit into 32-bit floating-point</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void AppendFloat64(double value)
        {
            if (value < float.MinValue || value > float.MaxValue)
                throw new DicomDataException(String.Format("Invalid FL value {0} for tag {1}", value, Tag));

            AppendValue((float)value);
        }



        #endregion
    }
    #endregion

    #region DicomAttributeOB
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing OB value representation tags.
    /// </summary>
    public class DicomAttributeOB : DicomAttributeBinary<byte>
    {
        #region Protected Members
        protected List<uint> _table;
        protected List<ByteBuffer> _fragments = new List<ByteBuffer>();
        #endregion

        #region Constructors

        public DicomAttributeOB(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeOB(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.OBvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeOB(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeOB(DicomAttributeOB attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Abstract Method Implementation


        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is byte[])
                {
                    _values = (byte[])value;
                    SetStreamLength();
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeOB(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeOB(this);
        }

        protected override byte ParseNumber(string val)
        {
            if (val == null)
                throw new DicomDataException("Null values invalid for OB VR");

            byte parseVal;
            if (false == byte.TryParse(val.Trim(), NumberStyle, null, out parseVal))
                throw new DicomDataException(
                    String.Format("Invalid byte format value for tag {0}: {1}", Tag, val));
            return parseVal;
        }

        #endregion

        #region Public Properties
        public bool HasOffsetTable
        {
            get { return _table != null; }
        }

        public ByteBuffer OffsetTableBuffer
        {
            get
            {
                ByteBuffer offsets = new ByteBuffer();
                if (_table != null)
                {
                    foreach (uint offset in _table)
                    {
                        offsets.Writer.Write(offset);
                    }
                }
                return offsets;
            }
        }

        public List<uint> OffsetTable
        {
            get
            {
                if (_table == null)
                    _table = new List<uint>();
                return _table;
            }
        }

        public IList<ByteBuffer> Fragments
        {
            get { return _fragments; }
        }
        #endregion

        #region Public Methods
        public void SetOffsetTable(ByteBuffer table)
        {
            _table = new List<uint>();
            _table.AddRange(table.ToUInt32s());
        }
        public void SetOffsetTable(List<uint> table)
        {
            _table = new List<uint>(table);
        }

        public void AddFragment(ByteBuffer fragment)
        {
            _fragments.Add(fragment);
        }
        #endregion

        internal override uint CalculateWriteLength(TransferSyntax syntax, DicomWriteOptions options)
        {
            uint length = 0;
            length += 4; // element tag
            if (syntax.ExplicitVr)
            {
                length += 2; // vr
                length += 6; // length
            }
            else
            {
                length += 4; // length
            }
            if ((Tag.TagValue == DicomTags.PixelData) && syntax.Encapsulated)
            {
                length += 4 + 4; // offset tag
                if (Flags.IsSet(options, DicomWriteOptions.WriteFragmentOffsetTable) && _table != null)
                    length += (uint)(_table.Count * 4);
                foreach (ByteBuffer bb in _fragments)
                {
                    length += 4; // item tag
                    length += 4; // fragment length
                    length += (uint)bb.Length;
                }
                foreach (ByteBuffer bb in _fragments)
                {
                    length += (uint)bb.Length;
                }
            }
            else
            {
                if (_values != null)
                {
                    length += (uint)_values.Length;
                }
            }
            return length;
        }

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet)
        {
             return new ByteBuffer(_values, ByteBuffer.LocalMachineEndian);
        }

    }
    #endregion

    #region DicomAttributeOF
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing OF value representation tags.
    /// </summary>
    public class DicomAttributeOF : DicomAttributeBinary<float>
    {
        public DicomAttributeOF(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeOF(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.OFvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeOF(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }


        internal DicomAttributeOF(DicomAttributeOF attrib)
            : base(attrib)
        {
        }


        #region Abstract Method Implementation

        public override string ToString()
        {
            return Tag + " of length " + base.StreamLength;
        }

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is float[])
                {
                    _values = (float[])value;
                    SetStreamLength();
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeOF(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeOF(this);
        }

        protected override float ParseNumber(string val)
        {
            if (val == null)
                throw new DicomDataException("Null values invalid for OF VR");

            float parseVal;
            if (false == float.TryParse(val.Trim(), NumberStyle, null, out parseVal))
                throw new DicomDataException(
                    String.Format("Invalid float format value for tag {0}: {1}", Tag, val));
            return parseVal;
        }

        #endregion
    }
    #endregion

    #region DicomAttributeOW
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing OW value representation tags.
    /// </summary>
    public class DicomAttributeOW : DicomAttributeBinary<byte>
    {
        public DicomAttributeOW(uint tag)
            : base(tag)
        {
        }

        public DicomAttributeOW(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.OWvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);
        }

        internal DicomAttributeOW(DicomTag tag, ByteBuffer item)
            : base(tag)
        {

            if (ByteBuffer.LocalMachineEndian != item.Endian)
                item.Swap(tag.VR.UnitSize);

            _values = new byte[item.Length];
            Buffer.BlockCopy(item.ToBytes(), 0, _values, 0, _values.Length);

            SetStreamLength();
        }


        internal DicomAttributeOW(DicomAttributeOW attrib)
            : base(attrib)
        {
        }

        #region Abstract Method Implementation

        protected override void SetStreamLength()
        {
            Count = _values.Length;
            StreamLength = (uint)(_values.Length);
        }

        public override string ToString()
        {
            return Tag + " of length " + base.StreamLength;
        }

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is ushort[])
                {
                    ushort[] vals = (ushort[])value;

                    _values = new byte[vals.Length * Tag.VR.UnitSize];
                    Buffer.BlockCopy(vals, 0, _values, 0, _values.Length);

                    SetStreamLength();
                }
                else if (value is byte[])
                {
                    _values = (byte[])value;

                    SetStreamLength();
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeOW(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeOW(this);
        }

        protected override byte ParseNumber(string val)
        {
            if (val == null)
                throw new DicomDataException("Null values invalid for OW VR");

            byte parseVal;
            if (false == byte.TryParse(val.Trim(), NumberStyle, null, out parseVal))
                throw new DicomDataException(
                    String.Format("Invalid byte format value for tag {0}: {1}", Tag, val));
            return parseVal;
        }

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet)
        {
            int len = _values.Length;
            if (syntax.Endian != ByteBuffer.LocalMachineEndian)
            {
                byte[] byteVal = new byte[len];

                Buffer.BlockCopy(_values, 0, byteVal, 0, len);

                ByteBuffer bb = new ByteBuffer(byteVal, syntax.Endian);
                bb.Swap(Tag.VR.UnitSize);

                return bb;
            }

            return new ByteBuffer(_values, syntax.Endian);
        }

        #endregion


    }
    #endregion

    #region DicomAttributeSL
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing SL value representation tags.
    /// </summary>
    public class DicomAttributeSL : DicomAttributeBinary<int>
    {
        public DicomAttributeSL(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeSL(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.SLvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeSL(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeSL(DicomAttributeSL attrib)
            : base(attrib)
        {
        }

        #region Abstract Method Implementation

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is int[])
                {
                    _values = (int[])value;
                    SetStreamLength();
                }
                else if (value is int)
                {
                    _values = new int[1];
                    _values[0] = (int)value;
                    SetStreamLength();
                }
                else if (value is string)
                {
                    SetStringValue((string)value);
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeSL(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeSL(this);
        }
        protected override int ParseNumber(string val)
        {
            if (val == null)
                throw new DicomDataException("Null values invalid for SL VR");

            int parseVal;
            if (false == int.TryParse(val.Trim(), NumberStyle, null, out parseVal))
                throw new DicomDataException(
                    String.Format("Invalid int format value for tag {0}: {1}", Tag, val));
            return parseVal;
        }



        /// <summary>
        /// Sets an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt16(int index, Int16 value)
        {
            SetValue(index, value);
        }

        /// <summary>
        /// Sets an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt16(int index, UInt16 value)
        {
            SetValue(index, value);
        }

        /// <summary>
        /// Sets an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt32(int index, int value)
        {
            SetValue(index, value);
        }

        /// <summary>
        /// Sets an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt32(int index, UInt32 value)
        {
            if (value > int.MaxValue)
                throw new DicomDataException(String.Format("Invalid SL value '{0}' for {1}.", value, Tag));
            SetValue(index, (int) value);    
        }

        /// <summary>
        /// Sets an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt64(int index, Int64 value)
        {
            if (value < int.MinValue || value > int.MaxValue)
                throw new DicomDataException(String.Format("Invalid SL value '{0}' for {1}.", value, Tag));

            SetValue(index, (int)value);    
        }
        /// <summary>
        /// Sets an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt64(int index, UInt64 value)
        {
            if (value > int.MaxValue)
                throw new DicomDataException(String.Format("Invalid SL value '{0}' for {1}.", value, Tag));

            SetValue(index, (int)value);    
        }

        /// <summary>
        /// Appends an SL value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// 
        public override void AppendInt16(Int16 value)
        {
            AppendValue(value);
        }
        /// <summary>
        /// Appends an SL value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit signed int</exception>
        ///
        public override void AppendInt32(int value)
        {
            AppendValue(value);
        }
        /// <summary>
        /// Appends an SL value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit signed int</exception>
        ///
        public override void AppendInt64(Int64 value)
        {
            if (value < Int32.MinValue || value > Int32.MaxValue)
                throw new DicomDataException(String.Format("Invalid SL value '{0}' tag {1}.", value, Tag));

            AppendValue( (int) value);
        }
        /// <summary>
        /// Appends an SL value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        ///
        public override void AppendUInt16(UInt16 value)
        {
            AppendValue(value);
        }
        /// <summary>
        /// Appends an SL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit signed int</exception>
        ///
        public override void AppendUInt32(UInt32 value)
        {
            if (value > int.MaxValue)
                throw new DicomDataException(String.Format("Invalid SL value '{0}' for {1}.", value, Tag));

            AppendValue( (int) value);
        }

        /// <summary>
        /// Appends an SL value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit signed int</exception>
        ///
        public override void AppendUInt64(UInt64 value)
        {
            if (value > Int32.MaxValue)
                throw new DicomDataException(String.Format("Invalid SL value '{0}' for {1}.", value, Tag));

            AppendValue((int)value);
        }


        /// <summary>
        /// Retrieves an Int16 value from an SL attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int16 (eg, floating-point number 1.102 cannot be converted into Int16)
        ///     The value is an integer but outside the range of type  Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt16(int index,  out Int16 value)
        {
            if (index < 0 || index >= Count)
            {
                value = 0;
                return false;
            }


            if (_values[index] < Int16.MinValue || _values[index] > Int16.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (Int16)_values[index];
                return true;
            }
        }
        /// <summary>
        /// Retrieves an Int32 value from an SL attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into Int32 (eg, floating-point number 1.102 cannot be converted into Int32)
        ///     The value is an integer but outside the range of type  Int16 (eg, 100000)
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt32(int index,  out Int32 value)
        {
            if (index < 0 || index >= Count)
            {
                value = 0;
                return false;
            }

            value = _values[index];


            return true;
        }
        /// <summary>
        /// Retrieves an Int64 value from an SL attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        ///     If the value doesn't exist
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt64(int index,  out Int64 value)
        {
            if (index < 0 || index >= Count)
            {
                value = 0;
                return false;
            }


            value = _values[index];
            return true;
            
        }
        /// <summary>
        /// Retrieves an UInt16 value from an SL attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt16 (eg, floating-point number 1.102 cannot be converted into UInt16)
        ///     The value is an integer but outside the range of type  UInt16 (eg, -100)
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt16(int index,  out UInt16 value)
        {
            if (index < 0 || index >= Count)
            {
                value = 0;
                return false;
            }


            if (_values[index] < UInt16.MinValue || _values[index] > UInt16.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (UInt16)_values[index];
                return true;
            }
        }
        /// <summary>
        /// Retrieves an UInt32 value from an SL attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt32 (eg, floating-point number 1.102 cannot be converted into UInt32)
        ///     The value is an integer but outside the range of type  UInt32 (eg, -100)
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt32(int index,  out UInt32 value)
        {
            if (index < 0 || index >= Count)
            {
                value = 0;
                return false;
            }


            if (_values[index] < UInt32.MinValue)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (UInt32)_values[index];
                return true;
            }
        }
        /// <summary>
        /// Retrieves an UInt64 value from a SL attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        ///     If the value doesn't exist
        ///     The value cannot be converted into UInt64 (eg, floating-point number 1.102 cannot be converted into UInt64)
        ///     The value is an integer but outside the range of type  UInt64 (eg, -100)
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt64(int index,  out UInt64 value)
        {
            if (index < 0 || index >= Count)
            {
                value = 0;
                return false;
            }


            if (_values[index] < 0)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (UInt16)_values[index];
                return true;
            }
        }


        #endregion
    }
    #endregion

    #region DicomAttributeSS
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing SS value representation tags.
    /// </summary>
    public class DicomAttributeSS : DicomAttributeBinary<short>
    {
        #region Constructors

        public DicomAttributeSS(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeSS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.SSvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeSS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeSS(DicomAttributeSS attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Operators

        #endregion

        #region Abstract Method Implementation


        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is short[])
                {
                    _values = (short[])value;
                    SetStreamLength();
                }
                else if (value is short)
                {
                    _values = new short[1];
                    _values[0] = (short)value;
                    SetStreamLength();
                }
                else if (value is String)
                {
                    SetStringValue((String)value);
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeSS(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeSS(this);
        }

        protected override short ParseNumber(string val)
        {
            if (val == null)
                throw new DicomDataException("Null values invalid for SS VR");

            short parseVal;
            if (false == short.TryParse(val.Trim(), NumberStyle, null, out parseVal))
                throw new DicomDataException(
                    String.Format("Invalid short format value for tag {0}: {1}", Tag, val));
            return parseVal;
        }


        /// <summary>
        /// Appends an SS value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit signed int</exception>
        /// 
        public override void AppendInt16(Int16 value)
        {
            AppendValue(value);
        }

        /// <summary>
        /// Appends an SS value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit signed int</exception>
        /// 
        public override void AppendInt32(Int32 value)
        {
            if (value < Int16.MinValue || value > Int16.MaxValue)
                throw new DicomDataException(String.Format("Invalid SS value {0} for tag {1}.", value, Tag));

            AppendValue( (short) value);
        }
        /// <summary>
        /// Appends an SS value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit signed int</exception>
        /// 
        public override void AppendInt64(Int64 value)
        {
            if (value < Int16.MinValue || value > Int16.MaxValue)
                throw new DicomDataException(String.Format("Invalid SS value {0} for tag {1}.", value, Tag));

            AppendValue((short)value);
        }
        /// <summary>
        /// Appends an SS value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit signed int</exception>
        /// 
        public override void AppendUInt16(UInt16 value)
        {
            if (value > Int16.MaxValue)
                throw new DicomDataException(String.Format("Invalid SS value {0} for tag {1}.", value, Tag));

            AppendValue((short)value);
        }
        /// <summary>
        /// Appends an SS value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit signed int</exception>
        /// 
        public override void AppendUInt32(UInt32 value)
        {
            if (value > Int16.MaxValue)
                throw new DicomDataException(String.Format("Invalid SS value {0} for tag {1}.", value, Tag));

            AppendValue((short)value);
        }
        /// <summary>
        /// Appends an SL value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit signed int</exception>
        /// 
        public override void AppendUInt64(UInt64 value)
        {
            if (value > (UInt64)Int16.MaxValue)
                throw new DicomDataException(String.Format("Invalid SS value {0} for tag {1}.", value, Tag));

            AppendValue((short)value);
        }

        /// <summary>
        /// Retrieves an Int16 value from an SS attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist</item>
        /// </list>
        ///     
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt16(int index,  out Int16 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = _values[index];
            return true;
            
        }
        /// <summary>
        /// Retrieves an Int32 value from an SS attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// </list>
        ///     
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt32(int index,  out Int32 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = _values[index];
            return true;
        }
        /// <summary>
        /// Retrieves an Int64 value from an SS attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt64(int index,  out Int64 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = _values[index];
            return true;
            
        }
        /// <summary>
        /// Retrieves an UInt16 value from an SS attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// <item>The value exceeds the UInt16 range.</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt16(int index,  out UInt16 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            if (_values[index] < UInt16.MinValue)
            {
                value = 0;
                return false;
            }
            else
            {

                value = (UInt16)_values[index];
                return true;
            }
        }
        /// <summary>
        /// Retrieves an UInt32 value from an SS attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// <item>The value exceeds the UInt32 range.</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt32(int index,  out UInt32 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            if (_values[index] < UInt32.MinValue)
            {
                value = 0;
                return false;
            }
            else
            {

                value = (UInt32)_values[index];
                return true;
            }
        }
        /// <summary>
        /// Retrieves an UInt64 value from an SS attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// <item>The value exceeds the UInt64 range.</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt64(int index,  out UInt64 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            if ((UInt64)_values[index] < UInt64.MinValue)
            {
                value = 0;
                return false;
            }
            else
            {

                value = (UInt64)_values[index];
                return true;
            }
        }

        /// <summary>
        /// Sets an SS value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt16(int index, Int16 value)
        {
            SetValue(index, value);
        }
        /// <summary>
        /// Sets an SS value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt32(int index, Int32 value)
        {
            if (value < Int16.MinValue || value > Int16.MaxValue)
                throw new DicomDataException(String.Format("Invalid SS value {0} for tag {1}.", value, Tag));

            SetValue(index, (short) value);
        }
        /// <summary>
        /// Sets an SS value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt64(int index, Int64 value)
        {
            if (value < Int16.MinValue || value > Int16.MaxValue)
                throw new DicomDataException(String.Format("Invalid SS value {0} for tag {1}.", value, Tag));

            SetValue(index, (short)value);
        }

        /// <summary>
        /// Sets an SS value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt16(int index, UInt16 value)
        {
            if (value > Int16.MaxValue)
                throw new DicomDataException(String.Format("Invalid SS value {0} for tag {1}.", value, Tag));

            SetValue(index, (short)value);
        }
        /// <summary>
        /// Sets an SS value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt32(int index, UInt32 value)
        {
            if (value > Int16.MaxValue)
                throw new DicomDataException(String.Format("Invalid SS value {0} for tag {1}.", value, Tag));

            SetValue(index, (short) value);
        }

        /// <summary>
        /// Sets an SS value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit signed int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt64(int index, UInt64 value)
        {
            if (value > (UInt64)Int16.MaxValue)
                throw new DicomDataException(String.Format("Invalid SS value {0} for tag {1}.", value, Tag));

            SetValue(index, (short)value);
        }

        #endregion
    }
    #endregion

    #region DicomAttributeUL
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing UL value representation tags.
    /// </summary>
    public class DicomAttributeUL : DicomAttributeBinary<uint>
    {
        #region Constructors

        public DicomAttributeUL(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeUL(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.ULvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeUL(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeUL(DicomAttributeUL attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Operators


        #endregion

        #region Abstract Method Implementation

        public override DicomAttribute Copy()
        {
            return new DicomAttributeUL(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeUL(this);
        }

        protected override uint ParseNumber(string val)
        {
            if (val == null)
                throw new DicomDataException("Null values invalid for UL VR");

            uint parseVal;
            if (false == uint.TryParse(val.Trim(), NumberStyle, null, out parseVal))
                throw new DicomDataException(
                    String.Format("Invalid uint format value for tag {0}: {1}", Tag, val));
            return parseVal;
        }

        public override Object Values
        {
            get { return _values; }
            set
            {
                if (value is uint[])
                {
                    _values = (uint[])value;
                    SetStreamLength();
                }
                else if (value is uint)
                {
                    SetUInt32(0, (uint)value);
                }
                else if (value is string)
                {
                    SetStringValue((string)value);
                }
                else
                {
                    uint parsedValue;
                    if (UInt32.TryParse(value.ToString(), out parsedValue))
                    {
                        SetUInt32(0, parsedValue);
                    }
                    else
                    {
                        throw new DicomException(SR.InvalidType);
                    }
                }
            }
        }
        /// <summary>
        /// Appends an UL value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit unsigned int</exception>
        /// 
        public override void AppendInt16(Int16 value)
        {
            if (value < uint.MinValue)
                throw new DicomDataException(String.Format("Invalid UL value '{0}' for {1}.", value, Tag));
            AppendValue((uint)value);
        }
        /// <summary>
        /// Appends an UL value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit unsigned int</exception>
        /// 
        public override void AppendInt32(Int32 value)
        {
            if (value < uint.MinValue)
                throw new DicomDataException(String.Format("Invalid UL value '{0}' for {1}.", value, Tag));

            AppendValue((uint)value);
        }
        /// <summary>
        /// Appends an UL value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit unsigned int</exception>
        /// 
        public override void AppendInt64(Int64 value)
        {
            if (value < uint.MinValue || value > uint.MaxValue)
                throw new DicomDataException(String.Format("Invalid UL value '{0}' for {1}.", value, Tag));

            AppendValue((uint)value);
        }
        /// <summary>
        /// Appends an UL value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// 
        public override void AppendUInt16(UInt16 value)
        {
            AppendValue(value);
        }
        /// <summary>
        /// Appends an UL value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// 
        public override void AppendUInt32(UInt32 value)
        {
            AppendValue(value);
        }
        /// <summary>
        /// Appends an UL value.
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit unsigned int</exception>
        /// 
        public override void AppendUInt64(UInt64 value)
        {
            if (value < uint.MinValue || value > uint.MaxValue)
                throw new DicomDataException(String.Format("Invalid UL value '{0}' for {1}.", value, Tag));

            AppendValue((uint)value);
        }

        /// <summary>
        /// Sets an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt16(int index, Int16 value)
        {
            if (value < uint.MinValue)
                throw new DicomDataException(String.Format("Invalid UL value '{0}' for {1}.", value, Tag));

            SetValue(index, (uint) value);
        }
        /// <summary>
        /// Sets an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt32(int index, Int32 value)
        {
            if (value < uint.MinValue)
                throw new DicomDataException(String.Format("Invalid UL value '{0}' for {1}.", value, Tag));

            SetValue(index, (uint)value);
        }
        /// <summary>
        /// Sets an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt64(int index, Int64 value)
        {
            if (value < uint.MinValue || value > uint.MaxValue)
                throw new DicomDataException(String.Format("Invalid UL value '{0}' for {1}.", value, Tag));
            SetValue(index, (uint)value);
        }

        /// <summary>
        /// Sets an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt16(int index, UInt16 value)
        {
            SetValue(index,value);
        }
        /// <summary>
        /// Sets an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt32(int index, UInt32 value)
        {
            SetValue(index,value);
        }
        /// <summary>
        /// Sets an UL value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 32-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetUInt64(int index, UInt64 value)
        {
            if (value > uint.MaxValue)
                throw new DicomDataException(String.Format("Invalid UL value '{0}' for {1}.", value, Tag));
            SetValue(index, (uint)value);
        }

        /// <summary>
        /// Retrieves an Int16 value from an UL attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// <item>The value exceeds the Int16 range</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt16(int index,  out Int16 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            if (_values[index] > (UInt32)Int16.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (Int16)_values[index];
                return true;
            }

        }
        /// <summary>
        /// Retrieves an Int32 value from an UL attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// <item>The value exceeds the Int32 range</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt32(int index,  out Int32 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            if ( _values[index] > Int32.MaxValue)
            {
                value = 0;
                return false;
            }
            else
            {
                value = (Int32)_values[index];
                return true;
            }

        }
        /// <summary>
        /// Retrieves an Int64 value from an UL attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetInt64(int index,  out Int64 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

           value = _values[index];
           return true;
           

        }

        /// <summary>
        /// Retrieves an UInt16 value from an UL attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// </list>
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt16(int index,  out UInt16 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = (UInt16)_values[index];
            
            if (_values[index] > UInt16.MaxValue)
            {
                return false;
            }
            else
                return true;
            

        }
        /// <summary>
        /// Retrieves an UInt32 value from an UL attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt32(int index,  out UInt32 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = _values[index];

            return true;
        }
        /// <summary>
        /// Retrieves an UInt64 value from an UL attribute.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// 
        /// </remarks>
        /// 
        public override bool TryGetUInt64(int index,  out UInt64 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = _values[index];
            return true;
            

        }

        #endregion
    }
    #endregion

    #region DicomAttributeUN
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing UN value representation tags.
    /// </summary>
    public class DicomAttributeUN : DicomAttributeBinary<byte>
    {
        #region Constructors

        public DicomAttributeUN(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeUN(DicomTag tag)
            : base(tag)
        {
        }

        internal DicomAttributeUN(DicomTag tag, ByteBuffer item)
            : base(tag)
        {
            _values = item.ToBytes();

            SetStreamLength();
        }

        internal DicomAttributeUN(DicomAttributeUN attrib)
            : base(attrib)
        {
        }

        #endregion

        #region Abstract Method Implementation

        public override string ToString()
        {
            return Tag; // TODO
        }

        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            return false;  //TODO
        }

        public override int GetHashCode()
        {
            if (_values == null)
                return 0; // TODO
            else
                return _values.GetHashCode(); // TODO
        }

        public override bool IsNull
        {
            get
            {
                if ((Count == 1) && (_values != null) && (_values.Length == 0))
                    return true;
                return false;
            }
        }
        public override bool IsEmpty
        {
            get
            {
                if ((Count == 0) && (_values == null))
                    return true;
                return false;
            }
        }

        public override object Values
        {
            get { return _values; }
            set
            {
                if (value is byte[])
                {
                    _values = (byte[])value;
                    SetStreamLength();
                }
                else
                {
                    throw new DicomException(SR.InvalidType);
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeUN(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeUN(this);
        }

        protected override byte ParseNumber(string val)
        {
            if (val == null)
                throw new DicomDataException("Null values invalid for UN VR");

            byte parseVal;
            if (false == byte.TryParse(val.Trim(), NumberStyle, null, out parseVal))
                throw new DicomDataException(
                    String.Format("Invalid byte format value for tag {0}: {1}", Tag, val));
            return parseVal;
        }

        internal override ByteBuffer GetByteBuffer(TransferSyntax syntax, String specificCharacterSet)
        {
            ByteBuffer bb = new ByteBuffer(syntax.Endian);

            bb.FromBytes(_values);

            return bb;
        }
        #endregion
    }
    #endregion

    #region DicomAttributeUS
    /// <summary>
    /// <see cref="DicomAttributeBinary"/> derived class for storing US value representation tags.
    /// </summary>
    public class DicomAttributeUS : DicomAttributeBinary<ushort>
    {
        #region Constructors

        public DicomAttributeUS(uint tag)
            : base(tag)
        {

        }

        public DicomAttributeUS(DicomTag tag)
            : base(tag)
        {
            if (!tag.VR.Equals(DicomVr.USvr)
             && !tag.MultiVR)
                throw new DicomException(SR.InvalidVR);

        }

        internal DicomAttributeUS(DicomTag tag, ByteBuffer item)
            : base(tag, item)
        {
        }

        internal DicomAttributeUS(DicomAttributeUS attrib)
            : base(attrib)
        {
            ushort[] values = (ushort[])attrib.Values;

            _values = new ushort[values.Length];

            values.CopyTo(_values, 0);
        }


        #endregion

        #region Operators

        #endregion

        #region Abstract Method Implementation
        public override Object Values
        {
            get { return _values; }
            set
            {
                _values = value as ushort[];
                if (_values != null)
                {
                    SetStreamLength();
                    return;
                }

                String str = value as string;
                if (str != null)
                {
                    SetStringValue((String)value);
                    return;
                }
                if (value is ushort)
                {
                    SetUInt16(0, (ushort)value);
                }
                else
                {
                    ushort parsedValue;
                    if (ushort.TryParse(value.ToString(), out parsedValue))
                    {
                        SetUInt16(0, parsedValue);
                    }
                    else
                    {
                        throw new DicomException(SR.InvalidType);
                    }
                }
            }
        }

        public override DicomAttribute Copy()
        {
            return new DicomAttributeUS(this);
        }

        internal override DicomAttribute Copy(bool copyBinary)
        {
            return new DicomAttributeUS(this);
        }

        protected override ushort ParseNumber(string val)
        {
            if (val == null)
                throw new DicomDataException("Null values invalid for US VR");

            ushort parseVal;
            if (false == ushort.TryParse(val.Trim(), NumberStyle, null, out parseVal))
                throw new DicomDataException(
                    String.Format("Invalid ushort format value for tag {0}: {1}", Tag, val));
            return parseVal;
        }

        /// <summary>
        /// Sets an US value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        public override void SetInt16(int index, short value)
        {
            if (value < UInt16.MinValue)
                throw new DicomDataException(String.Format("Invalid US value '{0}' for tag {1}.", value, Tag));
            SetValue(index, (ushort) value);
        }

        /// <summary>
        /// Sets an US value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        public override void SetInt32(int index, int value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(String.Format("Invalid US value '{0}' for tag {1}.", value, Tag));
            SetValue(index, (ushort)value);
        }

        /// <summary>
        /// Sets an US value.
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        /// 
        public override void SetInt64(int index, Int64 value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(String.Format("Invalid US value '{0}' for tag {1}.", value, Tag));
            SetValue(index, (ushort)value);
        }
        /// <summary>
        /// Sets an US value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        public override void SetUInt16(int index, UInt16 value)
        {
            SetValue(index, value);
        }
        /// <summary>
        /// Sets an US value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        public override void SetUInt32(int index, uint value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(String.Format("Invalid US value '{0}' for tag {1}.", value, Tag));
            SetValue(index, (ushort)value);
        }
        /// <summary>
        /// Sets an US value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        /// <exception cref="IndexOutofBoundException">if <paramref name="index"/> is negative or greater than <seealso cref="Count"/></exception>
        public override void SetUInt64(int index, UInt64 value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(String.Format("Invalid US value '{0}' for tag {1}.", value, Tag));
            SetValue(index, (ushort)value);
        }

        /// <summary>
        /// Appends an US value.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        public override void AppendInt16(Int16 value)
        {
            if (value < UInt16.MinValue)
                throw new DicomDataException(String.Format("Invalid US value '{0}' for tag {1}.", value, Tag));
            AppendValue((UInt16)value);
        }
        /// <summary>
        /// Appends an US value.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        public override void AppendInt32(Int32 value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(String.Format("Invalid US value '{0}' for tag {1}.", value, Tag));

            AppendValue((UInt16)value);
        }
        /// <summary>
        /// Appends an US value.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        public override void AppendInt64(Int64 value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(String.Format("Invalid US value '{0}' for tag {1}.", value, Tag));
            AppendValue((UInt16)value);
        }
        /// <summary>
        /// Appends an US value.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        public override void AppendUInt16(UInt16 value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(String.Format("Invalid US value '{0}' for tag {1}.", value, Tag));
            AppendValue(value);
        }
        /// <summary>
        /// Appends an US value.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        public override void AppendUInt32(UInt32 value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(String.Format("Invalid US value '{0}' for tag {1}.", value, Tag));
            AppendValue((UInt16)value);

        }
        /// <summary>
        /// Appends an US value.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="DicomDataException">If <paramref name="value"/> cannot be fit into 16-bit unsigned int</exception>
        public override void AppendUInt64(UInt64 value)
        {
            if (value < UInt16.MinValue || value > UInt16.MaxValue)
                throw new DicomDataException(String.Format("Invalid US value '{0}' for tag {1}.", value, Tag));
            AppendValue((UInt16)value);

        }

        /// <summary>
        /// Retrieves an Int16 value from an US attribute.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// <item>The value exceeds the Int16 range</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// </remarks>
        public override bool TryGetInt16(int index,  out short value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = (Int16)_values[index];
            if (_values[index] > Int16.MaxValue)
                return false;
            else
                return true;
        }
        /// <summary>
        /// Retrieves an Int32 value from an US attribute.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// </remarks>
        public override bool TryGetInt32(int index,  out Int32 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = _values[index];
            return true;
        }
        /// <summary>
        /// Retrieves an Int64 value from an US attribute.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// </remarks>
        public override bool TryGetInt64(int index,  out Int64 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = _values[index];
            return true;
        }

        /// <summary>
        /// Retrieves an UInt16 value from an US attribute.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// </remarks>
        public override bool TryGetUInt16(int index,  out ushort value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = _values[index];
            return true;
        }
        /// <summary>
        /// Retrieves an UInt32 value from an US attribute.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// </remarks>
        public override bool TryGetUInt32(int index,  out UInt32 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = _values[index];
            return true;
        }
        /// <summary>
        /// Retrieves an UInt64 value from an US attribute.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns><b>true</b>if value can be retrieved. <b>false</b> otherwise (see remarks)</returns>
        /// <remarks>
        /// This method returns <b>false</b> if
        /// <list type="bullet">
        /// <item>The value doesn't exist.</item>
        /// </list>
        /// 
        /// If the method returns false, the returned <paramref name="value"/> is not reliable.
        /// </remarks>
        public override bool TryGetUInt64(int index,  out UInt64 value)
        {
            if (_values == null || _values.Length <= index)
            {
                value = 0;
                return false;
            }

            value = _values[index];
            return true;
        }

        #endregion
    }
    #endregion

}


