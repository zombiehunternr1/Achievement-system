using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class OnCreateGameObjectInHierarchyEvent
{
    static OnCreateGameObjectInHierarchyEvent()
    {
        ObjectChangeEvents.changesPublished += ChangePublished;
    }
    private static void ChangePublished(ref ObjectChangeEventStream stream)
    {
        for (int i = 0; i < stream.length; i++)
        {
            ObjectChangeKind type = stream.GetEventType(i);
            switch (type)
            {
                case ObjectChangeKind.CreateGameObjectHierarchy:
                    stream.GetCreateGameObjectHierarchyEvent(i, out CreateGameObjectHierarchyEventArgs createGameObjectHierarchyEventArgs);
                    GameObject newGameObject = EditorUtility.InstanceIDToObject(createGameObjectHierarchyEventArgs.instanceId) as GameObject;
                    CollectableObject newCollectableObject = newGameObject.GetComponent<CollectableObject>();
                    if (newCollectableObject == null)
                    {
                        return;
                    }
                    newCollectableObject.CheckObjectId();
                break;
            }
        }
    }
}
