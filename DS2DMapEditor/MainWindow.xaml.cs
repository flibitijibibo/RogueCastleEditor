﻿#region File Description
//-----------------------------------------------------------------------------
// MainWindow.xaml.cs
//
// Copyright 2011, Nick Gravelyn.
// Licensed under the terms of the Ms-PL: http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Windows.Input;
using SpriteSystem;
using System.Collections.Generic;
using DS2DEngine;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Xml;
using System.ComponentModel;
using System.Windows.Interop;

namespace RogueCastleEditor
{
    public partial class MainWindow : Window
    {
        private List<string> m_spritesheetNameList;
        private List<ObservableCollection<GameObj>> m_objectList;

        public int SelectedLayerIndex = 0;

        private bool m_ctrlHeld = false;

        public MainWindow()
        {   
            InitializeComponent();
            this.WindowState = System.Windows.WindowState.Maximized;

            m_spritesheetNameList = new List<string>();
            XAML_spriteList.DataContext = m_spritesheetNameList;
            SpriteLibrary.Init();

            XAML_toolBarControl.ControllerRef = this;
            XAML_mapDisplayXnaControl.ControllerRef = this;
            XAML_objTreeControl.ControllerRef = this;
            XAML_propertiesControl.ControllerRef = this;
            XAML_spriteList.ControllerRef = this;
            XAML_spriteDisplayXnaControl.ControllerRef = this;
            XAML_mapTabControl.ControllerRef = this;
            XAML_enemyDisplayXnaControl.ControllerRef = this;
            UndoManager.ControllerRef = this;

            //MapTabControl needs to be initialized.
            XAML_mapTabControl.Initialize();

            //Linking a generic list of GameObjs to the map display and object tree.
            //It is an observable collection so that changes from either side are reflected to the other.
            m_objectList = new List<ObservableCollection<GameObj>>();
            m_objectList.Add(new ObservableCollection<GameObj>());
            XAML_objTreeControl.ObjectList = m_objectList[0];
            XAML_objTreeControl.ItemsSource = XAML_objTreeControl.ObjectList;
            XAML_mapDisplayXnaControl.ObjectList = XAML_objTreeControl.ObjectList;

            LoadConfigXML();

            this.Closing += ApplicationClosing;
            this.Loaded += OnLoad;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            HwndSource src = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            src.AddHook(new HwndSourceHook(WndProc));

            AddBackgroundLayers();
        }

        private void AddBackgroundLayers()
        {
            XAML_mapTabControl.AddLayer("Castle Layer", 1);
            XAML_mapTabControl.AddLayer("Garden Layer", 2);
            XAML_mapTabControl.AddLayer("Dungeon Layer", 3);
            XAML_mapTabControl.AddLayer("Tower Layer", 4);
        }

        //Forces mouse wheel input even if the focus is not on the XnaControl.
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // listen for messages that are meant for a hosted Win32 window.
            if (msg == NativeMethods.WM_MOUSEWHEEL) // WM_MOUSEWHEEL
            {
                XAML_mapDisplayXnaControl.ForceMouseWheelInput(wParam);
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void ApplicationClosing(object sender, CancelEventArgs e)
        {
            if (ChangeMade == true)
            {
                string msg = "Unsaved changes detected.\nAre you sure you want to close the application?";
                MessageBoxResult result = MessageBox.Show(msg, "Close Rogue Castle Editor", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    // If user doesn't want to close, cancel closure
                    e.Cancel = true;
                }
            }
        }

        // Sets the map display to respond to the specified tool on the tool bar.
        private void ToolBar_Clicked(object sender, RoutedEventArgs e)
        {
            XAML_mapDisplayXnaControl.SetTool(XAML_toolBarControl.ToolBar_Clicked(sender, e));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Escape) 
            {
                XAML_mapDisplayXnaControl.SelectedObjects.Clear();
                XAML_objTreeControl.DeselectAllItems();
            }

            if (e.Key == Key.F5)
                LoadLevel();
            else if (e.Key == Key.F6)
                LoadLevel("-t");
            else if (e.Key == Key.F7)
                LoadLevel("-d");
            else if (e.Key == Key.F8)
                LoadLevel("-g");

            // Special key handling for saving.
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                m_ctrlHeld = true;

            // Special key handling for saving.
            if (m_ctrlHeld == true && e.Key == Key.S && e.IsRepeat == false)
                SaveButton_Clicked(null, null);

            // Check to see if the user pressed any of the hotkeys.
            if (e.IsRepeat == false && m_ctrlHeld == false)
            {
                if (XAML_propertiesControl.IsKeyboardFocusWithin == false) // Prevents hotkeys from registering while you are typing in the properties boxes.
                    ToolHotkey_Clicked(e);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                m_ctrlHeld = false;
        }

        private void ToolHotkey_Clicked(KeyEventArgs e)
        {
            ToggleButton sender = null;
            switch (e.Key)
            {
                case (Key.Q):
                    sender = XAML_ToolBarRect;
                    break;
                case (Key.W):
                    sender = XAML_ToolbarRotate;
                    break;
                case(Key.E):
                    sender = XAML_ToolbarScale;
                    break;
                //case(Key.V):
                //    sender = XAML_ToolbarPlayer;
                //    break;
                case(Key.R):
                    sender = XAML_ToolBarRoom;
                    break;
            }

            if (sender != null && sender.IsChecked == false)
            {
                sender.IsChecked = true;
                ToolBar_Clicked(sender, null);
            }
        }

        private void ResetZoomTool_Clicked(object sender, RoutedEventArgs e)
        {
            XAML_resetZoomTool.IsChecked = false;
            XAML_mapDisplayXnaControl.ResetZoom();
        }

        public void ResetAllTools()
        {
            XAML_toolBarControl.ResetAllTools();
        }

        //////////////FILE MENU METHODS//////////////
        private void DisplayGrid_Clicked(object sender, RoutedEventArgs e)
        {
            XAML_mapDisplayXnaControl.GridVisible = XAML_displayGridMenuItem.IsChecked;
        }

        private void SnapGrid_Clicked(object sender, RoutedEventArgs e)
        {
            XAML_mapDisplayXnaControl.SnapToGrid = XAML_snapGridMenuItem.IsChecked;
        }

        private void SelectCollHulls_Clicked(object sender, RoutedEventArgs e)
        {
            XAML_mapDisplayXnaControl.SelectCollHulls = XAML_selectCollHullsItem.IsChecked;
        }

        private void SelectSprites_Clicked(object sender, RoutedEventArgs e)
        {
            XAML_mapDisplayXnaControl.SelectSpriteObjs = XAML_selectSpriteObjsItem.IsChecked;
        }

        private void DisplayCollHulls_Clicked(object sender, RoutedEventArgs e)
        {
            XAML_mapDisplayXnaControl.DisplayCollHulls = XAML_displayCollHullsItem.IsChecked;
        }

        private void DisplaySprites_Clicked(object sender, RoutedEventArgs e)
        {
            XAML_mapDisplayXnaControl.DisplaySpriteObjs = XAML_displaySpriteObjsItem.IsChecked;
        }
       
        private void Directories_Clicked(object sender, RoutedEventArgs e)
        {
            DirectoriesWindow window = new DirectoriesWindow(this);
            window.ShowDialog();
        }
        /////////////////////////////////////////////

        //////////////////////////// METHODS USED TO LINK BETWEEN TWO CONTROLS ///////////////////////
        //Not being used yet.
        private void XAML_spriteDisplayScroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
           // Console.WriteLine(e.VerticalOffset);
        }

