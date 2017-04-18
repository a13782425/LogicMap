using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Logic.Core.Editor
{
    public static class LogicPanelUtility
    {
        private static Action<GenericMenu, LogicObject, Action<LogicNodeBase>, Vector2> OnShowMenu = null;
        public static void ShowCreateMenu(GenericMenu menu, LogicObject logic, Action<LogicNodeBase> add, Vector2 mousePos)
        {
            if (OnShowMenu == null)
                SetMenu(logic);
            OnShowMenu(menu, logic, add, mousePos);
        }

        private static void SetMenu(LogicObject logic)
        {
            List<Action<GenericMenu, LogicObject, Action<LogicNodeBase>, Vector2>> setAct = new List<Action<GenericMenu, LogicObject, Action<LogicNodeBase>, Vector2>>();
            foreach (Type type in typeof(LogicNodeBase).Assembly.GetTypes())
            {
                if (!type.IsClass)
                    continue;
                foreach (object item in type.GetCustomAttributes(typeof(LogicNodeAttribute), true))
                {
                    Type classType = type;
                    if (item.GetType() == typeof(LogicNodeAttribute))
                    {
                        LogicNodeAttribute node = item as LogicNodeAttribute;
                        setAct.Add((menu, lo, add, mousePos) => menu.Add(logic, node.MenuText, classType, add, mousePos));
                        break;
                    }
                }
            }
            OnShowMenu = (menu, lo, add, mousePos) => setAct.ForEach(x => x(menu, lo, add, mousePos));
        }

        private static void Add(this GenericMenu menu, LogicObject logic, string text, Type type, Action<LogicNodeBase> add, Vector2 mousePos)
        {
            menu.AddItem(new GUIContent(text), false, () => { add(LogicNodeBase.Create(logic, mousePos, type)); });
        }
    }
}