﻿using Playnite.Behaviors;
using Playnite.Commands;
using Playnite.Common;
using Playnite.FullscreenApp.ViewModels;
using Playnite.Input;
using Playnite.SDK;
using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace Playnite.FullscreenApp.Controls
{
    [TemplatePart(Name = "PART_MenuHost", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_ButtonBack", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_ButtonClear", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_ItemsHost", Type = typeof(ItemsControl))]
    public class FilterDbItemtSelection : Control, IDisposable
    {
        private bool isPrimaryFilter = false;
        private FullscreenAppViewModel mainModel;
        private FrameworkElement MenuHost;
        private ButtonBase ButtonBack;
        private ButtonBase ButtonClear;
        private ItemsControl ItemsHost;

        internal bool IgnoreChanges { get; set; }
        public string Title { get; set; }

        public SelectableIdItemList ItemsList
        {
            get
            {
                return (SelectableIdItemList)GetValue(ItemsListProperty);
            }

            set
            {
                SetValue(ItemsListProperty, value);
            }
        }

        public static readonly DependencyProperty ItemsListProperty = DependencyProperty.Register(
            nameof(ItemsList),
            typeof(SelectableIdItemList),
            typeof(FilterDbItemtSelection),
            new PropertyMetadata(null, ItemsListPropertyChangedCallback));

        private static void ItemsListPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var box = sender as FilterDbItemtSelection;
            if (box.IgnoreChanges)
            {
                return;
            }

            var list = (SelectableIdItemList)e.NewValue;
            if (list == null)
            {
                var oldList = (SelectableIdItemList)e.OldValue;
                oldList.SelectionChanged -= box.List_SelectionChanged;
            }
            else
            {
                box.IgnoreChanges = true;
                list.SelectionChanged += box.List_SelectionChanged;
                if (box.FilterProperties != null)
                {
                    list.SetSelection(box.FilterProperties.Ids);
                }

                box.IgnoreChanges = false;
            }
        }

        public void List_SelectionChanged(object sender, EventArgs e)
        {
            if (!IgnoreChanges)
            {
                IgnoreChanges = true;
                FilterProperties = new FilterItemProperties { Ids = ItemsList.GetSelectedIds() };
                IgnoreChanges = false;
            }
        }

        public FilterItemProperties FilterProperties
        {
            get
            {
                return (FilterItemProperties)GetValue(FilterPropertiesProperty);
            }

            set
            {
                SetValue(FilterPropertiesProperty, value);
            }
        }

        public static readonly DependencyProperty FilterPropertiesProperty = DependencyProperty.Register(
            nameof(FilterProperties),
            typeof(FilterItemProperties),
            typeof(FilterDbItemtSelection),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, FilterPropertiesPropertyChangedCallback));

        private static void FilterPropertiesPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var box = sender as FilterDbItemtSelection;
            if (box.IgnoreChanges)
            {
                return;
            }

            box.IgnoreChanges = true;
            if (box.FilterProperties != null && box.FilterProperties.Text.IsNullOrEmpty())
            {
                box.ItemsList?.SetSelection(box.FilterProperties.Ids);
            }
            else if (box.FilterProperties == null)
            {
                box.ItemsList?.SetSelection(null);
            }

            box.IgnoreChanges = false;
        }

        static FilterDbItemtSelection()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FilterDbItemtSelection), new FrameworkPropertyMetadata(typeof(FilterDbItemtSelection)));
        }

        public FilterDbItemtSelection() : this(FullscreenApplication.Current?.MainModel)
        {
        }

        public FilterDbItemtSelection(FullscreenAppViewModel mainModel, bool isPrimaryFilter = false) : base()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                Title = "Design Title";
                var games = new List<Game>()
                {
                    new Game("Game 1"),
                    new Game("Game 2"),
                    new Game("Game 3")
                };

                ItemsList = new SelectableIdItemList<Game>(games, a => a.Id);
            }

            this.isPrimaryFilter = isPrimaryFilter;
            this.mainModel = mainModel;
            DataContext = this;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template != null)
            {
                var closeCommand = isPrimaryFilter ? mainModel.CloseSubFilterCommand : mainModel.CloseAdditionalFilterCommand;
                MenuHost = Template.FindName("PART_MenuHost", this) as FrameworkElement;
                if (MenuHost != null)
                {
                    MenuHost.InputBindings.Add(new KeyBinding(closeCommand, new KeyGesture(Key.Back)));
                    MenuHost.InputBindings.Add(new KeyBinding(closeCommand, new KeyGesture(Key.Escape)));
                    MenuHost.InputBindings.Add(new XInputBinding(closeCommand, XInputButton.B));
                }

                ButtonBack = Template.FindName("PART_ButtonBack", this) as ButtonBase;
                if (ButtonBack != null)
                {
                    ButtonBack.Command = closeCommand;
                }

                ButtonClear = Template.FindName("PART_ButtonClear", this) as ButtonBase;
                if (ButtonClear != null)
                {
                    ButtonClear.Command = new RelayCommand<object>(a =>
                    {
                        FilterProperties = null;
                        IgnoreChanges = true;
                        ItemsList?.SetSelection(null);
                        IgnoreChanges = false;
                    });
                    BindingTools.SetBinding(ButtonClear,
                         FocusBahaviors.FocusBindingProperty,
                         mainModel,
                         nameof(mainModel.SubFilterVisible));
                }

                ItemsHost = Template.FindName("PART_ItemsHost", this) as ItemsControl;
                if (ItemsHost != null)
                {
                    AssignItemListPanel(ItemsHost);
                    BindingTools.SetBinding(
                        ItemsHost,
                        ItemsControl.ItemsSourceProperty,
                        this,
                        nameof(ItemsList));
                }
            }
        }

        public void Dispose()
        {
            if (ItemsList != null)
            {
                ItemsList.SelectionChanged -= List_SelectionChanged;
                ItemsList.SetSelection(null);
            }

            BindingOperations.ClearBinding(this, ItemsListProperty);
            BindingOperations.ClearBinding(this, FilterPropertiesProperty);
            ItemsList = null;
            FilterProperties = null;
        }

        public static void AssignItemListPanel(ItemsControl itemsHost)
        {
            XNamespace pns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            itemsHost.ItemsPanel = Xaml.FromString<ItemsPanelTemplate>(new XDocument(
                new XElement(pns + nameof(ItemsPanelTemplate),
                    new XElement(pns + nameof(VirtualizingStackPanel)))
            ).ToString());

            itemsHost.Template = Xaml.FromString<ControlTemplate>(new XDocument(
                 new XElement(pns + nameof(ControlTemplate),
                    new XElement(pns + nameof(ScrollViewer),
                        new XAttribute(nameof(ScrollViewer.Focusable), false),
                        new XAttribute(nameof(ScrollViewer.HorizontalScrollBarVisibility), ScrollBarVisibility.Disabled),
                        new XAttribute(nameof(ScrollViewer.VerticalScrollBarVisibility), ScrollBarVisibility.Auto),
                        new XAttribute(nameof(ScrollViewer.CanContentScroll), true),
                        new XElement(pns + nameof(ItemsPresenter))))
            ).ToString());

            itemsHost.ItemTemplate = Xaml.FromString<DataTemplate>(new XDocument(
                new XElement(pns + nameof(DataTemplate),
                    new XElement(pns + nameof(CheckBoxEx),
                        new XAttribute(nameof(CheckBoxEx.IsChecked), "{Binding Selected}"),
                        new XAttribute(nameof(CheckBoxEx.Content), "{Binding Item.Name}"),
                        new XAttribute(nameof(CheckBoxEx.Style), $"{{DynamicResource FilterItemtSelectionStyle}}")))
            ).ToString());
        }
    }
}
