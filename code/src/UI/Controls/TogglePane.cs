﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Templates.UI.Extensions;
using Microsoft.Templates.UI.ViewModels.NewProject;
using Microsoft.Templates.UI.Views.NewProject;

namespace Microsoft.Templates.UI.Controls
{
    public sealed partial class TogglePane : Control
    {
        private Border _togglePaneShadowGrid;
        private Border _dragAndDropShadowBorder;
        private Grid _menuGrid;
        private Grid _mainGrid;
        private bool _isInitialized = false;

        public object TogglePaneContent
        {
            get => GetValue(TogglePaneContentProperty);
            set => SetValue(TogglePaneContentProperty, value);
        }
        public static readonly DependencyProperty TogglePaneContentProperty = DependencyProperty.Register("TogglePaneContent", typeof(object), typeof(TogglePane), new PropertyMetadata(null));

        public DataTemplate MainViewTemplate
        {
            get => (DataTemplate)GetValue(MainViewTemplateProperty);
            set => SetValue(MainViewTemplateProperty, value);
        }
        public static readonly DependencyProperty MainViewTemplateProperty = DependencyProperty.Register("MainViewTemplate", typeof(DataTemplate), typeof(TogglePane), new PropertyMetadata(null));

        public DataTemplate SecondaryViewTemplate
        {
            get => (DataTemplate)GetValue(SecondaryViewTemplateProperty);
            set => SetValue(SecondaryViewTemplateProperty, value);
        }
        public static readonly DependencyProperty SecondaryViewTemplateProperty = DependencyProperty.Register("SecondaryViewTemplate", typeof(DataTemplate), typeof(TogglePane), new PropertyMetadata(null));

        public DataTemplate OpenButtonTemplate
        {
            get => (DataTemplate)GetValue(OpenButtonTemplateProperty);
            set => SetValue(OpenButtonTemplateProperty, value);
        }
        public static readonly DependencyProperty OpenButtonTemplateProperty = DependencyProperty.Register("OpenButtonTemplate", typeof(DataTemplate), typeof(TogglePane), new PropertyMetadata(null));

        public DataTemplate CloseButtonTemplate
        {
            get => (DataTemplate)GetValue(CloseButtonTemplateProperty);
            set => SetValue(CloseButtonTemplateProperty, value);
        }
        public static readonly DependencyProperty CloseButtonTemplateProperty = DependencyProperty.Register("CloseButtonTemplate", typeof(DataTemplate), typeof(TogglePane), new PropertyMetadata(null));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(TogglePane), new PropertyMetadata(false, OnIsOpenPropertyChanged));

        public bool AllowDragAndDrop
        {
            get => (bool)GetValue(AllowDragAndDropProperty);
            set => SetValue(AllowDragAndDropProperty, value);
        }
        public static readonly DependencyProperty AllowDragAndDropProperty = DependencyProperty.Register("AllowDragAndDrop", typeof(bool), typeof(TogglePane), new PropertyMetadata(false));

        static TogglePane()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TogglePane), new FrameworkPropertyMetadata(typeof(TogglePane)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _togglePaneShadowGrid = GetTemplateChild("togglePaneShadowGrid") as Border;
            _dragAndDropShadowBorder = GetTemplateChild("dragAndDropShadowBorder") as Border;
            _menuGrid = GetTemplateChild("menuGrid") as Grid;
            _mainGrid = GetTemplateChild("mainGrid") as Grid;
            _mainGrid.GotFocus += OnMainGridGotFocus;
            MainView.Current.KeyDown += OnMainViewKeyDown;
            _isInitialized = true;
            UpdateOpenStatus();
        }

        private void OnMainViewKeyDown(object sender, KeyEventArgs e)
        {
            if (!AllowDragAndDrop)
            {
                return;
            }
            if (e.Key == Key.Space && _mainGrid.IsFocused)
            {
                if (MainViewModel.Current.SavedTemplateSetDrag(TogglePaneContent as SavedTemplateViewModel))
                {
                    _dragAndDropShadowBorder.Opacity = 1;
                }
                else
                {
                    MainViewModel.Current.SavedTemplateSetDrop(TogglePaneContent as SavedTemplateViewModel);
                }
            }
            if (e.Key == Key.Escape)
            {
                _dragAndDropShadowBorder.Opacity = 0;
            }
        }

        private void OnMainGridGotFocus(object sender, RoutedEventArgs e) => MainViewModel.Current.SavedTemplateGotFocus(TogglePaneContent as SavedTemplateViewModel);

        private void UpdateOpenStatus(bool newValue = false, bool oldValue = false)
        {
            if (_isInitialized)
            {
                if (IsOpen)
                {
                    _menuGrid.AnimateWidth(90);
                    _togglePaneShadowGrid.AnimateWidth(90);
                    _togglePaneShadowGrid.FadeIn();
                }
                else
                {
                    _menuGrid.AnimateWidth(30);
                    _togglePaneShadowGrid.AnimateWidth(30);
                    _togglePaneShadowGrid.FadeOut();
                }
            }
        }

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TogglePane;
            if (control != null)
            {
                control.UpdateOpenStatus((bool)e.NewValue, (bool)e.OldValue);
            }
        }
    }
}
