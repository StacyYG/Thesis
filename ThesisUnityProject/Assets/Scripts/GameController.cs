using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject reload, next, prev;
    private void Awake()
    {
        var gameControllers = GameObject.FindGameObjectsWithTag("GameController");
        if (gameControllers.Length > 0)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        Services.GameController = this;
        transform.tag = "GameController";
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
    
    public void ShowMenu(bool isShow)
    {
        reload.SetActive(isShow);
        next.SetActive(isShow);
        prev.SetActive(isShow);
    }
}
