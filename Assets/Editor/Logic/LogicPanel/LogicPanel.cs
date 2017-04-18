using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace Logic.Core.Editor
{
    public class LogicPanel : EditorWindow
    {
        public static string defPath;

        public static LogicObject logicObject;

        public Vector3 scroll;
        //public float logicHeight = 2000;
        //public float logicWidth = 2000;
        public Rect logicRect;

        private Rect windowRect;

        public LogicNodeBase moveSelect, logicSelect, linkSelect;
        private Vector3 offset = Vector3.zero;
        private Vector3 mousePos;

        private LogicBox logicBox { get { return logicObject.CurrentLogicBox; } }

        public static void ShowLogicMap(LogicObject logic)
        {
            EditorWindow.GetWindow(typeof(LogicPanel)).titleContent = new GUIContent("逻辑图");
            logicObject = logic;
            if (!Application.isPlaying)
                logicObject.SetValue();
            string boxPath = AssetDatabase.GetAssetPath(logic.CurrentLogicBox);
            if (!string.IsNullOrEmpty(boxPath))
            {
                string showName = boxPath.Substring(boxPath.LastIndexOf("/") + 1);
                showName = showName.Substring(0, showName.LastIndexOf("."));
                logic.CurrentLogicBox.name = showName;
            }
        }

        [MenuItem("Tools/窗口 &4", false, 4)]
        public static void Open()
        {
            EditorWindow.GetWindow(typeof(LogicPanel)).titleContent = new GUIContent("逻辑图");
        }

        void OnEnable()
        {
            defPath = Application.dataPath + "/ProjectAssets/Logic";
            wantsMouseMove = true;
        }

        void OnDestroy()
        {
            Save();
            logicObject = null;

        }

        void OnSelectionChange()
        {
            if (logicObject == null && Selection.activeTransform != null)
            {
                LogicObject temp = Selection.activeTransform.GetComponent<LogicObject>();
                if (temp != null)
                    ShowLogicMap(temp);
            }
        }

        void OnGUI()
        {
            if (logicObject == null)
                return;
            if (logicBox == null)
                return;
            EditorGUIUtility.labelWidth = 60;
            windowRect = new Rect(20, 20, position.width - 40, position.height - 60);
            mousePos = Event.current.mousePosition;

            GUILayout.BeginHorizontal();
            GUILayout.Label("逻辑图:" + logicBox.name);
            GUILayout.Space(40);
            LogicObject tempObj = EditorGUILayout.ObjectField("物体", logicObject, typeof(LogicObject), true, GUILayout.Width(200)) as LogicObject;
            GUILayout.Space(50);
            logicBox.BoxWidth = EditorGUILayout.FloatField("宽:", logicBox.BoxWidth);
            logicBox.BoxHeight = EditorGUILayout.FloatField("高:", logicBox.BoxHeight);

            logicRect = new Rect(0, 0, logicBox.BoxWidth, logicBox.BoxHeight);
            if (tempObj != logicObject)
                ShowLogicMap(tempObj);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (logicObject == null)
                return;

            GUILayout.BeginArea(windowRect, "", "box");
            scroll = GUI.BeginScrollView(new Rect(0, 0, position.width - 40, position.height - 60), scroll, logicRect);
            logicBox.OnGUI();

            GUI.EndScrollView();
            GUILayout.EndArea();

            Event evt = Event.current;

            if (linkSelect != null)
                Handles.DrawLine((Vector3)linkSelect.Pos - scroll + new Vector3(20, 20, 0), mousePos);

            if (evt.type == EventType.MouseDown)
                OnMonseDown();
            else if (evt.type == EventType.ContextClick)
                OnContextClick();
            else if (evt.type == EventType.MouseUp)
                moveSelect = null;

            if (moveSelect != null)
                moveSelect.Pos = mousePos + offset;

            if (logicSelect != null && evt.control && evt.type == EventType.KeyUp && evt.keyCode == KeyCode.D)
            {
                LogicNodeBase node = LogicNodeBase.Create(logicObject, mousePos + scroll, logicSelect.GetType());
                Add(node);
            }

            if (logicSelect != null && evt.type == EventType.KeyUp && evt.keyCode == KeyCode.Delete)
                RemoveNode(logicSelect);

            if (evt.type == EventType.KeyUp && evt.control && evt.keyCode == KeyCode.S)
                Save();

            GUILayout.BeginArea(new Rect(0, position.height - 40, position.width, 60));
            GUILayout.BeginVertical();
            GUILayout.Label(AssetDatabase.GetAssetPath(logicBox));

            if (GUILayout.Button("另存为模板"))
                SaveAsTemplate();
            GUILayout.EndVertical();
            GUILayout.EndArea();

        }

        private void Save()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void SaveAsTemplate()
        {
            string path = EditorUtility.SaveFilePanel("保存模板", defPath, "New Logic", "asset");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            if (!path.Contains(defPath))
            {
                EditorUtility.DisplayDialog("路径错误", "不能保存当前路径", "确定");
                return;
            }
            path = path.Replace(defPath, "Assets/ProjectAssets/Logic");
            if (path == AssetDatabase.GetAssetPath(logicBox))
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return;
            }
            AssetDatabase.CreateAsset(logicBox, path);
            AssetDatabase.AddObjectToAsset(logicBox.DefaultNode, path);
            logicBox.LogicNodeList.ForEach(x => AssetDatabase.AddObjectToAsset(x, path));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private void OnContextClick()
        {
            if (linkSelect != null)
            {
                linkSelect = null;
                return;
            }
            GenericMenu menu = new GenericMenu();
            if (logicSelect != null)
                ShowNodeOption(menu);
            else if (windowRect.Contains(mousePos))
                LogicPanelUtility.ShowCreateMenu(menu, logicObject, Add, mousePos + scroll);
            menu.ShowAsContext();
        }

        private void RemoveNode(LogicNodeBase node)
        {
            if (node.IsDefault)
            {
                EditorUtility.DisplayDialog("移除错误", "不能移除开始Node,请更换开始Node", "确定");
                return;
            }
            logicBox.LogicNodeList.Remove(node);
            node.OnRemove(logicObject.Value);
            DestroyImmediate(node, true);
            if (logicBox != null)
            {
                EditorUtility.SetDirty(logicBox);
                AssetDatabase.SaveAssets();
            }
        }

        private void ShowNodeOption(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("链接"), false, () => linkSelect = logicSelect);
            logicSelect.OnGenericMenu(menu);
            logicSelect.OnGenericMenu(menu, () => linkSelect = logicSelect);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("设置默认"), false, () =>
            {
                logicBox.LogicNodeList.Remove(logicSelect);
                logicBox.LogicNodeList.Add(logicBox.DefaultNode);
                logicBox.DefaultNode.DefaultColor = Color.white;
                logicBox.DefaultNode.IsDefault = false;
                logicSelect.IsDefault = true;
                logicBox.DefaultNode = logicSelect;
                logicSelect.DefaultColor = Color.green;
                Save();
            });
            //if (!(logicSelect is StartNode))
            {
                menu.AddItem(new GUIContent("删除" + logicSelect.ShowName), false, () => RemoveNode(logicSelect));
                menu.AddItem(new GUIContent("打开脚本"), false, () =>
                {
                    string[] guids = AssetDatabase.FindAssets(logicSelect.GetType().Name);
                    if (guids.Length != 1)
                    {
                        Debug.LogError("guids存在多个");
                    }
                    else
                    {
                        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(UnityEngine.Object)), -1);
                    }
                    //AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(textBuffer.guid), typeof(UnityEngine.Object)), caretPosition.line + 1);
                });
            }
        }

        private void Add(LogicNodeBase node)
        {
            node.Guid = logicBox.GetNewId();
            logicBox.LogicNodeList.Add(node);
            node.InitData();
            //node.GetLogicValue(logicObject.AddValue);

            string boxPath = AssetDatabase.GetAssetPath(logicBox);
            if (!string.IsNullOrEmpty(boxPath))
            {
                AssetDatabase.AddObjectToAsset(node, boxPath);
                AssetDatabase.SaveAssets();
            }
            EditorUtility.SetDirty(logicBox);
            EditorUtility.SetDirty(logicObject);
        }



        private void OnMonseDown()
        {
            LogicNodeBase crtSelect = null;
            Vector3 detectPos = mousePos - new Vector3(20, 20, 0) + scroll;

            logicSelect = null;

            logicBox.LogicNodeList.ForEach(x => { if (x.OnDetectRect(detectPos)) crtSelect = x; });
            if (logicBox.DefaultNode.OnDetectRect(detectPos))
                crtSelect = logicBox.DefaultNode;

            if (crtSelect == null)
                return;

            logicSelect = crtSelect;
            if (linkSelect != null && crtSelect != linkSelect && crtSelect.CanLinkTo(linkSelect))
                linkSelect.OnLinkTo(crtSelect);
            else
            {
                moveSelect = crtSelect;
                offset = (Vector3)moveSelect.Pos - mousePos;
            }
            linkSelect = null;
        }


        private void Update()
        {
            Repaint();
        }
    }
}