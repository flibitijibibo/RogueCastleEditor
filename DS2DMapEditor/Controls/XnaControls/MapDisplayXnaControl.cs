using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Windows.Input;
using SpriteSystem;
using System.Collections.Generic;
using DS2DEngine;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Specialized;

namespace RogueCastleEditor
{
    public class MapDisplayXnaControl : XnaControl
    {
        //The current tool selected.
        private ToolObj m_toolObject;

        //The selection tool is always selected.
        private SelectionToolObj m_selectToolObj;

        private float m_rButtonStartX; // Only used for moving the camera.
        private float m_rButtonStartY; // Only used for moving the camera.

        private float m_camPosX; // This stores the camera's initial position before the right mouse is clicked.
        private float m_camPosY; // This stores the camera's initial position before the right mouse is clicked.

        public ObservableCollection<GameObj> ObjectList { get; set; } // All objects on the map.
        public ObservableCollection<GameObj> RoomObjectList { get; set; } //All rooms on the map.

        private ObservableCollection<GameObj> m_selectedObjects;
        public ObservableCollection<GameObj> SelectedObjects { get { return m_selectedObjects; } }

        private PhysicsManager m_physicsManager;

        // Variables needed for copy-pasting objects.
        private List<GameObj> m_clipboard;
        private List<Vector2> m_clipboardPositions;
        private bool m_clipboardKeyPressed = false;
        private bool m_ctrlHeld;
        
        // Variables needed for Undomanager when moving objects via arrow keys.
        private List<Vector2> m_objStartPos;
        private bool m_arrowKeyHeld = false;

        // Grid properties.
        private GridObj m_grid;
        public bool GridVisible { get; set; }
        public PlayerStartObj PlayerStart { get; set; }

        // Selection tool properties.
        public bool SelectCollHulls { get; set; }
        public bool SelectSpriteObjs { get; set; }

        public static SpriteFont Font;

        public MapDisplayXnaControl()
        {
            GridVisible = true;
            m_selectedObjects = new ObservableCollection<GameObj>();
            m_selectedObjects.CollectionChanged += SelectedObjectsChanged;

            m_physicsManager = new PhysicsManager();
            m_clipboard = new List<GameObj>();
            m_clipboardPositions = new List<Vector2>();
            SnapToGrid = true;

            RoomObjectList = new ObservableCollection<GameObj>();
            m_objStartPos = new List<Vector2>();
            SelectCollHulls = true;
            SelectSpriteObjs = true;
        }

        /// <summary>
        /// Invoked after either control has created its graphics device.
        /// </summary>
        protected override void loadContent(object sender, GraphicsDeviceEventArgs e)
        {
            //m_camera defined in base.loadContent
            base.loadContent(sender, e);

            //Creating map grid.
            m_grid = new GridObj(GraphicsDevice);
            m_grid.CreateGrid(Consts.GridWidth, Consts.GridHeight);

            //Creating selection tool to select objects.
            m_selectToolObj = new SelectionToolObj(m_camera, m_grid);
            m_selectToolObj.ControllerRef = this;

            // Creating a generic texture for use.
            Consts.GenericTexture = new Texture2D(GraphicsDevice, 1, 1);
            Consts.GenericTexture.SetData(new Color[] { Color.White });

            //Creating the Selection box textures.
            FileStream fs = new FileStream(@"Images/SelectionBorderWidth.gif", FileMode.Open);
            Consts.SelectionBoxHorizontal = Texture2D.FromStream(GraphicsDevice, fs);
            fs.Close();

            fs = new FileStream(@"Images/SelectionBorderHeight.gif", FileMode.Open);
            Consts.SelectionBoxVertical = Texture2D.FromStream(GraphicsDevice, fs);
            fs.Close();

            m_physicsManager.Initialize(m_camera);

            Font = Content.Load<SpriteFont>("Arial14");
        }

