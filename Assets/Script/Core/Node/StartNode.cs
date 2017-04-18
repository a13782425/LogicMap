using UnityEngine;
using System.Collections;

namespace Logic.Core.Node
{
    [LogicNode("添加 开始/开始")]
    public class StartNode : LogicNodeBase
    {
        public override string ShowName
        {
            get
            {
                return "开始";
            }
        }
        public override void OnEnable()
        {
            Pos = new Vector2(80, 40);
            HalfSize = new Vector2(60, 20);
        }

#if UNITY_EDITOR

        public override bool CanLinkTo(LogicNodeBase node)
        {
            return false;
        }

        public override void OnEditorGUI()
        {
            GUI.color = DefaultColor;
            GUILayout.BeginArea(GetMyRect(), "Start", "button");
            GUILayout.EndArea();
            GUI.color = Color.white;
        }

        //protected override Rect GetMyRect()
        //{
        //    return new Rect(20, 20, 120, 40);
        //}

#endif
    }
}