using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace HyrphusQ.Events
{
    [EventCode]
    public enum LevelEventCode
    {
        OnWinLevel,
        OnLoseLevel
    }
    [EventCode]
    public enum GameplayEventCode
    {
        
    }
    [EventCode]
    public enum PlayerEventCode
    {
        
    }

    public class UnityParamEvent : UnityEvent<object[]> { }
    public static class LevelEventHandler
    {
        private static Dictionary<Enum, UnityEvent> listActionEvent = new Dictionary<Enum, UnityEvent>();
        private static Dictionary<Enum, UnityParamEvent> listParamActionEvent = new Dictionary<Enum, UnityParamEvent>();

        public static void AddNewActionEvent(Enum eventID, UnityAction callback)
        {
            UnityEvent actionEvent;
            if (listActionEvent.TryGetValue(eventID, out actionEvent))
            {
                actionEvent.AddListener(callback);
            }
            else
            {
                actionEvent = new UnityEvent();
                actionEvent.AddListener(callback);
                listActionEvent.Add(eventID, actionEvent);
            }
        }

        public static void AddNewActionEvent(Enum eventID, UnityAction<object[]> callback)
        {
            UnityParamEvent actionEvent;
            if (listParamActionEvent.TryGetValue(eventID, out actionEvent))
            {
                actionEvent.AddListener(callback);
            }
            else
            {
                actionEvent = new UnityParamEvent();
                actionEvent.AddListener(callback);
                listParamActionEvent.Add(eventID, actionEvent);
            }
        }

        public static void Invoke(Enum eventID, Action onActionComplete = null, params object[] param)
        {
            try
            {
                if (listActionEvent.TryGetValue(eventID, out UnityEvent events))
                    events?.Invoke();
                if (listParamActionEvent.TryGetValue(eventID, out UnityParamEvent paramEvents))
                    paramEvents?.Invoke(param);
                onActionComplete?.Invoke();
            }
            catch (Exception exc)
            {
                Debug.LogError(eventID);
                Debug.LogError("Error: " + exc.Message);
            }
        }

        public static bool IsEventExist(Enum eventID)
        {
            return listActionEvent.ContainsKey(eventID) || listParamActionEvent.ContainsKey(eventID);
        }
        public static void RemoveAction(Enum eventID)
        {
            try
            {
                listActionEvent[eventID].RemoveAllListeners();
                listParamActionEvent[eventID].RemoveAllListeners();
            }
            catch (Exception exc)
            {
                Debug.LogError("Error: " + exc.Message);
            }
        }
        public static void RemoveAllAction()
        {
            listActionEvent.Clear();
            listParamActionEvent.Clear();
        }
    }
}