using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace RogueCastleEditor
{
    public class ToolbarControl : StackPanel, IControl
    {
        public MainWindow ControllerRef { get; set; }

        public int ToolBar_Clicked(object sender, RoutedEventArgs e)
        {
            ToggleButton buttonClicked = sender as ToggleButton;
            int toolType = Consts.TOOLTYPE_NONE;

            if (buttonClicked.IsChecked == true)
            {
                // Some quick code to reset the checked state of all tools, then set the clicked button back to a checked state.
                // Necessary since only one tool can be active at a time.
                ResetAllTools();
                buttonClicked.IsChecked = true;

                switch (buttonClicked.Tag as string)
                {
                    case ("RectangleTool"):
                        toolType = Consts.TOOLTYPE_RECTANGLE;
                        break;
                    case ("RotationTool"):
                        toolType = Consts.TOOLTYPE_ROTATE;
                        break;
                    case ("ScaleTool"):
                        toolType = Consts.TOOLTYPE_SCALE;
                        break;
                    case ("RoomTool"):
                        toolType = Consts.TOOLTYPE_ROOM;
                        break;
                    case ("PlayerTool"):
                        toolType = Consts.TOOLTYPE_PLAYER_PLACEMENT;
                        break;
                    case ("TriggerTool"):
                        toolType = Consts.TOOLTYPE_TRIGGER_PLACEMENT;
                        break;
                    case("OrbTool"):
                        toolType = Consts.TOOLTYPE_ORB;
                        break;
                    default:
                        break;
                }

                ControllerRef.SetFocusToMapDisplay();
            }

            return toolType;
        }

        public void ResetAllTools()
        {
            foreach (ToggleButton button in this.Children)
            {
                button.IsChecked = false;
            }
        }
    }
}
