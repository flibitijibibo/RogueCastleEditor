using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DS2DEngine;
using SpriteSystem;
using System.Diagnostics;

namespace RogueCastleEditor
{
    class SpriteDisplayXnaControl : XnaControl
    {
        private const float M_SPRITEDISPLAYSIZE = 50;
        private List<GameObj> m_spriteList;
        private int m_posCountX;
        private int m_posCountY;

        private Rectangle m_separationLine;

        public SpriteDisplayXnaControl()
        {
            m_spriteList = new List<GameObj>();
        }

        protected override void loadContent(object sender, GraphicsDeviceEventArgs e)
        {
            base.loadContent(sender, e);
            ControllerRef.LoadSpriteDirectories();
        }

        public void LoadSpritesheet(string fileName)
        {
            m_spriteList.Clear();
            List<string> charNames = SpriteLibrary.LoadSpritesheet(GraphicsDevice, fileName, true);
            List<string> spriteNames = SpriteLibrary.GetAllSpriteNames(fileName);
            LoadSprites(charNames, spriteNames);
        }

        public void LoadSprites(List<string> charDataList, List<string> spriteDataList)
        {
            m_spriteList.Clear();
            m_separationLine = Rectangle.Empty;

            m_posCountX = 0;
            m_posCountY = 0;

            if (charDataList != null)
            {
                foreach (string name in charDataList)
                {
                    MapObjContainer newSprite = new MapObjContainer(name);

                        float scaleX = M_SPRITEDISPLAYSIZE / newSprite.Width;
                        float scaleY = M_SPRITEDISPLAYSIZE / newSprite.Height;
                        newSprite.Scale = new Vector2(scaleX, scaleY);
                    
                    newSprite.X = m_camera.TopLeftCorner.X - newSprite.Bounds.Left + (m_posCountX * M_SPRITEDISPLAYSIZE);
                    newSprite.Y = m_camera.TopLeftCorner.Y - newSprite.Bounds.Top + (m_posCountY * M_SPRITEDISPLAYSIZE);
                    m_spriteList.Add(newSprite);
                    newSprite.PlayAnimation();

                    if (m_posCountX >= 3)
                    {
                        m_posCountX = 0;
                        m_posCountY++;
                    }
                    else
                        m_posCountX++;
                }
            }

            if (charDataList != null)
                m_separationLine = new Rectangle((int)m_camera.TopLeftCorner.X, (int)(m_camera.TopLeftCorner.Y + (m_posCountY + 1) * M_SPRITEDISPLAYSIZE) + 10, m_camera.Width, 2);

            if (spriteDataList != null)
            {

                m_posCountX = 0;
                m_posCountY = 1;

                foreach (string name in spriteDataList)
                {
                    if (SpriteLibrary.ContainsCharacter(name) == false) // This makes sure no character data appears as sprites.
                    {
                        MapSpriteObj newSprite = new MapSpriteObj(name);
                       
                        float scaleX = (M_SPRITEDISPLAYSIZE /2f) / newSprite.Width;
                        float scaleY = (M_SPRITEDISPLAYSIZE /2f) / newSprite.Height;
                        newSprite.Scale = new Vector2(scaleX, scaleY);
                        newSprite.X = m_camera.TopLeftCorner.X - newSprite.Bounds.Left + (m_posCountX * M_SPRITEDISPLAYSIZE/2f);
                        newSprite.Y = m_separationLine.Y + (m_posCountY * M_SPRITEDISPLAYSIZE/2f);

                        m_spriteList.Add(newSprite);
                        newSprite.PlayAnimation();

                        if (m_posCountX >= 3)
                        {
                            m_posCountX = 0;
                            m_posCountY++;
                        }
                        else
                            m_posCountX++;
                    }
                }
            }
        }

        protected override void Update(Stopwatch gameTime) { }

        protected override void Draw(Stopwatch gameTime) 
        {
            GraphicsDevice.Clear(Color.Black);
            m_camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, m_camera.GetTransformation());

            m_camera.Draw(Consts.GenericTexture, m_separationLine, Color.White);

            foreach (GameObj obj in m_spriteList)
            {
                //obj.DrawBounds(m_camera, Consts.GenericTexture, Color.Red);
                obj.Draw(m_camera);
            }
            m_camera.End();
        }

        protected override void LeftButton_MouseDown(object sender, HwndMouseEventArgs e) 
        {
            foreach (GameObj obj in m_spriteList)
            {
                if (CollisionMath.Intersects(obj.Bounds, new Rectangle((int)(e.Position.X + m_camera.TopLeftCorner.X), 
                                                                        (int)(e.Position.Y + m_camera.TopLeftCorner.Y), 1, 1)))
                {
                    Vector2 storedScale = obj.Scale;
                    obj.Scale = new Vector2(1, 1); // Set object scale back to 1 before cloning.
                    GameObj objToClone = obj.Clone() as GameObj;
                    objToClone.ScaleX = 1;
                    objToClone.ScaleY = 1;
                    ControllerRef.AddSprite(objToClone);
                    obj.Scale = storedScale;
                }
            }
        }

        protected override void LeftButton_MouseUp(object sender, HwndMouseEventArgs e) { }

        protected override void RightButton_MouseDown(object sender, HwndMouseEventArgs e) { }

        protected override void RightButton_MouseUp(object sender, HwndMouseEventArgs e) { }

        protected override void MiddleButton_Scroll(object sender, HwndMouseEventArgs e) { }

        protected override void Mouse_MouseMove(object sender, HwndMouseEventArgs e) { }
    }
}
