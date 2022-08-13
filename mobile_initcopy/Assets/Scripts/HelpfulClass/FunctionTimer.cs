using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class FunctionTimer 
{
    private static List<FunctionTimer> activeTimerList;
    private static GameObject initGameObject;

    private static void InitIfNeeded()
    {
        if (initGameObject == null)
        {
            initGameObject = new GameObject("FunctionTimer_InitGameObject");
            activeTimerList = new List<FunctionTimer>();
        }
    }

    public static FunctionTimer Create(UnityAction action, float timer)
    {
        return Create(action, timer, "");
    }


    public static FunctionTimer Create(UnityAction action, float timer, string timerName)
    {
        InitIfNeeded();
        GameObject gameObject = new GameObject("FunctionTimer", typeof(MonoBehaviourHook));

        FunctionTimer functionTimer = new FunctionTimer(action, timer, timerName, gameObject);

        gameObject.GetComponent<MonoBehaviourHook>().onUpdate = functionTimer.Update;
        
        activeTimerList.Add(functionTimer);

        return functionTimer;
    }


    private static void RemoveTimer(FunctionTimer functionTimer)
    {
        InitIfNeeded();
        activeTimerList.Remove(functionTimer);
    }

    private static void StopTimer(string timerName)
    {
        for (int i = 0; i < activeTimerList.Count; i++)
        {
            if (activeTimerList[i].timerName == timerName)
            {
                // stop this timer
                activeTimerList[i].DestroySelf();
                i--;
            }
        }
    }

    private class MonoBehaviourHook : MonoBehaviour
    {
        public UnityAction onUpdate;
        private void Update()
        {
            if (onUpdate != null) onUpdate();
        }
    }

    private UnityAction action;
    private float timer;
    private GameObject gameObject;
    private bool isDestroyed;
    private string timerName;

    public FunctionTimer(UnityAction action, float timer, string timerName, GameObject gameObject)
    {
        this.action = action;
        this.timer = timer;
        this.timerName = timerName;
        this.gameObject = gameObject;
        isDestroyed = false;
    }

    public void Update()
    {
        if (!isDestroyed)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                action();
                DestroySelf();
            }
        }
    }

    private void DestroySelf()
    {
        isDestroyed = true;
        RemoveTimer(this);
        UnityEngine.Object.Destroy(gameObject);
    }
}
