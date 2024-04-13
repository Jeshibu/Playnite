using Playnite.SDK.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Playnite.Filtering
{
    public class AggregateGameFilter : IGameFilter
    {
        private ObservableCollection<IGameFilter> children;

        public ObservableCollection<IGameFilter> Children
        {
            get => children;
            set
            {
                children = value;
                children.CollectionChanged += (o, ea) => FilterChanged?.Invoke(o, ea);
            }
        }
        public CollectionFilterMode FilterMode { get; set; }
        public bool Positive { get; set; }
        public bool Pinned { get; set; }
        public event EventHandler FilterChanged;

        public AggregateGameFilter()
        {
            Children = new ObservableCollection<IGameFilter>();
        }

        public bool Match(Game game)
        {
            if (Children.Count == 0)
                return true;

            switch (FilterMode)
            {
                case CollectionFilterMode.Any:
                    return Children.Any(c => c.Match(game)) == Positive;
                case CollectionFilterMode.All:
                    return Children.All(c => c.Match(game)) == Positive;
                default:
                    throw new Exception("Unkown CollectionFilterMode: " + FilterMode.ToString());
            }
        }
    }
}
