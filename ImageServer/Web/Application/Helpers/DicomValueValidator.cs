﻿#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageServer.Web.Application.Helpers
{
    internal class DicomValueValidator
    {
        public static bool IsValidDicomPatientSex(string value)
        {
            if (string.IsNullOrEmpty(value))
                return true;

            return value.Equals("M", StringComparison.InvariantCultureIgnoreCase) ||
                   value.Equals("F", StringComparison.InvariantCultureIgnoreCase) ||
                   value.Equals("O", StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
