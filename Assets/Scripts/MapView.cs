using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapView : MonoBehaviour
{

    public void OpenOptions()
    {
        SceneManager.LoadSceneAsync("MapViewOptions");
    }
    public void CharacterMenu()
    {
        SceneManager.LoadSceneAsync("MapViewOptions");
    }
    public void InventoryMenu()
    {
        SceneManager.LoadSceneAsync("MapViewOptions");
    }

    public void SkillsrMenu()
    {
        SceneManager.LoadSceneAsync("MapViewOptions");
    }

    public void ManageMenu()
    {
        SceneManager.LoadSceneAsync("MapViewOptions");
    }


}
