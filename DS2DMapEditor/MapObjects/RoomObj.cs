using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpriteSystem;

namespace RogueCastleEditor
{
    public class RoomObj : CollHullObj, IRoomPropertiesObj
    {
        public int SelectionMode { get; set; }
        MapDisplayXnaControl m_mapDisplayRef;
        MainWindow m_mainWindowRef;
        private RenderTarget2D m_backgroundTexture;

        public bool AddToCastlePool { get; set; }
        public bool AddToGardenPool { get; set; }
        public bool AddToTowerPool { get; set; }
        public bool AddToDungeonPool { get; set; }
        public bool DisplayBG { get; set; }

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
            DisplayBG = false;
            Camera2D cam = mapDisplayRef.Camera;

            // Creating a background texture that is a power of 2. Otherwise SamplerState.LinearWrap will not work in the draw call.
            SpriteObj background = new SpriteObj("CastleBG1_Sprite");
            background.Scale = new Vector2(512 / background.Width, 512 / background.Height);
            m_backgroundTexture = new RenderTarget2D(cam.GraphicsDevice, 512, 512);
            cam.GraphicsDevice.SetRenderTarget(m_backgroundTexture);
            cam.Begin();
            background.Draw(cam);
            cam.End();
            cam.GraphicsDevice.SetRenderTarget(null);
        }

        public override void Draw(Camera2D camera)
        {
            float bgOpacity = 1;

            if (ID == 1)
            {
                if (this.X >= 10000)
                    this.TextureColor = Consts.ROOM_OUTOFBOUNDS_COLOR_SELECTED;
                else
                    this.TextureColor = Consts.ROOM_SELECTED_COLOR;
            }
            else
            {
                bgOpacity = 0.5f;
                if (this.X >= 10000)
                    this.TextureColor = Consts.ROOM_OUTOFBOUNDS_COLOR;
                else
                    this.TextureColor = Consts.ROOM_COLOR;
            }

            if (DisplayBG == false)
                camera.Draw(Consts.GenericTexture, this.Bounds, TextureColor);
            else
            {
                camera.End();
                camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null, null, camera.GetTransformation());
                camera.Draw(m_backgroundTexture, this.Position, new Rectangle(0, 0, this.Width, this.Height), Color.White * bgOpacity, 0, Vector2.Zero, this.Scale, SpriteEffects.None, 0);
                camera.End();
                camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation());
            }

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
                                if (obj is CollHullObj)
                                {
                                    if (CollisionMath.RotatedRectIntersects(this.Bounds, 0, Vector2.Zero, new Rectangle((int)obj.X, (int)obj.Y, obj.Width, obj.Height), obj.Rotation, Vector2.Zero))
                                        objListToReturn.Add(obj);
                                }
                                else
                                {
                                    if (CollisionMath.RotatedRectIntersects(this.Bounds, 0, Vector2.Zero, obj.Bounds, obj.Rotation, new Vector2(0.5f,0.5f)))
                                        objListToReturn.Add(obj);
                                }
                                //if (CollisionMath.RotatedRectIntersects(this.Bounds, 0, Vector2.Zero, obj.Bounds, obj.Rotation, new Vector2(obj.Bounds.Width * 0.5f, obj.Bounds.Height * 0.5f)))
                                //{
                                //    objListToReturn.Add(obj);
                                //}
                                //else // Why is this here?
                                //{
                                //    if (CollisionMath.Intersects(this.Bounds, obj.Bounds))
                                //        objListToReturn.Add(obj);
                                //}
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
                            if (obj is CollHullObj)
                            {
                                if (CollisionMath.RotatedRectIntersects(this.Bounds, 0, Vector2.Zero, new Rectangle((int)obj.X, (int)obj.Y, obj.Width, obj.Height), obj.Rotation, Vector2.Zero))
                                    objListToReturn.Add(obj);
                            }
                            else
                            {
                                if (CollisionMath.RotatedRectIntersects(this.Bounds, 0, Vector2.Zero, obj.Bounds, obj.Rotation, new Vector2(0.5f, 0.5f)))
                                    objListToReturn.Add(obj);
                            }
                            //if (CollisionMath.RotatedRectIntersects(this.Bounds, 0, Vector2.Zero, obj.Bounds, obj.Rotation, new Vector2(obj.Bounds.Width * 0.5f, obj.Bounds.Height * 0.5f)))
                            //{
                            //    objListToReturn.Add(obj);
                            //}
                            //else
                            //{
                            //    if (CollisionMath.Intersects(this.Bounds, obj.Bounds))
                            //        objListToReturn.Add(obj);
                            //}
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
            if (IsDisposed == false)
            {
                m_mainWindowRef = null;
                m_mapDisplayRef = null;
                m_backgroundTexture.Dispose();
                m_backgroundTexture = null;
                base.Dispose();
            }
        }
    }
}