        //Shows the property data for the currently selected object in the PropertiesControl.
        //Called by MapDisplayXnaControl and ObjTree whenever an item is selected.
        public void ShowObjProperties(IPropertiesObj obj)
        {
            XAML_propertiesControl.ShowObjProperties(obj);
        }

        //Refreshes the name data in ObjTreeControl.
        //Called by PropertiesControl whenever you change the name of the item.
        public void RefreshObjTreeData()
        {
            XAML_objTreeControl.RefreshTreeData();
        }

        //Refreshes all selected items in the ObjTreeControl.
        //Called by MapDisplayXnaControl whenever an object is selected from the map, and not from the ObjTree.
        public void RefreshObjTreeSelectedObjects()
        {
            XAML_objTreeControl.RefreshSelectedObjects();
        }

        //Displays all loaded sprite directories into SpriteListControl.
        //Called whenever you close DirectoriesWindow. Also called when initializing SpriteDisplayXnaControl.
        public void LoadSpriteDirectories()
        {
            XAML_spriteList.LoadSpriteDirectories(XAML_spriteDisplayXnaControl.GraphicsDevice);
        }

        //Displays all loaded sprites in SpriteDisplayXnaControl
        //Called from SpriteListControl, whenever someone clicks an item in the list.
        public void DisplaySprites(List<string> charDataList, List<string> spriteDataList)
        {
            XAML_spriteDisplayXnaControl.LoadSprites(charDataList, spriteDataList);
        }

        //Refreshes the property data of a selected object in PropertiesControl.
        //Called from MapDisplayXnaControl, whenever you move your mouse on a selected object.
        public void RefreshPropertiesData()
        {
            XAML_propertiesControl.RefreshPropertiesData();
        }

        // Adds a sprite to the MapDisplayXnaControl.
        // Called from SpriteDisplayXnaControl. Whenever you click a sprite it places one on the map.
        public void AddSprite(GameObj obj)
        {
            // A hack to set the tool to selection after adding a sprite to the stage.
            XAML_toolBarControl.ResetAllTools();
            (XAML_toolBarControl.Children[0] as ToggleButton).IsChecked = true;
            XAML_mapDisplayXnaControl.SetTool(Consts.TOOLTYPE_SELECTION);
            //////////////////////////////////////////////////////////////////////////
            XAML_mapDisplayXnaControl.AddSprite(obj);
        }

        // Called whenever a button on the map tabcontrol is clicked.
        private void MapTabControl_Click(object sender, RoutedEventArgs e)
        {
            XAML_mapTabControl.ContextMenu_ClickHandler(sender, e);
        }

        // Called from MapTabControl whenever a layer is added.
        public void AddLayer(bool isForeground)
        {
            if (isForeground == true)
            {
                m_objectList.Add(new ObservableCollection<GameObj>());
            }
            else
            {
                m_objectList.Insert(0, new ObservableCollection<GameObj>());
            }
        }

