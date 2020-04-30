using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CometManager : MonoBehaviour
{
    public GameObject comet;
    public static ObjectPool objectPool;
    public Vector2 spawnDistance;
    public int initialPoolNumber;
    private float _width, _height, _spawnInterval, _spawnTimer;
    private int _columnNum, _rowNum;
    private Vector2[,] _spawnPositions;
    public float averageSpeed, speedDeviation;

    // Start is called before the first frame update
    void Start()
    {
        objectPool = new ObjectPool(this, initialPoolNumber);
        objectPool.Add(comet);
        _width = Services.CameraController.cameraBoundHalfX * 2f;
        _height = Services.CameraController.cameraBoundHalfY * 2f;
        _columnNum = (int) (_width / spawnDistance.x);
        _rowNum = (int) (_height / spawnDistance.y);
        _spawnPositions = new Vector2[_columnNum, _rowNum];
        _spawnInterval = spawnDistance.x / averageSpeed;
        SetPosAndSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        _spawnTimer += Time.deltaTime;
        if (_spawnTimer > _spawnInterval)
        {
            _spawnTimer = 0f;
            for (int i = 0; i < _rowNum; i++)
            {
                var obj = objectPool.Spawn(_spawnPositions[0, i]);
                obj.GetComponent<Rigidbody2D>().velocity =
                    new Vector2(Random.Range(averageSpeed - speedDeviation, averageSpeed + speedDeviation), 0f);
            }
        }
    }
    
    private void SetPosAndSpawn()
    {
        for (int i = 0; i < _columnNum; i++)
        {
            for (int j = 0; j < _rowNum; j++)
            {
                _spawnPositions[i, j] = (Vector2) transform.position +
                                        new Vector2(
                                            -Services.CameraController.cameraBoundHalfX + (i + 0.5f) * spawnDistance.x,
                                            -Services.CameraController.cameraBoundHalfY + (j + 0.5f) * spawnDistance.y) +
                                        0.5f * Random.insideUnitCircle;
                var spawnedObj = objectPool.Spawn(_spawnPositions[i, j]);
                spawnedObj.GetComponent<Rigidbody2D>().velocity =
                    new Vector2(Random.Range(averageSpeed - speedDeviation, averageSpeed + speedDeviation), 0f);
            }
        }
        
    }
}

public class ObjectPool
{
    private Stack<GameObject> _pool = new Stack<GameObject>();
    private readonly int _initialNum;
    private GameObject _pooledObject;
    private readonly Transform _parentTransform;
    private List<GameObject> _spawned = new List<GameObject>();

    public ObjectPool(CometManager cometManager, int initialNumber = 50)
    {
        _parentTransform = cometManager.transform;
        _initialNum = initialNumber;
    }
    public void Add(GameObject toPool)
    {
        _pooledObject = toPool;
        for (int i = 0; i < _initialNum; i++)
        {
            var oneCopy = Object.Instantiate(toPool, _parentTransform);
            var sr = oneCopy.GetComponent<SpriteRenderer>();
            sr.color = Random.ColorHSV(0.5f, 0.7f, 0.3f, 0.8f, 0.8f, 1f);
            _pool.Push(oneCopy);
            oneCopy.SetActive(false);
        }
    }

    public GameObject Spawn(Vector2 position) => Spawn(position, Quaternion.Euler(0f, 0f, Random.Range(0f, 90f)));
    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        GameObject toSpawn;
        toSpawn = _pool.Pop();
        toSpawn.SetActive(true);
        _spawned.Add(toSpawn);
        
        if (_pool.Count <= 1)
            Despawn(_spawned[0]);
        
        toSpawn.transform.position = position;
        toSpawn.transform.rotation = rotation;

        return toSpawn;
    }

    public void Despawn(GameObject toDespawn)
    {
        _pool.Push(toDespawn);
        _spawned.Remove(toDespawn);
        toDespawn.SetActive(false);
    }
    
}
