using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LogicObject : LogicObjectBase
{

    public List<LogicValue> Value = new List<LogicValue>();

    public bool AutoStart = false;

    public LogicBox CurrentLogicBox;

    public List<Action<LogicEvent>> LogicEventList = new List<Action<LogicEvent>>();

    public bool IsProcess { get; private set; }

    private LogicData data;
    public static void TerminateAll()
    {
        UnityEngine.Object[] logicObj = UnityEngine.Object.FindObjectsOfType(typeof(LogicObject));
        Loop.ForEach<UnityEngine.Object>(logicObj, x => (x as LogicObject).Terminate());
    }

    public void SetValue()
    {
        if (CurrentLogicBox == null)
        {
            Value = new List<LogicValue>();
            return;
        }
        CurrentLogicBox.OnSetValue(Value);
    }

    public void SendEvent(LogicEvent logicEvent)
    {
        LogicEventList.ForEach(x => x(logicEvent));
    }

    public LogicValue AddValue()
    {
        LogicValue temp = new LogicValue();
        Value.Add(temp);
        return temp;
    }

    public void Terminate()
    {
        IsProcess = false;
    }

    public override void Begin()
    {
        Debug.Log("Begin " + name);
        IsProcess = true;
        SendEvent(LogicEvent.Start);
        CurrentLogicBox.Begin(data);
    }

    public override void End()
    {
        IsProcess = false;
        CurrentLogicBox.OnTerminated();
        base.End();
        SendEvent(LogicEvent.End);
    }

    void Awake()
    {
        SetValue();
        data = new LogicData(this);
        CurrentLogicBox.Init(data);
    }

}
