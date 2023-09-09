using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Playnite.Filtering
{
    public class MultipleValueCollectionGameFieldFilter : CollectionGameFieldFilter<IEnumerable<Guid>>
    {
        private string searchText;
        private CollectionFilterMode filterMode;
        private CollectionFilterMode[] availableFilterModes = new CollectionFilterMode[0];
        public string SearchText { get => searchText; set => SetValue(ref searchText, value); }
        public CollectionFilterMode FilterMode { get => filterMode; set => SetValue(ref filterMode, value); }
        public CollectionFilterMode[] AvailableFilterModes { get => availableFilterModes; set => SetValue(ref availableFilterModes, value); }

        public MultipleValueCollectionGameFieldFilter(GameFilterField field, IEnumerable<FilterEntity> entities, Func<Game, IEnumerable<Guid>> valueSelector, CollectionFilterMode filterMode = CollectionFilterMode.Any)
            :base(field, entities, valueSelector)
        {
            FilterMode = filterMode;
            Items = new SelectableIdItemList<FilterEntity>(entities, x => x.Id);
        }

        protected override bool? MatchPositive(Game game)
        {
            var checkedIds = Entities.Where(e => e.Checked).Select(e => e.Id).ToList();
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
