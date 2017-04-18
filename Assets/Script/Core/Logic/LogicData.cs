using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Logic.Core.Utils;
using Object = UnityEngine.Object;

//using System.Linq;

namespace Logic.Core
{
    public class LogicData
    {
        public LogicObject LogicContainer;

        public int TargetIndex = -1;

        private Dictionary<string, LogicValue> ValueDic;

        public LogicValue this[string key]
        {
            get
            {
                if (!this.ContainsKey(key))
                {
                    LogicValue logicValue = new LogicValue();
                    logicValue.Key = key;
                    logicValue.TargetIndex = this.TargetIndex;
                    ValueDic.Add(key, logicValue);
                }
                return this.ValueDic[key];
            }
            set
            {
                if (!this.ContainsKey(key))
                {
                    ValueDic.Add(key, value);
                }
                else
                {
                    ValueDic[key] = value;
                }
            }

        }


        public LogicData(LogicObject logic)
            : this(-1, logic)
        { }

        public LogicData(int index, LogicObject logic)
            : this(index, logic, new List<LogicValue>())
        { }

        public LogicData(int index, LogicObject logic, List<LogicValue> list)
        {
            this.LogicContainer = logic;
            this.TargetIndex = index;
            this.ValueDic = list.ToDictionary(x => x.Key);
        }


        public T GetValue<T>(string key) where T : Object
        {
            return this.ValueDic[key].GetData<T>();
        }

        public void SetValue(string key, Object data)
        {
            this.ValueDic[key].SetData(data);
        }
        public void SetValue(LogicValue value)
        {
            this.SetValue(value.Key, value.Data);
        }

        public bool ContainsKey(string key)
        {
            return ValueDic.ContainsKey(key);
        }


        //public LogicObject LogicContainer;
        //public Dictionary<int, LogicValue> ValueDic;
        //public bool IsProcess { get { return LogicContainer.IsProcess; } }
        //public LogicValue this[int guid] { get { return ValueDic[guid]; } }

        //public LogicData() { }

        //public LogicData(LogicObject logicContainer)
        //{
        //    ValueDic = new Dictionary<int, LogicValue>();
        //    this.LogicContainer = logicContainer;
        //    foreach (LogicValue item in logicContainer.Value)
        //    {
        //        if (!ValueDic.ContainsKey(item.GUID))
        //        {
        //            ValueDic.Add(item.GUID, item);
        //        }
        //        else
        //        {
        //            Debug.LogError("重复的Key");
        //        }
        //    }
        //    //ValueDic = logicContainer.Value.ToDictionary(x => x.TargetIndex);
        //}

        //public bool ContainsKey(int guid)
        //{
        //    return ValueDic.ContainsKey(guid);
        //}

     
    }
}