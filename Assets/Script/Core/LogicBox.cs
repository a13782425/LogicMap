using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
public class LogicBox : ScriptableObject
{
    public List<LogicNodeBase> LogicNodeList = new List<LogicNodeBase>();
    public int idSet = 0;

    private ToggleAction terminated = new ToggleAction();

    public int GetNewId()
    {
        return idSet++;
    }

    void OnEnable()
    {

    }

    public void OnSetValue(List<LogicValue> Value)
    {
        Dictionary<int, LogicNodeBase> dic = LogicNodeList.ToDictionary(x => x.Guid);

    }

    public void Begin(LogicData data)
    {
        throw new System.NotImplementedException();
    }

    public void OnTerminated()
    {
        terminated.Execute();
    }

    public void Init(LogicData data)
    {
        throw new System.NotImplementedException();
    }
}

public class ToggleAction
{
    public Action Execute = () => { };
    public void Set(Action act)
    {
        Execute = () =>
        {
            act();
            Execute = () => { };
        };
    }
}