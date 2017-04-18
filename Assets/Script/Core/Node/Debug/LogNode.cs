using UnityEngine;
using System.Collections;

namespace Logic.Core.Node
{
    [LogicNode("添加 Debug/打印日志")]
    public class LogNode : LogicNodeBase
    {
        public string log = "触发";

        public override string ShowName
        {
            get
            {
                return "日志";
            }
        }

        public override void Begin(LogicData data)
        {
            Debug.Log("[日志] " + log, data.LogicContainer);
            base.Continue(data);
        }

#if UNITY_EDITOR
        protected override void OnGUI()
        {
            log = UnityEditor.EditorGUILayout.TextField("内容:", log);
        }
#endif
    }
}
