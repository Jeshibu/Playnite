using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Playnite.Filtering
{
    public class MultipleValueCollectionGameFieldFilter : CollectionGameFieldFilter<IEnumerable<Guid>>
    {
        private CollectionFilterMode filterMode;
        private CollectionFilterMode[] availableFilterModes = new CollectionFilterMode[0];
        public CollectionFilterMode FilterMode
        {
            get => filterMode;
            set
            {
                SetValue(ref filterMode, value);
                CallChanged();
            }
        }
        public CollectionFilterMode[] AvailableFilterModes { get => availableFilterModes; set => SetValue(ref availableFilterModes, value); }

        public MultipleValueCollectionGameFieldFilter(GameFilterField field, SelectableIdItemList items, Func<Game, IEnumerable<Guid>> valueSelector, CollectionFilterMode filterMode = CollectionFilterMode.Any)
            : base(field, items, valueSelector)
        {
            FilterMode = filterMode;
        }

        protected override bool? MatchPositive(Game game)
        {
            var checkedIds = Items.GetSelectedIds();
            if (checkedIds.Count == 0)
                return true;

            var values = ValueSelector(game).ToList();
            switch (FilterMode)
            {
                case CollectionFilterMode.Any:
                    return values.Any(checkedIds.Contains);
                case CollectionFilterMode.All:
                    return values.Contains(checkedIds);
                case CollectionFilterMode.Empty:
                    return values.Count == 0;
            }
            return true;
        }
    }
}
