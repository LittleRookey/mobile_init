using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
	public bool logStatus;
	public Transform root;
	
	private Dictionary<GameObject, ObjectPool<GameObject>> prefabLookup;
	private Dictionary<GameObject, ObjectPool<GameObject>> instanceLookup;

	private bool dirty = false;

	void Awake()
	{
		prefabLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
		instanceLookup = new Dictionary<GameObject, ObjectPool<GameObject>>();
	}

	void Update()
	{
		if (logStatus && dirty)
		{
			PrintStatus();
			dirty = false;
		}
	}

	public void warmPool(GameObject prefab, int size)
	{
		if (prefabLookup.ContainsKey(prefab))
		{
			throw new Exception("Pool for prefab " + prefab.name + " has already been created");
		}
		var pool = new ObjectPool<GameObject>(() => { return InstantiatePrefab(prefab); }, size);
		prefabLookup[prefab] = pool;

		dirty = true;
	}

	public GameObject spawnObject(GameObject prefab)
	{
		return spawnObject(prefab, Vector3.zero, Quaternion.identity);
	}

	public GameObject spawnObject(GameObject prefab, Vector2 position)
	{
		if (!prefabLookup.ContainsKey(prefab))
		{
			WarmPool(prefab, 1);
		}

		var pool = prefabLookup[prefab];

		var clone = pool.GetItem();
		clone.transform.SetPositionAndRotation(position, clone.transform.localRotation);
		//clone.transform.SetPositionAndRotation(position, rotation);
		clone.SetActive(true);

		instanceLookup.Add(clone, pool);
		dirty = true;
		return clone;
	}

	public GameObject spawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		if (!prefabLookup.ContainsKey(prefab))
		{
			WarmPool(prefab, 1);
		}

		var pool = prefabLookup[prefab];

		var clone = pool.GetItem();
		clone.transform.SetPositionAndRotation(position, rotation);
		clone.SetActive(true);

		instanceLookup.Add(clone, pool);
		dirty = true;
		return clone;
	}

	public void releaseObject(GameObject clone)
	{
		clone.SetActive(false);

		if (instanceLookup.ContainsKey(clone))
		{
			instanceLookup[clone].ReleaseItem(clone);
			instanceLookup.Remove(clone);
			dirty = true;
		}
		else
		{
			Debug.LogWarning("No pool contains the object: " + clone.name);
		}
	}


	private GameObject InstantiatePrefab(GameObject prefab)
	{
		var go = Instantiate(prefab) as GameObject;
		if (root != null) go.transform.SetParent(root, false);
		return go;
	}

	public void PrintStatus()
	{
		foreach (KeyValuePair<GameObject, ObjectPool<GameObject>> keyVal in prefabLookup)
		{
			Debug.Log(string.Format("Object Pool for Prefab: {0} In Use: {1} Total {2}", keyVal.Key.name, keyVal.Value.CountUsedItems, keyVal.Value.Count));
		}
	}

	#region Static API

	public static void WarmPool(GameObject prefab, int size)
	{
		Instance.warmPool(prefab, size);
	}

	public static GameObject SpawnObject(GameObject prefab)
	{
		return Instance.spawnObject(prefab);
	}

	public static GameObject SpawnObject(GameObject prefab, Vector2 pos)
	{
		return Instance.spawnObject(prefab, pos);
	}

	public static GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return Instance.spawnObject(prefab, position, rotation);
	}

	public static void ReleaseObject(GameObject clone)
	{
		Instance.releaseObject(clone);
	}

	#endregion
}


public class ObjectPoolContainer<T>
{
	private T item;

	public bool Used { get; private set; }

	public void Consume()
	{
		Used = true;
	}

	public T Item
	{
		get
		{
			return item;
		}
		set
		{
			item = value;
		}
	}

	public void Release()
	{
		Used = false;
	}
}

public class ObjectPool<T>
{
	private List<ObjectPoolContainer<T>> list;
	private Dictionary<T, ObjectPoolContainer<T>> lookup;
	private Func<T> factoryFunc;
	private int lastIndex = 0;

	public ObjectPool(Func<T> factoryFunc, int initialSize)
	{
		this.factoryFunc = factoryFunc;

		list = new List<ObjectPoolContainer<T>>(initialSize);
		lookup = new Dictionary<T, ObjectPoolContainer<T>>(initialSize);

		Warm(initialSize);
	}

	private void Warm(int capacity)
	{
		for (int i = 0; i < capacity; i++)
		{
			CreateContainer();
		}
	}

	private ObjectPoolContainer<T> CreateContainer()
	{
		var container = new ObjectPoolContainer<T>();
		container.Item = factoryFunc();
		list.Add(container);
		return container;
	}

	public T GetItem()
	{
		ObjectPoolContainer<T> container = null;
		for (int i = 0; i < list.Count; i++)
		{
			lastIndex++;
			if (lastIndex > list.Count - 1) lastIndex = 0;

			if (list[lastIndex].Used)
			{
				continue;
			}
			else
			{
				container = list[lastIndex];
				break;
			}
		}

		if (container == null)
		{
			container = CreateContainer();
		}

		container.Consume();
		lookup.Add(container.Item, container);
		return container.Item;
	}

	public void ReleaseItem(object item)
	{
		ReleaseItem((T)item);
	}

	public void ReleaseItem(T item)
	{
		if (lookup.ContainsKey(item))
		{
			var container = lookup[item];
			container.Release();
			lookup.Remove(item);
		}
		else
		{
			Debug.LogWarning("This object pool does not contain the item provided: " + item);
		}
	}

	public int Count
	{
		get { return list.Count; }
	}

	public int CountUsedItems
	{
		get { return lookup.Count; }
	}
}

