using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DS2DEngine;

namespace RogueCastleEditor
{
    public class CollHullObj : GameObj, IPropertiesObj
    {
        public bool IsChest = false;
        public bool IsTrigger = false;
        public bool IsHazard = false;
        public bool CollidesTop = true;
        public bool CollidesBottom = true;
        public bool CollidesLeft = true;
        public bool CollidesRight = true;

        public CollHullObj(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            _width = width;
            _height = height;
        }

        public override void Draw(Camera2D camera)
        {
            if (ID == 1)
            {
                this.TextureColor = Consts.COLLHULL_SELECTED_COLOR;
                if (IsTrigger == true)
                    this.TextureColor = Consts.TRIGGEROBJ_SELECTED_COLOR;
                else if (IsChest == true)
                    this.TextureColor = Consts.CHESTOBJ_SELECTED_COLOR;
            }
            else
            {
                this.TextureColor = Consts.COLLHULL_COLOR;
                if (IsTrigger == true)
                    this.TextureColor = Consts.TRIGGEROBJ_COLOR;
                else if (IsChest == true)
                    this.TextureColor = Consts.CHESTOBJ_COLOR;
            }

            if (this.Name.Contains("Top") || this.Name.Contains("Bottom") || this.Name.Contains("Left") || this.Name.Contains("Right"))
            {
                if (ID == 1)
                    this.TextureColor = Consts.COLLHULL_UNIQUE_SELECTED_COLOR;
                else
                    this.TextureColor = Consts.COLLHULL_UNIQUE_COLOR;
            }

            if (this.Name.Contains("!Top") || this.Name.Contains("!Bottom") || this.Name.Contains("!Left") || this.Name.Contains("!Right"))
            {
                if (ID == 1)
                    this.TextureColor = Consts.COLLHULL_UNIQUE2_SELECTED_COLOR;
                else
                    this.TextureColor = Consts.COLLHULL_UNIQUE2_COLOR;
            }

            if (IsHazard)
            {
                if (ID == 1)
                    this.TextureColor = Consts.COLLHULL_KILL_SELECTED_COLOR;
                else
                    this.TextureColor = Consts.COLLHULL_KILL_COLOR;
            }
            //else if (this.Name.Contains("Damage"))
            //{
            //    if (ID == 1)
            //        this.TextureColor = Consts.COLLHULL_DAMAGE_SELECTED_COLOR;
            //    else
            //        this.TextureColor = Consts.COLLHULL_DAMAGE_COLOR;
            //}
 

            //camera.Draw(Consts.GenericTexture, this.Bounds, this.TextureColor);
            Rectangle hullRect = HullRect;

            camera.Draw(Consts.GenericTexture, hullRect, null, TextureColor, MathHelper.ToRadians(Rotation), Vector2.Zero, SpriteEffects.None, 0);

            // Top Border
            camera.Draw(Consts.GenericTexture, new Rectangle(hullRect.X, hullRect.Y, hullRect.Width, Consts.SELECTION_BORDERWIDTH), null,
                        Consts.COLLHULL_BORDER_COLOR * this.Opacity, MathHelper.ToRadians(Rotation), Vector2.Zero, SpriteEffects.None, 0);
            Vector2 rotatedPoint = CDGMath.RotatedPoint(new Vector2(0, hullRect.Height), Rotation);
            // Bottom Border
            camera.Draw(Consts.GenericTexture, new Rectangle((int)(hullRect.X + rotatedPoint.X), (int)(hullRect.Y + rotatedPoint.Y - Consts.SELECTION_BORDERWIDTH), hullRect.Width, Consts.SELECTION_BORDERWIDTH), null,
                        Consts.COLLHULL_BORDER_COLOR * this.Opacity, MathHelper.ToRadians(Rotation), Vector2.Zero, SpriteEffects.None, 0);
            // Left Border
            camera.Draw(Consts.GenericTexture, new Rectangle(hullRect.X, hullRect.Y, Consts.SELECTION_BORDERWIDTH, hullRect.Height), null,
                        Consts.COLLHULL_BORDER_COLOR * this.Opacity, MathHelper.ToRadians(Rotation), Vector2.Zero, SpriteEffects.None, 0);
            // Right Border
            rotatedPoint = CDGMath.RotatedPoint(new Vector2(hullRect.Width, 0), Rotation);
            camera.Draw(Consts.GenericTexture, new Rectangle((int)(hullRect.X + rotatedPoint.X - Consts.SELECTION_BORDERWIDTH), (int)(hullRect.Y + rotatedPoint.Y), Consts.SELECTION_BORDERWIDTH, hullRect.Height), null,
                        Consts.COLLHULL_BORDER_COLOR * this.Opacity, MathHelper.ToRadians(Rotation), Vector2.Zero, SpriteEffects.None, 0);

            //if (IsTrigger)
            //    camera.DrawString(MapDisplayXnaControl.Font, this.Name, this.Position, Color.White, MathHelper.ToRadians(Rotation), Vector2.Zero, 1 / camera.Zoom, SpriteEffects.None, 0);

            if (CollidesTop == false)
            {
                camera.Draw(Consts.GenericTexture, new Rectangle((int)(this.X + this.Width / 2 - 2), (int)(this.Y - 50), 4, 50), new Rectangle(0, 0, 1, 1), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                camera.Draw(Consts.GenericTexture, new Rectangle((int)(this.X + this.Width / 2 - 3), (int)(this.Y - 49), 4, 20), new Rectangle(0, 0, 1, 1), Color.White, MathHelper.ToRadians(-45), Vector2.Zero, SpriteEffects.None, 0);
                camera.Draw(Consts.GenericTexture, new Rectangle((int)(this.X + this.Width / 2 - 2), (int)(this.Y - 50), 4, 17), new Rectangle(0, 0, 1, 1), Color.White, MathHelper.ToRadians(45), Vector2.Zero, SpriteEffects.None, 0);
            }

            if (CollidesBottom == false)
            {
                camera.Draw(Consts.GenericTexture, new Rectangle((int)(this.X + this.Width / 2 - 2), (int)(this.Y + this.Height), 4, 50), new Rectangle(0, 0, 1, 1), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                camera.Draw(Consts.GenericTexture, new Rectangle((int)(this.X + this.Width / 2 + 2), (int)(this.Y + this.Height + 49), 4, 20), new Rectangle(0, 0, 1, 1), Color.White, MathHelper.ToRadians(135), Vector2.Zero, SpriteEffects.None, 0);
                camera.Draw(Consts.GenericTexture, new Rectangle((int)(this.X + this.Width / 2 + 2), (int)(this.Y + this.Height + 50), 4, 17), new Rectangle(0, 0, 1, 1), Color.White, MathHelper.ToRadians(-135), Vector2.Zero, SpriteEffects.None, 0);
            }

            if (CollidesLeft == false)
            {
                camera.Draw(Consts.GenericTexture, new Rectangle((int)(this.X - 50), (int)(this.Y + this.Height / 2 - 2), 50, 4), new Rectangle(0, 0, 1, 1), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                camera.Draw(Consts.GenericTexture, new Rectangle((int)(this.X - 50), (int)(this.Y + this.Height / 2 - 2), 20, 4), new Rectangle(0, 0, 1, 1), Color.White, MathHelper.ToRadians(45), Vector2.Zero, SpriteEffects.None, 0);
                camera.Draw(Consts.GenericTexture, new Rectangle((int)(this.X - 50), (int)(this.Y + this.Height / 2 - 2), 17, 4), new Rectangle(0, 0, 1, 1), Color.White, MathHelper.ToRadians(-45), Vector2.Zero, SpriteEffects.None, 0);
            }

            if (CollidesRight == false)
            {
                camera.Draw(Consts.GenericTexture, new Rectangle((int)(this.X + this.Width), (int)(this.Y + this.Height / 2 - 2), 50, 4), new Rectangle(0, 0, 1, 1), Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                camera.Draw(Consts.GenericTexture, new Rectangle((int)(this.X + this.Width + 50), (int)(this.Y + this.Height / 2 + 2), 20, 4), new Rectangle(0, 0, 1, 1), Color.White, MathHelper.ToRadians(-135), Vector2.Zero, SpriteEffects.None, 0);
                camera.Draw(Consts.GenericTexture, new Rectangle((int)(this.X + this.Width + 50), (int)(this.Y + this.Height / 2 + 2), 17, 4), new Rectangle(0, 0, 1, 1), Color.White, MathHelper.ToRadians(135), Vector2.Zero, SpriteEffects.None, 0);
            }
        }

        public override object Clone()
        {
            CollHullObj collHullToClone = new CollHullObj((int)this.X, (int)this.Y, _width, _height);
            collHullToClone.Name = this.Name;
            collHullToClone.Scale = this.Scale;
            collHullToClone.Position = this.Position;

            collHullToClone.CollidesBottom = this.CollidesBottom;
            collHullToClone.CollidesTop= this.CollidesTop;
            collHullToClone.CollidesLeft= this.CollidesLeft;
            collHullToClone.CollidesRight= this.CollidesRight;

            collHullToClone.IsTrigger = this.IsTrigger;
            collHullToClone.Rotation = this.Rotation;
            collHullToClone.IsChest = this.IsChest;
            collHullToClone.IsHazard = this.IsHazard;

            return collHullToClone;
        }

        public Rectangle HullRect
        {
            get { return new Rectangle((int)this.X, (int)this.Y, this.Width, this.Height); }
        }

        public new int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public new int Height
        {
            get { return _height; }
            set { _height = value; }
        }
    }
}
