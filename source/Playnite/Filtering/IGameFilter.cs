using Playnite.SDK.Models;

namespace Playnite.Filtering
{
    public interface IGameFilter
    {
        bool Positive { get; set; }
        bool Pinned { get; set; }
        bool Match(Game game);
    }

    public enum CollectionFilterMode
    {
        Any,
        All,
        Empty,
    }

    public enum GameFilterField
    {
        Platform,
        Library,
        Name,
        Genre,
        ReleaseDate,
        Developer,
        Publisher,
        Category,
        Tags,
        Features,
        TimePlayed,
        InstallSize,
        CompletionStatus,
        Series,
        Region,
        Source,
        AgeRating,
        Version,
        UserScore,
        CommunityScore,
        CriticScore,
        LastPlayed,
        RecentActivity,
        DateAdded,
        DateModified,
    }

    public enum GameFieldType
    {
        String,
        SingleEntity,
        EntityCollection,
        Date,
        Integer,
        ReleaseDate
    }
}
