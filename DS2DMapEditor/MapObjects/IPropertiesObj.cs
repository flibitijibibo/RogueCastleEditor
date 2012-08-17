using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RogueCastleEditor
{
    public interface IPropertiesObj
    {
        string Name { get; set; }
        float X { get; set; }
        float Y { get; set; }
        int Width { get; }
        int Height { get; }

        float Rotation { get; set; }
        float ScaleX { get; set; }
        float ScaleY { get; set; }

        SpriteEffects Flip {get;set;}

        int LevelType { get; set; }
    }
}
