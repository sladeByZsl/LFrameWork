using System;

public class SingletonBase<T> where T: class, new()
{
    private static T _instance;
    private static readonly object syslock;

    static SingletonBase()
    {
        SingletonBase<T>.syslock = new object();
    }

    public static T GetInstance()
    {
        if (SingletonBase<T>._instance == null)
        {
            object syslock = SingletonBase<T>.syslock;
            lock (syslock)
            {
                if (SingletonBase<T>._instance == null)
                {
                    SingletonBase<T>._instance = Activator.CreateInstance<T>();
                }
            }
        }
        return SingletonBase<T>._instance;
    }
}

