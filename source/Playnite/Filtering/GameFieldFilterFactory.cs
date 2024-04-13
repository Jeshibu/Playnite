using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Playnite.Filtering
{
    public class GameFieldFilterFactory
    {
        public IPlayniteAPI PlayniteAPI { get; }

        public GameFieldFilterFactory(IPlayniteAPI playniteAPI)
        {
            PlayniteAPI = playniteAPI;
        }

        public IGameFilter GetFilter(GameFilterField field)
        {
            var db = PlayniteAPI.Database;
            switch (field)
            {
                case GameFilterField.Name:
                    return new StringGameFieldFilter(field, g => g.Name);
                case GameFilterField.Version:
                    return new StringGameFieldFilter(field, g => g.Version);

                case GameFilterField.Library:
                    var plugins = PlayniteAPI.Addons.Plugins.OfType<LibraryPlugin>().Select(FilterEntity.Create);
                    return new SingleValueCollectionGameFieldFilter(field, GetItems(PlayniteAPI.Addons), g => g.PluginId);
                case GameFilterField.CompletionStatus:
                    return new SingleValueCollectionGameFieldFilter(field, GetItems(db.CompletionStatuses, true), g => g.CompletionStatusId);
                case GameFilterField.Source:
                    return new SingleValueCollectionGameFieldFilter(field, GetItems(db.Sources, true), g => g.SourceId);

                case GameFilterField.Platform:
                    return new MultipleValueCollectionGameFieldFilter(field, GetItems(db.Platforms, false), g => g.PlatformIds);
                case GameFilterField.Developer:
                    return new MultipleValueCollectionGameFieldFilter(field, GetItems(db.Companies, false), g => g.DeveloperIds);
                case GameFilterField.Publisher:
                    return new MultipleValueCollectionGameFieldFilter(field, GetItems(db.Companies, false), g => g.PublisherIds);
                case GameFilterField.Genre:
                    return new MultipleValueCollectionGameFieldFilter(field, GetItems(db.Genres, false), g => g.GenreIds);
                case GameFilterField.Category:
                    return new MultipleValueCollectionGameFieldFilter(field, GetItems(db.Categories, false), g => g.CategoryIds);
                case GameFilterField.Tags:
                    return new MultipleValueCollectionGameFieldFilter(field, GetItems(db.Tags, false), g => g.TagIds);
                case GameFilterField.Features:
                    return new MultipleValueCollectionGameFieldFilter(field, GetItems(db.Features, false), g => g.FeatureIds);
                case GameFilterField.Series:
                    return new MultipleValueCollectionGameFieldFilter(field, GetItems(db.Series, false), g => g.SeriesIds);
                case GameFilterField.Region:
                    return new MultipleValueCollectionGameFieldFilter(field, GetItems(db.Regions, false), g => g.RegionIds);
                case GameFilterField.AgeRating:
                    return new MultipleValueCollectionGameFieldFilter(field, GetItems(db.AgeRatings, false), g => g.AgeRatingIds);

                default: return null;
            }
        }

        public static GameFieldType GetFieldType(GameFilterField field)
        {
            switch (field)
            {
                case GameFilterField.Library:
                case GameFilterField.CompletionStatus:
                case GameFilterField.Source:
                    return GameFieldType.SingleEntity;

                case GameFilterField.Name:
                case GameFilterField.Version:
                    return GameFieldType.String;

                case GameFilterField.Platform:
                case GameFilterField.Developer:
                case GameFilterField.Publisher:
                case GameFilterField.Genre:
                case GameFilterField.Category:
                case GameFilterField.Tags:
                case GameFilterField.Features:
                case GameFilterField.Series:
                case GameFilterField.Region:
                case GameFilterField.AgeRating:
                    return GameFieldType.EntityCollection;

                case GameFilterField.ReleaseDate:
                    return GameFieldType.ReleaseDate;

                case GameFilterField.TimePlayed:
                case GameFilterField.InstallSize:
                case GameFilterField.UserScore:
                case GameFilterField.CommunityScore:
                case GameFilterField.CriticScore:
                    return GameFieldType.Integer;

                case GameFilterField.LastPlayed:
                case GameFilterField.RecentActivity:
                case GameFilterField.DateAdded:
                case GameFilterField.DateModified:
                    return GameFieldType.Date;

                default:
                    throw new Exception($"Unknown field: {field}");
            }
        }

        private static SelectableIdItemList GetItems(IAddons addons)
        {
            return new SelectableLibraryPluginList(addons.Plugins.OfType<LibraryPlugin>());
        }

        private static SelectableDbItemList GetItems(IEnumerable<DatabaseObject> entities, bool includeNoneItem)
        {
            return new SelectableDbItemList(entities, includeNoneItem: includeNoneItem);
        }
    }

    public class GameFilterConfig<TField>
    {
        GameFilterField Field { get; set; }
        GameFieldType FieldType { get; set; }
        Func<IGameDatabase, IEnumerable<DatabaseObject>> DatabaseEntitiesSelector { get; set; }
        Func<Game, TField> FieldSelector { get; set; }
    }
}
