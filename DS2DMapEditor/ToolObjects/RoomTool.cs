using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using System.Windows.Input;
using Microsoft.Xna.Framework;

namespace RogueCastleEditor
{
    public class RoomTool : ToolObj
    {
        private int m_rectStartX;
        private int m_rectStartY;
        private int m_rectWidth;
        private int m_rectHeight;
        private bool m_mouseDown = false;
        MainWindow m_mainWindowRef;

        public RoomTool(Camera2D camera, GridObj gridObj, MainWindow mainWindowRef)
            : base(camera, gridObj)
        {
            ToolType = Consts.TOOLTYPE_ROOM;
            m_mainWindowRef = mainWindowRef;
        }

        public override void Action_MouseDown(object sender, HwndMouseEventArgs e)
        {
            if (ControllerRef.SelectedObjects.Count == 0)
            {
                ControllerRef.ClearAllSelectedObjs();
                m_rectStartX = 0;
                m_rectStartY = 0;
                m_rectWidth = 0;
                m_rectHeight = 0;

                // Starting code for creating a room.
                m_rectStartX = (int)((e.Position.X * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.X);
                m_rectStartY = (int)((e.Position.Y * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.Y);

                int snapX = 0;
                int snapY = 0;
                if (ControllerRef.SnapToGrid == true)
                    CalculateSnapTo(m_rectStartX, m_rectStartY, ref snapX, ref snapY);

                m_rectStartX += snapX;
                m_rectStartY += snapY;
                m_mouseDown = true;
            }
        }

        public override void Action_MouseMove(object sender, HwndMouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                m_rectWidth = (int)Math.Round(((e.Position.X * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.X) - m_rectStartX, MidpointRounding.AwayFromZero);
                m_rectHeight = (int)Math.Round(((e.Position.Y * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.Y) - m_rectStartY, MidpointRounding.AwayFromZero);

                int snapX = 0;
                int snapY = 0;
                if (ControllerRef.SnapToGrid == true)
                    CalculateSnapTo(m_rectWidth, m_rectHeight, ref snapX, ref snapY);

                m_rectWidth += snapX;
                m_rectHeight += snapY;
            }
        }

        public override void Action_MouseUp(object sender, HwndMouseEventArgs e)
        {
            if (ControllerRef.SelectedObjects.Count == 0)
            {
                int width, height, x, y = 0;

                if (m_rectWidth < 0)
                {
                    x = m_rectStartX + m_rectWidth;
                    width = m_rectWidth * -1;
                }
                else
                {
                    x = m_rectStartX;
                    width = m_rectWidth;
                }

                if (m_rectHeight < 0)
                {
                    y = m_rectStartY + m_rectHeight;
                    height = m_rectHeight * -1;
                }
                else
                {
                    y = m_rectStartY;
                    height = m_rectHeight;
                }

                if (width > 0 && height > 0)
                {
                    bool addRoom = true;
                    foreach (RoomObj roomObj in ControllerRef.RoomObjectList)
                    {
                        if (CollisionMath.Intersects(roomObj.Bounds, new Rectangle(x, y, width, height)))
                        {
                            addRoom = false;
                            OutputControl.Trace("ERROR: Cannot create room object as it is colliding with another room object.");
                        }
                    }
                    if (addRoom == true)
                        ControllerRef.AddSprite(new RoomObj(x, y, width, height, this.ControllerRef, m_mainWindowRef), true);
                }
            }

            m_rectStartX = 0;
            m_rectStartY = 0;
            m_rectWidth = 0;
            m_rectHeight = 0;
            m_mouseDown = false;
        }

        public override void Draw()
        {
            if (m_mouseDown == true)
            {
                int width, height, x, y = 0;

                if (m_rectWidth < 0)
                {
                    x = m_rectStartX + m_rectWidth;
                    width = m_rectWidth * -1;
                }
                else
                {
                    x = m_rectStartX;
                    width = m_rectWidth;
                }

                if (width <= 1) width = 0;

                if (m_rectHeight < 0)
                {
                    y = m_rectStartY + m_rectHeight;
                    height = m_rectHeight * -1;
                }
                else
                {
                    y = m_rectStartY;
                    height = m_rectHeight;
                }

                if (height <= 1) height = 0;

                m_camera.Draw(Consts.GenericTexture, new Rectangle(x, y, width, height), Consts.ROOM_COLOR);
            }
        }

        public override void Dispose()
        {
            m_mainWindowRef = null;
            base.Dispose();
        }
    }
}
