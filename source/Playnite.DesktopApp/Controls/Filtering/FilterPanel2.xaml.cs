using Playnite.Database;
using Playnite.Filtering;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Playnite.DesktopApp.Controls
{
    /// <summary>
    /// Interaction logic for FilterPanel2.xaml
    /// </summary>
    public partial class FilterPanel2 : UserControl
    {
        public FilterPanel2()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selected = (GameFilterField)Enum.Parse(typeof(GameFilterField), button.Content.ToString());
            var filterFactory = new GameFieldFilterFactory(PlayniteApplication.Current.PlayniteApiGlobal);
            var filter = filterFactory.GetFilter(selected);
            var control = new MultipleValueGameFilterControl() { DataContext = filter };
            FilterContainer.Children.Add(control);
        }
    }
}
