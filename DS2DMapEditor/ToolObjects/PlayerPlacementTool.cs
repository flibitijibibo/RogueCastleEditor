using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastleEditor
{
    public class PlayerPlacementTool : ToolObj
    {
        public PlayerPlacementTool(Camera2D camera, GridObj gridObj)
            : base(camera, gridObj)
        {
            ToolType = Consts.TOOLTYPE_PLAYER_PLACEMENT;
        }

        public override void Action_MouseDown(object sender, HwndMouseEventArgs e)
        {
            if (ControllerRef.PlayerStart == null)
            {
                PlayerStartObj playerStart = new PlayerStartObj();
                playerStart.X = (float)((e.Position.X * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.X);
                playerStart.Y = (float)((e.Position.Y * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.Y);
            
                ControllerRef.PlayerStart = playerStart;
                ControllerRef.PlayerStart.X -= ControllerRef.PlayerStart.Width * 0.5f;
                ControllerRef.PlayerStart.Y -= ControllerRef.PlayerStart.Height * 0.5f;

                ControllerRef.AddSprite(ControllerRef.PlayerStart, true);
            }
            else
            {
                List<Vector2> oldPosition = new List<Vector2>();
                oldPosition.Add(new Vector2(ControllerRef.PlayerStart.X, ControllerRef.PlayerStart.Y));

                ControllerRef.PlayerStart.X = (float)((e.Position.X * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.X);
                ControllerRef.PlayerStart.Y = (float)((e.Position.Y * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.Y);
                ControllerRef.PlayerStart.X -= ControllerRef.PlayerStart.Width * 0.5f;
                ControllerRef.PlayerStart.Y -= ControllerRef.PlayerStart.Height * 0.5f;
                List<Vector2> newPosition = new List<Vector2>();
                newPosition.Add(new Vector2(ControllerRef.PlayerStart.X, ControllerRef.PlayerStart.Y));
                
                List<GameObj> objList = new List<GameObj>();
                objList.Add(ControllerRef.PlayerStart);

                //A check to make sure the player start isn't already selected, otherwise it will create two UndoObjMovement actions.
                bool playerStartSelected = false;
                foreach(GameObj obj in ControllerRef.SelectedObjects)
                {
                    if (obj is PlayerStartObj)
                    {
                        playerStartSelected = true;
                        break;
                    }
                }
                if (playerStartSelected == false)
                    UndoManager.AddUndoAction(new UndoObjMovement(objList, oldPosition, newPosition));
            }

            base.Action_MouseDown(sender, e);
        }
    }
}
