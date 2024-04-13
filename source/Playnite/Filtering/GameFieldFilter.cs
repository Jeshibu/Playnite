using Playnite.SDK.Models;
using System;
using System.Collections.Generic;

namespace Playnite.Filtering
{
    public class FilterEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }

        public static FilterEntity Create(DatabaseObject databaseObject) => new FilterEntity { Id = databaseObject.Id, Name = databaseObject.Name };
        public static FilterEntity Create(SDK.Plugins.LibraryPlugin plugin) => new FilterEntity { Id = plugin.Id, Name = plugin.Name };

        public override string ToString() => Name;
    }

    public abstract class GameFieldFilter<TField> : ObservableObject, IGameFilter
    {
        private bool positive;
        private bool pinned;

        public event EventHandler FilterChanged;

        public Func<Game, TField> ValueSelector { get; }

        public GameFieldFilter(GameFilterField field, Func<Game, TField> valueSelector)
        {
            Field = field;
            ValueSelector = valueSelector;
        }

        protected void CallChanged() => FilterChanged?.Invoke(this, new FilterChangedEventArgs(new List<string> { Field.ToString() }));

        public bool Positive
        {
            get => positive;
            set
            {
                SetValue(ref positive, value);
                CallChanged();
            }
        }

        public bool Pinned { get => pinned; set => SetValue(ref pinned, value); }

        public GameFilterField Field { get; private set; }

        /// <summary>
        /// Match games assuming that the filter is positive (include games that match the criteria instead of excluding them) - flipping the logic will be done in the base class if needed.
        /// Return null if the filter should be considered empty and not affecting the filtering.
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        protected abstract bool? MatchPositive(Game game);

        public bool Match(Game game)
        {
            var match = MatchPositive(game);
            return match.HasValue ? match.Value == Positive : true;
        }
    }

    public abstract class CollectionGameFieldFilter<TField> : GameFieldFilter<TField>
    {
        public CollectionGameFieldFilter(GameFilterField field, SelectableIdItemList items, Func<Game, TField> valueSelector) : base(field, valueSelector)
        {
            Items = items;
            items.SelectionChanged += (o, ea) => CallChanged();
        }

        public SelectableIdItemList Items { get; set; }
    }
}
