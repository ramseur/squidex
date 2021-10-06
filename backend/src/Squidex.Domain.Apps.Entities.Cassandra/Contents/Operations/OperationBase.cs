// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Collections.Generic;
using MongoDB.Driver;

namespace Squidex.Domain.Apps.Entities.Cassandra.Contents.Operations
{
    public abstract class OperationBase
    {
        protected static readonly SortDefinitionBuilder<ContentEntity> Sort = Builders<ContentEntity>.Sort;
        protected static readonly UpdateDefinitionBuilder<ContentEntity> Update = Builders<ContentEntity>.Update;
        protected static readonly FilterDefinitionBuilder<ContentEntity> Filter = Builders<ContentEntity>.Filter;
        protected static readonly IndexKeysDefinitionBuilder<ContentEntity> Index = Builders<ContentEntity>.IndexKeys;
        protected static readonly ProjectionDefinitionBuilder<ContentEntity> Projection = Builders<ContentEntity>.Projection;

        public IMongoCollection<ContentEntity> Collection { get; private set; }

        public void Setup(IMongoCollection<ContentEntity> collection)
        {
            Collection = collection;
        }

        public virtual IEnumerable<CreateIndexModel<ContentEntity>> CreateIndexes()
        {
            yield break;
        }
    }
}
