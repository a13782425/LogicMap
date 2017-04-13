using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LogicObject))]
public class LogicObject_Inspector : Editor
{
    private const string ASSET_PATH = "/ProjectAssets/Logic";

    //private Vector3 mouseClickPoint = Vector2.zero;

    public LogicObject logicObject;

    void OnEnable()
    {
        logicObject = target as LogicObject;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();

        if (logicObject.CurrentLogicBox == null)
        {
            if (GUILayout.Button("创建"))
            {
                logicObject.CurrentLogicBox = ScriptableObject.CreateInstance<LogicBox>();
                logicObject.CurrentLogicBox.name = "NewLogic";
            }
        }
        else
        {
            if (GUILayout.Button("打开"))
            {
                ShowLogicMap();
            }
        }


        GUILayout.EndHorizontal();
    }

    private void ShowLogicMap()
    {
        LogicPanel.ShowLogicMap(logicObject);
    }
}
