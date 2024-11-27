using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseApp()
    {
        Application.Quit();
        //Debug.Log("Application QUIT");
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void UnPauseGame()
    {
        Time.timeScale = 1;
    }

    public void LoadMenu()
    {
        DestroyCurrentScene();
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }

    public void LoadLevel1()
    {
        DestroyCurrentScene();
        SceneManager.LoadScene("Level1");
    }

    public void DestroyCurrentScene()
    {
        // Destruir todos los objetos de la escena actual para evitar problemas de duplicación
        Scene currentScene = SceneManager.GetActiveScene();
        foreach (GameObject obj in currentScene.GetRootGameObjects())
        {
            Destroy(obj);
        }
    }
}
