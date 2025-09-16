using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private List<T> pool = new List<T>();
    private GameObject prefab;
    private int initialSize;

    public ObjectPool(GameObject prefab, int initialSize)
    {
        this.prefab = prefab;
        this.initialSize = initialSize;
        for (int i = 0; i < initialSize; i++)
        {
            CreateObject();
        }
    }

    private T CreateObject()
    {
        GameObject go = Object.Instantiate(prefab);
        go.SetActive(false);
        T component = go.GetComponent<T>();
        pool.Add(component);
        return component;
    }

    public T Get()
    {
        foreach (T obj in pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                obj.gameObject.SetActive(true);
                return obj;
            }
        }
        return CreateObject();
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
    }
}