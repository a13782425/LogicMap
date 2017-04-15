using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//using System.Linq;
public class LogicBox : ScriptableObject
{
    public List<LogicNodeBase> LogicNodeList = new List<LogicNodeBase>();
    public int idSet = 0;
    //public StartNode startNode = null;

#if UNITY_EDITOR
    public float BoxHeight = 2000;
    public float BoxWidth = 2000;
#endif

    public LogicNodeBase DefaultNode = null;

    private ToggleAction terminated = new ToggleAction();

    public int GetNewId()
    {
        return idSet++;
    }

    void OnEnable()
    {
#if UNITY_EDITOR
        if (DefaultNode == null)
            DefaultNode = ScriptableObject.CreateInstance<StartNode>();
        DefaultNode.IsDefault = true;
        DefaultNode.DefaultColor = Color.green;
#endif
    }

    public void OnSetValue(List<LogicValue> logicValue)
    {
        Dictionary<int, LogicNodeBase> dic = new Dictionary<int, LogicNodeBase>();
        foreach (LogicNodeBase item in LogicNodeList)
        {
            if (!dic.ContainsKey(item.Guid))
            {
                dic.Add(item.Guid, item);
            }
            else
            {
                Debug.LogError("重复的Key");
            }
        }
        //Dictionary<int, LogicNodeBase> dic = LogicNodeList.ToDictionary(x => x.Guid);
        List<LogicValue> removeList = new List<LogicValue>();

        logicValue.ForEach(x =>
        {
            if (dic.ContainsKey(x.GUID))
                dic[x.GUID].SetValue(x);
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
        if (DefaultNode == null)
        {
            Debug.LogError("没有启动的Node");
            return;
        }
        DefaultNode.Begin(data);
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
        DefaultNode.OnDrawLink();
        LogicNodeList.ForEach(x => x.OnDrawLink());

        DefaultNode.OnEditorGUI();
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