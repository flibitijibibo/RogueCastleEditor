using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastleEditor
{
    public class SelectionToolObj : ToolObj
    {
        private int m_rectStartX;
        private int m_rectStartY;
        private int m_rectWidth;
        private int m_rectHeight;
        private Vector2 m_mouseOffset;
        private List<Vector2> m_startPositions; // Stores a list of all selected object starting positions, for use when undoing/redoing actions.

        public SelectionToolObj(Camera2D camera, GridObj gridObj)
            : base(camera, gridObj)
        {
            m_startPositions = new List<Vector2>();
            ToolType = Consts.TOOLTYPE_SELECTION;
        }
        
        public override void Action_MouseDown(object sender, HwndMouseEventArgs e)
        {
            // If shift is being held down.
            if (ShiftHeld == true)
            {
                ControllerRef.ClearAllSelectedObjs();

                m_rectStartX = (int)((e.Position.X * 1/m_camera.Zoom) + m_camera.TopLeftCorner.X);
                m_rectStartY = (int)((e.Position.Y * 1/m_camera.Zoom) + m_camera.TopLeftCorner.Y);
            }
            // If shift is not held down, and no objects have been selected, select only one object.
            else if (ShiftHeld == false)
            {
                ObservableCollection<GameObj> listToCheck = ControllerRef.ObjectList;
                if (ControllerRef.SelectedTool == Consts.TOOLTYPE_ROOM)
                    listToCheck = ControllerRef.RoomObjectList;

                // Code for selecting only ONE collision hull object. Breaks out after finding one. To find multiple, shift must be held.
                //foreach (GameObj obj in ControllerRef.ObjectList.Reverse())
                foreach (GameObj obj in listToCheck.Reverse())
                {
                    Vector2 anchorPt = new Vector2(obj.Bounds.Width * 0.5f, obj.Bounds.Height * 0.5f);
                    Rectangle objRect = obj.Bounds;
                    bool selectObject = false;

                    if (obj is CollHullObj)
                    {
                        anchorPt = Vector2.Zero;
                        objRect = (obj as CollHullObj).HullRect;
                        if (ControllerRef.SelectCollHulls == true)
                            selectObject = true;
                    }

                    if (obj is MapSpriteObj && ControllerRef.SelectSpriteObjs == true)
                        selectObject = true;

                    if (obj is EnemyMapObject)
                        selectObject = true;

                    if (obj is EnemyOrbObj)
                        selectObject = true;

                    //if (CollisionMath.Intersects(obj.Bounds, new Rectangle((int)((e.Position.X * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.X), (int)((e.Position.Y * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.Y), 1, 1)))
                    if (CollisionMath.RotatedRectIntersects(objRect, obj.Rotation, anchorPt, 
                        new Rectangle((int)((e.Position.X * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.X), (int)((e.Position.Y * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.Y), 10, 10), 0, new Vector2(5,5)) && selectObject == true)
                    {
                        if (!ControllerRef.SelectedObjects.Contains(obj))
                        {
                            if (CtrlHeld == false)
                                ControllerRef.ClearAllSelectedObjs();
                            ControllerRef.SelectedObjects.Add(obj);
                        }
                        else if (ControllerRef.SelectedObjects.Contains(obj) && CtrlHeld == true)
                            ControllerRef.SelectedObjects.Remove(obj);

                        break;
                    }
                }

                if (ControllerRef.SelectedTool == Consts.TOOLTYPE_ROOM)
                {
                    SelectAllTouchingObjects();
                }
            }
            
            // Code for selecting ALREADY SELECTED objects in SelectedObjectsRect to move them.
            if (CollisionMath.Intersects(ControllerRef.SelectedObjectsRect, new Rectangle((int)((e.Position.X * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.X), (int)((e.Position.Y * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.Y), 1, 1)))
            {
                m_startPositions.Clear();
                int snapX = 0;
                int snapY = 0;
                if (ControllerRef.SnapToGrid == true)
                    CalculateSnapTo((int)(e.Position.X * 1 / m_camera.Zoom), (int)(e.Position.Y * 1 / m_camera.Zoom), ref snapX, ref snapY);

                float mouseOffsetX = (float)((e.Position.X * 1 / m_camera.Zoom) - ControllerRef.SelectedObjects[0].X + m_camera.TopLeftCorner.X + snapX);
                float mouseOffsetY = (float)((e.Position.Y * 1 / m_camera.Zoom) - ControllerRef.SelectedObjects[0].Y + m_camera.TopLeftCorner.Y + snapY);
                m_mouseOffset = new Vector2(mouseOffsetX, mouseOffsetY);

                foreach (GameObj obj in ControllerRef.SelectedObjects)
                {
                    m_startPositions.Add(obj.Position);
                }
            }
            else if (CtrlHeld == false)
                ControllerRef.ClearAllSelectedObjs();
        }
        
        public override void Action_MouseMove(object sender, HwndMouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && ShiftHeld == false)
            {
                if (ControllerRef.SelectedObjects.Count > 0)
                {
                    GameObj obj = ControllerRef.SelectedObjects[0];
                    int xMovement = (int)Math.Round(((e.Position.X * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.X) - m_mouseOffset.X, MidpointRounding.AwayFromZero);
                    int yMovement = (int)Math.Round(((e.Position.Y * 1 / m_camera.Zoom) + m_camera.TopLeftCorner.Y) - m_mouseOffset.Y, MidpointRounding.AwayFromZero);

                    int snapX = 0;
                    int snapY = 0;
                    if (ControllerRef.SnapToGrid == true)
                        CalculateSnapTo(xMovement, yMovement, ref snapX, ref snapY);

                    xMovement += snapX;
                    yMovement += snapY;

                    xMovement -= (int)obj.X;
                    yMovement -= (int)obj.Y;
;
                    for (int i = 0; i < ControllerRef.SelectedObjects.Count; i++)
                    {
                        ControllerRef.SelectedObjects[i].X += xMovement;
                        ControllerRef.SelectedObjects[i].Y += yMovement;
                    }
                }
            }
            // Make the selection box while shift is held down.
            else if (e.LeftButton == MouseButtonState.Pressed && ShiftHeld == true)
            {
                m_rectWidth = (int)((e.Position.X * 1/m_camera.Zoom) - m_rectStartX + m_camera.TopLeftCorner.X);
                m_rectHeight = (int)((e.Position.Y * 1/m_camera.Zoom) - m_rectStartY + m_camera.TopLeftCorner.Y);
            }
        }

        public override void Action_MouseUp(object sender, HwndMouseEventArgs e)
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

            // Code to make sure two rooms do not collide with one another.
            bool acceptObjectPlacements = true;
            foreach (GameObj obj in ControllerRef.SelectedObjects)
            {
                if (obj is RoomObj && acceptObjectPlacements == true)
                {
                    foreach (RoomObj roomObj in ControllerRef.RoomObjectList)
                    {
                        if (obj != roomObj && CollisionMath.Intersects(obj.Bounds, roomObj.Bounds))
                        {
                            acceptObjectPlacements = false;
                            OutputControl.Trace("ERROR: Cannot set object placements. Two or more rooms are colliding with one another.");
                            break;
                        }
                    }
                }
                //else
                //    break;
            }

            if (acceptObjectPlacements == true)
            {
                // Code for adding the undo logic.
                // Must be called before the Shift code since that changes the number of objects selected.
                List<Vector2> newPositions = new List<Vector2>();
                List<GameObj> objList = new List<GameObj>();
                List<Vector2> previousPositions = new List<Vector2>();
                for (int i = 0; i < ControllerRef.SelectedObjects.Count; i++)
                {
                    newPositions.Add(ControllerRef.SelectedObjects[i].Position);
                    objList.Add(ControllerRef.SelectedObjects[i]);
                    previousPositions.Add(m_startPositions[i]);
                }
                if (ControllerRef.SelectedObjects.Count > 0 && ControllerRef.SelectedObjects[0].Position != m_startPositions[0]) // Only add an undo action if the objects were actually moved.
                    UndoManager.AddUndoAction(new UndoObjMovement(objList, previousPositions, newPositions));
                ///////////////////////////////////
            }
            else
            {
                for (int i = 0; i < ControllerRef.SelectedObjects.Count; i++)
                {
                    ControllerRef.SelectedObjects[i].Position = m_startPositions[i];
                }
            }

            if (ShiftHeld == true)
            {
                ControllerRef.ClearAllSelectedObjs();
                Rectangle selectionRectangle = new Rectangle(x, y, width, height);

                ObservableCollection<GameObj> listToCheck = ControllerRef.ObjectList;
                if (ControllerRef.SelectedTool == Consts.TOOLTYPE_ROOM)
                    listToCheck = ControllerRef.RoomObjectList;

                foreach (GameObj obj in listToCheck)
                {
                    //if (CollisionMath.Intersects(obj.Bounds, selectionRectangle))
                    if (CollisionMath.RotatedRectIntersects(obj.Bounds, obj.Rotation, new Vector2(obj.Bounds.Width * 0.5f, obj.Bounds.Height * 0.5f), selectionRectangle, 0, Vector2.Zero))
                        ControllerRef.SelectedObjects.Add(obj);
                }
            }
            else if (ControllerRef.SelectedObjects.Count == 1)
            {
                ControllerRef.ShowObjProperties(ControllerRef.SelectedObjects[0] as IPropertiesObj);
            }

            m_rectStartX = 0;
            m_rectStartY = 0;
            m_rectWidth = 0;
            m_rectHeight = 0;
        }


        public override void Action_KeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                ShiftHeld = true;

            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                CtrlHeld = true;

            if  (e.Key == Key.Delete || e.Key == Key.Back)
            {
                List<GameObj> objList = new List<GameObj>();
                foreach (GameObj obj in ControllerRef.SelectedObjects)
                {
                    objList.Add(obj);
                    if (obj is RoomObj)
                        ControllerRef.RoomObjectList.Remove(obj);
                    else
                        ControllerRef.ObjectList.Remove(obj);
                    if (obj is IPhysicsObj)
                        (obj as IPhysicsObj).RemoveFromPhysicsManager();
                    if (obj is PlayerStartObj)
                        ControllerRef.PlayerStart = null;
                }
                if (objList.Count > 0)
                    UndoManager.AddUndoAction(new UndoObjDelete(objList, this.ControllerRef));

                //m_mouseDown = false;
                m_rectStartX = 0;
                m_rectStartY = 0;
                m_rectWidth = 0;
                m_rectHeight = 0;
                ControllerRef.ClearAllSelectedObjs();
            }
        }

        public override void Action_KeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                ShiftHeld = false;

            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                CtrlHeld = false;
        }

        public override void Draw()
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

            if (ShiftHeld == true)
            {
                //Top line
                m_camera.Draw(Consts.SelectionBoxHorizontal, new Vector2(x, y), new Rectangle(0, 0, width, 2), Color.White);
                //Bottom line
                m_camera.Draw(Consts.SelectionBoxHorizontal, new Vector2(x, y + height - 2), new Rectangle(0, 0, width, 2), Color.White);
                //Left line
                m_camera.Draw(Consts.SelectionBoxVertical, new Vector2(x, y), new Rectangle(0, 0, 2, height), Color.White);
                //Right line
                m_camera.Draw(Consts.SelectionBoxVertical, new Vector2(x + width - 2, y), new Rectangle(0, 0, 2, height), Color.White);
            }
        }

        public override void Dispose()
        {
            ControllerRef.ClearAllSelectedObjs();
            m_startPositions.Clear();
            m_startPositions = null;
            base.Dispose();
        }

        private void SelectAllTouchingObjects()
        {
            List<GameObj> objsToAdd = new List<GameObj>();
            foreach (GameObj obj in ControllerRef.SelectedObjects)
            {
                RoomObj selectedRoom = obj as RoomObj;
                if (selectedRoom != null && selectedRoom.SelectionMode != 2)
                {
                    foreach (GameObj touchingObj in selectedRoom.TouchingObjList)
                    {
                        if (!(ControllerRef.SelectedObjects.Contains(touchingObj)))
                            objsToAdd.Add(touchingObj);
                    }
                }
            }

            foreach (GameObj obj in objsToAdd)
            {
                ControllerRef.SelectedObjects.Add(obj);
            }
        }
    }
}