        // Called from MapTabControl whenever a layer is removed.
        // Removes all objects associated with that layer.
        public void RemoveLayer(int indexRemoved)
        {
            m_objectList.RemoveAt(indexRemoved);
        }

        // Called from MapTabControl whenever the actively selected layer has changed.
        public void ActiveLayerChanged()
        {
            XAML_mapDisplayXnaControl.ClearAllSelectedObjs(); // Clears all selected objects in the map display.
            XAML_objTreeControl.ObjectList = m_objectList[SelectedLayerIndex];
            XAML_objTreeControl.ItemsSource = XAML_objTreeControl.ObjectList;
            XAML_mapDisplayXnaControl.ObjectList = XAML_objTreeControl.ObjectList;
        }

        // Called from MapTabControl whenever tabs are swapped positions.
        public void SwapObjectLists(int indexRemoved, int insertionIndex)
        {
            ObservableCollection<GameObj> listRemoved = m_objectList[indexRemoved];
            m_objectList.RemoveAt(indexRemoved);
            m_objectList.Insert(insertionIndex, listRemoved);
        }

        // Sets the focus to the map display.
        public void SetFocusToMapDisplay()
        {
            XAML_mapDisplayXnaControl.Focus();
        }

        private void NewButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ChangeMade == true)
            {
                if (MessageBox.Show("Are you sure you want to close the current map?\nAll unsaved data will be lost.", "Confirm New Map",
                               MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    ClearProject();
            }
            else
                ClearProject();
        }

        public void ClearProject()
        {
            EditorConfig.SaveDirectory = "";
            XAML_mapDisplayXnaControl.ResetControl();
            XAML_mapTabControl.ResetControl();
            m_objectList.Clear();
            m_objectList.Add(new ObservableCollection<GameObj>());
            XAML_objTreeControl.ObjectList = m_objectList[0];
            XAML_objTreeControl.ItemsSource = XAML_objTreeControl.ObjectList;
            XAML_mapDisplayXnaControl.ObjectList = XAML_objTreeControl.ObjectList;
            ChangeMade = false;
            ChangeTitle("Rogue Castle Editor - New Map");
            UndoManager.ResetManager(); //Disabled for now because it was causing diposed objects to be disposed again.

            AddBackgroundLayers(); // Adds the Castle, Garden, Etc. layers.
        }

        public void ChangeTitle(string newTitle)
        {
            this.Title = newTitle;
        }

        public void LoadLevel(string levelTypeArg = "")
        {
            if (EditorConfig.ExecutableDirectory == "")
            {
                OutputControl.Trace("ERROR: Cannot run Game. Please specify the file path for the Executable Directory under Project > Directories");
            }
            else
            {
                if (!File.Exists("CompileGame.bat"))
                    CreateBatchFile();

                try
                {
                    using (Process batchProcess = Process.Start("CompileGame.bat"))
                    {
                        batchProcess.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not run batch file. Original Error: " + ex.Message);
                }

                //Creating a temporary XML file to load into DS2D.
                string tempFilePath = Directory.GetCurrentDirectory() + "tempLevel.xml";
                SaveXMLFile(tempFilePath, false);

                int lastSlashIndex= EditorConfig.ExecutableDirectory.LastIndexOf("\\") + 1;
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = EditorConfig.ExecutableDirectory.Substring(lastSlashIndex);
                startInfo.WorkingDirectory = EditorConfig.ExecutableDirectory.Substring(0, lastSlashIndex);
                startInfo.Arguments = tempFilePath + levelTypeArg;
                try
                {
                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        exeProcess.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not load DS2D.exe.  Original Error: " + ex.Message);
                }
            }
        }

        private void CreateBatchFile()
        {
            StreamWriter sw = new StreamWriter("CompileGame.bat");
            sw.WriteLine("\"C:\\Program Files (x86)\\Microsoft Visual Studio 10.0\\Common7\\IDE\\vcsexpress.exe\" " + EditorConfig.ExecutableDirectory.Substring(0, EditorConfig.ExecutableDirectory.IndexOf("DS2D") + 5) + "DS2D.sln /build");
            sw.Close();
        }

        // CODE FOR SAVING A LOADING FILES
        public void SaveButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (EditorConfig.SaveDirectory == "")
                SaveAsButton_Clicked(sender, e);
            else
                SaveXMLFile(EditorConfig.SaveDirectory);

            this.ChangeMade = false;
        }

        private void SaveAsButton_Clicked(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            if (EditorConfig.SaveDirectory == "")
                saveDialog.FileName = EditorConfig.SaveDirectory;
            else
            {
                int lastIndex = (EditorConfig.SaveDirectory.LastIndexOf("\\"));
                saveDialog.FileName = EditorConfig.SaveDirectory.Substring(lastIndex + 1);
            }
            saveDialog.DefaultExt = ".xml";
            saveDialog.Filter = "XML file (.xml) |*.xml";
            saveDialog.RestoreDirectory = false;
            saveDialog.Title = "Save As...";
            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    EditorConfig.SaveDirectory = saveDialog.FileName;
                    SaveXMLFile(saveDialog.FileName);
                    SaveConfigXML();
                    ChangeTitle("Rogue Castle Editor - " + EditorConfig.SaveDirectory);
                    this.ChangeMade = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ERROR: Could not write to disk. Original error: " + ex.Message);
                }
            }
        }

        //Exports all data into an XML doc.
        private void SaveXMLFile(string filePath, bool outputSaveTrace = true)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            //settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlWriter writer = XmlWriter.Create(filePath, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Map");

            //This code takes all spritesheets used in the editor and saves them in a list so that the level knows what art assets to load.
            List<string> spritesheetList = new List<string>();
            foreach (ObservableCollection<GameObj> objList in this.GlobalLayerList)
            {
                foreach (GameObj obj in objList)
                {
                    if (obj is MapObjContainer)
                    {
                        foreach (string spritesheetName in (obj as MapObjContainer).SpritesheetNameList)
                        {
                            int lastIndexSlash = spritesheetName.LastIndexOf("\\");
                            string sheetName = spritesheetName.Substring(lastIndexSlash + 1, spritesheetName.LastIndexOf(".") - lastIndexSlash - 1);
                        
                            if (!spritesheetList.Contains(sheetName))
                                spritesheetList.Add(sheetName);
                        }
                    }
                    else if (obj is MapSpriteObj)
                    {
                        string sheetName = (obj as MapSpriteObj).SpritesheetName;
                        int lastIndexSlash = sheetName.LastIndexOf("\\");
                        sheetName = sheetName.Substring(lastIndexSlash + 1, sheetName.LastIndexOf(".") - lastIndexSlash - 1);

                        if (!spritesheetList.Contains(sheetName))
                            spritesheetList.Add(sheetName);
                    }
                }
            }
            /////////////////////////////////////////////////

            foreach (string sheetName in spritesheetList)
            {
                writer.WriteStartElement("Spritesheet");
                writer.WriteAttributeString("Name", sheetName);
                writer.WriteEndElement();
            }

            foreach (RoomObj room in XAML_mapDisplayXnaControl.RoomObjectList)
            {
                writer.WriteStartElement("RoomObject");
                writer.WriteAttributeString("Name", room.Name);
                writer.WriteAttributeString("X", room.X.ToString());
                writer.WriteAttributeString("Y", room.Y.ToString());
                writer.WriteAttributeString("Width", room.Width.ToString());
                writer.WriteAttributeString("Height", room.Height.ToString());
                writer.WriteAttributeString("ScaleX", room.ScaleX.ToString());
                writer.WriteAttributeString("ScaleY", room.ScaleY.ToString());
                writer.WriteAttributeString("Tag", room.Tag);
                writer.WriteAttributeString("SelectionMode", room.SelectionMode.ToString());
                writer.WriteAttributeString("DisplayBG", room.DisplayBG.ToString());

                writer.WriteAttributeString("CastlePool", room.AddToCastlePool.ToString());
                writer.WriteAttributeString("GardenPool", room.AddToGardenPool.ToString());
                writer.WriteAttributeString("TowerPool", room.AddToTowerPool.ToString());
                writer.WriteAttributeString("DungeonPool", room.AddToDungeonPool.ToString());

                foreach (GameObj obj in room.TouchingObjList)
                {
                    string type = "GameObj";
                    string doorPos = "NULL";
                    string spriteName = ""; // Only for storing the sprite name if the object is a sprite. This game doesn't need this value, only the editor does.

                    if (obj is CollHullObj)
                    {
                        type = "CollHullObj";
                        if ((obj as CollHullObj).IsTrigger == true)
                        {
                            type = "DoorObj";

                            if ((obj as CollHullObj).IsBossDoor == false)
                            {
                                if (obj.X == room.X)
                                    doorPos = "Left";
                                else if (obj.Y == room.Y)
                                    doorPos = "Top";
                                else if ((obj.X + obj.Width) == (room.X + room.Width))
                                    doorPos = "Right";
                                else
                                    doorPos = "Bottom";
                            }
                            else
                                doorPos = "None";
                        }
                        else if ((obj as CollHullObj).IsChest == true)
                            type = "ChestObj";
                        else if ((obj as CollHullObj).IsHazard == true)
                            type = "HazardObj";
                        else if ((obj as CollHullObj).IsBorder == true)
                            type = "BorderObj";
                    }
                    else if (obj is EnemyMapObject)
                    {
                        type = "EnemyObj";
                        spriteName = (obj as EnemyMapObject).SpriteName;
                    }
                    else if (obj is EnemyOrbObj)
                    {
                        type = "EnemyOrbObj";
                    }
                    else if (obj is MapSpriteObj)
                    {
                        if ((obj as MapSpriteObj).IsCollidable == true || (obj as MapSpriteObj).IsWeighted == true)
                            type = "PhysicsObj";
                        else
                            type = "SpriteObj";
                    }
                    else if (obj is MapObjContainer)
                    {
                        if ((obj as MapObjContainer).IsCollidable == true || (obj as MapObjContainer).IsWeighted == true || (obj as MapObjContainer).Breakable == true)
                            type = "PhysicsObjContainer";
                        else
                            type = "ObjContainer";
                    }
                    else if (obj is PlayerStartObj)
                        type = "PlayerStartObj";

                    writer.WriteStartElement("GameObject");
                    writer.WriteAttributeString("Type", type);
                    if (spriteName != "")
                        writer.WriteAttributeString("SpriteName", (obj as MapObjContainer).SpriteName);
                    if (obj is EnemyMapObject)
                    {
                        EnemyMapObject enemy = obj as EnemyMapObject;
                        writer.WriteAttributeString("Procedural", enemy.Procedural.ToString());
                        writer.WriteAttributeString("EnemyType", enemy.Type.ToString());
                        writer.WriteAttributeString("Difficulty", enemy.Difficulty.ToString());
                        writer.WriteAttributeString("InitialDelay", enemy.InitialLogicDelay.ToString());
                    }
                    writer.WriteAttributeString("Name", obj.Name);
                    writer.WriteAttributeString("X", obj.X.ToString());
                    writer.WriteAttributeString("Y", obj.Y.ToString());
                    writer.WriteAttributeString("Width", obj.Width.ToString());
                    writer.WriteAttributeString("Height", obj.Height.ToString());
                    writer.WriteAttributeString("ScaleX", obj.ScaleX.ToString());
                    writer.WriteAttributeString("ScaleY", obj.ScaleY.ToString());
                    writer.WriteAttributeString("Rotation", obj.Rotation.ToString());
                    writer.WriteAttributeString("Flip", (obj.Flip == SpriteEffects.FlipHorizontally).ToString());
                    writer.WriteAttributeString("Tag", obj.Tag);

                    if (type == "DoorObj")
                    {
                        writer.WriteAttributeString("DoorPos", doorPos);
                        writer.WriteAttributeString("BossDoor", (obj as CollHullObj).IsBossDoor.ToString());
                    }

                    if (type == "CollHullObj" || type == "BorderObj")
                    {
                        CollHullObj collHull = obj as CollHullObj;
                        writer.WriteAttributeString("CollidesTop", collHull.CollidesTop.ToString());
                        writer.WriteAttributeString("CollidesBottom", collHull.CollidesBottom.ToString());
                        writer.WriteAttributeString("CollidesLeft", collHull.CollidesLeft.ToString());
                        writer.WriteAttributeString("CollidesRight", collHull.CollidesRight.ToString());
                    }

                    if (type == "EnemyOrbObj")
                    {
                        writer.WriteAttributeString("OrbType", (obj as EnemyOrbObj).OrbType.ToString());
                        writer.WriteAttributeString("ForceFlying", (obj as EnemyOrbObj).ForceFlying.ToString());
                        writer.WriteAttributeString("IsWaypoint", (obj as EnemyOrbObj).IsWaypoint.ToString());
                    }

                    if (type == "SpriteObj" || type == "PhysicsObj")
                    {
                        writer.WriteAttributeString("SpriteName", (obj as MapSpriteObj).SpriteName);
                        writer.WriteAttributeString("BGLayer", (obj as MapSpriteObj).OnBGLayer.ToString());
                        if (type == "PhysicsObj")
                        {
                            writer.WriteAttributeString("Weighted", (obj as MapSpriteObj).IsWeighted.ToString());
                            writer.WriteAttributeString("Collidable", (obj as MapSpriteObj).IsCollidable.ToString());
                        }
                    }

                    if (type == "PhysicsObjContainer" || type == "ObjContainer")
                    {
                        writer.WriteAttributeString("SpriteName", (obj as MapObjContainer).SpriteName);
                        writer.WriteAttributeString("Weighted", (obj as MapObjContainer).IsWeighted.ToString());
                        writer.WriteAttributeString("Collidable", (obj as MapObjContainer).IsCollidable.ToString());
                        writer.WriteAttributeString("Breakable", (obj as MapObjContainer).Breakable.ToString());
                        writer.WriteAttributeString("BGLayer", (obj as MapObjContainer).OnBGLayer.ToString());
                    }

                    if (type == "ChestObj")
                        writer.WriteAttributeString("Fairy", (obj as CollHullObj).IsFairyChest.ToString());

                    if (obj is IPropertiesObj)
                        writer.WriteAttributeString("LevelType", (obj as IPropertiesObj).LevelType.ToString());
                    
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            writer.WriteEndDocument();

            writer.Flush();
            writer.Close();

            if (outputSaveTrace == true)
                OutputControl.Trace("File saved successfully");
        }

        public void SaveConfigXML()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            //settings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlWriter writer = XmlWriter.Create(Directory.GetCurrentDirectory() + @"\DS2DEdConfig.xml", settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Config");

            writer.WriteStartElement("SpriteDirectory");
            writer.WriteAttributeString("name", EditorConfig.SpriteDirectory);
            writer.WriteEndElement();

            writer.WriteStartElement("ExecutableDirectory");
            writer.WriteAttributeString("name", EditorConfig.ExecutableDirectory);
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        private void OpenButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (EditorConfig.SpriteDirectory == "")
            {
                OutputControl.Trace("Could not load file. Please specify the file path for the Spritesheet Directory under Project > Directories");
            }
            else
            {
                if (ChangeMade == true)
                {
                    if (MessageBox.Show("Are you sure you want to close the current map?\nAll unsaved data will be lost.", "Confirm Open Map",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        OpenFileDialog openDialog = new OpenFileDialog();
                        openDialog.FileName = "*.xml";
                        openDialog.DefaultExt = ".xml";
                        openDialog.Filter = "XML file (.xml) |*.xml";
                        openDialog.RestoreDirectory = false;
                        openDialog.Title = "Open File...";
                        if (openDialog.ShowDialog() == true)
                        {
                            try
                            {
                                LoadXMLFile(openDialog.FileName);
                                ChangeTitle("Rogue Castle Editor - " + EditorConfig.SaveDirectory);
                                this.ChangeMade = false;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("ERROR: Could not load file. Original error: " + ex.Message);
                            }
                        }
                    }
                }
                else
                {
                    OpenFileDialog openDialog = new OpenFileDialog();
                    openDialog.FileName = "*.xml";
                    openDialog.DefaultExt = ".xml";
                    openDialog.Filter = "XML file (.xml) |*.xml";
                    openDialog.RestoreDirectory = false;
                    openDialog.Title = "Open File...";
                    if (openDialog.ShowDialog() == true)
                    {
                        //try
                        {
                            LoadXMLFile(openDialog.FileName);
                            ChangeTitle("Rogue Castle Editor - " + EditorConfig.SaveDirectory);
                            this.ChangeMade = false;
                        }
                        //catch (Exception ex)
                        {
                          //  MessageBox.Show("ERROR: Could not load file. Original error: " + ex.Message);
                        }
                    }
                }
            }
        }

        public void LoadXMLFile(string filePath)
        {
            ClearProject();
            EditorConfig.SaveDirectory = filePath;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;

            // try
            {
                XmlReader reader = XmlReader.Create(filePath, settings);
                int currentLayerIndex = 0;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "RoomObject")
                        {
                            RoomObj newRoom = new RoomObj(0, 0, 0, 0, XAML_mapDisplayXnaControl, this);
                            ParseGenericXML(newRoom, reader);
                            this.XAML_mapDisplayXnaControl.AddSprite(newRoom, true, true);
                        }
                        else if (reader.Name == "Layer")
                        {
                            reader.MoveToAttribute("Name");
                            string layerName = reader.Value;
                            reader.MoveToAttribute("Index");
                            currentLayerIndex = int.Parse(reader.Value);
                            if (layerName != "Game Layer") // Do not add the game layer. It is automatically added whenever the project is cleared, which happens at the start of this load function.
                                XAML_mapTabControl.AddLayer(layerName, currentLayerIndex);
                            XAML_mapTabControl.SelectedIndex = currentLayerIndex; // Set the active layer to the currently added layer.
                        }
                        else if (reader.Name == "GameObject")
                        {
                            reader.MoveToAttribute("Type");
                            string type = reader.Value;
                            string spriteName = "";
                            if (reader.MoveToAttribute("SpriteName"))
                                spriteName = reader.Value;

                            GameObj newObj = null;
                            switch (type)
                            {
                                case ("CollHullObj"):
                                    newObj = new CollHullObj(0, 0, 0, 0);
                                    break;
                                case("DoorObj"):
                                case ("TriggerObj"):
                                    newObj = new CollHullObj(0, 0, 0, 0) { IsTrigger = true };
                                    if (reader.MoveToAttribute("BossDoor"))
                                        (newObj as CollHullObj).IsBossDoor = bool.Parse(reader.Value);
                                    break;
                                case ("ChestObj"):
                                    newObj = new CollHullObj(0, 0, 0, 0) { IsChest = true };
                                    if (reader.MoveToAttribute("Fairy"))
                                        (newObj as CollHullObj).IsFairyChest = bool.Parse(reader.Value);
                                    break;
                                case ("HazardObj"):
                                    newObj = new CollHullObj(0, 0, 0, 0) { IsHazard = true };
                                    break;
                                case ("BorderObj"):
                                    newObj = new CollHullObj(0, 0, 0, 0) { IsBorder = true };
                                    break;
                                case ("EnemyOrbObj"):
                                    newObj = new EnemyOrbObj();
                                    break;
                                case ("EnemyObj"):
                                    newObj = new EnemyMapObject(spriteName);
                                    EnemyMapObject enemy = newObj as EnemyMapObject;
                                    if (enemy != null)
                                    {
                                        if (reader.MoveToAttribute("Procedural"))
                                            enemy.Procedural = bool.Parse(reader.Value);
                                        if (reader.MoveToAttribute("EnemyType"))
                                        {
                                            //byte fixedValue = FixEnemyTypesString(reader.Value);
                                            //enemy.Type = fixedValue;
                                            enemy.Type = byte.Parse(reader.Value);
                                        }
                                        if (reader.MoveToAttribute("Difficulty"))
                                        {
                                            string enumValue = reader.Value;
                                            if (Enum.IsDefined(typeof(EnemyDifficulty), enumValue))
                                                enemy.Difficulty = (EnemyDifficulty)Enum.Parse(typeof(EnemyDifficulty), enumValue, true);
                                        }
                                        if (reader.MoveToAttribute("InitialDelay"))
                                            enemy.InitialLogicDelay = float.Parse(reader.Value);
                                        foreach (EnemyMapObject enemyObj in XAML_enemyDisplayXnaControl.EnemyList)
                                        {
                                            if (enemy.Type == enemyObj.Type)
                                            {
                                                enemy.BasicScale = enemyObj.BasicScale;
                                                enemy.AdvancedScale = enemyObj.AdvancedScale;
                                                enemy.ExpertScale = enemyObj.ExpertScale;
                                                enemy.MinibossScale = enemyObj.MinibossScale;
                                                switch (enemy.Difficulty)
                                                {
                                                    case (EnemyDifficulty.BASIC):
                                                        enemy.TextureColor = Color.White;
                                                        enemy.Scale = enemy.BasicScale;
                                                        break;
                                                    case (EnemyDifficulty.ADVANCED):
                                                        enemy.TextureColor = Color.Yellow;
                                                        enemy.Scale = enemy.AdvancedScale;
                                                        break;
                                                    case (EnemyDifficulty.EXPERT):
                                                        enemy.TextureColor = Color.Orange;
                                                        enemy.Scale = enemy.ExpertScale;
                                                        break;
                                                    case (EnemyDifficulty.MINIBOSS):
                                                        enemy.TextureColor = Color.Red;
                                                        enemy.Scale = enemy.MinibossScale;
                                                        break;
                                                }
                                                break;
                                            }
                                        }
                                        if (reader.MoveToAttribute("Flip"))
                                        {
                                            if (bool.Parse(reader.Value) == true)
                                                enemy.Flip = SpriteEffects.FlipHorizontally;
                                        }
                                    }
                                    break;
                                case ("PhysicsObjContainer"):
                                case ("ObjContainer"):
                                    newObj = new MapObjContainer(spriteName);
                                    if (reader.MoveToAttribute("Collidable"))
                                        (newObj as IPhysicsObj).IsCollidable = bool.Parse(reader.Value);
                                    if (reader.MoveToAttribute("Weighted"))
                                        (newObj as IPhysicsObj).IsWeighted = bool.Parse(reader.Value);
                                    if (reader.MoveToAttribute("Breakable"))
                                        (newObj as MapObjContainer).Breakable = bool.Parse(reader.Value);
                                    break;
                                case ("PhysicsObj"):
                                case ("SpriteObj"):
                                    newObj = new MapSpriteObj(spriteName);
                                    if (reader.MoveToAttribute("Collidable"))
                                        (newObj as IPhysicsObj).IsCollidable = bool.Parse(reader.Value);
                                    if (reader.MoveToAttribute("Weighted"))
                                        (newObj as IPhysicsObj).IsWeighted = bool.Parse(reader.Value);
                                    break;
                                case ("PlayerStartObj"):
                                    newObj = new PlayerStartObj();
                                    XAML_mapDisplayXnaControl.PlayerStart = newObj as PlayerStartObj;
                                    break;
                            }

                            if (newObj != null)
                                ParseGenericXML(newObj, reader);

                            XAML_mapDisplayXnaControl.AddSprite(newObj, true, true);
                            XAML_mapDisplayXnaControl.ObjectList = GlobalLayerList[LayerType.GAME]; // This must be changed back to the default object list once the sprite has been added.
                        }
                    }
                }
                reader.Close();

                if (XAML_mapDisplayXnaControl.PlayerStart != null)
                    XAML_mapDisplayXnaControl.Camera.Position = XAML_mapDisplayXnaControl.PlayerStart.Position;
            }

            XAML_mapTabControl.SelectedIndex = XAML_mapTabControl.GameLayerIndex; // Set the active layer to the game layer.
        }

        private void ParseGenericXML(GameObj obj, XmlReader reader)
        {
            reader.MoveToAttribute("Name");
            string name = reader.Value;
            reader.MoveToAttribute("X");
            float X = float.Parse(reader.Value);
            reader.MoveToAttribute("Y");
            float Y = float.Parse(reader.Value);
            reader.MoveToAttribute("Width");
            int width = int.Parse(reader.Value);
            reader.MoveToAttribute("Height");
            int height = int.Parse(reader.Value);
            reader.MoveToAttribute("ScaleX");
            float scaleX = float.Parse(reader.Value);
            reader.MoveToAttribute("ScaleY");
            float scaleY = float.Parse(reader.Value);

            obj.Name = name;
            obj.X = X;
            obj.Y = Y;
            if (obj is CollHullObj)
            {
                (obj as CollHullObj).Width = width;
                (obj as CollHullObj).Height = height;
            }
            if (!(obj is EnemyMapObject))// Do not scale enemies.
            {
                obj.ScaleX = scaleX;
                obj.ScaleY = scaleY;
            }

            IPropertiesObj propertiesObj = obj as IPropertiesObj;
            if (propertiesObj != null)
            {
                if (reader.MoveToAttribute("LevelType"))
                    propertiesObj.LevelType = int.Parse(reader.Value);
                XAML_mapDisplayXnaControl.ObjectList = GlobalLayerList[propertiesObj.LevelType]; // This is changed so that when AddSprite is called, it is added to the correct layer.
            }

            if (obj is RoomObj)
            {
                RoomObj room = obj as RoomObj;
                if (reader.MoveToAttribute("SelectionMode"))
                    room.SelectionMode = int.Parse(reader.Value);

                if (reader.MoveToAttribute("CastlePool"))
                    room.AddToCastlePool = bool.Parse(reader.Value);
                if (reader.MoveToAttribute("GardenPool"))
                    room.AddToGardenPool = bool.Parse(reader.Value);
                if (reader.MoveToAttribute("TowerPool"))
                    room.AddToTowerPool = bool.Parse(reader.Value);
                if (reader.MoveToAttribute("DungeonPool"))
                    room.AddToDungeonPool = bool.Parse(reader.Value);
                if (reader.MoveToAttribute("DisplayBG"))
                    room.DisplayBG = bool.Parse(reader.Value);
            }
            if (reader.MoveToAttribute("Rotation"))
            {
                float rotation = float.Parse(reader.Value);
                obj.Rotation = rotation;
            }

            if (reader.MoveToAttribute("Flip"))
            {
                bool flip = bool.Parse(reader.Value);
                if (flip == true)
                    obj.Flip = SpriteEffects.FlipHorizontally;
            }

            if (reader.MoveToAttribute("Tag"))
            {
                string tag = reader.Value;
                obj.Tag = tag;
            }
            if (reader.MoveToAttribute("CollidesTop"))
                (obj as CollHullObj).CollidesTop = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("CollidesBottom"))
                (obj as CollHullObj).CollidesBottom = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("CollidesLeft"))
                (obj as CollHullObj).CollidesLeft = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("CollidesRight"))
                (obj as CollHullObj).CollidesRight = bool.Parse(reader.Value);

            if (reader.MoveToAttribute("ForceFlying"))
                (obj as EnemyOrbObj).ForceFlying = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("OrbType"))
                (obj as EnemyOrbObj).OrbType = int.Parse(reader.Value);
            if (reader.MoveToAttribute("IsWaypoint"))
                (obj as EnemyOrbObj).IsWaypoint = bool.Parse(reader.Value);
            if (reader.MoveToAttribute("BGLayer"))
            {
                MapObjContainer mapObjContainer = obj as MapObjContainer;
                if (mapObjContainer == null)
                    (obj as MapSpriteObj).OnBGLayer = bool.Parse(reader.Value);
                else
                    mapObjContainer.OnBGLayer = bool.Parse(reader.Value);
            }
        }

        public void LoadConfigXML()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;

            try
            {
                XmlReader reader = XmlReader.Create(Directory.GetCurrentDirectory() + @"\DS2DEdConfig.xml", settings);
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == "SpriteDirectory")
                        {
                            reader.MoveToAttribute("name");
                            EditorConfig.SpriteDirectory = reader.Value;
                        }
                        else if (reader.Name == "ExecutableDirectory")
                        {
                            reader.MoveToAttribute("name");
                            EditorConfig.ExecutableDirectory = reader.Value;
                        }
                    }
                }
            }
            catch //(Exception ex)
            {
                //MessageBox.Show("WARNING: Could not load config file. This is OK if this is the first time running the editor. Original Error: " + ex.Message);
            }

        }

        private void ExitButton_Clicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        /////////////////////////////

        //A list of all currently selected objects in the game.
        //Usually needed by ObjTreeControl, to know which items to highlight in the tree if they were selected in the map.
        public ObservableCollection<GameObj> SelectedObjects
        {
            get { return XAML_mapDisplayXnaControl.SelectedObjects; }
        }

        public List<ObservableCollection<GameObj>> GlobalLayerList
        {
            get { return m_objectList; }
        }

        public bool GameLayerSelected
        {
            get { return XAML_mapTabControl.GameLayerSelected; }
        }

        private bool m_changeMade = false;
        public bool ChangeMade
        {
            get { return m_changeMade; }
            set
            {
                m_changeMade = value;
                if (value == true && this.Title.IndexOf("*") != this.Title.Length - 1)
                    this.ChangeTitle(this.Title + "*");
                else if (value == false && this.Title.IndexOf("*") != -1)
                    this.ChangeTitle(this.Title.Substring(0, this.Title.Length - 1));
            }
        }

        public static byte FixEnemyTypesString(string value)
        {
            switch (value)
            {
                case ("EnemyObj_BallAndChain"):
                    return 1;
                case ("EnemyObj_Blob"):
                    return 2;
                case ("EnemyObj_BouncySpike"):
                    return 3;
                case ("EnemyObj_Eagle"):
                    return 4;
                case ("EnemyObj_EarthWizard"):
                    return 5;
                case ("EnemyObj_Eyeball"):
                    return 6;
                case ("EnemyObj_Fairy"):
                    return 7;
                case ("EnemyObj_Fireball"):
                    return 8;
                case ("EnemyObj_FireWizard"):
                    return 9;
                case ("EnemyObj_Horse"):
                    return 10;
                case ("EnemyObj_IceWizard"):
                    return 11;
                case ("EnemyObj_Knight"):
                    return 12;
                case ("EnemyObj_Ninja"):
                    return 13;
                case ("EnemyObj_ShieldKnight"):
                    return 14;
                case ("EnemyObj_Skeleton"):
                    return 15;
                case ("EnemyObj_SwordKnight"):
                    return 16;
                case ("EnemyObj_Turret"):
                    return 17;
                case ("EnemyObj_Wall"):
                    return 18;
                case ("EnemyObj_Wolf"):
                    return 19;
                case ("EnemyObj_Zombie"):
                    return 20;
                case ("EnemyObj_SpikeTrap"):
                    return 21;
            }
            return 0;
        }
    }
}
