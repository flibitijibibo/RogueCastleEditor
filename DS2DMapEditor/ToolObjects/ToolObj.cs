using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Input;
using DS2DEngine;

namespace RogueCastleEditor
{
    public class ToolObj
    {
        public bool IsSelected {get; set;}
        public int ToolType { get; set; }
        protected Camera2D m_camera;
        protected GridObj m_gridObj;
        public bool ShiftHeld { get; set; }
        public bool CtrlHeld { get; set; }
        public bool ZHeld { get; set; }
        public MapDisplayXnaControl ControllerRef;

        public ToolObj(Camera2D camera, GridObj gridObj)
        {
            ToolType = Consts.TOOLTYPE_NONE;
            IsSelected = false;
            m_camera = camera;
            m_gridObj = gridObj;
        }

        public virtual void Action_MouseMove(object sender, HwndMouseEventArgs e) { }
        public virtual void Action_MouseDown(object sender, HwndMouseEventArgs e) { }
        public virtual void Action_MouseUp(object sender, HwndMouseEventArgs e) { }

        public virtual void Draw() { }
        public virtual void Action_KeyDown(KeyEventArgs e) { }
        public virtual void Action_KeyUp(KeyEventArgs e) { }

        public virtual void Dispose() 
        {
            m_camera = null;
            m_gridObj = null;
        }

        protected void CalculateSnapTo(int posX, int posY, ref int snapX, ref int snapY)
        {
            snapX = (int)(posX / m_gridObj.ColDistance);
            snapX *= m_gridObj.ColDistance;
            snapX = (int)(posX - snapX);

            if (snapX > m_gridObj.ColDistance * 0.5f)
                snapX = m_gridObj.ColDistance - snapX;
            else if (snapX < -m_gridObj.ColDistance * 0.5f) // Necessary for when snapping to negative values.
                snapX = -m_gridObj.ColDistance - snapX;
            else
                snapX = -snapX;
            

            snapY = (int)(posY / m_gridObj.RowDistance);
            snapY *= m_gridObj.RowDistance;
            snapY = (int)(posY - snapY);

            if (snapY > m_gridObj.RowDistance * 0.5f)
                snapY = m_gridObj.RowDistance - snapY;
            else if (snapY < -m_gridObj.RowDistance * 0.5f) // Necessary for when snapping to negative values.
                snapY = -m_gridObj.RowDistance - snapY;
            else
                snapY = -snapY;
        }
    }
}
