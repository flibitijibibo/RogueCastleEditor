using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastleEditor
{
    public class UndoObjScale : UndoAction
    {
        List<GameObj> m_objList;
        List<Vector2> m_startingScale;
        List<Vector2> m_endingScale;
        List<Vector2> m_startPositions;
        List<Vector2> m_endPositions;

        public UndoObjScale(List<GameObj> objList, List<Vector2> previousScale, List<Vector2> newScale, List<Vector2> startPositions, List<Vector2> endPositions)
        {
            m_objList = objList;
            m_startingScale = previousScale;
            m_endingScale = newScale;
            m_startPositions = startPositions;
            m_endPositions = endPositions;
        }

        public override void ExecuteUndo()
        {
            if (m_objList.Count == 1 && m_objList[0] is CollHullObj)
            {
                (m_objList[0] as CollHullObj).Width = (int)m_startingScale[0].X;
                (m_objList[0] as CollHullObj).Height = (int)m_startingScale[0].Y;
            }
            else
            {
                for (int i = 0; i < m_objList.Count; i++)
                {
                    m_objList[i].Scale = m_startingScale[i];
                    m_objList[i].Position = m_startPositions[i];
                }
            }
        }

        public override void ExecuteRedo()
        {
            if (m_objList.Count == 1 && m_objList[0] is CollHullObj)
            {
                (m_objList[0] as CollHullObj).Width = (int)m_endingScale[0].X;
                (m_objList[0] as CollHullObj).Height = (int)m_endingScale[0].Y;
            }
            else
            {
                for (int i = 0; i < m_objList.Count; i++)
                {
                    m_objList[i].Scale = m_endingScale[i];
                    m_objList[i].Position = m_endPositions[i];
                }
            }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                m_objList.Clear();
                m_objList = null;
                m_startingScale.Clear();
                m_startingScale = null;
                m_endingScale.Clear();
                m_endingScale = null;
                m_startPositions.Clear();
                m_startPositions = null;
                m_endPositions.Clear();
                m_endPositions = null;

                base.Dispose();
            }
        }
    }
}
