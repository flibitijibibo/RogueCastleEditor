using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;
using SpriteSystem;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Input;

namespace RogueCastleEditor
{
    class SpriteListControl : ListBox, IControl
    {
        public MainWindow ControllerRef { get; set; }

        private string[] m_folderNames;
        private string m_baseDirectoryPath;

        public SpriteListControl()
        {
            this.PreviewMouseLeftButtonUp += LeftButtonHandler_Addon;
        }

        void LeftButtonHandler_Addon(object sender, MouseButtonEventArgs e)
        {
            if (this.SelectedItem != null)
                ControllerRef.DisplaySprites((this.SelectedItem as FolderItem).CharDataList, (this.SelectedItem as FolderItem).SpriteDataList);
        }

        public void LoadSpriteDirectories(GraphicsDevice graphicsDevice)
        {
            this.Items.Clear();
            
            // Saving all the full pathes to the folder names found inside the sprite directory.
            if (EditorConfig.SpriteDirectory != "")
            {
                try
                {
                    m_folderNames = Directory.GetDirectories(EditorConfig.SpriteDirectory);
                    // Saving the base folder of the sprite directory.
                    m_baseDirectoryPath = m_folderNames[0].Substring(0, m_folderNames[0].LastIndexOf(@"\") + 1);

                    for (int i = 0; i < m_folderNames.Length; i++)
                    {
                        // Converting all full path folder names to just their folder name. Ex. C:\Folder\TestFolder\ = TestFolder.
                        m_folderNames[i] = m_folderNames[i].Substring(m_folderNames[i].LastIndexOf(@"\") + 1);

                        // Finding all the PNG files located inside each folder.
                        string[] filePaths = Directory.GetFiles(m_baseDirectoryPath + m_folderNames[i], "*.png");

                        // Creating a temp list to store all character data names found in all pngs in a specific folder.
                        List<string> charDataList = new List<string>();
                        List<string> spriteDataList = new List<string>();

                        foreach (string fileName in filePaths)
                        {
                            List<string> charDataNames = SpriteLibrary.LoadSpritesheet(graphicsDevice, fileName, true);
                            List<string> spriteDataNames = SpriteLibrary.GetAllSpriteNames(fileName);

                            foreach (string charDataName in charDataNames)
                            {
                                charDataList.Add(charDataName);
                            }

                            foreach (string spriteDataName in spriteDataNames)
                            {
                                spriteDataList.Add(spriteDataName);
                            }
                        }

                        // Storing the found data inside the listbox.
                        this.Items.Add(new FolderItem(m_folderNames[i], charDataList, spriteDataList));
                    }
                }
                catch (Exception ex)
                {
                    OutputControl.Trace("ERROR: Could not load sprite directories. Original Error: " + ex.Message);
                }
            }
        }

        private class FolderItem : ListBoxItem
        {
            private string m_folderName;
            private List<string> m_charDataList;
            private List<string> m_spriteDataList;

            public FolderItem(string folderName, List<string> charDataList, List<string> spriteDataList)
            {
                m_folderName = folderName;
                m_charDataList = charDataList;
                m_spriteDataList = spriteDataList;

                Content = folderName;
            }

            public List<string> CharDataList
            {
                get { return m_charDataList; }
            }

            public List<string> SpriteDataList
            {
                get { return m_spriteDataList; }
            }

            public string FolderName
            {
                get { return m_folderName; }
            }
        }
    }
}
