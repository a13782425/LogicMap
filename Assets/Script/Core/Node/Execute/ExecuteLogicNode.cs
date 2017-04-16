using UnityEngine;
using System.Collections;

namespace Logic.Core.Node
{
    [LogicNode("添加 执行/逻辑图")]
    public class ExecuteLogicNode : LogicNodeBase
    {
        public override bool HasValue
        {
            get
            {
                return true;
            }
        }

        public override void Begin(LogicData data)
        {
            base.Begin(data);
            LogicObject obj = GetValue<LogicObject>(data);
            obj.Begin();
            base.Continue(data);
        }


#if UNITY_EDITOR
        public override void OnEnable()
        {
            base.OnEnable();
            ShowName = "执行";
        }

        protected override void OnGUI()
        {
            Assign<LogicObject>(logicValue, "逻辑图");
        }
#endif
    }
}