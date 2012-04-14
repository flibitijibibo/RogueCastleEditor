using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastleEditor
{
    public class UndoObjDelete : UndoAction
    {
        private List<GameObj> m_objList;
        private MapDisplayXnaControl m_controllerRef;

        public UndoObjDelete(List<GameObj> objsToDeleteList, MapDisplayXnaControl controllerRef)
        {
            m_objList = objsToDeleteList;
            m_controllerRef = controllerRef;
        }

        public override void ExecuteUndo()
        {
            foreach (GameObj obj in m_objList)
            {
                // AddSprite overwrites the name of objects.
                string objName = obj.Name;
                m_controllerRef.AddSprite(obj, true, true);
                obj.Name = objName;

                if (obj is PlayerStartObj)
                    m_controllerRef.PlayerStart = obj as PlayerStartObj;
            }
        }

        public override void ExecuteRedo()
        {
            foreach (GameObj obj in m_objList)
            {
                m_controllerRef.ObjectList.Remove(obj);
                if (obj is IPhysicsObj)
                    (obj as IPhysicsObj).RemoveFromPhysicsManager();

                if (obj is PlayerStartObj)
                    m_controllerRef.PlayerStart = null;
            }
            m_controllerRef.ClearAllSelectedObjs();
        }

        public override void Dispose()
        {
            if (IsDisposed == true)
            {
                foreach (GameObj obj in m_objList)
                {
                    obj.Dispose();
                }
                m_objList.Clear();
                m_objList = null;

                m_controllerRef = null;

                base.Dispose();
            }
        }

    }
}
