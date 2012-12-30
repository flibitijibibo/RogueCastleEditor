using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastleEditor
{
    public class EnemyOrbObj : SpriteObj, IPropertiesObj
    {
        public int OrbType = 0;
        public int LevelType { get; set; }
        public bool ForceFlying { get; set; }
        public bool IsWaypoint { get; set; }

        public EnemyOrbObj() : base("Orb_Sprite")
        {
            //_width = 50;
            //_height = 50;
            this.Scale = new Vector2(3, 3);
        }

        public override void Draw(Camera2D camera)
        {
            if (OrbType == 0)
                TextureColor = Color.Red;
            else if (OrbType == 1)
                TextureColor = Color.Blue;
            else if (OrbType == 2)
                TextureColor = Color.LightSeaGreen;
            else
                TextureColor = Color.Yellow;

            if (ID == 1)
                this.Opacity = 1;
            else
                this.Opacity = 0.5f;

            if (IsWaypoint)
                camera.Draw(Consts.GenericTexture, new Rectangle((int)this.X - 20, (int)this.Y - 20, 40, 40), this.TextureColor);
            else
                base.Draw(camera);
        }

        protected override GameObj CreateCloneInstance()
        {
            return new EnemyOrbObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            EnemyOrbObj clone = obj as EnemyOrbObj;
            clone.OrbType = this.OrbType;
            clone.LevelType = this.LevelType;
            clone.ForceFlying = this.ForceFlying;
            clone.IsWaypoint = this.IsWaypoint;
        }

        //public override object Clone()
        //{
        //    EnemyOrbObj clonedSprite = new EnemyOrbObj();

        //    clonedSprite.Name = this.Name;
        //    clonedSprite.Position = this.Position;
        //    clonedSprite.Scale = this.Scale;
        //    clonedSprite.Rotation = this.Rotation;
        //    clonedSprite.Flip = this.Flip;

        //    clonedSprite.OrbType = this.OrbType;
        //    clonedSprite.LevelType = this.LevelType;
        //    clonedSprite.ForceFlying = this.ForceFlying;
        //    clonedSprite.IsWaypoint = this.IsWaypoint;
        //    return clonedSprite;
        //}
    }
}
