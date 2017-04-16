using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Logic.Core
{
    public abstract class LogicObjectBase : MonoBehaviour
    {
        public bool IsFinish { get; protected set; }
        public List<Action> FinishCallBack = new List<Action>();

        public abstract void Begin();

        public virtual void End()
        {
            IsFinish = true;
            FinishCallBack.ForEach(x => x());
        }
    }
}