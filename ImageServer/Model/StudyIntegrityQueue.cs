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
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Model
{
    partial class StudyIntegrityQueue
    {
        private Study _study;
        private readonly object _syncLock = new object();
        protected StudyStorage _studyStorage;


        /// <summary>
        /// Loads the related <see cref="Study"/> entity.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Study LoadStudy(IPersistenceContext context)
        {
            StudyStorage storage = LoadStudyStorage(context);

            if (_study == null)
            {
                lock (_syncLock)
                {
                    _study = storage.LoadStudy(context);
                }
            }
            return _study;
        }

        /// <summary>
        /// Loads the related <see cref="StudyStorage"/> entity.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private StudyStorage LoadStudyStorage(IPersistenceContext context)
        {
            if (_studyStorage == null)
            {
                lock (_syncLock)
                {
                    _studyStorage = StudyStorage.Load(context, StudyStorageKey);
                }
            }
            return _studyStorage;
        }
    }
}
