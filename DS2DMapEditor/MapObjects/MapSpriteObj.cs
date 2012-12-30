using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastleEditor
{
    class MapSpriteObj : PhysicsObj, IPropertiesObj
    {
        public int LevelType { get; set; }
        public bool OnBGLayer { get; set; }

        public MapSpriteObj(string spriteName, PhysicsManager physicsManager)
            : base(spriteName, physicsManager)
        {
            IsCollidable = false;
            IsWeighted = false;
        }

        public MapSpriteObj(string spriteName)
            : base(spriteName)
        {
            IsCollidable = false;
            IsWeighted = false;
        }

        protected override GameObj CreateCloneInstance()
        {
            return new MapSpriteObj(_spriteName);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            MapSpriteObj clone = obj as MapSpriteObj;
            clone.LevelType = this.LevelType;
            clone.OnBGLayer = this.OnBGLayer;
        }


        //public override object Clone()
        //{
        //    MapSpriteObj clonedSprite = new MapSpriteObj(_spriteName);
            
        //    clonedSprite.Name = this.Name;
        //    clonedSprite.Position = this.Position;
        //    clonedSprite.Scale = this.Scale;
        //    clonedSprite.Rotation = this.Rotation;
        //    clonedSprite.Flip = this.Flip;
        //    clonedSprite.LevelType = this.LevelType;
        //    clonedSprite.OnBGLayer = this.OnBGLayer;

        //    return clonedSprite;
        //}

    }
}
