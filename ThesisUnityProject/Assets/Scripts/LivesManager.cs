using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesManager : MonoBehaviour
{
    private List<GameObject> _lifeBoxes;
    private List<SpriteRenderer> _spriteRenderers;
    public Color liveColor;
    public Color deadColor;
    private int _currentLifeIndex;
    public int wholeLifeNum;
    public GameObject oneLife;
    private const float GapSize = 0.5f;
    private readonly Vector3 _startPos = new Vector3(-8.3f, 4.4f, 0f);
    
    
    // Start is called before the first frame update
    void Start()
    {
        _lifeBoxes = new List<GameObject>();
        _spriteRenderers = new List<SpriteRenderer>();
        for (int i = 0; i < wholeLifeNum; i++)
        {
            var box = Instantiate(oneLife, _startPos + i * new Vector3(GapSize, 0f, 0f), Quaternion.identity,
                transform);
            var sr = box.GetComponent<SpriteRenderer>();
            sr.color = liveColor;
            _lifeBoxes.Add(box);
            _spriteRenderers.Add(sr);
        }

        _currentLifeIndex = wholeLifeNum - 1;
        
        Services.EventManager.Register<LoseLife>(OnLoseLife);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnLoseLife(AGPEvent e)
    {
        _spriteRenderers[_currentLifeIndex].color = deadColor;
        if (_currentLifeIndex == 0)
        {
            Debug.Log("game over");
            return;
        }
        _currentLifeIndex--;
    }

    private void OnDestroy()
    {
        Services.EventManager.Unregister<LoseLife>(OnLoseLife);
    }
}
