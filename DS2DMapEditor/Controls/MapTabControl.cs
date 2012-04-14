using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace RogueCastleEditor
{
    public class MapTabControl : TabControl, IControl
    {
        public MainWindow ControllerRef { get; set; }

        private TabItem m_currentTab;
        private TabItem m_gameLayerTab;

        public void Initialize()
        {
            this.SelectionChanged += ActiveLayerChanged;
            m_currentTab = this.Items[0] as TabItem;
            m_gameLayerTab = this.Items[0] as TabItem;
        }

        public void ContextMenu_ClickHandler(object sender, RoutedEventArgs e)
        {
            MenuItem itemClicked = sender as MenuItem;
            TabItem newItem = new TabItem();
            switch (itemClicked.Tag as string)
            {
                case ("AddForeground"):
                    newItem.Header = "Layer " + this.Items.Count;
                    this.Items.Add(newItem);
                    this.ControllerRef.AddLayer(true);
                    break;
                case ("AddBackground"):
                    newItem.Header = "Layer " + this.Items.Count;
                    this.Items.Insert(0, newItem);
                    this.ControllerRef.AddLayer(false);
                    break;
                case ("RemoveLayer"):
                    if (this.SelectedItem != m_gameLayerTab) // Do not allow the removal of the active layer tab.
                    {
                        if (MessageBox.Show("Are you sure you want to delete layer '" + (this.Items[SelectedIndex] as TabItem).Header + "'?\nAll layer info will be lost.", "Confirm Layer Delete", 
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            this.ControllerRef.RemoveLayer(this.SelectedIndex);
                            this.Items.Remove(m_currentTab);
                        }
                    }
                    break;
            }
        }

        public void AddLayer(string name, int index)
        {
            TabItem newItem = new TabItem();
            newItem.Header = name;
            this.Items.Insert(index, newItem);
            bool isForeground = (index > GameLayerIndex);
            this.ControllerRef.AddLayer(isForeground);
        }

        public void TabsRearranged(int indexRemoved, int insertionIndex)
        {
            this.ControllerRef.SwapObjectLists(indexRemoved, insertionIndex);
        }

        private void ActiveLayerChanged(object sender, RoutedEventArgs e)
        {
            if (m_currentTab == null)
            {
                m_currentTab = this.Items[0] as TabItem;
                m_gameLayerTab = this.Items[0] as TabItem;
            }

            TabItem tab = this.SelectedItem as TabItem;
            if (tab == null)
                tab = this.Items[0] as TabItem;
                //tab = m_gameLayerTab;

            if (m_currentTab != tab)
            {
                object tabContent = m_currentTab.Content;
                m_currentTab.Content = null;
                tab.Content = tabContent;
                m_currentTab = tab;
            }

            this.ControllerRef.SelectedLayerIndex = this.Items.IndexOf(m_currentTab);
            this.ControllerRef.ActiveLayerChanged();
        }

        public void ResetControl()
        {
            this.SelectionChanged -= ActiveLayerChanged;
            object tabContent = m_currentTab.Content;
            m_currentTab.Content = null;
            this.Items.Clear();
            TabItem gameLayer = new TabItem();
            gameLayer.Header = "Game Layer";
            this.Items.Add(gameLayer);
            m_currentTab = this.Items[0] as TabItem;
            m_gameLayerTab = this.Items[0] as TabItem;
            m_currentTab.Content = tabContent;
            this.SelectedIndex = this.GameLayerIndex; // Set the active layer to the game layer.
            this.ControllerRef.SelectedLayerIndex = this.SelectedIndex;
            this.SelectionChanged += ActiveLayerChanged;
        }

        public bool GameLayerSelected
        {
            get { return (m_currentTab == m_gameLayerTab); }
        }

        public int GameLayerIndex
        {
            get { return (Items.IndexOf(m_gameLayerTab)); }
        }
    }
}
