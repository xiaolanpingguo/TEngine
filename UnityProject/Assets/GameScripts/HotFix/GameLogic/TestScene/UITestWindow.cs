using UnityEngine;
using UnityEngine.UI;
using TEngine;

namespace GameLogic
{
    [Window(UILayer.UI)]
    class UITestWindow : UIWindow
    {
        #region 脚本工具生成的代码
        private Button m_btnTest;
        protected override void ScriptGenerator()
        {
            m_btnTest = FindChildComponent<Button>("m_btnTest");
            m_btnTest.onClick.AddListener(OnClickTestBtn);
            Log.Debug("UITestWindow ScriptGenerator");
        }
        #endregion

        protected override void OnRefresh()
        {
        }

        #region 事件
        private void OnClickTestBtn()
        {
            Log.Debug("OnClickTestBtn");
        }
        #endregion

    }

    [Window(UILayer.UI)]
    class UIHome : UIWindow
    {
        #region 脚本工具生成的代码
        private Button m_btnTest;
        protected override void ScriptGenerator()
        {
            m_btnTest = FindChildComponent<Button>("Start");
            m_btnTest.onClick.AddListener(OnClickTestBtn);
            Log.Debug("UIHome ScriptGenerator");
        }
        #endregion

        protected override void OnRefresh()
        {
        }

        #region 事件
        private void OnClickTestBtn()
        {
            Log.Debug("UIHome:OnClickTestBtn");
        }
        #endregion

    }
}
