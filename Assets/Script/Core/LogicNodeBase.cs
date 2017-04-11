using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

}
