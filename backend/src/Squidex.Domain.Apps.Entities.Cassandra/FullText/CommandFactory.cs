// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Squidex.Domain.Apps.Entities.Contents.Text;

namespace Squidex.Domain.Apps.Entities.Cassandra.FullText
{
    public static class CommandFactory
    {
        private static readonly FilterDefinitionBuilder<TextIndexEntity> Filter = Builders<TextIndexEntity>.Filter;
        private static readonly UpdateDefinitionBuilder<TextIndexEntity> Update = Builders<TextIndexEntity>.Update;

        public static void CreateCommands(IndexCommand command, List<WriteModel<TextIndexEntity>> writes)
        {
            switch (command)
            {
                case DeleteIndexEntry delete:
                    DeleteEntry(delete, writes);
                    break;
                case UpsertIndexEntry upsert:
                    UpsertEntry(upsert, writes);
                    break;
                case UpdateIndexEntry update:
                    UpdateEntry(update, writes);
                    break;
            }
        }

        private static void UpsertEntry(UpsertIndexEntry upsert, List<WriteModel<TextIndexEntity>> writes)
        {
            writes.Add(
                new UpdateOneModel<TextIndexEntity>(
                    Filter.And(
                        Filter.Eq(x => x.DocId, upsert.DocId),
                        Filter.Exists(x => x.GeoField, false),
                        Filter.Exists(x => x.GeoObject, false)),
                    Update
                        .Set(x => x.ServeAll, upsert.ServeAll)
                        .Set(x => x.ServePublished, upsert.ServePublished)
                        .Set(x => x.Texts, upsert.Texts?.Values.Select(TextIndexEntityText.FromText).ToList())
                        .SetOnInsert(x => x.Id, Guid.NewGuid().ToString())
                        .SetOnInsert(x => x.DocId, upsert.DocId)
                        .SetOnInsert(x => x.AppId, upsert.AppId.Id)
                        .SetOnInsert(x => x.ContentId, upsert.ContentId)
                        .SetOnInsert(x => x.SchemaId, upsert.SchemaId.Id))
                {
                    IsUpsert = true
                });

            if (upsert.GeoObjects?.Any() == true)
            {
                if (!upsert.IsNew)
                {
                    writes.Add(
                        new DeleteOneModel<TextIndexEntity>(
                            Filter.And(
                                Filter.Eq(x => x.DocId, upsert.DocId),
                                Filter.Exists(x => x.GeoField),
                                Filter.Exists(x => x.GeoObject))));
                }

                foreach (var (field, geoObject) in upsert.GeoObjects)
                {
                    writes.Add(
                        new InsertOneModel<TextIndexEntity>(
                            new TextIndexEntity
                            {
                                Id = Guid.NewGuid().ToString(),
                                AppId = upsert.AppId.Id,
                                DocId = upsert.DocId,
                                ContentId = upsert.ContentId,
                                GeoField = field,
                                GeoObject = geoObject,
                                SchemaId = upsert.SchemaId.Id,
                                ServeAll = upsert.ServeAll,
                                ServePublished = upsert.ServePublished
                            }));
                }
            }
        }

        private static void UpdateEntry(UpdateIndexEntry update, List<WriteModel<TextIndexEntity>> writes)
        {
            writes.Add(
                new UpdateOneModel<TextIndexEntity>(
                    Filter.Eq(x => x.DocId, update.DocId),
                    Update
                        .Set(x => x.ServeAll, update.ServeAll)
                        .Set(x => x.ServePublished, update.ServePublished)));
        }

        private static void DeleteEntry(DeleteIndexEntry delete, List<WriteModel<TextIndexEntity>> writes)
        {
            writes.Add(
                new DeleteOneModel<TextIndexEntity>(
                    Filter.Eq(x => x.DocId, delete.DocId)));
        }
    }
}
