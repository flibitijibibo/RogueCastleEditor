using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using DS2DEngine;
using System.Windows.Input;
using System.Windows;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections;

namespace RogueCastleEditor
{
    class ObjTreeControl : ListView, IControl
    {
        public MainWindow ControllerRef { get; set; }
        public ObservableCollection<GameObj> ObjectList { get; set; }
        ListViewDragDropManager<GameObj> m_listViewDragDrop;

        private bool m_objectTreeSelected;

        public ObjTreeControl()
        {
            //Third-party code to support drag-and-drop of Object tree items.
            m_listViewDragDrop = new ListViewDragDropManager<GameObj>(this, true);
            this.SelectionMode = SelectionMode.Multiple;
            this.PreviewMouseLeftButtonUp += LeftButtonHandler_Addon;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                if (this.SelectedItem != null)
                    ObjectList.Remove(this.SelectedItem as GameObj);
            }
        }

        void LeftButtonHandler_Addon(object sender, MouseButtonEventArgs e)
        {
            m_objectTreeSelected = true;
            IList selectedItems = SelectedItems;

            ControllerRef.SelectedObjects.Clear();
            foreach (GameObj obj in selectedItems)
            {
                ControllerRef.SelectedObjects.Add(obj);
            }

            if (SelectedItems.Count == 1)
                ControllerRef.ShowObjProperties(SelectedItem as IPropertiesObj);
            else
                ControllerRef.ShowObjProperties(null);

            m_objectTreeSelected = false;
        }

        public void RefreshTreeData()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource);
            view.Refresh();
        }

        public void RefreshSelectedObjects()
        {
            if (m_objectTreeSelected == false)
                SetSelectedItems(ControllerRef.SelectedObjects);
        }

        public void DeselectAllItems()
        {
            SelectedItems.Clear();
        }

        public void SelectObjects(ObservableCollection<GameObj> list)
        {
            SetSelectedItems(list);
        }
    }
}
