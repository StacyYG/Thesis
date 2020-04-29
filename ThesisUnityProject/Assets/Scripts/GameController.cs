using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameController : MonoBehaviour
{
    public GameCfg gameCfg;
    public GameObject reload, next, prev;
    public bool isSub = true;
    private void Awake()
    {
        var gameControllers = GameObject.FindGameObjectsWithTag("GameController");
        if (gameControllers.Length > 0)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        isSub = false;
        Services.GameController = this;
        transform.tag = "GameController";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadPrevScene()
    {
        var currentIndex = SceneManager.GetActiveScene().buildIndex;
        if(currentIndex == 0) return;
        SceneManager.LoadScene(currentIndex - 1);
    }
    
    public void LoadNextScene()
    {
        var currentIndex = SceneManager.GetActiveScene().buildIndex;
        if(currentIndex == SceneManager.sceneCountInBuildSettings - 1) return;
        SceneManager.LoadScene(currentIndex + 1);
    }
    
    public void Reload()
    {
        var currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }
    
    public void ShowButtons(bool isShow)
    {
        reload.SetActive(isShow);
        next.SetActive(isShow);
        prev.SetActive(isShow);
    }
}
