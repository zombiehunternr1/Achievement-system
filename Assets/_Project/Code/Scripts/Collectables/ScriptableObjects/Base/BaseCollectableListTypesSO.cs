using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCollectableListTypesSO<T> : ScriptableObject
{
    [SerializeField] private List<T> _Collectables;
    public List<T> collectablesList
    {
        get
        {
            return _Collectables;
        }
    }
}
