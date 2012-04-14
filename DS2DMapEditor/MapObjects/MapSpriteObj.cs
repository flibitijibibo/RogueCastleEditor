using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastleEditor
{
    class MapSpriteObj : PhysicsObj, IPropertiesObj
    {
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

        public override object Clone()
        {
            MapSpriteObj clonedSprite = new MapSpriteObj(_spriteName);
            
            clonedSprite.Name = this.Name;
            clonedSprite.Position = this.Position;
            clonedSprite.Scale = this.Scale;
            clonedSprite.Rotation = this.Rotation;
            clonedSprite.Flip = this.Flip;

            return clonedSprite;
        }

    }
}
