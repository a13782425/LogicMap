using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;

public class LogicData
{
    public LogicObject LogicContainer;
    public Dictionary<int, LogicValue> ValueDic;
    public bool IsProcess { get { return LogicContainer.IsProcess; } }
    public LogicValue this[int guid] { get { return ValueDic[guid]; } }

    public LogicData() { }

    public LogicData(LogicObject logicContainer)
    {
        this.LogicContainer = logicContainer;
        foreach (LogicValue item in logicContainer.Value)
        {
            if (!ValueDic.ContainsKey(item.GUID))
            {
                ValueDic.Add(item.GUID, item);
            }
            else
            {
                Debug.LogError("重复的Key");
            }
        }
        //ValueDic = logicContainer.Value.ToDictionary(x => x.TargetIndex);
    }

    public bool ContainsKey(int guid)
    {
        return ValueDic.ContainsKey(guid);
    }
}
