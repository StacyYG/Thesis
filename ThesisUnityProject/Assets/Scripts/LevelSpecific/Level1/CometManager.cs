using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometManager : MonoBehaviour
{
    public GameObject comet;
    public static ObjectPool objectPool;
    public int spawnNum;
    // Start is called before the first frame update
    void Start()
    {
        objectPool = new ObjectPool(transform);
        objectPool.Add(comet);
        for (int i = 0; i < spawnNum; i++)
        {
            var spawnedObj = objectPool.Spawn();
            spawnedObj.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(0.5f, 2f), 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class ObjectPool
{
    private Stack<GameObject> pool = new Stack<GameObject>();
    private const int initialNum = 50;
    private GameObject _pooledObject;

    private Transform _parentTransform;

    public ObjectPool(Transform transform)
    {
        _parentTransform = transform;
    }
    public void Add(GameObject toPool)
    {
        _pooledObject = toPool;
        for (int i = 0; i < initialNum; i++)
        {
            var oneCopy = Object.Instantiate(toPool, _parentTransform);
            pool.Push(oneCopy);
            oneCopy.SetActive(false);
        }
    }

    public GameObject Spawn() => Spawn(3f * Random.insideUnitCircle, Quaternion.identity);
    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        GameObject toSpawn;
        if (pool.Count > 0)
        {
            toSpawn = pool.Pop();
            toSpawn.SetActive(true);
        }
        else
            toSpawn = Object.Instantiate(_pooledObject, _parentTransform);
        
        toSpawn.transform.position = position;
        toSpawn.transform.rotation = rotation;

        return toSpawn;
    }

    public void Despawn(GameObject toDespawn)
    {
        pool.Push(toDespawn);
        toDespawn.SetActive(false);
    }
}
