using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;


namespace LFrameWork.EditorUtils
{
    #region  GUIBeginVertical
    public class GUIBeginVertical : IDisposable
    {
        public GUIBeginVertical()
        {
            GUILayout.BeginVertical();
        }

        public GUIBeginVertical(params GUILayoutOption[] layoutOptions)
        {
            GUILayout.BeginVertical(layoutOptions);
        }


        public void Dispose()
        {
            GUILayout.EndVertical();
        }
    }
    #endregion // GUIBeginVertical



    #region  GUIBeginHorizontal
    public class GUIBeginHorizontal : IDisposable
    {
        public GUIBeginHorizontal()
        {
            GUILayout.BeginHorizontal();
        }

        public GUIBeginHorizontal(params GUILayoutOption[] layoutOptions)
        {
            GUILayout.BeginHorizontal(layoutOptions);
        }

        public void Dispose()
        {
            GUILayout.EndHorizontal();
        }
    }
    #endregion // GUIBeginHorizontal

    #region GUIColor
    public class GUIColor : IDisposable
    {
        private Color oldColor;
        public GUIColor( Color color  )
        {
            oldColor = GUI.color;
            GUI.color = color;
        }
        public void Dispose()
        {
            GUI.color = oldColor;
        }
    }
    #endregion // GUIColor
    #region GUIIndentLevel
    public class GUIIndentLevel : IDisposable
    {
        public readonly int level;
        public GUIIndentLevel(int level_ = 1)
        {
            level = level_;
            EditorGUI.indentLevel += level;
        }
        public void Dispose()
        {
            EditorGUI.indentLevel -= level;
        }
    }
    #endregion // GUIIndentLevel

    public class GUIFoldout
    {
        private static readonly Dictionary<string, bool> foldoutDic = new Dictionary<string, bool>();
        public static bool IsFoldout(string str, bool defaultFoldout)
        {
            bool isFoldout;
            if (foldoutDic.TryGetValue(str, out isFoldout))
            {
            }
            else
            {
                isFoldout = defaultFoldout;
                foldoutDic.Add(str, isFoldout);
            }

            return isFoldout;
        }
        public static void SetFoldout(string str, bool isFoldout)
        {
            foldoutDic[str] = isFoldout;
        }

        public static bool Foldout(string foldName, string content, bool defaultFoldout = true)
        {
            bool foldout = EditorGUILayout.Foldout(IsFoldout(foldName, defaultFoldout), content);
            SetFoldout(foldName, foldout);
            return foldout;
        }
    }
}