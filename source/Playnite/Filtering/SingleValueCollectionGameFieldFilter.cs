using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Playnite.Filtering
{
    public class SingleValueCollectionGameFieldFilter : CollectionGameFieldFilter<Guid>
    {
        public SingleValueCollectionGameFieldFilter(GameFilterField field, IEnumerable<FilterEntity> entities, Func<Game, Guid> valueSelector)
            : base(field, entities, valueSelector)
        {
        }

        protected override bool? MatchPositive(Game game)
        {
            var checkedEntities = Entities.Where(e => e.Checked).ToList();
            if (checkedEntities.Count == 0)
                return null;

            var value = ValueSelector(game);

            return Entities.Any(e => e.Checked && e.Id == value);
        }
    }
}
