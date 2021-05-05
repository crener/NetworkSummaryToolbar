using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using CSDeskBand;
using CSDeskBand.ContextMenu;
using NetworkToolbar.Views;

namespace NetworkToolbarDeskBand
{
    [ComVisible(true), Guid("AA0888B3-6CCC-497C-9CE6-9218FEEDFC10"), CSDeskBandRegistration(Name = "Network Summary")]
    public class Deskband : CSDeskBandWpf
    {
        public Deskband()
        {
            Options.MinHorizontalSize = new DeskBandSize((int)NetworkSummary.AbsoluteMinWidth, (int)NetworkSummary.AbsoluteMinHeight);
            //Options.ContextMenuItems = ContextMenuItems;
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