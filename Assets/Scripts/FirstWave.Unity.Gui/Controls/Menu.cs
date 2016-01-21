using FirstWave.Messaging;
using FirstWave.Unity.Core.Input;
using FirstWave.Unity.Gui.Panels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FirstWave.Unity.Gui.Controls
{
    /// <summary>
    /// Technically this class may need to be created inside of a unity project consuming UPF.
    /// However for now, I'll keep this in here, and maybe one day ill focus on project control templating.
    /// On that day, this class will belong in here
    /// </summary>
    public class Menu : ContentControl
    {
        private Panel itemPanel;
        private IList<MenuItem> menuItems;
        private int selectedIndex;
        private string selectKey;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (value != selectedIndex)
                {
                    selectedIndex = value;
                    UpdateMenuItemSelection();
                }
            }
        }

        public MenuItem SelectedItem
        {
            get { return menuItems.FirstOrDefault(mi => mi.IsSelected); }
        }

        public string SelectKey
        {
            get { return selectKey ?? "Submit"; }
            set { selectKey = value; }
        }

        public Menu()
        {
            menuItems = new List<MenuItem>();

            itemPanel = new StackPanel();

            var border = new Border();
            border.Padding = new Thickness(10);

            border.AddChild(itemPanel);

            Child = border;

            selectedIndex = -1;
        }

        public void AddItem(string label, Action onSelect, BorderTextures textures = null)
        {
            AddItem(new MenuItem(label, onSelect, textures));
        }

        public void AddItem(MenuItem item)
        {
            menuItems.Add(item);

            itemPanel.AddChild(item);

            // Automatically select the first menu item
            if (menuItems.Count == 1)
                SelectedIndex = 0;

            Messenger.Default.SendMessage(Constants.INVALIDATE);
        }

        private void UpdateMenuItemSelection()
        {
            // First let's unselect the current menu item
            var currentSelection = menuItems.FirstOrDefault(mi => mi.IsSelected);
            if (currentSelection != null)
                currentSelection.IsSelected = false;

            // Now select the new one
            menuItems[SelectedIndex].IsSelected = true;
        }

        private void SelectNextItem()
        {
            SelectedIndex = (SelectedIndex + 1) % menuItems.Count;
        }

        private void SelectPreviousItem()
        {
            if (SelectedIndex == 0)
                SelectedIndex = menuItems.Count - 1;
            else
                SelectedIndex = SelectedIndex - 1;
        }

        private void SelectItem()
        {
            if (SelectedItem != null)
                SelectedItem.SelectedAction();
        }

        protected override void OnKeyReleased(string key)
        {
            if (key == InputManager.DOWN)
                SelectNextItem();
            else if (key == InputManager.UP)
                SelectPreviousItem();
            else if (key == SelectKey)
                SelectItem();
        }

        internal override void InvalidateLayout()
        {
            base.InvalidateLayout();

            itemPanel.InvalidateLayout();
        }
    }
}
