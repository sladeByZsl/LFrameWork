using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorExtension
{
    [MenuItem("GameObject/Copy Path", priority = 49)]
    public static void CopySelectWidgetName()
    {
        string result = "";
        foreach (var item in Selection.gameObjects)
        {
            string item_name = item.name;
            Transform root_trans = item.transform.parent;
            while (root_trans != null)
            {
                if (root_trans.parent != null && !root_trans.parent.name.Contains("Canvas"))
                    item_name = root_trans.name + "/" + item_name;
                else
                    break;
                root_trans = root_trans.parent;
            }
            result = result + "\"" + item_name + "\"";
        }

        //复制到系统全局的粘贴板上
        result = result.Replace("\"", "");
        GUIUtility.systemCopyBuffer = result;
        Debug.Log(result);
    }
}
