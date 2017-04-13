using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
public class LogicBox : ScriptableObject
{
    public List<LogicNodeBase> LogicNodeList = new List<LogicNodeBase>();
    public int idSet = 0;
    public StartNode startNode = null;

    private ToggleAction terminated = new ToggleAction();

    public int GetNewId()
    {
        return idSet++;
    }

    void OnEnable()
    {
        if (startNode == null)
            startNode = ScriptableObject.CreateInstance<StartNode>();
    }

    public void OnSetValue(List<LogicValue> logicValue)
    {
        Dictionary<int, LogicNodeBase> dic = LogicNodeList.ToDictionary(x => x.Guid);
        List<LogicValue> removeList = new List<LogicValue>();

        logicValue.ForEach(x =>
        {
            if (dic.ContainsKey(x.TargetIndex))
                dic[x.TargetIndex].SetValue(x);
            else
                removeList.Add(x);
        });
        removeList.ForEach(x => logicValue.Remove(x));

        LogicNodeList.ForEach(x =>
        {
            if (!x.IsValueSet(logicValue))
                x.GetLogicValue(GetNewValueFrom(logicValue));
        });
    }

    private Func<LogicValue> GetNewValueFrom(List<LogicValue> logicValue)
    {
        return () =>
        {
            LogicValue temp = new LogicValue();
            logicValue.Add(temp);
            return temp;
        };
    }

    public void Begin(LogicData data)
    {
        startNode.Begin(data);
        terminated.Set(() => LogicNodeList.ForEach(x => x.OnTerminated(data)));
    }

    public void OnTerminated()
    {
        terminated.Execute();
    }

    public void Init(LogicData data)
    {
        LogicNodeList.ForEach(x => x.Init(data));
    }

#if UNITY_EDITOR
    public void OnGUI()
    {
        startNode.OnDrawLink();
        LogicNodeList.ForEach(x => x.OnDrawLink());

        startNode.OnEditorGUI();
        LogicNodeList.ForEach(x => x.OnEditorGUI());
    }
#endif
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