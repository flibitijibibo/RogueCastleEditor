using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastleEditor
{
    public class RoomObj : CollHullObj, IRoomPropertiesObj
    {
        public int SelectionMode { get; set; }
        MapDisplayXnaControl m_mapDisplayRef;
        MainWindow m_mainWindowRef;

        public bool AddToCastlePool { get; set; }
        public bool AddToGardenPool { get; set; }
        public bool AddToTowerPool { get; set; }
        public bool AddToDungeonPool { get; set; }

        public RoomObj(int x, int y, int width, int height, MapDisplayXnaControl mapDisplayRef, MainWindow mainWindowRef)
            : base(x, y, width, height)
        {
            this.X = x;
            this.Y = y;
            _width = width;
            _height = height;
            m_mapDisplayRef = mapDisplayRef;
            m_mainWindowRef = mainWindowRef;

            AddToCastlePool = true;
            AddToGardenPool = true;
            AddToTowerPool = true;
            AddToDungeonPool = true;
        }

        public override void Draw(Camera2D camera)
        {
            if (ID == 1)
                this.TextureColor = Consts.ROOM_SELECTED_COLOR;
            else
                this.TextureColor = Consts.ROOM_COLOR;

            camera.Draw(Consts.GenericTexture, this.Bounds, TextureColor);

            // Top Border
            camera.Draw(Consts.GenericTexture, new Rectangle(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, Consts.SELECTION_BORDERWIDTH),
                        Consts.ROOM_BORDER_COLOR);
            // Bottom Border
            camera.Draw(Consts.GenericTexture, new Rectangle(this.Bounds.X, this.Bounds.Y + this.Bounds.Height - Consts.SELECTION_BORDERWIDTH, this.Bounds.Width, Consts.SELECTION_BORDERWIDTH),
                        Consts.ROOM_BORDER_COLOR);
            // Left Border
            camera.Draw(Consts.GenericTexture, new Rectangle(this.Bounds.X, this.Bounds.Y, Consts.SELECTION_BORDERWIDTH, this.Bounds.Height),
                        Consts.ROOM_BORDER_COLOR);
            // Top Border
            camera.Draw(Consts.GenericTexture, new Rectangle(this.Bounds.X + this.Bounds.Width - Consts.SELECTION_BORDERWIDTH, this.Bounds.Y, Consts.SELECTION_BORDERWIDTH, this.Bounds.Height),
                        Consts.ROOM_BORDER_COLOR);
        }

        public List<GameObj> TouchingObjList
        {
            get
            {
                List<GameObj> objListToReturn = new List<GameObj>();
                if (SelectionMode == 0)
                {
                    for (int i = 0; i < m_mainWindowRef.GlobalLayerList.Count; i++)
                    {
                        foreach (GameObj obj in m_mainWindowRef.GlobalLayerList[i])
                        {
                            // Used so that objects touching the room that are rotated are also registered as touching.
                            if (obj != this && (!(obj is RoomObj)))
                            {
                                if (CollisionMath.RotatedRectIntersects(this.Bounds, 0, Vector2.Zero, obj.Bounds, obj.Rotation, new Vector2(obj.Bounds.Width * 0.5f, obj.Bounds.Height * 0.5f)))
                                {
                                    objListToReturn.Add(obj);
                                }
                                else
                                {
                                    if (CollisionMath.Intersects(this.Bounds, obj.Bounds))
                                      objListToReturn.Add(obj);
                                }
                            }
                        }
                    }
                }
                else if (SelectionMode == 1)
                {
                    foreach (GameObj obj in m_mapDisplayRef.ObjectList)
                    {
                        if (obj != this && (!(obj is RoomObj)))
                        {
                            if (CollisionMath.RotatedRectIntersects(this.Bounds, 0, Vector2.Zero, obj.Bounds, obj.Rotation, new Vector2(obj.Bounds.Width * 0.5f, obj.Bounds.Height * 0.5f)))
                            {
                                objListToReturn.Add(obj);
                            }
                            else
                            {
                                if (CollisionMath.Intersects(this.Bounds, obj.Bounds))
                                    objListToReturn.Add(obj);
                            }
                        }
                    }
                }
                return objListToReturn;
            }
        }

        public override object Clone()
        {
            RoomObj collHullToClone = new RoomObj((int)this.X, (int)this.Y, _width, _height, m_mapDisplayRef, m_mainWindowRef);
            collHullToClone.Name = this.Name;
            collHullToClone.Scale = this.Scale;
            collHullToClone.Position = this.Position;
            return collHullToClone;
        }

        public override void Dispose()
        {
            m_mainWindowRef = null;
            m_mapDisplayRef = null;
            base.Dispose();
        }
    }
}
