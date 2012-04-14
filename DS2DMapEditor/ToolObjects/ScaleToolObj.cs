using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using System.Windows.Input;

namespace RogueCastleEditor
{
    public class ScaleToolObj : ToolObj
    {
        private int m_mouseStartX;
        private int m_mouseStartY;
        private int m_scaleWidth;
        private int m_scaleHeight;
        private Vector2 m_centrePosition;

        private List<Vector2> m_objStartScale;
        private List<Vector2> m_objStartPosition;

        public ScaleToolObj(Camera2D camera, GridObj gridObj)
            : base(camera, gridObj)
        {
            ToolType = Consts.TOOLTYPE_SCALE;
            m_objStartScale = new List<Vector2>();
            m_objStartPosition = new List<Vector2>();
        }

        public override void Action_KeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                ShiftHeld = true;
        }

        public override void Action_KeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                ShiftHeld = false;
        }

        public override void Action_MouseDown(object sender, HwndMouseEventArgs e)
        {
            m_mouseStartX = (int)((e.Position.X * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.X);
            m_mouseStartY = (int)((e.Position.Y * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.Y);

            if (ControllerRef.SelectedObjects.Count == 1)
                m_centrePosition = ControllerRef.SelectedObjects[0].Position;
            else
                m_centrePosition = new Vector2(ControllerRef.SelectedObjectsRect.Center.X, ControllerRef.SelectedObjectsRect.Center.Y);

            m_scaleWidth = 0;
            m_scaleHeight = 0;

            m_objStartScale.Clear();
            m_objStartPosition.Clear();
            foreach (GameObj obj in ControllerRef.SelectedObjects)
            {
                if (obj is CollHullObj && ControllerRef.SelectedObjects.Count == 1)
                    m_objStartScale.Add(new Vector2(obj.Width, obj.Height));
                else
                    m_objStartScale.Add(obj.Scale);
                m_objStartPosition.Add(obj.Position);
            }
        }

        public override void Action_MouseMove(object sender, HwndMouseEventArgs e)
        {
            if (ControllerRef.SelectedObjects.Count > 0)
            {
                m_scaleWidth = (int)((e.Position.X * 1 / m_camera.Zoom) - m_mouseStartX + m_camera.TopLeftCorner.X);
                m_scaleHeight = (int)((e.Position.Y * 1 / m_camera.Zoom) - m_mouseStartY + m_camera.TopLeftCorner.Y);

                // Activate uniform scaling if Shift is held down.
                if (ShiftHeld == true)
                    m_scaleHeight = -m_scaleWidth;

                for (int i = 0; i < ControllerRef.SelectedObjects.Count; i++)
                {
                    if (ControllerRef.SelectedObjects[i] is CollHullObj && ControllerRef.SelectedObjects.Count == 1)
                    {
                        // Some hack to support snap to grid when resizing individual CollHullObjects.
                        if (ControllerRef.SnapToGrid == true)
                        {
                            int snapX = 0;
                            int snapY = 0;
                            if (ControllerRef.SnapToGrid == true)
                                CalculateSnapTo(m_scaleWidth, m_scaleHeight, ref snapX, ref snapY);
                            m_scaleWidth += snapX;
                            m_scaleHeight += snapY;
                            if (ShiftHeld == true)
                                m_scaleHeight = m_scaleWidth;
                        }

                        (ControllerRef.SelectedObjects[i] as CollHullObj).Width = (int)m_objStartScale[i].X + m_scaleWidth;
                        (ControllerRef.SelectedObjects[i] as CollHullObj).Height = (int)m_objStartScale[i].Y + m_scaleHeight;

                    }
                    else
                    {
                        ControllerRef.SelectedObjects[i].ScaleX = m_objStartScale[i].X + (m_scaleWidth * 0.005f);
                        ControllerRef.SelectedObjects[i].ScaleY = m_objStartScale[i].Y - (m_scaleHeight * 0.005f);
                    }

                    float diffPosX = (m_objStartPosition[i].X - m_centrePosition.X) * (1/m_objStartScale[i].X);
                    float diffPosY = (m_objStartPosition[i].Y - m_centrePosition.Y) * (1/m_objStartScale[i].Y);

                    ControllerRef.SelectedObjects[i].X = m_objStartPosition[i].X + ((m_scaleWidth * 0.005f) * diffPosX);
                    ControllerRef.SelectedObjects[i].Y = m_objStartPosition[i].Y - ((m_scaleHeight * 0.005f) * diffPosY);
                }
            }
        }

        public override void Action_MouseUp(object sender, HwndMouseEventArgs e)
        {
            // Only add an undo action if the scale of the object actually changed.
            if (ControllerRef.SelectedObjects.Count > 0 && m_objStartScale.Count > 0 && ControllerRef.SelectedObjects[0].Scale != m_objStartScale[0])
            {
                List<GameObj> objList = new List<GameObj>();
                List<Vector2> startingScale = new List<Vector2>();
                List<Vector2> endingScale = new List<Vector2>();
                List<Vector2> startPositions = new List<Vector2>();
                List<Vector2> endPositions = new List<Vector2>();
                for (int i = 0; i < ControllerRef.SelectedObjects.Count; i++)
                {
                    objList.Add(ControllerRef.SelectedObjects[i]);
                    startPositions.Add(m_objStartPosition[i]);
                    startingScale.Add(m_objStartScale[i]);
                    endPositions.Add(ControllerRef.SelectedObjects[i].Position);
                    if (ControllerRef.SelectedObjects.Count == 1 && ControllerRef.SelectedObjects[i] is CollHullObj)
                        endingScale.Add(new Vector2(ControllerRef.SelectedObjects[i].Width, ControllerRef.SelectedObjects[i].Height));
                    else
                        endingScale.Add(ControllerRef.SelectedObjects[i].Scale);
                }
                UndoManager.AddUndoAction(new UndoObjScale(objList, startingScale, endingScale, startPositions, endPositions));
            }
        }

        public override void Dispose()
        {
            m_objStartPosition.Clear();
            m_objStartPosition = null;
            m_objStartScale.Clear();
            m_objStartScale = null;
            base.Dispose();
        }
    }
}
