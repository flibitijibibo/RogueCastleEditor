using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastleEditor
{
    public class MapObjContainer : PhysicsObjContainer, IPropertiesObj
    {

        public int LevelType { get; set; }
        public bool OnBGLayer { get; set; }
        public bool Breakable { get; set; }

        public MapObjContainer() 
        {
            IsCollidable = false;
            IsWeighted = false;
            Breakable = false;
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
            clonedObj.Anchor = this.Anchor;
            clonedObj.LevelType = this.LevelType;
            clonedObj.Breakable = this.Breakable;
            clonedObj.IsCollidable = this.IsCollidable;
            clonedObj.IsWeighted = this.IsWeighted;
            clonedObj.OnBGLayer = this.OnBGLayer;

            return clonedObj;
        }
    }
}
