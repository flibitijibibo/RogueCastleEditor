using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastleEditor
{
    public class EnemyMapObject : MapObjContainer
    {
        public bool LockedToLedge = false;
        public EnemyDifficulty Difficulty = EnemyDifficulty.BASIC;
        public string Type = "";
        public bool Procedural { get; set; }

        public Vector2 BasicScale = new Vector2(1,1);
        public Vector2 AdvancedScale = new Vector2(1, 1);
        public Vector2 ExpertScale = new Vector2(1, 1);
        public Vector2 MinibossScale = new Vector2(1, 1);

        public float InitialLogicDelay = 0;

        public EnemyMapObject(string spriteName)
            : base(spriteName)
        {
            //m_type = type;
            Procedural = false;
        }

        public override object Clone()
        {
            EnemyMapObject clonedObj = new EnemyMapObject(_spriteName);
            clonedObj._objectList.Clear();

            foreach (GameObj obj in _objectList)
            {
                clonedObj.AddChild(obj.Clone() as GameObj);
            }

            clonedObj.Name = this.Name;
            clonedObj.Position = this.Position;
            clonedObj.Scale = this.Scale;
            clonedObj.Rotation = this.Rotation;
            clonedObj.Flip = this.Flip;
            clonedObj.SpriteName = this.SpriteName;
            clonedObj.LockedToLedge = this.LockedToLedge;
            clonedObj.Difficulty = this.Difficulty;
            clonedObj.Type = this.Type;
            clonedObj.Tag = this.Tag;
            clonedObj.InitialLogicDelay = this.InitialLogicDelay;

            clonedObj.BasicScale = this.BasicScale;
            clonedObj.AdvancedScale = this.AdvancedScale;
            clonedObj.ExpertScale = this.ExpertScale;
            clonedObj.MinibossScale = this.MinibossScale;

            clonedObj.Procedural = this.Procedural;

            return clonedObj;
        }
    }
}
