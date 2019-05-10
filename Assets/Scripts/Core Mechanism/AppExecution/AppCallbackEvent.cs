using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public class AppCallbackEvent : UnityEvent<AppCallbackEvent.EventData>
{
    public class EventData {
        Dictionary<UnityAction<EventData>, object> hashMap = new Dictionary<UnityAction<EventData>, object>();
        public void AddEventData(UnityAction<EventData> function, object data)
        {
            hashMap.Add(function, data);
        }
        public T GetEventData<T>(UnityAction<EventData> function)
        {
            object value;
            if (hashMap.TryGetValue(function, out value))
            {
                T output = (T)value;
                return output;
            }
            return default(T);
        }
    }
    private EventData m_args = new EventData();
    public EventData Args { get => m_args; }

    public void AddListener(UnityAction<EventData> function, object arg)
    {
        AddListener(function);
        Args.AddEventData(function, arg);
    }
}
