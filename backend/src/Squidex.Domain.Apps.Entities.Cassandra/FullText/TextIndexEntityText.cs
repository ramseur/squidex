// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using MongoDB.Bson.Serialization.Attributes;

namespace Squidex.Domain.Apps.Entities.Cassandra.FullText
{
    public sealed class TextIndexEntityText
    {
        [BsonRequired]
        [BsonElement("t")]
        public string Text { get; set; }

        [BsonIgnoreIfNull]
        [BsonElement("language")]
        public string Language { get; set; } = "none";

        public static TextIndexEntityText FromText(string text)
        {
            return new TextIndexEntityText { Text = text };
        }
    }
}