        GameTime nullTime = new GameTime();
        protected override void Update(Stopwatch gameTime)
        {
            m_physicsManager.Update(nullTime);
            /*
            foreach (GameObj obj in ObjectList)
            {
               // Console.WriteLine(obj.Rotation);
               // Console.WriteLine(CollisionMath.LowerRightCorner(obj.Bounds, obj.Rotation, Vector2.Zero));
              //  foreach (CollisionBox box in (obj as IPhysicsObj).CollisionBoxes)
                //    Console.WriteLine(box.X);
            }

            for (int i = 0; i < ObjectList.Count - 1; i++)
            {
                foreach (GameObj obj in ObjectList)
                {
                    if (ObjectList[i] != obj)
                    {
                        //if (CollisionMath.RotatedRectIntersects(ObjectList[i].Bounds, ObjectList[i].Rotation, Vector2.Zero, obj.Bounds, obj.Rotation, Vector2.Zero))
                          //  Console.WriteLine("Collisin");
                    }
                }
            }*/

        }

        protected override void Draw(Stopwatch gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //The grid must be in its own draw call.
            if (GridVisible == true)
            {
                // Grid matrix is used so that the grid zooms in and out from 0,0 relative to camera, not 0,0 relative to screen (which is top left corner).
                Matrix gridMatrix = 
                                         Matrix.CreateTranslation(new Vector3(m_camera.Width * 0.5f,
                                                                              m_camera.Height * 0.5f, 0));
                m_camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, gridMatrix);
                m_grid.Draw(m_camera);
                m_camera.End();
            }

            m_camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, m_camera.GetTransformation());
            foreach (RoomObj roomObj in RoomObjectList)
            {
                roomObj.Draw(m_camera);
            }

            ObservableCollection<GameObj> activeLayer = ControllerRef.GlobalLayerList[ControllerRef.SelectedLayerIndex];

            foreach (ObservableCollection<GameObj> objList in this.ControllerRef.GlobalLayerList)
            {
                if (activeLayer != objList)
                {
                    foreach (GameObj obj in objList)
                    {
                        obj.Opacity = 0.1f;
                        obj.Draw(m_camera);
                    }
                }
            }

            foreach (GameObj obj in activeLayer)
            {
                obj.Opacity = 1.0f;
                obj.Draw(m_camera);
            }


            m_physicsManager.DrawAllCollisionBoxes(m_camera, Consts.GenericTexture, DS2DEngine.Consts.TERRAIN_HITBOX);
            m_physicsManager.DrawAllCollisionBoxes(m_camera, Consts.GenericTexture, DS2DEngine.Consts.WEAPON_HITBOX);

            if (m_toolObject != null)
                m_toolObject.Draw();

            m_selectToolObj.Draw();
            //Draws a rectangle around all selected objects in this control.
            DrawSelectedObjectsRect();

            foreach (RoomObj room in RoomObjectList)
                m_camera.DrawString(MapDisplayXnaControl.Font, room.Width / Consts.ScreenWidth + "x" + room.Height / Consts.ScreenHeight, room.Position, Color.White, MathHelper.ToRadians(room.Rotation), Vector2.Zero, 1 / m_camera.Zoom, SpriteEffects.None, 0);

            m_camera.Draw(Consts.GenericTexture, new Rectangle(10000, (int)Camera.TopLeftCorner.Y, (int)(5  * 1 / m_camera.Zoom), (int)(2000 * 1 / m_camera.Zoom)), Color.Black);
            m_camera.DrawString(MapDisplayXnaControl.Font, "Pool Bounds", new Vector2(10000 + (10 * 1/Camera.Zoom), Camera.TopLeftCorner.Y), Color.Black, 0, Vector2.Zero, 1 * 1 / m_camera.Zoom, SpriteEffects.None, 1);

