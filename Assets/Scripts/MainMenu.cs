using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        MainManager.Instance.NewGame();
        SceneManager.LoadSceneAsync("MapView");
    }

    public void ContinueGame()
    {
        MainManager.Instance.LoadGame();
        SceneManager.LoadSceneAsync("MapView");
    }

    public void LoadGame()
    {
        MainManager.Instance.LoadGame();
        SceneManager.LoadSceneAsync("MapView");
    }

    public void ExitGame()
    {
        Application.Quit();
    }    
}
