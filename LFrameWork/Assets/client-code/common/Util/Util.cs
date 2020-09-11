using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public static class Util
{
    /// <summary>
    /// 获取unity的信息，mono的版本信息
    /// </summary>
    public static void GetUnityInfo()
    {
        Debug.LogError(Application.unityVersion);

        Type type = Type.GetType("Mono.Runtime");
        if (type != null)
        {
            MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
            if (displayName != null)
                Debug.LogError(displayName.Invoke(null, null));

            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo m = methods[i];
                Debug.LogError((m.IsPublic ? "public " : (m.IsPrivate ? "private " : "")) + (m.IsStatic ? "static " : " ") + m.ReturnType.Name + " " + m.Name + " " + m.GetParameters().Length);
            }
        }
    }

    /// <summary>
    /// 触发崩溃
    /// </summary>
    /// <param name="category"></param>
    public static void TriggerCrash(UnityEngine.Diagnostics.ForcedCrashCategory category)
    {
        UnityEngine.Diagnostics.Utils.ForceCrash(category);
    }

    /// <summary>
    /// 获取一个List数组里所有元素的toString字符串
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string GetListObjectString<T>(List<T> list)
    {
        if (list == null)
        {
            return string.Empty;
        }
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < list.Count; i++)
        {
            sb.AppendLine(string.Format("{0}:{1}", i, list[i].ToString()));
        }
        return sb.ToString();
    }

    /// <summary>
    /// 根据反射获取一个类的所有字段的值
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string GetObjectString(this object obj)
    {
        string result = string.Empty;
        Type t = obj.GetType();
        FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        StringBuilder sb = new StringBuilder();
        string base_str = "[{0}]{{{1}}}";

        foreach (FieldInfo field in fields)
        {
            var property = t.GetField(field.Name);
            if (property == null)
            {
                continue;
            }
            var value = property.GetValue(obj);
            if (value != null)
            {
                sb.AppendFormat("{0}:{1},", field.Name, value.ToString());
            }
        }
        result = string.Format(base_str, t.ToString(), sb.ToString().TrimEnd(','));
        return result;
    }

    /// <summary>
    /// 返回游戏帧数和时间
    /// </summary>
    /// <returns></returns>
    public static string GetGameTimeStr()
    {
        return string.Format("TIME:frame:{0},realtimeSinceStartup:{1}:time:{2}", UnityEngine.Time.frameCount, UnityEngine.Time.realtimeSinceStartup, UnityEngine.Time.time);
    }

    /// <summary>
    /// 打印堆栈信息
    /// </summary>
    /// <returns></returns>
    public static string PrintStackTrace()
    {
        string stackStr = null;
        //设置为true，这样才能捕获到文件路径名和当前行数，当前行数为GetFrames代码的函数，也可以设置其他参数
        System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
        //得到当前的所以堆栈
        System.Diagnostics.StackFrame[] sf = st.GetFrames();
        for (int i = 0; i < sf.Length; ++i)
        {
            stackStr = stackStr + "\r\n" + " FileName=" + sf[i].GetFileName() + " fullname=" + sf[i].GetMethod().DeclaringType.FullName + " function=" + sf[i].GetMethod().Name + " FileLineNumber=" + sf[i].GetFileLineNumber();
        }
        return stackStr;
    }

    /// <summary>
    /// 整数除法,返回小数
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static float Div(int x, int y)
    {
        if (y==0)
        {
            Debug.LogError("div cant be zero");
            return 0;
        }
        return (float)(x * 1.0 / y);
    }

    /// <summary>
    /// 整数除法,返回小数
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static float Div(long x, long y)
    {
        if (y == 0)
        {
            Debug.LogError("div cant be zero");
            return 0;
        }
        return (float)(x * 1.0 / y);
    }

    /// <summary>
    ///  计算指定文件的CRC32值
    /// </summary>
    /// <param name="fileName">指定文件的完全限定名称</param>
    /// <returns>返回值的字符串形式</returns>
    public static uint ComputeCRC32(String fileName)
    {
        uint crcValue = 0;
        //检查文件是否存在，如果文件存在则进行计算，否则返回空值
        if (System.IO.File.Exists(fileName))
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                //计算文件的CSC32值
                Common_Base.CRC32 calculator = new Common_Base.CRC32();
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
                fs.Close();
                calculator.update(buffer);
                crcValue = calculator.getValue();

            }//关闭文件流
        }
        return crcValue;
    }//ComputeCRC32
}
