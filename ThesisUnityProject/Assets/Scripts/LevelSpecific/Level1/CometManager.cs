using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometManager : MonoBehaviour
{
    public GameObject comet;
    public static ObjectPool objectPool;

    public Vector2 spawnDistance;
    // Start is called before the first frame update
    void Start()
    {
        objectPool = new ObjectPool(this);
        objectPool.Add(comet);
        SpawnAndSetPos();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void SpawnAndSetPos()
    {
        var width = Services.CameraController.cameraBoundHalfX * 2f;
        var height = Services.CameraController.cameraBoundHalfY * 2f;
        var columnNum = (int) (width / spawnDistance.x);
        var rowNum = (int) (height / spawnDistance.y);
        Vector2[,] spawnPositions = new Vector2[columnNum, rowNum];
        for (int i = 0; i < columnNum; i++)
        {
            for (int j = 0; j < rowNum; j++)
            {
                spawnPositions[i, j] = (Vector2) transform.position +
                                       new Vector2(
                                           -Services.CameraController.cameraBoundHalfX + (i + 0.5f) * spawnDistance.x,
                                           -Services.CameraController.cameraBoundHalfY + (j + 0.5f) * spawnDistance.y) +
                                       0.5f * Random.insideUnitCircle;
                var spawnedObj = objectPool.Spawn(spawnPositions[i, j]);
                spawnedObj.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(1.5f, 2f), 0f);
            }
        }
        
    }
}

public class ObjectPool
{
    private Stack<GameObject> pool = new Stack<GameObject>();
    private const int initialNum = 50;
    private GameObject _pooledObject;
    private Transform _parentTransform;

    public ObjectPool(CometManager cometManager)
    {
        _parentTransform = cometManager.transform;
    }
    public void Add(GameObject toPool)
    {
        _pooledObject = toPool;
        for (int i = 0; i < initialNum; i++)
        {
            var oneCopy = Object.Instantiate(toPool, _parentTransform);
            var sr = oneCopy.GetComponent<SpriteRenderer>();
            sr.color = Random.ColorHSV(0.5f, 0.6f, 0.3f, 0.8f, 0.8f, 1f);
            pool.Push(oneCopy);
            oneCopy.SetActive(false);
        }
    }

    public GameObject Spawn(Vector2 position) => Spawn(position, Quaternion.identity);
    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        GameObject toSpawn;
        if (pool.Count > 0)
        {
            toSpawn = pool.Pop();
            toSpawn.SetActive(true);
        }
        else
        {
            toSpawn = Object.Instantiate(_pooledObject, _parentTransform);
            var sr = toSpawn.GetComponent<SpriteRenderer>();
            sr.color = Random.ColorHSV(0.5f, 0.6f, 0.3f, 0.8f, 0.8f, 1f);
        }
        
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
