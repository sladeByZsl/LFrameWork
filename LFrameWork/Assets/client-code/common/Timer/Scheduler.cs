using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// The singleton scheduler used to schedule tasks, include:
/// * Frame update event.
/// * Work thread send notify task to main thread.
/// * Send task to next frame.
/// * Wait an <see cref="AsyncOperation"/> finish.
/// * Wait a specify of time to execute.
/// </summary>
[DisallowMultipleComponent]
public sealed class Scheduler : MonoBehaviour
{
    private static Scheduler instance;
   
    // The update tasks.
    private static LinkedList<Action> updateTasks =
        new LinkedList<Action>();

    private static LinkedList<Action> fixedUpdateTasks =
        new LinkedList<Action>();

    private static LinkedList<Action> lateUpdateTasks =
        new LinkedList<Action>();

    // The executing buffer.
    private static List<Action> executing = new List<Action>();

    // The thread safe task queue.
    private static List<Action> postTasks = new List<Action>();

    // The task to execute on the next frame.
    private static List<Action> nextFrameTasks1 = new List<Action>();
    private static List<Action> nextFrameTasks2 = new List<Action>();
    private static int lastUpdateFrame;

    // The task to execute on the next task.
    private static List<DelayTime> delayTasks = new List<DelayTime>();

    private static Scheduler Instance
    {
        get
        {
            CheckInstance();
            return instance;
        }
    }

    /// <summary>
    /// Register an action to listen the update event.
    /// </summary>
    public static LinkedListNode<Action> ListenUpdate(Action action)
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError(
                "Scheduler can not work when application is not playing.");
            return null;
        }
#endif

        return updateTasks.AddLast(action);
    }

    /// <summary>
    /// Remove the update listener.
    /// </summary>
    /// <remarks>This method can be invoked on finalize thread.</remarks>
    public static void UnlistenUpdate(LinkedListNode<Action> handle)
    {
        if (instance != null)
        {
            updateTasks.Remove(handle);
        }
    }

    /// <summary>
    /// Register an action to listen the fixed update event.
    /// </summary>
    public static LinkedListNode<Action> ListenFixedUpdate(Action action)
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError(
                "Scheduler can not work when application is not playing.");
            return null;
        }
#endif

        return fixedUpdateTasks.AddLast(action);
    }

    /// <summary>
    /// Remove the fixed update listener.
    /// </summary>
    /// <remarks>This method can be invoked on finalize thread.</remarks>
    public static void UnlistenFixedUpdate(LinkedListNode<Action> handle)
    {
        if (instance != null)
        {
            fixedUpdateTasks.Remove(handle);
        }
    }

    /// <summary>
    /// Register an action to listen the late update event.
    /// </summary>
    public static LinkedListNode<Action> ListenLateUpdate(Action action)
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError(
                "Scheduler can not work when application is not playing.");
            return null;
        }
#endif

        return lateUpdateTasks.AddLast(action);
    }

    /// <summary>
    /// Remove the late update listener.
    /// </summary>
    /// <remarks>This method can be invoked on finalize thread.</remarks>
    public static void UnlistenLateUpdate(LinkedListNode<Action> handle)
    {
        if (instance != null)
        {
            lateUpdateTasks.Remove(handle);
        }
    }

    /// <summary>
    /// Start to run a coroutine on this scheduler.
    /// </summary>
    public static Coroutine RunCoroutine(IEnumerator coroutine)
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError(
                "Scheduler can not work when application is not playing.");
            return null;
        }
#endif

        return Instance.StartCoroutine(coroutine);
    }

    /// <summary>
    /// Work thread post a task to the main thread.
    /// </summary>
    public static void PostTask(Action task)
    {
        lock (postTasks)
        {
            postTasks.Add(task);
        }
    }

    /// <summary>
    /// Delay a task and execute it next frame.
    /// </summary>
    public static void Delay(Action task)
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError(
                "Scheduler can not work when application is not playing.");
            return;
        }
