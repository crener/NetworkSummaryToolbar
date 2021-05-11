using System.Runtime.InteropServices;
using System.Windows;
using CSDeskBand;
using NetworkToolbar.Views;

namespace NetworkToolbarDeskBand
{
    [ComVisible(true), Guid("413318c9-a7e2-4511-b394-9c409022ba4d"), CSDeskBandRegistration(Name = "Network Summary", ShowDeskBand = true)]
    public class Deskband : CSDeskBandWpf
    {
        public Deskband()
        {
            Options.MinHorizontalSize = new DeskBandSize((int)NetworkSummary.AbsoluteMinWidth, (int)NetworkSummary.AbsoluteMinHeight);
            //Options.ContextMenuItems = ContextMenuItems;

            const double padding = 3;
            _rootVisual.Margin = new Thickness(0, padding, 0, padding);

            Options.PropertyChanged += (sender, args) =>
            {
                if(args.PropertyName == nameof(CSDeskBandOptions.VerticalSize))
                {
                    _rootVisual.Width = Options.VerticalSize.Width;
                    _rootVisual.Height = Options.VerticalSize.Height - (padding * 2);
                }
            };
        }

        protected override UIElement UIElement { get; } = new ToolbarView();

        /*private List<DeskBandMenuItem> ContextMenuItems
        {
            get
            {
                DeskBandMenuAction action = new DeskBandMenuAction("Action - Toggle submenu");
                DeskBandMenuSeparator separator = new DeskBandMenuSeparator();
                DeskBandMenuAction submenuAction = new DeskBandMenuAction("Submenu Action - Toggle checkmark");
                DeskBandMenu submenu = new DeskBandMenu("Submenu")
                {
                    Items = { submenuAction }
                };

                action.Clicked += (sender, args) => submenu.Enabled = !submenu.Enabled;
                submenuAction.Clicked += (sender, args) => submenuAction.Checked = !submenuAction.Checked;

                return new List<DeskBandMenuItem>() { action, separator, submenu };
            }
        }*/
    }
}