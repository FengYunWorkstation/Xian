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
using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Healthcare
{
    public sealed class WorklistOwner : ValueObject, IEquatable<WorklistOwner>, IAuditFormattable
    {
        /// <summary>
        /// The Administrative worklist owner.
        /// </summary>
        public static WorklistOwner Admin = new WorklistOwner();

        private Staff _staff;
        private StaffGroup _group;

        /// <summary>
        /// No-args constructor required for NHibernate.
        /// </summary>
        private WorklistOwner()
        {

        }

        /// <summary>
        /// Creates a staff owner.
        /// </summary>
        /// <param name="staff"></param>
        public WorklistOwner(Staff staff)
        {
            _staff = staff;
        }

        /// <summary>
        /// Creates a group owner.
        /// </summary>
        /// <param name="group"></param>
        public WorklistOwner(StaffGroup group)
        {
            _group = group;
        }

        /// <summary>
        /// Gets a value indicating if this is a Staff owner.
        /// </summary>
        public bool IsStaffOwner
        {
            get { return _staff != null; }
        }

        /// <summary>
        /// Gets a value indicating if this is a Group owner.
        /// </summary>
        public bool IsGroupOwner
        {
            get { return _group != null; }
        }

        /// <summary>
        /// Gets a value indicating if this is the Admin owner.
        /// </summary>
        public bool IsAdminOwner
        {
            get { return Equals(this, Admin); }
        }

        public string Name
        {
            get
            {
                return this.IsStaffOwner ? _staff.Name.ToString() :
                    this.IsGroupOwner ? _group.Name : null;
            }
        }

        [PersistentProperty]
        public Staff Staff
        {
            get { return _staff; }
            private set { _staff = value; }
        }

        [PersistentProperty]
        public StaffGroup Group
        {
            get { return _group; }
            private set { _group = value; }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WorklistOwner);
        }

        public override int GetHashCode()
        {
            return (_staff == null ? 0 : _staff.GetHashCode()) ^
                (_group == null ? 0 : _group.GetHashCode());
        }

        public override object Clone()
        {
            WorklistOwner copy = new WorklistOwner();
            copy._group = this._group;
            copy._staff = this._staff;
            return copy;
        }

        #region IEquatable<Address> Members

        public bool Equals(WorklistOwner other)
        {
            if (other == null)
                return false;
            return Equals(_staff, other._staff)
                && Equals(_group, other._group);
        }

        #endregion

        #region IAuditFormattable Members

        public void Write(IObjectWriter writer)
        {
            writer.WriteProperty("Staff", _staff);
            writer.WriteProperty("Group", _group);
        }

        #endregion
    }
}
