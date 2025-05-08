using System.Collections.Generic;

public class EventDataPool
{
    private static readonly Stack<EventData> _pool = new Stack<EventData>(64);

    public static void GetEventData(ulong packageKey, out EventData eventData)
    {
        if (_pool.Count > 0)
        {
            eventData = _pool.Pop();
            eventData.Reset(packageKey);
        }
        else
        {
            eventData = new EventData();
            eventData.Reset(packageKey);
        }
    }

    public static void Release(EventData data)
    {
        if (data == null)
        {
            return;
        }
        data.Clear();
        _pool.Push(data);
    }
}
