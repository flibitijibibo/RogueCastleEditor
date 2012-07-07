using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueCastleEditor
{
    public interface IRoomPropertiesObj
    {
        int SelectionMode { get; set; }
        bool AddToCastlePool { get; set; }
        bool AddToGardenPool { get; set; }
        bool AddToTowerPool { get; set; }
        bool AddToDungeonPool { get; set; }
        bool DisplayBG { get; set; }
    }
}
