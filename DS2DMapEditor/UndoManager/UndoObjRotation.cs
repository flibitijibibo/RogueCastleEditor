using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;
using Microsoft.Xna.Framework;

namespace RogueCastleEditor
{
    public class UndoObjRotation : UndoAction
    {
        List<GameObj> m_objList;
        List<float> m_startingRotation;
        List<float> m_endingRotation;
        List<Vector2> m_startPositions;
        List<Vector2> m_endPositions;

        public UndoObjRotation(List<GameObj> objList, List<float> previousRotations, List<float> newRotations, List<Vector2> startPositions, List<Vector2> endPositions)
        {
            m_objList = objList;
            m_startingRotation = previousRotations;
            m_endingRotation = newRotations;
            m_startPositions = startPositions;
            m_endPositions = endPositions;
        }

        public override void ExecuteUndo()
        {
            for (int i = 0; i < m_objList.Count; i++)
            {
                m_objList[i].Rotation = m_startingRotation[i];
                m_objList[i].Position = m_startPositions[i];
            }
        }

        public override void ExecuteRedo()
        {
            for (int i = 0; i < m_objList.Count; i++)
            {
                m_objList[i].Rotation = m_endingRotation[i];
                m_objList[i].Position = m_endPositions[i];
            }
        }

        public override void Dispose()
        {
            if (IsDisposed == false)
            {
                m_objList.Clear();
                m_objList = null;
                m_startingRotation.Clear();
                m_startingRotation = null;
                m_endingRotation.Clear();
                m_endingRotation = null;
                m_startPositions.Clear();
                m_startPositions = null;
                m_endPositions.Clear();
                m_endPositions = null;

                base.Dispose();
            }
        }
    }
}
