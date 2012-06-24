using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastleEditor
{
    public class EnemyOrbTool : ToolObj
    {
        public EnemyOrbTool(Camera2D camera, GridObj gridObj)
            : base(camera, gridObj)
        {
            ToolType = Consts.TOOLTYPE_ORB;
        }

        public override void Action_MouseDown(object sender, HwndMouseEventArgs e)
        {
            if (ControllerRef.SelectedObjects.Count == 0)
            {
                EnemyOrbObj enemyOrb = new EnemyOrbObj();
                enemyOrb.X = (float)((e.Position.X * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.X);
                enemyOrb.Y = (float)((e.Position.Y * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.Y);
                ControllerRef.AddSprite(enemyOrb, true);
            }
            base.Action_MouseDown(sender, e);
        }
    }
}