            DrawCameraView();
            m_camera.End();
        }

        private void SelectedObjectsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //De-highlights all previously selected objects.
            foreach (GameObj obj in ObjectList)
                obj.ID = 0;

            //De-highlights all previously selected room objects.
            foreach (GameObj obj in RoomObjectList)
                obj.ID = 0;

            //Highlights all selected objects.
            foreach (GameObj obj in SelectedObjects)
                obj.ID = 1;

            if (SelectedObjects.Count == 1)
                ControllerRef.ShowObjProperties(SelectedObjects[0] as IPropertiesObj);
            else if (SelectedRoomObjs.Count == 1)
                ControllerRef.ShowObjProperties(SelectedRoomObjs[0] as IPropertiesObj);
            else
                ControllerRef.ShowObjProperties(null);
            ControllerRef.RefreshObjTreeSelectedObjects();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            //Special code to convert cursor to selection tool cursor.
            if (e.Key == Key.LeftShift && this.Cursor == null && SelectedTool != Consts.TOOLTYPE_SCALE)
                this.SetCursor(Cursors.Cross);

            m_selectToolObj.Action_KeyDown(e);

            if (m_toolObject != null)
                m_toolObject.Action_KeyDown(e);

            // Special key handling for copy pasting.
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                m_ctrlHeld = true;

            if (m_ctrlHeld == true && m_clipboardKeyPressed == false)
            {
                CtrlKeyHandler(e);
            }

            if (SelectedObjects.Count > 0 && (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down))
            {
                if (m_arrowKeyHeld == false)
                {
                    m_arrowKeyHeld = true;
                    m_objStartPos.Clear();
                    foreach (GameObj obj in SelectedObjects)
                    {
                        m_objStartPos.Add(obj.Position);
                    }
                }
                ArrowKeysHandler(e);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            //Special code to convert cursor to selection tool cursor.
            if (e.Key == Key.LeftShift && this.Cursor == Cursors.Cross)
                this.SetCursor(null);

            m_selectToolObj.Action_KeyUp(e);

            if (m_toolObject != null)
                m_toolObject.Action_KeyUp(e);

            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                m_ctrlHeld = false;

            if (e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up || e.Key == Key.Down)
            {
                m_arrowKeyHeld = false;
                List<GameObj> objList = new List<GameObj>();
                List<Vector2> endPosList = new List<Vector2>();

                foreach (GameObj obj in SelectedObjects)
                {
                    objList.Add(obj);
                    endPosList.Add(obj.Position);
                }
                UndoManager.AddUndoAction(new UndoObjMovement(objList, m_objStartPos, endPosList));
            }

            m_clipboardKeyPressed = false;
        }

        private void ArrowKeysHandler(KeyEventArgs e)
        {
            int moveAmountX = 0;
            int moveAmountY = 0;

            if (e.Key == Key.Left)
                moveAmountX = -1;
            else if (e.Key == Key.Right)
                moveAmountX = 1;

            if (e.Key == Key.Up)
                moveAmountY = -1;
            else if (e.Key == Key.Down)
                moveAmountY = 1;

            if (SnapToGrid == true)
            {
                moveAmountX *= m_grid.ColDistance;
                moveAmountY *= m_grid.RowDistance;
            }

            foreach (GameObj obj in SelectedObjects)
            {
                obj.X += moveAmountX;
                obj.Y += moveAmountY;
            }

            ControllerRef.RefreshPropertiesData();
        }

        private void CtrlKeyHandler(KeyEventArgs e)
        {
            if (e.Key == Key.C)
            {
                m_clipboardKeyPressed = true;
                CopyToClipBoard();
            }
            else if (e.Key == Key.X)
            {
                m_clipboardKeyPressed = true;
                CutToClipboard();
            }
            else if (e.Key == Key.V)
            {
                m_clipboardKeyPressed = true;
                PasteFromClipboard();
            }
            else if (e.Key == Key.Z)
            {
                m_clipboardKeyPressed = true;
                UndoManager.UndoLastAction();
                ControllerRef.RefreshPropertiesData();
            }
            else if (e.Key == Key.Y)
            {
                m_clipboardKeyPressed = true;
                UndoManager.RedoLastAction();
                ControllerRef.RefreshPropertiesData();
            }
        }

        private void CopyToClipBoard()
        {
            foreach (GameObj obj in m_clipboard)
            {
                obj.Dispose();
            }
            m_clipboard.Clear();
            m_clipboardPositions.Clear();
            foreach (GameObj obj in SelectedObjects)
            {
                // Do not copy the player start location.
                if (!(obj is PlayerStartObj))
                {
                    m_clipboard.Add(obj.Clone() as GameObj);
                    m_clipboardPositions.Add(new Vector2(obj.X - SelectedObjectsRect.Center.X, obj.Y - SelectedObjectsRect.Center.Y));
                }
            }
        }

        private void CutToClipboard()
        {
            foreach (GameObj obj in m_clipboard)
            {
                obj.Dispose();
            }
            m_clipboard.Clear();
            m_clipboardPositions.Clear();
            List<GameObj> undoList = new List<GameObj>();
            foreach (GameObj obj in SelectedObjects)
            {
                // Do not copy the player start location.
                if (!(obj is PlayerStartObj))
                {
                    m_clipboardPositions.Add(new Vector2(obj.X - m_camera.X, obj.Y - m_camera.Y));
                    if (obj is RoomObj)
                        this.RoomObjectList.Remove(obj);
                    else
                        this.ObjectList.Remove(obj);
                    m_clipboard.Add(obj);
                    undoList.Add(obj);
                    if (obj is IPhysicsObj)
                        (obj as IPhysicsObj).RemoveFromPhysicsManager();
                }
            }
            if (undoList.Count > 0)
                UndoManager.AddUndoAction(new UndoObjDelete(undoList, this));
            this.ClearAllSelectedObjs();
        }

        private void PasteFromClipboard()
        {
            this.ClearAllSelectedObjs();

            ////Checking to make sure you do not copy paste rooms ontop of each other.
            //bool acceptObjectPlacements = true;
            //foreach (GameObj obj in m_clipboard)
            //{
            //    if (obj is RoomObj && acceptObjectPlacements == true)
            //    {
            //        RoomObj objToClone = obj.Clone() as RoomObj;
            //        objToClone.X += m_camera.X;
            //        objToClone.Y += m_camera.Y;
            //        //objToClone.Y += startHeight;
            //        foreach (RoomObj roomObj in RoomObjectList)
            //        {
            //            if (CollisionMath.Intersects(objToClone.Bounds, roomObj.Bounds))
            //            {
            //                acceptObjectPlacements = false;
            //                OutputControl.Trace("ERROR: Cannot paste object(s) to map. Two or more rooms are colliding with one another.");
            //                break;
            //            }
            //        }
            //    }
            //  //  else
            //    //    break;
            //}

            //if (m_clipboard.Count > 0 && acceptObjectPlacements == true)
            if (m_clipboard.Count > 0)
            {
                List<GameObj> objectsToAdd = new List<GameObj>();
                for (int i = 0; i < m_clipboard.Count; i++)
                {
                    GameObj objToCopy = m_clipboard[i].Clone() as GameObj;
                    objToCopy.X = m_camera.X + m_clipboardPositions[i].X;
                    objToCopy.Y = m_camera.Y + m_clipboardPositions[i].Y;
                    //objToCopy.X += m_camera.X;
                    //objToCopy.Y += m_camera.Y;
                    //objToCopy.Y += startHeight;
                    this.AddSprite(objToCopy, true, true);
                    this.SelectedObjects.Add(objToCopy);
                    objectsToAdd.Add(objToCopy);
                }


                UndoManager.AddUndoAction(new UndoAddSprite(objectsToAdd, this));
            }
        }

        protected override void LeftButton_MouseDown(object sender, HwndMouseEventArgs e)
        {
            this.CaptureMouse();
            OutputControl.Clear();
            m_selectToolObj.Action_MouseDown(sender, e);

            if (m_toolObject != null && m_selectToolObj.ShiftHeld == false && m_selectToolObj.CtrlHeld == false)
                m_toolObject.Action_MouseDown(sender, e);
        }

        protected override void LeftButton_MouseUp(object sender, HwndMouseEventArgs e)
        {
            this.ReleaseMouseCapture();

            m_selectToolObj.Action_MouseUp(sender, e);

            if (m_toolObject != null && m_selectToolObj.ShiftHeld == false && m_selectToolObj.CtrlHeld == false || (m_toolObject != null && m_toolObject.ToolType == Consts.TOOLTYPE_SCALE))
                m_toolObject.Action_MouseUp(sender, e);

        }

        protected override void RightButton_MouseDown(object sender, HwndMouseEventArgs e)
        {
            this.CaptureMouse();
            m_rButtonStartX = (float)e.Position.X;
            m_rButtonStartY = (float)e.Position.Y;
            m_camPosX = m_camera.X;
            m_camPosY = m_camera.Y;
        }

        protected override void RightButton_MouseUp(object sender, HwndMouseEventArgs e)
        {
            this.ReleaseMouseCapture();
            if (m_toolObject != null)
                m_toolObject.Action_MouseUp(sender, e);
        }

        protected override void MiddleButton_Scroll(object sender, HwndMouseEventArgs e)
        {
            // Up is positive, down is negative.
            if (e.WheelDelta > 0)
                m_camera.Zoom += m_camera.Zoom * 0.1f; //0.05f;
            else
                m_camera.Zoom -= m_camera.Zoom * 0.1f;//0.05f;
        }

        protected override void Mouse_MouseMove(object sender, HwndMouseEventArgs e)
        {
            //Console.WriteLine("Holder x:" + (int)e.Position.X + " Holder y:" + (int)e.Position.Y);
            //Console.WriteLine("World x:" + (int)(e.Position.X * 1/m_camera.Zoom + m_camera.TopLeftCorner.X) + " World y:" + (int)(e.Position.Y * 1/m_camera.Zoom + m_camera.TopLeftCorner.Y));

            if (e.RightButton == MouseButtonState.Pressed)
            {
                m_camera.X = (int)(m_camPosX - (e.Position.X - m_rButtonStartX) * 1/m_camera.Zoom);
                m_camera.Y = (int)(m_camPosY - (e.Position.Y - m_rButtonStartY) * 1/m_camera.Zoom);

                m_grid.RecalculatePosition(m_camera);
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //When moving the mouse, update the select tool move actions. Only move the object if it isn't set to scale or rotate.
                if (SelectedTool != Consts.TOOLTYPE_SCALE && SelectedTool != Consts.TOOLTYPE_ROTATE)
                    m_selectToolObj.Action_MouseMove(sender, e);

                //Run the selected tool's move code.
                if (m_toolObject != null && m_selectToolObj.ShiftHeld == false && m_selectToolObj.CtrlHeld == false)
                    m_toolObject.Action_MouseMove(sender, e);
                //Run the tool if shift is held and the tool is the scale tool. (Scale tool supports shift to scale uniformly).
                else if (SelectedTool == Consts.TOOLTYPE_SCALE && m_selectToolObj.CtrlHeld == false)
                    m_toolObject.Action_MouseMove(sender, e);

                //Refresh property data to reflect any changes to selected objects.
                ControllerRef.RefreshPropertiesData();
            }
        }

        public void SetTool(int toolType)
        {
            //Remove the previous tool.
            if (m_toolObject != null)
            {
                m_toolObject.Dispose();
                m_toolObject = null;
            }

            switch (toolType)
            {
                case (Consts.TOOLTYPE_RECTANGLE):
                    m_toolObject = new RectToolObj(m_camera, m_grid);
                    break;
                case (Consts.TOOLTYPE_ROTATE):
                    m_toolObject = new RotateToolObj(m_camera, m_grid);
                    break;
                case (Consts.TOOLTYPE_SCALE):
                    m_toolObject = new ScaleToolObj(m_camera, m_grid);
                    break;
                case (Consts.TOOLTYPE_ROOM):
                    m_toolObject = new RoomTool(m_camera, m_grid, this.ControllerRef);
                    break;
                case (Consts.TOOLTYPE_PLAYER_PLACEMENT):
                    m_toolObject = new PlayerPlacementTool(m_camera, m_grid);
                    break;
                case (Consts.TOOLTYPE_ORB):
                    m_toolObject = new EnemyOrbTool(m_camera, m_grid);
                    break;
                default:
                    ControllerRef.ResetAllTools();
                    break;
            }

            if (m_toolObject != null)
                m_toolObject.ControllerRef = this;
        }

        public void ResetZoom()
        {
            m_camera.Zoom = 1;
        }

        public bool SnapToGrid { get; set; }

        public void ShowObjProperties(IPropertiesObj obj)
        {
            ControllerRef.ShowObjProperties(obj);
        }

        private void DrawSelectedObjectsRect()
        {
            Rectangle selectedObjectsRect = SelectedObjectsRect;

            //Top line
            m_camera.Draw(Consts.GenericTexture,
                new Rectangle(selectedObjectsRect.X, selectedObjectsRect.Y - Consts.SELECTION_BORDERWIDTH, selectedObjectsRect.Width, Consts.SELECTION_BORDERWIDTH), Consts.SELECTION_BOX);
            //Bottom line
            m_camera.Draw(Consts.GenericTexture,
                new Rectangle(selectedObjectsRect.X, selectedObjectsRect.Y + selectedObjectsRect.Height, selectedObjectsRect.Width, Consts.SELECTION_BORDERWIDTH), Consts.SELECTION_BOX);
            //Left line
            m_camera.Draw(Consts.GenericTexture,
                new Rectangle(selectedObjectsRect.X - Consts.SELECTION_BORDERWIDTH, selectedObjectsRect.Y, Consts.SELECTION_BORDERWIDTH, selectedObjectsRect.Height), Consts.SELECTION_BOX);
            //Right line
            m_camera.Draw(Consts.GenericTexture,
                new Rectangle(selectedObjectsRect.X + selectedObjectsRect.Width, selectedObjectsRect.Y, Consts.SELECTION_BORDERWIDTH, selectedObjectsRect.Height), Consts.SELECTION_BOX);
        }

        public Rectangle SelectedObjectsRect
        {
            get
            {
                if (SelectedObjects.Count > 0)
                {
                    int leftBound = int.MaxValue, topBound = int.MaxValue, rightBound = -int.MaxValue, bottomBound = -int.MaxValue;

                    foreach (GameObj obj in SelectedObjects)
                    {
                        Vector2 upperLeft;
                        Vector2 upperRight;
                        Vector2 lowerLeft;
                        Vector2 lowerRight;

                        CollHullObj collObj = obj as CollHullObj;
                        if (collObj != null)
                        {
                            Rectangle hullRect = collObj.HullRect;
                            upperLeft = CollisionMath.UpperLeftCorner(hullRect, obj.Rotation, Vector2.Zero);
                            upperRight = CollisionMath.UpperRightCorner(hullRect, obj.Rotation, Vector2.Zero);
                            lowerLeft = CollisionMath.LowerLeftCorner(hullRect, obj.Rotation, Vector2.Zero);
                            lowerRight = CollisionMath.LowerRightCorner(hullRect, obj.Rotation, Vector2.Zero);
                        }
                        else
                        {
                            upperLeft = CollisionMath.UpperLeftCorner(obj.Bounds, obj.Rotation);
                            upperRight = CollisionMath.UpperRightCorner(obj.Bounds, obj.Rotation);
                            lowerLeft = CollisionMath.LowerLeftCorner(obj.Bounds, obj.Rotation);
                            lowerRight = CollisionMath.LowerRightCorner(obj.Bounds, obj.Rotation);
                        }

                        if (upperLeft.X < leftBound) leftBound = (int)upperLeft.X;
                        if (upperRight.X < leftBound) leftBound = (int)upperRight.X;
                        if (lowerLeft.X < leftBound) leftBound = (int)lowerLeft.X;
                        if (lowerRight.X < leftBound) leftBound = (int)lowerRight.X;

                        if (upperLeft.Y < topBound) topBound = (int)upperLeft.Y;
                        if (upperRight.Y < topBound) topBound = (int)upperRight.Y;
                        if (lowerLeft.Y < topBound) topBound = (int)lowerLeft.Y;
                        if (lowerRight.Y < topBound) topBound = (int)lowerRight.Y;

                        if (upperLeft.X > rightBound) rightBound = (int)(upperLeft.X);
                        if (upperRight.X  > rightBound) rightBound = (int)(upperRight.X );
                        if (lowerLeft.X > rightBound) rightBound = (int)(lowerLeft.X );
                        if (lowerRight.X  > rightBound) rightBound = (int)(lowerRight.X );

                        if (upperLeft.Y  > bottomBound) bottomBound = (int)(upperLeft.Y);
                        if (upperRight.Y  > bottomBound) bottomBound = (int)(upperRight.Y);
                        if (lowerLeft.Y > bottomBound) bottomBound = (int)(lowerLeft.Y);
                        if (lowerRight.Y  > bottomBound) bottomBound = (int)(lowerRight.Y);
                    }

                    return new Rectangle(leftBound, topBound, rightBound - leftBound, bottomBound - topBound);
                }

                return Rectangle.Empty;
            }
        }

        // overrideAddUndoAction is a bool that determines whether you want to add an addsprite undo action when calling this method.
        // This is necessary because the addsprite undo action calls AddSprite() when calling it's Redo execution.
        public void AddSprite(GameObj obj, bool overridePosition = false, bool overrideAddUndoAction = false)
        {
            if (overrideAddUndoAction == false)
                UndoManager.AddUndoAction(new UndoAddSprite(obj, this));

            bool addToPhysicsManager = false;

            if (obj is EnemyMapObject)
            {
                if (obj.Name == null)
                    obj.Name = "Enemy_Obj " + this.ObjectList.Count;
                addToPhysicsManager = true;
            }
            else if (obj is MapObjContainer)
            {
                if (obj.Name == null)
                    obj.Name = "Container_Obj " + this.ObjectList.Count;
                addToPhysicsManager = true;
            }
            else if (obj is MapSpriteObj)
            {
                if (obj.Name == null)
                    obj.Name = "Sprite_Obj " + this.ObjectList.Count;
                addToPhysicsManager = true;
            }
            else if (obj is RoomObj)
            {
                if (obj.Name == null)
                    obj.Name = "Room_Obj " + this.RoomObjectList.Count;
            }
            else if (obj is CollHullObj)
            {
                if (obj.Name == null)
                    obj.Name = "Coll_Hull " + this.ObjectList.Count;
            }
            else if (obj is PlayerStartObj)
            {
                if (obj.Name == null)
                    obj.Name = "Player_Start_Obj";
            }
            else if (obj is EnemyOrbObj)
            {
                if (obj.Name == null)
                    obj.Name = "Enemy_Orb_Obj " + this.ObjectList.Count;
            }
            else
            {
                if (obj.Name == null)
                    obj.Name = "Game_Obj " + this.ObjectList.Count;
            }

            if (overridePosition == false)
            {
                obj.X = m_camera.X;
                obj.Y = m_camera.Y;
            }

            if (obj is RoomObj)
                this.RoomObjectList.Add(obj as RoomObj);
            else
                this.ObjectList.Add(obj);

            if (addToPhysicsManager == true)
                m_physicsManager.AddObject(obj as IPhysicsObj);

            //if (obj is IAnimateableObj)
             //   (obj as IAnimateableObj).PlayAnimation();

            //Code to force the player object to the front of the list at all times
            if (this.ObjectList.Contains(PlayerStart))
            {
                this.ObjectList.Remove(this.PlayerStart);
                this.ObjectList.Insert(0, this.PlayerStart);
            }

            IPropertiesObj propertiesObj = obj as IPropertiesObj;
            if (propertiesObj != null)
                propertiesObj.LevelType = ControllerRef.GlobalLayerList.IndexOf(this.ObjectList);
        }

        private void DrawCameraView()
        {
            int lineWidth = 2;
            //Top line
            m_camera.Draw(Consts.GenericTexture, new Rectangle((int)(m_camera.X - (Consts.ScreenWidth * 0.5f)), (int)(m_camera.Y - (Consts.ScreenHeight * 0.5f)), Consts.ScreenWidth, lineWidth), Consts.CAMERA_BOX_COLOR);
            //Left Line
            m_camera.Draw(Consts.GenericTexture, new Rectangle((int)(m_camera.X - (Consts.ScreenWidth * 0.5f)), (int)(m_camera.Y - (Consts.ScreenHeight * 0.5f)), lineWidth, Consts.ScreenHeight), Consts.CAMERA_BOX_COLOR);
            //Bottom Line
            m_camera.Draw(Consts.GenericTexture, new Rectangle((int)(m_camera.X - (Consts.ScreenWidth * 0.5f)), (int)(m_camera.Y + (Consts.ScreenHeight * 0.5f)) - lineWidth, Consts.ScreenWidth, lineWidth), Consts.CAMERA_BOX_COLOR);
            //Right line
            m_camera.Draw(Consts.GenericTexture, new Rectangle((int)(m_camera.X + (Consts.ScreenWidth * 0.5f)) - lineWidth, (int)(m_camera.Y - (Consts.ScreenHeight * 0.5f)), lineWidth, Consts.ScreenHeight), Consts.CAMERA_BOX_COLOR);
        }

        public void ClearAllSelectedObjs()
        {
            this.ShowObjProperties(null);
            this.SelectedObjects.Clear();
        }

        public void ResetControl()
        {
            this.ClearAllSelectedObjs();
            //foreach (GameObj clipObj in m_clipboard)
            //{
            //    if (!(ObjectList.Contains(clipObj)))
            //        clipObj.Dispose();
            //}
            //m_clipboard.Clear();

            foreach (RoomObj roomObj in RoomObjectList)
            {
                if (!(m_clipboard.Contains(roomObj))) // only room the rooms not found in the clipboard.
                    roomObj.Dispose();
            }
            RoomObjectList.Clear();

            for (int i = 0; i < ControllerRef.GlobalLayerList.Count; i++)
            {
                foreach (GameObj obj in ControllerRef.GlobalLayerList[i])
                {
                    if (!(m_clipboard.Contains(obj))) // only remove game objs not found in the clipboard.
                        obj.Dispose();
                }

                ControllerRef.GlobalLayerList[i].Clear();
            }
            if (PlayerStart != null)
                PlayerStart.Dispose();  
            PlayerStart = null;

            ResetZoom();
            m_camera.Position = Vector2.Zero;
        }

        public int SelectedTool
        {
            get { return (m_toolObject != null) ? m_toolObject.ToolType : Consts.TOOLTYPE_NONE; }
        }

        public List<RoomObj> SelectedRoomObjs
        {
            get
            {
                List<RoomObj> roomObjList = new List<RoomObj>();
                foreach (GameObj obj in SelectedObjects)
                {
                    if (obj is RoomObj)
                        roomObjList.Add(obj as RoomObj);
                }
                return roomObjList;
            }
        }
    }
}
