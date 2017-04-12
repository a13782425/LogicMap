using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

[Serializable]
public class LogicValue
{

    public Object Obj;

    public int TargetIndex = -1;

    public bool HasData { get { return data != null; } }

    public List<Action> CallBack = new List<Action>();

    private object data;
    public T GetObj<T>() where T : Object
    {
        return Obj as T;
    }

    public void SetData(object data)
    {
        this.data = data;
    }

    public T GetData<T>()
    {
        return (T)this.data;
    }
}
