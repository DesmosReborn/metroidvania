using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImagePool : MonoBehaviour
{
    [SerializeField]
    private GameObject afterimage;

    private Queue<GameObject> objects = new Queue<GameObject>();

    public static AfterImagePool Instance { get; private set; }  

    private void Awake()
    {
        Instance = this;
        growPool();
    }

    private void growPool()
    {
        for (int i = 0; i < 10; i++)
        {
            var instanceToAdd = Instantiate(afterimage);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }

    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        objects.Enqueue(instance);
    }

    public GameObject getFromPool()
    {
        if (objects.Count == 0)
        {
            growPool();
        }

        var instance = objects.Dequeue();
        instance.SetActive(true);
        return instance;
    }
}
