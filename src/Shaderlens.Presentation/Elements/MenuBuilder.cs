namespace Shaderlens.Presentation.Elements
{
    public interface IMenuBuilder
    {
        void AddSeparator();
        void AddEmptyItem(Func<bool>? isVisible = null);
        void AddItem(object header, IInputSpan? inputSpan, object? icon, object? tooltip, Action action, Action<IMenuItemState>? setState = null);
        void AddHeader(object header, object? icon);

        IDisposable PushSubmenu(object header, IInputSpan? inputSpan, object? icon, string? tooltip, Action<IMenuItemState>? setState = null, Action? resetState = null, bool centerVertically = false);
    }

    public static class MenuBuilderExtensions
    {
        public static void AddSubmenu(this IMenuBuilder builder, IMenuSource source, object header, IInputSpan? inputSpan, object? icon, string? tooltip, Action<IMenuItemState>? setState = null, Action? resetState = null, bool centerVertically = false)
        {
            using (builder.PushSubmenu(header, inputSpan, icon, tooltip, setState, resetState, centerVertically))
            {
                source.AddTo(builder);
            }
        }
    }

    public class MenuBuilder : IMenuBuilder
    {
        private interface IMenuItem
        {
            FrameworkElement Element { get; }
            void SetState();
        }

        private interface IMenuContainer : IMenuItem
        {
            void AddItem(IMenuItem item);
        }

        private class MenuItemState : IMenuItemState
        {
            public bool IsEnabled
            {
                get { return this.item.IsEnabled; }
                set { this.item.IsEnabled = value; }
            }

            public bool IsChecked
            {
                get { return this.item.IsChecked; }
                set { this.item.IsChecked = value; }
            }

            private bool isChanged;
            public bool IsChanged
            {
                get { return this.isChanged; }
                set
                {
                    this.isChanged = value;
                    this.item.Header = GetHeader();
                }
            }

            public bool IsVisible
            {
                get { return this.item.Visibility == Visibility.Visible; }
                set { this.item.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
            }

            public object? Tooltip
            {
                get { return this.item.ToolTip; }
                set { this.item.ToolTip = value; }
            }

            private readonly MenuItem item;
            private readonly object header;
            private StackPanel? changedHeader;

            public MenuItemState(MenuItem item, object header, string? gestureText)
            {
                this.item = item;
                this.header = header;

                this.item.Header = header;
                this.item.InputGestureText = gestureText;
            }

            private object GetHeader()
            {
                if (!this.isChanged)
                {
                    return this.header;
                }

                if (this.header is string headerString)
                {
                    return headerString + "*";
                }

                if (this.changedHeader == null)
                {
                    this.changedHeader = new StackPanel { Orientation = Orientation.Horizontal }.WithChildren
                    (
                        new ContentPresenter { Content = this.header },
                        new TextBlock { Text = "*" }
                    );
                }

                return this.changedHeader;
            }
        }

        private class ContextMenuAdapter : IMenuContainer
        {
            public FrameworkElement Element { get { return this.contextMenu; } }

            private readonly ContextMenu contextMenu;
            private readonly List<IMenuItem> items;

            public ContextMenuAdapter(ContextMenu contextMenu)
            {
                this.contextMenu = contextMenu;
                this.contextMenu.Opened += (sender, e) => SetState();
                this.items = new List<IMenuItem>();
            }

            public void AddItem(IMenuItem item)
            {
                this.items.Add(item);
                this.contextMenu.Items.Add(item.Element);
            }

            public void SetState()
            {
                if (this.contextMenu.IsOpen)
                {
                    foreach (var item in this.items)
                    {
                        item.SetState();
                    }
                }
            }
        }

        private class MenuItemAdapter : IMenuContainer
        {
            public FrameworkElement Element { get { return this.menuItem; } }

            private readonly MenuItem menuItem;
            private readonly IMenuItemState state;
            private readonly Action<IMenuItemState>? setState;
            private readonly List<IMenuItem> items;

            public MenuItemAdapter(MenuItem menuItem, IMenuItemState state, Action<IMenuItemState>? setState)
            {
                this.menuItem = menuItem;
                this.menuItem.SubmenuOpened += (sender, e) => SetState();
                this.state = state;
                this.setState = setState;
                this.items = new List<IMenuItem>();
            }

            public void AddItem(IMenuItem item)
            {
                this.items.Add(item);
                this.menuItem.Items.Add(item.Element);
            }

            public void SetState()
            {
                this.setState?.Invoke(this.state);

                if (this.menuItem.IsSubmenuOpen)
                {
                    foreach (var item in this.items)
                    {
                        item.SetState();
                    }
                }
            }
        }

        private class FrameworkElementAdapter : IMenuItem
        {
            public FrameworkElement Element { get; }

            private readonly Func<bool>? isVisible;

            public FrameworkElementAdapter(FrameworkElement element, Func<bool>? isVisible)
            {
                this.Element = element;
                this.isVisible = isVisible;
            }

            public void SetState()
            {
                this.Element.Visibility = this.isVisible?.Invoke() != false ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private class SeparatorMenuItem : IMenuItem
        {
            public FrameworkElement Element { get; }

            public SeparatorMenuItem(IMenuTheme theme)
            {
                this.Element = new StyledMenuSeparator(theme);
            }

            public void SetState()
            {
                this.Element.Dispatcher.InvokeAsync(() =>
                {
                    var items = (this.Element.Parent as ItemsControl)?.Items.OfType<FrameworkElement>().ToArray() ?? Array.Empty<FrameworkElement>();

                    var itemIndex = items.IndexOf(this.Element);

                    var previousVisibleItem = items.Take(itemIndex).LastOrDefault(item => item.Visibility != Visibility.Collapsed);
                    var nextVisibleItem = items.Skip(itemIndex + 1).FirstOrDefault(item => item.Visibility != Visibility.Collapsed && item is not Separator);

                    this.Element.Visibility = previousVisibleItem != null && previousVisibleItem is not Separator && nextVisibleItem != null ? Visibility.Visible : Visibility.Collapsed;
                });
            }
        }

        private class SubmenuScopeToken : IDisposable
        {
            private readonly MenuBuilder builder;
            private readonly IMenuContainer menu;

            private bool isDisposed;

            public SubmenuScopeToken(MenuBuilder builder, IMenuContainer menu)
            {
                this.builder = builder;
                this.menu = menu;
            }

            public void Dispose()
            {
                if (this.isDisposed)
                {
                    throw new Exception("Submenu scope has already been disposed");
                }

                this.isDisposed = true;
                this.builder.PopScope(this.menu);
            }
        }

        private readonly ContextMenu contextMenu;
        private readonly IMenuTheme theme;
        private readonly ContextMenuAdapter rootMenu;
        private readonly Stack<IMenuContainer> menuStack;
        private readonly InputDisplayNameFormatter inputFormatter;

        private IMenuContainer menu;

        public MenuBuilder(ContextMenu contextMenu, IMenuTheme theme)
        {
            this.contextMenu = contextMenu;
            this.theme = theme;
            this.rootMenu = new ContextMenuAdapter(contextMenu);
            this.contextMenu.Opened += (sender, e) => this.rootMenu.SetState();
            this.menuStack = new Stack<IMenuContainer>();
            this.inputFormatter = new InputDisplayNameFormatter(new InputValueSerializer());

            this.menu = this.rootMenu;
        }

        public void AddSeparator()
        {
            this.menu.AddItem(new SeparatorMenuItem(this.theme));
        }

        public void AddEmptyItem(Func<bool>? isVisible = null)
        {
            var menuItem = new StyledMenuItem(this.theme) { Header = "(empty)", IsEnabled = false };
            this.menu.AddItem(new FrameworkElementAdapter(menuItem, isVisible));
        }

        public void AddItem(object header, IInputSpan? inputSpan, object? icon, object? tooltip, Action action, Action<IMenuItemState>? setState = null)
        {
            var gestureText = inputSpan != null ? this.inputFormatter.GetDisplayName(inputSpan) : null;

            var menuItem = new StyledMenuItem(this.theme) { Icon = icon, ToolTip = tooltip };

            var state = new MenuItemState(menuItem, header, gestureText);

            menuItem.PreviewMouseUp += (sender, e) =>
            {
                var isChecked = state.IsChecked;
                action();
                this.rootMenu.SetState();
                var keepOpen = state.IsChecked != isChecked && e.ChangedButton != MouseButton.Right;
                e.Handled = keepOpen;
                this.contextMenu.IsOpen = keepOpen;
            };

            menuItem.PreviewKeyDown += (sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    action();
                }

                if (e.Key == Key.Space)
                {
                    var isChecked = state.IsChecked;
                    action();
                    this.rootMenu.SetState();
                    e.Handled = state.IsChecked != isChecked;
                }
            };

            this.menu.AddItem(new MenuItemAdapter(menuItem, state, setState));
        }

        public void AddHeader(object header, object? icon)
        {
            var menuHeader = new StyledMenuItem(new MenuHeaderStyle(this.theme)) { Header = header, Icon = icon };
            this.menu.AddItem(new FrameworkElementAdapter(menuHeader, null));
        }

        public IDisposable PushSubmenu(object header, IInputSpan? inputSpan, object? icon, string? tooltip, Action<IMenuItemState>? setState = null, Action? resetState = null, bool centerVertically = false)
        {
            var gestureText = inputSpan != null ? this.inputFormatter.GetDisplayName(inputSpan) : null;

            var menuItem = new StyledMenuItem(this.theme) { Icon = icon, ToolTip = tooltip };

            var state = new MenuItemState(menuItem, header, gestureText);

            menuItem.PreviewMouseUp += (sender, e) =>
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    var isChanged = state.IsChanged;
                    resetState?.Invoke();
                    this.rootMenu.SetState();
                    this.contextMenu.IsOpen = state.IsChanged != isChanged;
                }
            };

            if (centerVertically)
            {
                MenuVerticalCenterBehavior.Register(menuItem, 9);
            }

            var submenu = new MenuItemAdapter(menuItem, state, setState);
            this.menu.AddItem(submenu);

            this.menuStack.Push(this.menu);
            this.menu = submenu;

            return new SubmenuScopeToken(this, submenu);
        }

        private void PopScope(IMenuContainer menu)
        {
            if (this.menu != menu)
            {
                throw new Exception("Cannot restore submenu scope, submenus were disposed out of order");
            }

            this.menu = this.menuStack.Pop();
        }
    }
}
