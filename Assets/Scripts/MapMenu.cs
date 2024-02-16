using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapMenu : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void SaveGame()
    {
        MainManager.Instance.SaveGame();
    }
    public void LoadGame()
    {
        MainManager.Instance.LoadGame();
    }

    public void ExitGame()
    {
        Application.Quit();
    }    
}
