using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastleEditor
{
    public class PlayerStartObj : GameObj
    {
        public PlayerStartObj()
        {
            _width = Consts.PlayerWidth;
            _height = Consts.PlayerHeight;
        }

        public override void Draw(Camera2D camera)
        {
            if (ID == 1)
                this.TextureColor = Consts.PLAYER_START_SELECTED_COLOR;
            else
                this.TextureColor = Consts.PLAYER_START_COLOR;

            camera.Draw(Consts.GenericTexture, this.Bounds, this.TextureColor);
            // Top Border
            camera.Draw(Consts.GenericTexture, new Rectangle(this.Bounds.X, this.Bounds.Y, this.Bounds.Width, Consts.SELECTION_BORDERWIDTH),
                        Consts.COLLHULL_BORDER_COLOR * this.Opacity);
            // Bottom Border
            camera.Draw(Consts.GenericTexture, new Rectangle(this.Bounds.X, this.Bounds.Y + this.Bounds.Height - Consts.SELECTION_BORDERWIDTH, this.Bounds.Width, Consts.SELECTION_BORDERWIDTH),
                        Consts.COLLHULL_BORDER_COLOR * this.Opacity);
            // Left Border
            camera.Draw(Consts.GenericTexture, new Rectangle(this.Bounds.X, this.Bounds.Y, Consts.SELECTION_BORDERWIDTH, this.Bounds.Height),
                        Consts.COLLHULL_BORDER_COLOR * this.Opacity);
            // Top Border
            camera.Draw(Consts.GenericTexture, new Rectangle(this.Bounds.X + this.Bounds.Width - Consts.SELECTION_BORDERWIDTH, this.Bounds.Y, Consts.SELECTION_BORDERWIDTH, this.Bounds.Height),
                        Consts.COLLHULL_BORDER_COLOR * this.Opacity);
        }

        protected override GameObj CreateCloneInstance()
        {
            return new PlayerStartObj();
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);
        }

        //public override object Clone()
        //{
        //    PlayerStartObj clonedSprite = new PlayerStartObj();

        //    clonedSprite.Name = this.Name;
        //    clonedSprite.Position = this.Position;
        //    clonedSprite.Scale = this.Scale;
        //    clonedSprite.Rotation = this.Rotation;
        //    clonedSprite.Flip = this.Flip;

        //    return clonedSprite;
        //}
    }
}
