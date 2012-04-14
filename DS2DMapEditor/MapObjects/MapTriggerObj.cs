using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastleEditor
{
    public class MapTriggerObj : BlankObj, IPropertiesObj
    {
        private List<ITriggerableObj> m_triggerList; // Convert to observable collection?
        public bool IsPhysicsObj { get; set; }

        public MapTriggerObj(int width, int height)
            : base(width, height)
        {
            m_triggerList = new List<ITriggerableObj>();
        }
    }
}
