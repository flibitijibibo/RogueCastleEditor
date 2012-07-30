using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpriteSystem;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using DS2DEngine;

namespace RogueCastleEditor
{
    public class EnemyDisplayXnaControl : XnaControl
    {
        private const float M_SPRITEDISPLAYSIZE = 50;
        private int m_posCountX;
        private int m_posCountY;

        private List<EnemyMapObject> m_enemyList;

        public EnemyDisplayXnaControl()
        {
            m_enemyList = new List<EnemyMapObject>();
        }

        protected override void loadContent(object sender, GraphicsDeviceEventArgs e)
        {
            base.loadContent(sender, e); // Code goes after.
            ParseEnemyList();
        }

        public void ParseEnemyList()
        {
            if (EditorConfig.ExecutableDirectory == "")
                OutputControl.Trace("ERROR: Could not find enemy list. Please specify the file path for the Executable Directory under Project > Directories");
            else
            {
                try
                {
                    XmlReaderSettings settings = new XmlReaderSettings();
                    settings.IgnoreComments = true;
                    settings.IgnoreWhitespace = true;

                    string filePath = EditorConfig.ExecutableDirectory.Substring(0, EditorConfig.ExecutableDirectory.LastIndexOf("\\"));
                    XmlReader reader = XmlReader.Create(filePath + "\\EnemyList.xml", settings);

                    List<EnemyData> charDataList = new List<EnemyData>();

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "EnemyObj")
                            {
                                reader.MoveToAttribute("Type");
                                string type = reader.Value;
                                reader.MoveToAttribute("SpriteName");
                                string spriteName = reader.Value;

                                reader.MoveToAttribute("BasicScaleX");
                                float basicScaleX = float.Parse(reader.Value);
                                reader.MoveToAttribute("BasicScaleY");
                                float basicScaleY = float.Parse(reader.Value);

                                reader.MoveToAttribute("AdvancedScaleX");
                                float advancedScaleX = float.Parse(reader.Value);
                                reader.MoveToAttribute("AdvancedScaleY");
                                float advancedScaleY = float.Parse(reader.Value);

                                reader.MoveToAttribute("ExpertScaleX");
                                float expertScaleX = float.Parse(reader.Value);
                                reader.MoveToAttribute("ExpertScaleY");
                                float expertScaleY = float.Parse(reader.Value);

                                reader.MoveToAttribute("MinibossScaleX");
                                float minibossScaleX = float.Parse(reader.Value);
                                reader.MoveToAttribute("MinibossScaleY");
                                float minibossScaleY = float.Parse(reader.Value);

                                charDataList.Add(new EnemyData() { Type = type, SpriteName = spriteName, 
                                    BasicScale = new Vector2(basicScaleX, basicScaleY), 
                                    AdvancedScale = new Vector2(advancedScaleX, advancedScaleY),
                                    ExpertScale = new Vector2(expertScaleX, expertScaleY),
                                    MinibossScale = new Vector2(minibossScaleX, minibossScaleY)});
                            }
                        }
                    }
                    reader.Close();
                    LoadEnemyList(charDataList);
                }
                catch (Exception e)
                {
                    OutputControl.Trace("ERROR: Could not load enemy list. Original Error: " + e.Message);
                }
            }
        }

        private void LoadEnemyList(List<EnemyData> charDataList)
        {
            foreach (EnemyData data in charDataList)
            {
                EnemyMapObject enemy = new EnemyMapObject(data.SpriteName);

                float scaleX = M_SPRITEDISPLAYSIZE / enemy.Width;
                float scaleY = M_SPRITEDISPLAYSIZE / enemy.Height;
                enemy.Scale = new Vector2(scaleX, scaleY);
                enemy.X = m_camera.TopLeftCorner.X - enemy.Bounds.Left + (m_posCountX * M_SPRITEDISPLAYSIZE);
                enemy.Y = m_camera.TopLeftCorner.Y - enemy.Bounds.Top + (m_posCountY * M_SPRITEDISPLAYSIZE);
                enemy.Type = data.Type;
                enemy.BasicScale = data.BasicScale;
                enemy.AdvancedScale = data.AdvancedScale;
                enemy.ExpertScale = data.ExpertScale;
                enemy.MinibossScale = data.MinibossScale;

                m_enemyList.Add(enemy);
                enemy.PlayAnimation();

                if (m_posCountX >= 1)
                {
                    m_posCountX = 0;
                    m_posCountY++;
                }
                else
                    m_posCountX++;
            }
        }

        protected override void Update(Stopwatch gameTime) { }

        protected override void Draw(Stopwatch gameTime) 
        {
            GraphicsDevice.Clear(Color.Black);
            m_camera.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, m_camera.GetTransformation());
            foreach (EnemyMapObject enemy in m_enemyList)
            {
                enemy.Draw(m_camera);
            }
            m_camera.End();
        }

        protected override void LeftButton_MouseDown(object sender, HwndMouseEventArgs e) 
        {
            foreach (EnemyMapObject obj in m_enemyList)
            {
                if (CollisionMath.Intersects(obj.Bounds, new Rectangle((int)(e.Position.X + m_camera.TopLeftCorner.X),
                                                                        (int)(e.Position.Y + m_camera.TopLeftCorner.Y), 1, 1)))
                {
                    Vector2 storedScale = obj.Scale;
                    obj.Scale = new Vector2(1,1);
                    EnemyMapObject objToClone = obj.Clone() as EnemyMapObject;
                    //objToClone.Scale = objToClone.BasicScale;
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

        private struct EnemyData
        {
            public string Type;
            public string SpriteName;
            public Vector2 BasicScale;
            public Vector2 AdvancedScale;
            public Vector2 ExpertScale;
            public Vector2 MinibossScale;
        }

        public List<EnemyMapObject> EnemyList
        {
            get { return m_enemyList; }
        }
    }
}
