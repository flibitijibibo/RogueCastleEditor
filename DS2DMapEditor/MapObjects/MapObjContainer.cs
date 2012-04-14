using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastleEditor
{
    public class MapObjContainer : PhysicsObjContainer, IPropertiesObj
    {
        public MapObjContainer() 
        {
            IsCollidable = false;
            IsWeighted = false;
        }

        public MapObjContainer(string spriteName)
            : base(spriteName)
        {
            IsCollidable = false;
            IsWeighted = false;
        }

        public override object Clone()
        {
            MapObjContainer clonedObj = new MapObjContainer();

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

            return clonedObj;
        }
    }
}
