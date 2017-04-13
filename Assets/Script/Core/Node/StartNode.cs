using UnityEngine;
using System.Collections;

public class StartNode : LogicNodeBase
{
    public override void OnEnable()
    {
        Pos = new Vector2(80, 40);
    }

#if UNITY_EDITOR

    public override bool CanLinkTo(LogicNodeBase node)
    {
        return false;
    }

    public override void OnEditorGUI()
    {
        GUI.color = Color.green;
        GUILayout.BeginArea(GetMyRect(), "Start", "button");
        GUILayout.EndArea();
        GUI.color = Color.white;
    }

    protected override Rect GetMyRect()
    {
        return new Rect(20, 20, 120, 40);
    }

#endif
}
