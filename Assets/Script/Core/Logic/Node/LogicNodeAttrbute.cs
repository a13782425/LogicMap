using UnityEngine;
using System.Collections;
using System;

namespace Logic.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LogicNodeAttribute : Attribute
    {
        public string MenuText;
        public LogicNodeAttribute(string menuText)
        {
            this.MenuText = menuText;
        }
    }
}