#endif

        // Make sure the task is delay to next frame if the scheduler is
        // not update yet this frame.
        if (lastUpdateFrame != Time.frameCount)
        {
            nextFrameTasks2.Add(task);
        }
        else
        {
            nextFrameTasks1.Add(task);
        }
    }

    /// <summary>
    /// Delay a task for a specify time.
    /// </summary>
    public static void Delay(float time, Action task)
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.LogError(
                "Scheduler can not work when application is not playing.");
            return;
        }
#endif

        var delayTime = new DelayTime(
            Time.realtimeSinceStartup + time, task);
        delayTasks.Add(delayTime);
    }

    /// <summary>
    /// Start to run this scheduler.
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    private static void CheckInstance()
    {
        if (instance == null && Application.isPlaying)
        {
            var go = new GameObject("Scheduler", typeof(Scheduler));
            GameObject.DontDestroyOnLoad(go);

            instance = go.GetComponent<Scheduler>();
        }
    }

    private void Awake()
    {
        GameObject.DontDestroyOnLoad(this);
    }

    private void OnDestroy()
    {
        updateTasks.Clear();
        fixedUpdateTasks.Clear();
        lateUpdateTasks.Clear();
        executing.Clear();
        postTasks.Clear();
        nextFrameTasks1.Clear();
        nextFrameTasks2.Clear();
        delayTasks.Clear();
        instance = null;
    }

    private void Update()
    {
        var itr = updateTasks.First;
        while (itr != null)
        {
            var next = itr.Next;
            var value = itr.Value;

            try
            {
                value();
            }
            catch (Exception e)
            {
                Debug.LogError("Execute Update task failed."+e.Message);
                updateTasks.Remove(itr);
            }

            itr = next;
        }

        lock (postTasks)
        {
            if (postTasks.Count > 0)
            {
                for (int i = 0; i < postTasks.Count; ++i)
                {
                    executing.Add(postTasks[i]);
                }

                postTasks.Clear();
            }
        }

        lastUpdateFrame = Time.frameCount;
        if (nextFrameTasks1.Count > 0)
        {
            for (int i = 0; i < nextFrameTasks1.Count; ++i)
            {
                executing.Add(nextFrameTasks1[i]);
            }

            nextFrameTasks1.Clear();
        }

        Util.Swap(ref nextFrameTasks1, ref nextFrameTasks2);

        if (delayTasks.Count > 0)
        {
            var now = Time.realtimeSinceStartup;
            delayTasks.RemoveAll(task =>
            {
                if (now >= task.Time)
                {
                    executing.Add(task.Task);
                    return true;
                }

                return false;
            });
        }

        this.Executing();
    }

    private void FixedUpdate()
    {
        var itr = fixedUpdateTasks.First;
        while (itr != null)
        {
            var next = itr.Next;
            var value = itr.Value;

            try
            {
                value();
            }
            catch (Exception e)
            {
                Debug.LogError("Execute FixedUpdate task failed."+e.Message);
                fixedUpdateTasks.Remove(itr);
            }

            itr = next;
        }
    }

    private void LateUpdate()
    {
        var itr = lateUpdateTasks.First;
        while (itr != null)
        {
            var next = itr.Next;
            var value = itr.Value;

            try
            {
                value();
            }
            catch (Exception e)
            {
                Debug.LogError("Execute LateUpdate task failed."+e.Message);
                lateUpdateTasks.Remove(itr);
            }

            itr = next;
        }
    }

    private void Executing()
    {
        for (int i = 0; i < executing.Count; ++i)
        {
            var task = executing[i];
            try
            {
                task();
            }
            catch (Exception e)
            {
                Debug.LogError("Executing task failed."+e.Message);
            }
        }

        executing.Clear();
    }

    private readonly struct DelayTime
    {
        public readonly float Time;
        public readonly Action Task;

        public DelayTime(float time, Action task)
        {
            this.Time = time;
            this.Task = task;
        }
    }
}