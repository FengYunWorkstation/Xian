#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0//

#endregion

// This file is auto-generated by the ClearCanvas.Model.SqlServer.CodeGenerator project.

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    using System;
    using System.Xml;
    using ClearCanvas.Enterprise.Core;
    using ClearCanvas.ImageServer.Enterprise;

    public partial class ServerPartitionDataAccessSelectCriteria : EntitySelectCriteria
    {
        public ServerPartitionDataAccessSelectCriteria()
        : base("ServerPartitionDataAccess")
        {}
        public ServerPartitionDataAccessSelectCriteria(ServerPartitionDataAccessSelectCriteria other)
        : base(other)
        {}
        public override object Clone()
        {
            return new ServerPartitionDataAccessSelectCriteria(this);
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ServerPartitionDataAccess", ColumnName="ServerPartitionGUID")]
        public ISearchCondition<ServerEntityKey> ServerPartitionKey
        {
            get
            {
              if (!SubCriteria.ContainsKey("ServerPartitionKey"))
              {
                 SubCriteria["ServerPartitionKey"] = new SearchCondition<ServerEntityKey>("ServerPartitionKey");
              }
              return (ISearchCondition<ServerEntityKey>)SubCriteria["ServerPartitionKey"];
            } 
        }
        [EntityFieldDatabaseMappingAttribute(TableName="ServerPartitionDataAccess", ColumnName="DataAccessGroupGUID")]
        public ISearchCondition<ServerEntityKey> DataAccessGroupKey
        {
            get
            {
              if (!SubCriteria.ContainsKey("DataAccessGroupKey"))
              {
                 SubCriteria["DataAccessGroupKey"] = new SearchCondition<ServerEntityKey>("DataAccessGroupKey");
              }
              return (ISearchCondition<ServerEntityKey>)SubCriteria["DataAccessGroupKey"];
            } 
        }
    }
}
