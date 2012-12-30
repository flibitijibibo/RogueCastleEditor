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
        public int LevelType { get; set; }

        public MapTriggerObj(int width, int height)
            : base(width, height)
        {
            m_triggerList = new List<ITriggerableObj>();
        }

        protected override GameObj CreateCloneInstance()
        {
            return new MapTriggerObj(Width, Height);
        }

        protected override void FillCloneInstance(object obj)
        {
            base.FillCloneInstance(obj);

            MapTriggerObj clone = obj as MapTriggerObj;
            clone.LevelType = this.LevelType;
        }

        //public override object Clone()
        //{
        //    MapTriggerObj triggerObj = new MapTriggerObj(Width, Height);
        //    triggerObj.LevelType = this.LevelType;
        //    return triggerObj;
        //}
    }
}
