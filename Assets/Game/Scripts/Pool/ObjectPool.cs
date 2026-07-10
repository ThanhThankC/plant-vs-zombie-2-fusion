using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private readonly GameObject prefab;
    private readonly Transform parent;
    private readonly Queue<T> pool = new();

    public ObjectPool(GameObject prefab, Transform parent, int initialCapacity)
    {
        this.prefab = prefab;
        this.parent = parent;
        
        for (int i = 0; i < initialCapacity; i++)
        {
            pool.Enqueue(Create());
        }
    }

    public T Get(Vector3 position, Quaternion rotation)
    {
        var obj = pool.Count > 0 ? pool.Dequeue() : Create();
        obj.transform.SetLocalPositionAndRotation(position, rotation);
        obj.gameObject.SetActive(true);

        if (obj is IPoolable poolable)
            poolable.OnSpawn();

        return obj;
    }

    public void Relase(T obj)
    {
        if (obj is IPoolable poolable)
            poolable.OnDespawn();

        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }

    private T Create()
    {
        var obj = Object.Instantiate(prefab, parent);
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(parent);
        return obj.GetComponent<T>();
    }
}
