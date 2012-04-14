using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueCastleEditor
{
    public abstract class UndoAction
    {
        public UndoAction NextNode = null;
        public UndoAction PreviousNode = null;
        private bool m_isDisposed = false;

        public abstract void ExecuteUndo();
        public abstract void ExecuteRedo();
        public virtual void Dispose()
        {
            m_isDisposed = true;
        }

        public bool IsDisposed
        {
            get { return m_isDisposed; }
        }
    }
}
