using Playnite.Database;
using Playnite.Plugins;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Playnite.Filtering
{
    public class AggregateGameFilter : IGameFilter
    {
        public List<IGameFilter> Children { get; set; } = new List<IGameFilter>();
        public CollectionFilterMode FilterMode { get; set; }
        public bool Positive { get; set; }
        public bool Pinned { get; set; }

        public bool Match(Game game)
        {
            switch (FilterMode)
            {
                case CollectionFilterMode.Any:
                    return Children.Any(c => c.Match(game));
                case CollectionFilterMode.All:
                    return Children.All(c => c.Match(game));
                case CollectionFilterMode.Empty:
                    return Children.Count == 0;
                default:
                    throw new Exception("Unkown CollectionFilterMode: " + FilterMode.ToString());
            }
        }
    }
}
