using System;
using System.Text;
using System.Web.UI;
using System.Collections.Generic;
using MCS.Library.Core;

namespace MCS.Web.Library
{
    [Serializable]
    internal class ViewSateItemInternal
    {
        [NonSerialized]
        private IStateManager _Obj;

        private Type _StateType;

        private object _State;

        public ViewSateItemInternal()
        {
        }

        public ViewSateItemInternal(IStateManager obj)
        {
            this._Obj = obj;
            this._StateType = obj.GetType();

            //Style style = obj as Style;
            //if (style != null)
            //    style.SetDirty();

            this._State = obj.SaveViewState();
        }

        public Type StateType
        {
            get { return _StateType; }
            set { _StateType = value; }
        }

        public object State
        {
            get { return this._State; }
            set { this._State = value; }
        }

        public IStateManager GetObject()
        {
            if (this._Obj == null)
            {
				this._Obj = (IStateManager)TypeCreator.CreateInstance(this._StateType);
                this._Obj.LoadViewState(this._State);
            }
            return this._Obj;
        }
    }
}
