using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Playnite.Filtering
{
    public class SingleValueCollectionGameFieldFilter : CollectionGameFieldFilter<Guid>
    {
        public SingleValueCollectionGameFieldFilter(GameFilterField field, SelectableIdItemList items, Func<Game, Guid> valueSelector)
            : base(field, items, valueSelector)
        {
        }

        protected override bool? MatchPositive(Game game)
        {
            var checkedEntities = Items.GetSelectedIds();
            if (checkedEntities.Count == 0)
                return null;

            var value = ValueSelector(game);

            return checkedEntities.Contains(value);
        }
    }
}
