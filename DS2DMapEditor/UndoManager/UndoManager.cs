using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DS2DEngine;

namespace RogueCastleEditor
{
    public class UndoManager
    {
        private static UndoAction m_firstNode;
        private static UndoAction m_lastNode;
        private static UndoAction m_currentNode;

        public static MainWindow ControllerRef { get; set; }

        public static void AddUndoAction(UndoAction undoAction)
        {
            RemoveAllNodesPastCurrentNode();

            if (m_firstNode == null)
            {
                m_firstNode = undoAction;
                m_lastNode = undoAction;
            }
            else
            {
                undoAction.PreviousNode = m_lastNode;
                m_lastNode.NextNode = undoAction;
                m_lastNode = undoAction;
            }
            m_currentNode = m_lastNode;
            ControllerRef.ChangeMade = true;
        }

        public static void UndoLastAction()
        {
            if (m_currentNode != null)
            {
                m_currentNode.ExecuteUndo();
                m_currentNode = m_currentNode.PreviousNode;
            }
        }

        public static void RedoLastAction()
        {
            if (m_currentNode != null && m_currentNode.NextNode != null)
            {
                m_currentNode = m_currentNode.NextNode;
                m_currentNode.ExecuteRedo();
            }
            else if (m_currentNode == null && m_firstNode != null)
            {
                m_currentNode = m_firstNode;
                m_currentNode.ExecuteRedo();
            }
        }

        // Disposes all nodes past the current node.  Needed in case someone undoes up to a point, then adds a new undo action. Then every previous subsequent undo action needs to be disposed.
        private static void RemoveAllNodesPastCurrentNode()
        {
            if (m_currentNode != null)
            {
                UndoAction nodePointer = m_currentNode;
                while (nodePointer.NextNode != null)
                {
                    nodePointer = nodePointer.NextNode;
                    nodePointer.Dispose();
                }

                m_lastNode = m_currentNode;
            }
            else
                m_firstNode = null;
        }

        public static void ResetManager()
        {
            m_currentNode = m_firstNode;
            RemoveAllNodesPastCurrentNode();
            if (m_currentNode != null)
                m_currentNode.Dispose();
            m_currentNode = null;
            m_firstNode = null;
            m_lastNode = null;
        }

    }
}
