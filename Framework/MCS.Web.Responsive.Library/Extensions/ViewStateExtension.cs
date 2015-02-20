using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace MCS.Web.Responsive.Library
{
    public static class ViewStateExtension
    {
        /// <summary>
        /// 获取ViewSate中某一项的值
        /// </summary>
        /// <typeparam name="V">返回值得类型</typeparam>
        /// <param name="viewState">ViewSate</param>
        /// <param name="key">某一项的Key</param>
        /// <param name="nullValue">如果此项的值为空，则返回此默认值</param>
        /// <returns>返回值</returns>
        /// <remarks>用于控件开发中，获取属性值</remarks>
        public static V GetViewStateValue<V>(this StateBag viewState, string key, V nullValue)
        {
            return GetViewStateValueInternal<V>(viewState, key, nullValue, false, false);
        }

        /// <summary>
        ///	设置ViewSate中某一项的值
        /// </summary>
        /// <typeparam name="V">设置值得类型</typeparam>
        /// <param name="viewState">ViewSate</param>
        /// <param name="key">某一项的Key</param>
        /// <param name="value">设置的值</param>
        /// <remarks>用于控件开发中，设置属性值</remarks>
        public static void SetViewStateValue<V>(this StateBag viewState, string key, V value)
        {
            viewState[key] = value;
            IStateManager sm = value as IStateManager;

            if (sm != null)
                sm.TrackViewState();
        }

        /// <summary>
        /// 将ViewState中所有IStateManager类型项，设置TrackViewState
        /// </summary>
        /// <param name="viewState">ViewState</param>
        /// <remarks>
        /// 在控件的TrackViewState中调用		
        /// </remarks>
        internal static void TrackViewState(this StateBag viewState)
        {
            foreach (string key in viewState.Keys)
            {
                IStateManager o = viewState[key] as IStateManager;

                if (o != null)
                {
                    o.TrackViewState();
                }
            }
        }

        /// <summary>
        /// 在LoadViewState之前缓存ViewState中所有IStateManager类型项
        /// </summary>
        /// <param name="viewState">ViewState</param>
        /// <returns>缓存结果</returns>		
        /// <remarks>
        /// 在LoadViewState之前调用
        /// </remarks>
        /// <example>
        /// <![CDATA[
        ///protected override void LoadViewState(object savedState)
        ///{
        ///    StateBag backState = WebControlUtility.PreLoadViewState(ViewState);
        ///    base.LoadViewState(savedState);
        ///    WebControlUtility.AfterLoadViewState(ViewState, backState);
        ///}
        /// ]]>
        /// </example>
        internal static StateBag PreLoadViewState(this StateBag viewState)
        {
            StateBag backState = new StateBag();
            foreach (string key in viewState.Keys)
            {
                IStateManager o = viewState[key] as IStateManager;
                if (o != null)
                {
                    backState[key] = o;
                }
            }

            return backState;
        }

        /// <summary>
        /// 在LoadViewState之后从缓存中恢复将ViewState中ViewSateItemInternal类型项恢复成IStateManager类型项
        /// </summary>
        /// <param name="viewState">ViewState</param>
        /// <param name="backState">PreLoadViewState产生缓存的项</param>
        /// <remarks>在LoadViewState之后调用</remarks>
        /// <example>
        /// <![CDATA[
        ///protected override void LoadViewState(object savedState)
        ///{
        ///    StateBag backState = WebControlUtility.PreLoadViewState(ViewState);
        ///    base.LoadViewState(savedState);
        ///    WebControlUtility.AfterLoadViewState(ViewState, backState);
        ///}
        /// ]]>
        /// </example>
        internal static void AfterLoadViewState(this StateBag viewState, StateBag backState)
        {
            foreach (string key in viewState.Keys)
            {
                ViewSateItemInternal vTemp = viewState[key] as ViewSateItemInternal;
                if (vTemp != null)
                {
                    if (backState[key] != null)
                    {
                        viewState[key] = backState[key];
                        ((IStateManager)viewState[key]).LoadViewState(vTemp.State);
                    }
                    else
                        viewState[key] = ((ViewSateItemInternal)viewState[key]).GetObject();
                }
            }
        }

        /// <summary>
        /// 在SaveViewState之前，将ViewState中所有IStateManager类型项转换为可序列化的ViewSateItemInternal类型项
        /// </summary>
        /// <param name="viewState">ViewState</param>
        /// <remarks>在SaveViewState之前调用</remarks>		
        /// <example>
        /// <![CDATA[
        ///protected override object SaveViewState()
        ///{
        ///    WebControlUtility.PreSaveViewState(ViewState);
        ///    object o = base.SaveViewState();
        ///    WebControlUtility.AfterSavedViewState(ViewState);
        ///    return o;
        ///}
        /// ]]>
        /// </example>
        internal static void PreSaveViewState(this StateBag viewState)
        {
            foreach (string key in viewState.Keys)
            {
                IStateManager o = viewState[key] as IStateManager;
                if (o != null)
                {
                    viewState[key] = new ViewSateItemInternal(o);
                }
            }
        }

        /// <summary>
        /// 在SaveViewState之后，将ViewState中所有ViewSateItemInternal类型项恢复成IStateManager类型项
        /// </summary>
        /// <param name="viewState">ViewState</param>
        /// <remarks>在SaveViewState之后
        /// 
        /// 调用</remarks>		
        /// <example>
        /// <![CDATA[
        ///protected override object SaveViewState()
        ///{
        ///    WebControlUtility.PreSaveViewState(ViewState);
        ///    object o = base.SaveViewState();
        ///    WebControlUtility.AfterSavedViewState(ViewState);
        ///    return o;
        ///}
        /// ]]>
        /// </example>
        internal static void AfterSavedViewState(this StateBag viewState)
        {
            foreach (string key in viewState.Keys)
            {
                object o = viewState[key];
                if (o is ViewSateItemInternal)
                {
                    viewState[key] = ((ViewSateItemInternal)o).GetObject();
                }
            }
        }

        private static V GetViewStateValueInternal<V>(StateBag viewState, string key, V nullValue, bool setNullValue, bool isTrackingViewState)
        {
            if (viewState[key] == null)
            {
                if (setNullValue)
                {
                    if (isTrackingViewState)
                    {
                        IStateManager sm = nullValue as IStateManager;
                        if (sm != null)
                        {
                            sm.TrackViewState();
                        }
                    }
                    viewState[key] = nullValue;
                }

                return nullValue;
            }

            return (V)viewState[key];
        }
    }
}
