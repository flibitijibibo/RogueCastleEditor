using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastleEditor
{
    public class UndoObjMovement : UndoAction
    {
        private List<GameObj> m_obj;
        private List<Vector2> m_previousPosition;
        private List<Vector2> m_newPosition;

        public UndoObjMovement(List<GameObj> obj, List<Vector2> previousPosition, List<Vector2> newPosition)
        {
            m_obj = obj;
            m_previousPosition = previousPosition;
            m_newPosition = newPosition;
        }

        public override void ExecuteUndo()
        {
            for (int i = 0; i < m_obj.Count; i++)
            {
                m_obj[i].Position = m_previousPosition[i];
            }
        }

        public override void ExecuteRedo()
        {
            for (int i = 0; i < m_obj.Count; i++)
            {
                m_obj[i].Position = m_newPosition[i];
            }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                m_obj = null;
                m_previousPosition = null;
                m_newPosition = null;

                base.Dispose();
            }
        }
    }
}
