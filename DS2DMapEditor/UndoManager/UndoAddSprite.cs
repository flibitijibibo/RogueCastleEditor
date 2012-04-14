using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastleEditor
{
    public class UndoAddSprite : UndoAction
    {
        List<GameObj> m_obj;
        MapDisplayXnaControl m_controllerRef;
        
        public UndoAddSprite(List<GameObj> obj, MapDisplayXnaControl controllerRef)
        {
            m_obj = obj;
            m_controllerRef = controllerRef;
        }

        public UndoAddSprite(GameObj obj, MapDisplayXnaControl controllerRef)
        {
            m_obj = new List<GameObj>();
            m_obj.Add(obj);
            m_controllerRef = controllerRef;
        }

        public override void ExecuteUndo()
        {
            m_controllerRef.ClearAllSelectedObjs();
            foreach (GameObj obj in m_obj)
            {
                if (obj is RoomObj)
                    m_controllerRef.RoomObjectList.Remove(obj);
                else
                    m_controllerRef.ObjectList.Remove(obj);
                //DO NOT dispose this GameObj when removing it.  Otherwise ExecuteRedo() will not work since it was disposed.

                if (obj is IPhysicsObj)
                    (obj as IPhysicsObj).RemoveFromPhysicsManager();

                if (obj is PlayerStartObj)
                    m_controllerRef.PlayerStart = null;
            }
        }

        public override void ExecuteRedo()
        {
            m_controllerRef.ClearAllSelectedObjs();
            foreach (GameObj obj in m_obj)
            {
                m_controllerRef.AddSprite(obj, true, true);
                m_controllerRef.SelectedObjects.Add(obj);

                if (obj is PlayerStartObj)
                    m_controllerRef.PlayerStart = obj as PlayerStartObj;
            }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                foreach (GameObj obj in m_obj)
                {
                    obj.Dispose();
                }
                m_obj.Clear();
                m_obj = null;
                m_controllerRef = null;

                base.Dispose();
            }
        }
    }
}
