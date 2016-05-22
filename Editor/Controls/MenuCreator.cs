using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WarlordsRevengeEditor.Controls
{
    public class MenuCreator
    {
        public List<MenuItemList> CreateMenuDataStructure(Layers layers, EventHandler handler1, EventHandler handler2, EventHandler handler3, EventHandler handler4, EventHandler handler5)
        {
            var menuItems = new List<MenuItemList>();

            var subItem = new MenuItemList { Name = "&File", Items = new List<MenuItem>() };
            AddSubItem(subItem, "New", handler1);
            AddSubItem(subItem, "Open", handler2);
            AddSubItem(subItem, "Save", handler3, (Keys)Shortcut.CtrlS);
            AddSubItem(subItem, "-");
            AddSubItem(subItem, "Exit", handler4, (Keys)Shortcut.AltF4);
            menuItems.Add(subItem);

            subItem = new MenuItemList { Name = "&Layers", Items = new List<MenuItem>() };
            int cnt = 0;
            foreach (string layer in layers.GetLayers())
            {
                AddSubItem(subItem, layer, handler5, Keys.None, cnt == 0);
                cnt++;
            }
            menuItems.Add(subItem);

            subItem = new MenuItemList { Name = "&Help", Items = new List<MenuItem>() };
            var subSubItem = new MenuItem { Name = "About" };
            subItem.Items.Add(subSubItem);
            menuItems.Add(subItem);

            return menuItems;
        }

        private void AddSubItem(MenuItemList item, string name, EventHandler handler = null, Keys shortcutKeys = Keys.None, bool isChecked = false)
        {
            var subSubItem = new MenuItem { Name = name, Handler = handler, ShortcutKeys = shortcutKeys, IsChecked = isChecked };
            item.Items.Add(subSubItem);
        }

        public MenuStrip CreateMenu(List<MenuItemList> items)
        {
            var menuStrip = new MenuStrip();

            foreach (MenuItemList item in items)
            {
                var topLevelItem = new ToolStripMenuItem(item.Name);

                if (item.Items != null)
                {
                    foreach (MenuItem subItem in item.Items)
                    {
                        AddToMenuItem(topLevelItem, subItem.Name, subItem.Handler, subItem.ShortcutKeys, subItem.IsChecked);
                    }
                }

                menuStrip.Items.Add(topLevelItem);
            }

            return menuStrip;
        }

        private void AddToMenuItem(ToolStripMenuItem menuItem, string text, EventHandler handler, Keys shortcutKeys, bool isChecked)
        {
            ToolStripItem subItem;
            if (text == "-")
            {
                subItem = new ToolStripSeparator();
            }
            else
            {
                subItem = new ToolStripMenuItem(text, null, handler, shortcutKeys);
                if (isChecked)
                {
                    var l = (ToolStripMenuItem)subItem;
                    l.Checked = true;
                }

            }
            menuItem.DropDownItems.Add(subItem);
        }
    }
}

public struct MenuItemList
{
    public string Name { get; set; }
    public List<MenuItem> Items { get; set; }
}

public struct MenuItem
{
    public string Name { get; set; }
    public EventHandler Handler { get; set; }
    public EventHandler CheckedHandler { get; set; }
    public Keys ShortcutKeys { get; set; }
    public bool IsChecked { get; set; }
}