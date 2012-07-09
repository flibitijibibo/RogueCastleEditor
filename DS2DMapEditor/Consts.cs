using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastleEditor
{
    class Consts
    {
        //Base screen resolution size for the game.
        public const int ScreenWidth = 1320; //1366;
        public const int ScreenHeight = 720; //768;

        //Grid size
        public const int GridWidth = 60;
        public const int GridHeight = 60;

        //Size of player
        public const int PlayerWidth = 60;
        public const int PlayerHeight = 120;

        //Generic textures used for multiple objects.
        public static Texture2D GenericTexture;
        public static Texture2D SelectionBoxHorizontal;
        public static Texture2D SelectionBoxVertical;

        //Consts for ToolObjects
        public const int TOOLTYPE_NONE = 0;
        public const int TOOLTYPE_SELECTION = 1;
        public const int TOOLTYPE_RECTANGLE = 2;
        public const int TOOLTYPE_ROTATE = 3;
        public const int TOOLTYPE_SCALE = 4;
        public const int TOOLTYPE_ROOM = 5;
        public const int TOOLTYPE_PLAYER_PLACEMENT = 6;
        public const int TOOLTYPE_TRIGGER_PLACEMENT = 7;
        public const int TOOLTYPE_ORB = 8;

        //Consts for Colors
        public static Color COLLHULL_BORDER_COLOR = new Color(255, 255, 255) * 0.8f; // The colour of the borders around a collision hull.
        public static Color COLLHULL_SELECTED_COLOR = new Color(180, 0, 180); // The colour of a collision hull when it is selected.
        public static Color COLLHULL_COLOR = new Color(180, 0, 180) * 0.4f; // The colour of a collision hull when it is not selected.
        public static Color GRID_COLOR = new Color(50, 50, 50); // The colour of the grid lines.

        // Currently used for collision hulls that have the Top/!Top keyword.
        public static Color COLLHULL_UNIQUE_COLOR = new Color(255, 255, 255) * 0.5f;
        public static Color COLLHULL_UNIQUE_SELECTED_COLOR = new Color(255, 255, 255);
        public static Color COLLHULL_UNIQUE2_COLOR = new Color(0, 0, 0) * 0.5f;
        public static Color COLLHULL_UNIQUE2_SELECTED_COLOR = new Color(0, 0, 0);

        public static Color COLLHULL_KILL_COLOR = new Color(255, 0, 0) * 0.6f;
        public static Color COLLHULL_KILL_SELECTED_COLOR = new Color(255, 0, 0);
        public static Color COLLHULL_DAMAGE_COLOR = new Color(255, 140, 0) * 0.6f;
        public static Color COLLHULL_DAMAGE_SELECTED_COLOR = new Color(255, 140, 0);
        public static Color COLLHULL_BORDEROBJ_COLOR = new Color(80, 80, 80) * 0.6f;
        public static Color COLLHULL_BORDEROBJ_SELECTED_COLOR = new Color(80, 80, 80);


        public static Color ROOM_COLOR = new Color(0, 0, 255) * 0.4f; // The colour of a room.
        public static Color ROOM_SELECTED_COLOR = new Color(0, 0, 255) * 0.8f; // The colour of a room selected.
        public static Color ROOM_BORDER_COLOR = new Color(255, 255, 255) * 0.8f; // The colour of the border around a room.

        public static Color PLAYER_START_COLOR = new Color(255, 150, 0) * 0.4f; // The colour of the player's starting position.
        public static Color PLAYER_START_SELECTED_COLOR = new Color(255, 150, 0) * 0.8f; // The colour of the player's starting position selected.
        public static Color PLAYER_START_BORDER_COLOR = new Color(255, 255, 255) * 0.8f; // The colour of the border around the player's starting position.

        public static Color TRIGGEROBJ_COLOR = new Color(255, 255, 0) * 0.4f; // The colour of a collision hull when it is not selected.
        public static Color TRIGGEROBJ_SELECTED_COLOR = new Color(255, 255, 0) * 0.8f; // The colour of a collision hull when it is selected.

        public static Color CHESTOBJ_COLOR = new Color(0, 255, 0) * 0.4f; // The colour of a chest obj when it is not selected.
        public static Color CHESTOBJ_SELECTED_COLOR = new Color(0, 255, 0) * 0.8f; // The colour of a chest obj when it is selected.

        public static Color SELECTION_BOX = new Color(255, 255, 0); // The colour of the selection box around selected objects.

        public static Color CAMERA_BOX_COLOR = new Color(255, 0, 0, 50); // The colour of the camera's view box.

        public const int SELECTION_BORDERWIDTH = 2;       
    }

    public enum EnemyDifficulty
    {
        BASIC,
        ADVANCED,
        EXPERT,
        MINIBOSS
    }
}
