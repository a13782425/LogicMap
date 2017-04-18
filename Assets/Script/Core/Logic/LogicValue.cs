using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Logic.Core
{
    [Serializable]
    public class LogicValue
    {
        public string Key = string.Empty;

        public Object Data;

        public int TargetIndex = -1;

        public T GetData<T>() where T : Object
        {
            return this.Data as T;
        }

        public void SetData(Object data)
        {
            this.Data = data;
        }

        //public Object Obj;

        //public int GUID = -1;

        //public bool HasData { get { return data != null; } }

        //public List<Action> CallBack = new List<Action>();

        //private object data;
        //public T GetObj<T>() where T : Object
        //{
        //    return Obj as T;
        //}

        //public void SetData(object data)
        //{
        //    this.data = data;
        //}

        //public T GetData<T>()
        //{
        //    return (T)this.data;
        //}
    }
}
