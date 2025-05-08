using System.Collections.Generic;

public class EventDataPool
{
    private static readonly Stack<EventData> _pool = new Stack<EventData>(64);
    public static EventData GetEventData(ulong packageKey)
    {
        if (_pool.Count > 0)
        {
            EventData eventData = _pool.Pop();
            eventData.Reset(packageKey);
            return eventData;
        }
        return new EventData(packageKey);
    }
    public static void Release(EventData data)
    {
        if (data == null || _pool.Contains(data))
        {
            return;
        }
        data.Clear();
        _pool.Push(data);
    }
}
