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
using System.Xml;
using System.IO;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Default implementation of <see cref="IExceptionRecorder"/>.
    /// </summary>
    public class DefaultExceptionRecorder : IExceptionRecorder
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DefaultExceptionRecorder()
        {

        }

        #region IExceptionRecorder Members

        /// <summary>
        /// Creates a <see cref="ExceptionLogEntry"/> for the specified operation and exception.
        /// </summary>
        /// <param name="operation">The name of the operation.</param>
        /// <param name="e">The exception that was thrown.</param>
        /// <returns></returns>
        public ExceptionLogEntry CreateLogEntry(string operation, Exception e)
        {
            return new ExceptionLogEntry(operation, e, WriteXml(e));
        }

        #endregion

        private string WriteXml(Exception e)
        {
            StringWriter sw = new StringWriter();
            using (XmlTextWriter writer = new XmlTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("exception");
                WriteExceptionXml(writer, e);
                writer.WriteEndElement();
                writer.WriteEndDocument();
                return sw.ToString();
            }
        }

        private void WriteExceptionXml(XmlWriter writer, Exception e)
        {
            writer.WriteElementString("message", e.Message);
            writer.WriteElementString("source", e.Source);
            writer.WriteStartElement("stack-trace");
            writer.WriteCData(e.StackTrace);
            writer.WriteEndElement();

            if (e.InnerException != null)
            {
                writer.WriteStartElement("inner-exception");
                WriteExceptionXml(writer, e.InnerException);
                writer.WriteEndElement();
            }
        }
    }
}
