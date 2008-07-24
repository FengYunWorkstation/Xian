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
    using System.Collections.Generic;
    using ClearCanvas.ImageServer.Model.EntityBrokers;
    using ClearCanvas.ImageServer.Enterprise;
    using System.Reflection;

[Serializable]
public partial class ServerRuleApplyTimeEnum : ServerEnum
{
      #region Private Static Members
      private static readonly ServerRuleApplyTimeEnum _SopReceived = GetEnum("SopReceived");
      private static readonly ServerRuleApplyTimeEnum _SopProcessed = GetEnum("SopProcessed");
      private static readonly ServerRuleApplyTimeEnum _SeriesProcessed = GetEnum("SeriesProcessed");
      private static readonly ServerRuleApplyTimeEnum _StudyProcessed = GetEnum("StudyProcessed");
      private static readonly ServerRuleApplyTimeEnum _StudyArchived = GetEnum("StudyArchived");
      private static readonly ServerRuleApplyTimeEnum _StudyRestored = GetEnum("StudyRestored");
      #endregion

      #region Public Static Properties
      /// <summary>
      /// Apply rule when a SOP Instance has been received
      /// </summary>
      public static ServerRuleApplyTimeEnum SopReceived
      {
          get { return _SopReceived; }
      }
      /// <summary>
      /// Apply rule when a SOP Instance has been processed
      /// </summary>
      public static ServerRuleApplyTimeEnum SopProcessed
      {
          get { return _SopProcessed; }
      }
      /// <summary>
      /// Apply rule when a Series is initially processed
      /// </summary>
      public static ServerRuleApplyTimeEnum SeriesProcessed
      {
          get { return _SeriesProcessed; }
      }
      /// <summary>
      /// Apply rule when a Study is initially processed
      /// </summary>
      public static ServerRuleApplyTimeEnum StudyProcessed
      {
          get { return _StudyProcessed; }
      }
      /// <summary>
      /// Apply rule after a Study is archived
      /// </summary>
      public static ServerRuleApplyTimeEnum StudyArchived
      {
          get { return _StudyArchived; }
      }
      /// <summary>
      /// Apply rule after a Study has been restored
      /// </summary>
      public static ServerRuleApplyTimeEnum StudyRestored
      {
          get { return _StudyRestored; }
      }

      #endregion

      #region Constructors
      public ServerRuleApplyTimeEnum():base("ServerRuleApplyTimeEnum")
      {}
      #endregion
      #region Public Members
      public override void SetEnum(short val)
      {
          ServerEnumHelper<ServerRuleApplyTimeEnum, IServerRuleApplyTimeEnumBroker>.SetEnum(this, val);
      }
      static public List<ServerRuleApplyTimeEnum> GetAll()
      {
          return ServerEnumHelper<ServerRuleApplyTimeEnum, IServerRuleApplyTimeEnumBroker>.GetAll();
      }
      static public ServerRuleApplyTimeEnum GetEnum(string lookup)
      {
          return ServerEnumHelper<ServerRuleApplyTimeEnum, IServerRuleApplyTimeEnumBroker>.GetEnum(lookup);
      }
      #endregion
}
}
