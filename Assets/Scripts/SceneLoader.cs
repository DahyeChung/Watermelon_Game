using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneLoader : MonoBehaviour
{
    public string sceneNameToLoad;

   
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneNameToLoad); 
    }
}

