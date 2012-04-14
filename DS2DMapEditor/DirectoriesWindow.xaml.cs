using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DS2DEngine;

namespace RogueCastleEditor
{
    /// <summary>
    /// Interaction logic for DirectoriesWindow.xaml
    /// </summary>
    public partial class DirectoriesWindow : Window
    {
        private MainWindow m_controllerRef;
        private bool m_reloadSpriteDirectories = false;

        public DirectoriesWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            m_controllerRef = mainWindow;
            XAML_spriteDirectoryBox.Text = EditorConfig.SpriteDirectory;
            XAML_executableDirectoryBox.Text = EditorConfig.ExecutableDirectory;
        }

        private void ButtonClose_Clicked(object sender, RoutedEventArgs e)
        {
            if (m_reloadSpriteDirectories == true)
                m_controllerRef.LoadSpriteDirectories(); // Reloads the SpriteList directories listing.
            m_controllerRef.SaveConfigXML();
            this.Close();
        }

        private void DirectoryButton_Clicked(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Tag as string)
            {
                case ("Spritesheet"):
                    System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
                    dialog.Description = "Select a directory for your spritesheets";
                    dialog.SelectedPath = EditorConfig.SpriteDirectory;
                    dialog.ShowNewFolderButton = false;
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (EditorConfig.SpriteDirectory != dialog.SelectedPath)
                        {
                            EditorConfig.SpriteDirectory = dialog.SelectedPath;
                            m_reloadSpriteDirectories = true;
                        }

                        XAML_spriteDirectoryBox.Text = dialog.SelectedPath;
                    }
                    break;
                case("Executable"):
                    System.Windows.Forms.OpenFileDialog triggerDialog = new System.Windows.Forms.OpenFileDialog();
                    triggerDialog.FileName ="";
                    triggerDialog.DefaultExt=".exe";
                    triggerDialog.Filter = "EXE File (.exe) |*.exe";
                    triggerDialog.RestoreDirectory = true;
                    triggerDialog.Title = "Select the DS2D executable file";

                    if (triggerDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            if (EditorConfig.ExecutableDirectory != triggerDialog.FileName)
                            {
                                EditorConfig.ExecutableDirectory = triggerDialog.FileName;
                                XAML_executableDirectoryBox.Text = triggerDialog.FileName;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                        }
                    }
                    break;
            }
        }
    }
}
