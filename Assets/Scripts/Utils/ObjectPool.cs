using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolObjectLoadInfo
{
    public GameObject PrefabBlueprint;
    public int Quantity;
}

public class ObjectPool : MonoBehaviour {

    class PoolObjectSlot
    {
        public PoolObjectSlot(ObjectPool InOwningPool, GameObject InPrefabBlueprint)
        {
            _owningPool = InOwningPool;
            PrefabBlueprint = InPrefabBlueprint;
            Instances = new List<PooledObject>();
        }

        public PoolObjectSlot(PoolObjectLoadInfo Info, ObjectPool InOwningPool) : this(InOwningPool, Info.PrefabBlueprint)
        {
            for(int i = 0; i < Info.Quantity; i++)
            {
                GameObject go = GameObject.Instantiate(PrefabBlueprint);

                Instances.Add(go.AddComponent<PooledObject>());
            }
        }

        public PooledObject Get()
        {
            //Search for a pooled object to release
            PooledObject pooledObject = null;
            foreach(PooledObject obj in Instances)
            {
                if(obj.IsPooled)
                {
                    obj.IsPooled = false;
                    pooledObject = obj;
                }
            }

            //If no pooled object found, create one if we're allowed to
            if(!pooledObject && _owningPool.CanPoolAditionalObject())
            {
                GameObject go = GameObject.Instantiate(PrefabBlueprint);
                pooledObject = go.AddComponent<PooledObject>();
                pooledObject.IsPooled = false;

                Instances.Add(pooledObject);
            }

            return pooledObject;
        }

        public int GetCount()
        {
            return Instances.Count;
        }

        private GameObject PrefabBlueprint;
        private List<PooledObject> Instances;
        private ObjectPool _owningPool;
    }

    /// <summary>
    /// If we let the object pool instance objects dynamically when it runs out of allocated pool objects
    /// </summary>
    public bool EnableLazyLoading;

    /// <summary>
    /// Max stack size of the lazy loaded object pool, doesn't do squat when "EnableLazyLoading" is disabled
    /// </summary>
    public int LazyLoadMaxStackSize;

    /// <summary>
    /// Initial slots to load when initing the pool
    /// </summary>
    public PoolObjectLoadInfo[] PreloadedSlots;

    /// <summary>
    /// Pool containing the slot info
    /// </summary>
    private Dictionary<string, PoolObjectSlot> _pool; 

    public int GetNumPooledObjects()
    {
        int pooled = 0;
        foreach(KeyValuePair<string,PoolObjectSlot> kv in _pool)
        {
            pooled += kv.Value.GetCount();
        }

        return pooled;
    }

    public bool CanPoolAditionalObject()
    {
        return GetNumPooledObjects() < LazyLoadMaxStackSize && EnableLazyLoading;
    }

	// Use this for initialization
	void Start () {
        _pool = new Dictionary<string, PoolObjectSlot>();

		foreach(PoolObjectLoadInfo loadInfo in PreloadedSlots)
        {
            _pool.Add(loadInfo.PrefabBlueprint.ToString(), new PoolObjectSlot(loadInfo, this));
        }
	}

    public PooledObject GetPooled(GameObject Blueprint)
    {
         if(!_pool.ContainsKey(Blueprint.ToString()))
        {
            //The given blueprint object hasn't been pooled nor lazy loaded
            if(CanPoolAditionalObject() && EnableLazyLoading)
            {
                _pool.Add(Blueprint.ToString(), new PoolObjectSlot(this, Blueprint));
            }
            else
            {
                Debug.Log("Cannot pool additional objects!");
                return null; //No lazy loading or slots on the pool are available
            }
        }

        return _pool[Blueprint.ToString()].Get();
    }

    public T GetPooled<T>(GameObject Blueprint)
    {
        PooledObject pooled = GetPooled(Blueprint);

        if (!pooled)
            return default(T);

        return pooled.GetComponent<T>();
    }
}

public class PooledObject : MonoBehaviour
{
    private bool _isPooled = true;

    public bool IsPooled
    {
        get { return _isPooled; }
        set { _isPooled = value; gameObject.SetActive(!value); }
    }

    private void Start()
    {
        gameObject.SetActive(!_isPooled);
    }
}