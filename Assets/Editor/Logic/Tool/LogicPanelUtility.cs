using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

public static class LogicPanelUtility
{
    private static Action<GenericMenu, Action<LogicNodeBase>, Vector2> OnShowMenu = null;
    public static void ShowCreateMenu(GenericMenu menu, Action<LogicNodeBase> add, Vector2 mousePos)
    {
        if (OnShowMenu == null)
            SetMenu();
        OnShowMenu(menu, add, mousePos);
    }

    private static void SetMenu()
    {
        List<Action<GenericMenu, Action<LogicNodeBase>, Vector2>> setAct = new List<Action<GenericMenu, Action<LogicNodeBase>, Vector2>>();
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
                    setAct.Add((menu, add, mousePos) => menu.Add(node.MenuText, classType, add, mousePos));
                    break;
                }
            }
        }
        OnShowMenu = (menu, add, mousePos) => setAct.ForEach(x => x(menu, add, mousePos));
    }

    private static void Add(this GenericMenu menu, string text, Type type, Action<LogicNodeBase> add, Vector2 mousePos)
    {
        menu.AddItem(new GUIContent(text), false, () => { add(LogicNodeBase.Create(mousePos, type)); });
    }
}
