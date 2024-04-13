using Playnite.Database;
using Playnite.DesktopApp.Controls.Filtering;
using Playnite.Filtering;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Playnite.DesktopApp.Controls
{
    /// <summary>
    /// Interaction logic for FilterPanel2.xaml
    /// </summary>
    public partial class FilterPanel2 : UserControl
    {
        public AggregateGameFilter BaseFilter { get; } = new AggregateGameFilter { Positive = true, FilterMode = CollectionFilterMode.All };

        public FilterPanel2()
        {
            InitializeComponent();

            var main = DesktopApplication.Current.MainModel;
            main.GamesView.CollectionView.Filter = item =>
            {
                if (!(item is GamesCollectionViewEntry entry))
                    return false;

                return BaseFilter.Match(entry.Game);
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selected = (GameFilterField)Enum.Parse(typeof(GameFilterField), button.Content.ToString());
            var filterFactory = new GameFieldFilterFactory(PlayniteApplication.Current.PlayniteApiGlobal);
            var filter = filterFactory.GetFilter(selected);
            var control = GetControl(filter);
            if (control != null)
                AddFilter(filter, control);
        }

        private UserControl GetControl(IGameFilter filter)
        {
            if (filter == null)
                return null;

            if (filter is StringGameFieldFilter)
                return new StringGameFilterControl();

            if (filter is MultipleValueCollectionGameFieldFilter)
                return new MultipleValueGameFilterControl();

            return null;
        }

        public void AddFilter(IGameFilter filter, UserControl control)
        {
            filter.FilterChanged += HandleFilterChange;
            control.DataContext = filter;
            FilterContainer.Children.Add(control);
            HandleFilterChange(control, new EventArgs());
        }

        public void RemoveFilter(UserControl control)
        {
            FilterContainer.Children.Remove(control);
            if (control.DataContext is IGameFilter filter)
            {
                BaseFilter.Children.Remove(filter);
                HandleFilterChange(control, new EventArgs());
            }
        }

        private void HandleFilterChange(object sender, EventArgs e)
        {
            var main = DesktopApplication.Current.MainModel;
            main.GamesView.CollectionView.Refresh();
        }
    }

    public class FilterPanel2ViewModel
    {

    }
}
