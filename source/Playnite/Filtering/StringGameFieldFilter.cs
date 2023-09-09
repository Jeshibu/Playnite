using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Playnite.Filtering
{
    public class StringGameFieldFilter : GameFieldFilter<string>
    {
        public string SearchText { get; set; }

        public StringGameFieldFilter(GameFilterField field, Func<Game, string> valueSelector) :base(field, valueSelector)
        {
        }

        protected override bool? MatchPositive(Game game)
        {
            if (string.IsNullOrEmpty(SearchText))
                return null;

            var value = ValueSelector(game);

            return value.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
