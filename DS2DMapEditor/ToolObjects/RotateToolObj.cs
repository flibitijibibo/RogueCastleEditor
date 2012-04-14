using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastleEditor
{
    public class RotateToolObj : ToolObj
    {
        private Vector2 m_centrePosition;
        private List<Vector2> m_objStartPositions;
        private List<float> m_objStartRotations;
        private float m_rotation;

        public RotateToolObj(Camera2D camera, GridObj gridObj)
            : base(camera, gridObj)
        {
            ToolType = Consts.TOOLTYPE_ROTATE;
            m_objStartPositions = new List<Vector2>();
            m_objStartRotations = new List<float>();
        }

        public override void Action_MouseDown(object sender, HwndMouseEventArgs e)
        {
            if (ControllerRef.SelectedObjects.Count > 0)
            {
                m_rotation = 0;

                if (ControllerRef.SelectedObjects.Count == 1 && !(ControllerRef.SelectedObjects[0] is CollHullObj)) // If it is a collision hull, rotate around the centre.
                    m_centrePosition = ControllerRef.SelectedObjects[0].Position;
                else
                    m_centrePosition = new Vector2(ControllerRef.SelectedObjectsRect.Center.X, ControllerRef.SelectedObjectsRect.Center.Y);

                m_objStartPositions.Clear();
                m_objStartRotations.Clear();

                foreach (GameObj obj in ControllerRef.SelectedObjects)
                {
                    m_objStartPositions.Add(obj.Position);
                    m_objStartRotations.Add(obj.Rotation);
                }
            }
        }

        public override void Action_MouseMove(object sender, HwndMouseEventArgs e)
        {
            float x = m_centrePosition.X - m_camera.TopLeftCorner.X - (float)(e.Position.X * 1/m_camera.Zoom);
            float y = m_centrePosition.Y - m_camera.TopLeftCorner.Y - (float)(e.Position.Y * 1/m_camera.Zoom);

            float desiredAngle = (float)Math.Atan2(-y, -x);
            float difference = MathHelper.WrapAngle(desiredAngle - m_rotation);

            m_rotation = MathHelper.ToDegrees(MathHelper.WrapAngle(m_rotation + difference));
        
            for (int i = 0; i < ControllerRef.SelectedObjects.Count; i++)
            {
                //if (!(ControllerRef.SelectedObjects[i] is CollHullObj)) // Do not apply rotations to collision hulls.
                {
                    ControllerRef.SelectedObjects[i].Rotation = m_objStartRotations[i] + m_rotation;

                    Vector2 rotatedPoint = CDGMath.RotatedPoint(m_objStartPositions[i] - m_centrePosition, m_rotation);
                    ControllerRef.SelectedObjects[i].Position = rotatedPoint + m_centrePosition;
                }
            }
        }

        public override void Action_MouseUp(object sender, HwndMouseEventArgs e)
        {
            // Only add an undo action if the rotation of the object actually changed.
            if (ControllerRef.SelectedObjects.Count > 0 && m_objStartRotations.Count > 0 && ControllerRef.SelectedObjects[0].Rotation != m_objStartRotations[0])
            {
                List<GameObj> objList = new List<GameObj>();
                List<float> startingRotations = new List<float>();
                List<float> endingRotations = new List<float>();
                List<Vector2> startPositions = new List<Vector2>();
                List<Vector2> endPositions = new List<Vector2>();
                for (int i = 0; i < ControllerRef.SelectedObjects.Count; i++)
                {
                    objList.Add(ControllerRef.SelectedObjects[i]);
                    startPositions.Add(m_objStartPositions[i]);
                    startingRotations.Add(m_objStartRotations[i]);
                    endPositions.Add(ControllerRef.SelectedObjects[i].Position);
                    endingRotations.Add(ControllerRef.SelectedObjects[i].Rotation);
                }
                UndoManager.AddUndoAction(new UndoObjRotation(objList, startingRotations, endingRotations, startPositions, endPositions));
            }
        }

        public override void Dispose()
        {
            m_objStartPositions.Clear();
            m_objStartPositions = null;
            m_objStartRotations.Clear();
            m_objStartRotations = null;
            base.Dispose();
        }
    }
}
