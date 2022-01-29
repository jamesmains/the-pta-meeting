using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurtainCall : MonoBehaviour
{
    public void GotoMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    public void GotoScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);   
    }
}
