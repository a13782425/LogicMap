using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

public class LogicNodeBase : ScriptableObject
{
    public int Guid = 0;
    public string ShowName = "逻辑节点";
    public string Common = "";

    public Vector2 Pos = Vector2.zero;
    public Vector2 HalfSize = new Vector2(80, 25);

    public List<LogicNodeBase> Link = new List<LogicNodeBase>();

    protected LogicValue logicValue;

    public virtual bool HasValue { get { return false; } }

    public virtual bool HasSwitch { get { return false; } }

    public static LogicNodeBase Create(Vector2 pos, Type type)
    {
        LogicNodeBase nb = ScriptableObject.CreateInstance(type) as LogicNodeBase;
        nb.Pos = pos;
        return nb;
    }

    public virtual void SetValue(LogicValue value)
    {
        logicValue = value;
    }
    public virtual void Init(LogicData data)
    {
        Link.ForEach(x => { if (x == null)Debug.LogError("node " + ShowName + " has null link", this); });
    }

    public virtual void OnEnable() { }

    public virtual void SetActive(LogicData data, bool state) { }

    public virtual void Begin(LogicData data)
    {
        Continue(data);
    }

    public void Continue(LogicData data)
    {
        this.Continue(Link, data);
    }

    public void Continue(List<LogicNodeBase> nodeLink, LogicData data)
    {
        if (data.IsProcess && (nodeLink != null))
            nodeLink.ForEach(x => { if (x != null)x.Begin(data); });
#if UNITY_EDITOR
        else
            Debug.LogWarning("执行错误：" + data.LogicContainer.name, data.LogicContainer);
#endif
    }
    public virtual bool IsValueSet(List<LogicValue> valueList)
    {
        if (!HasValue)
            return true;
        return valueList.Contains(logicValue);
    }

    public void GetLogicValue(Func<LogicValue> OnGetValue)
    {
        if (!HasValue)
            return;
        logicValue = OnGetValue();
        logicValue.TargetIndex = this.Guid;
    }

    public virtual void OnTerminated(LogicData data) { }

    protected LogicValue GetLogicValue(LogicData data)
    {
        if (!data.ContainsKey(this.Guid))
            throw new System.Exception("");
        return data[this.Guid];
    }

    protected T GetValue<T>(LogicData data) where T : UnityEngine.Object
    {
        if (!data.ContainsKey(this.Guid))
            Exception("Not Exist");
        LogicValue value = data[this.Guid];
        if (value == null)
            Exception("assigned");
        T t = value.Obj as T;
        if (t = null)
            Exception("match");
        return t;
    }
    protected void Exception(string msg)
    {
        Debug.LogError("Error_" + ShowName + " : " + msg, this);
        throw new System.Exception(msg);
    }


#if UNITY_EDITOR
    public virtual void OnRemove(List<LogicValue> value)
    {
        if (logicValue != null)
            value.Remove(logicValue);
    }

    public virtual void OnGenericMenu(UnityEditor.GenericMenu menu)
    {
        DeleteLinkMenu(Link, menu, "移除链接");
    }

    public virtual void OnGenericMenu(UnityEditor.GenericMenu menu, Action act)
    {
        DeleteLinkMenuAndReLink(Link, menu, "移除链接并连接", act);
    }

    public virtual void OnLinkTo(LogicNodeBase node)
    {
        if (!Link.Contains(node) && node != this)
            Link.Add(node);
    }

    public virtual bool CanLinkTo(LogicNodeBase node)
    {
        return true;
    }

    public virtual void OnDrawLink()
    {
        DrawLink(Link);
    }

    public virtual void OnEditorGUI()
    {
        UnityEditor.Undo.RecordObject(this, "Logic");
        GUILayout.BeginArea(GetMyRect(), "", "button");
        GUILayout.Label(ShowName);
        OnGUI();
        GUILayout.EndArea();

    }

    protected virtual void OnGUI()
    {
    }

    public virtual bool OnDetectRect(Vector2 mousePoint)
    {
        return GetMyRect().Contains(mousePoint);
    }

    private Rect GetMyRect()
    {
        return new Rect(Pos.x - HalfSize.x, Pos.y - HalfSize.y, HalfSize.x * 2, HalfSize.y * 2);
    }

    public void Assign<T>(LogicValue value, string title) where T : UnityEngine.Object
    {
        if (value == null)
        {
            ShowName = title + "[Not Assigned]";
            return;
        }
        GUILayout.BeginHorizontal();
        T t = UnityEditor.EditorGUILayout.ObjectField(value.Obj, typeof(T), true) as T;
        value.Obj = t;
        if (t == null)
            ShowName = title + "[Not Assigned]";
        else
            ShowName = title + "[" + t.name + "]";

        if (GUILayout.Button("选择") && t != null)
            UnityEditor.Selection.activeObject = t;
        GUILayout.EndHorizontal();
    }


    public void DrawLink<T>(List<T> nodes) where T : LogicNodeBase
    {
        DrawLink<T>(nodes, Color.white);
    }

    public void DrawLink<T>(List<T> nodes, Color c) where T : LogicNodeBase
    {
        Handles.color = c;
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] != null)
                DrawArrowTo(nodes[i]);
            else
            {
                nodes.RemoveAt(i);
                i--;
            }
        }
        Handles.color = Color.white;
    }

    public void DrawLinkEach(Color c, params LogicNodeBase[] nodes)
    {
        Handles.color = c;
        Loop.Count(nodes.Length).Do(x => { if (nodes[x] != null)DrawArrowTo(nodes[x]); });
        Handles.color = Color.white;
    }


    public void DeleteLinkMenu<T>(List<T> nodes, UnityEditor.GenericMenu menu, string title) where T : LogicNodeBase
    {
        menu.AddSeparator("");
        nodes.ForEach(x => menu.AddItem(new GUIContent(title + "/" + x.ShowName), false, () => nodes.Remove(x)));
    }

    public void DeleteLinkMenuAndReLink<T>(List<T> nodes, UnityEditor.GenericMenu menu, string title, Action act) where T : LogicNodeBase
    {
        menu.AddSeparator("");
        nodes.ForEach(x => menu.AddItem(new GUIContent(title + "/" + x.ShowName), false, () => { nodes.Remove(x); act(); }));
    }


    public void DrawArrowTo(LogicNodeBase node)
    {
        float rTheta = Mathf.PI / 6.0f;
        float cosTheta = Mathf.Cos(rTheta);
        float sinTheta = Mathf.Sin(rTheta);

        float len = 10.0f;
        float len2 = 0.5f * len;

        Vector2 dir = (node.Pos - this.Pos).normalized;
        Vector2 mid = (node.Pos + this.Pos) / 2f;

        Vector2 q1 = mid - len * new Vector2(dir.x * cosTheta - dir.y * sinTheta, dir.x * sinTheta + dir.y * cosTheta);
        Vector2 q2 = mid - len * new Vector2(dir.x * cosTheta + dir.y * sinTheta, -dir.x * sinTheta + dir.y * cosTheta);
        Vector2 q3 = mid - dir * len2;

        Handles.DrawLine(mid, q1);
        Handles.DrawLine(mid, q2);
        Handles.DrawLine(q1, q3);
        Handles.DrawLine(q2, q3);

        Handles.DrawLine(this.Pos, node.Pos);
    }


    protected bool ValueMatch<T>(LogicNodeBase node, ref T t) where T : LogicNodeBase
    {
        bool isMatch = node is T;
        if (isMatch)
            t = node as T;
        return isMatch;
    }
#endif

}
