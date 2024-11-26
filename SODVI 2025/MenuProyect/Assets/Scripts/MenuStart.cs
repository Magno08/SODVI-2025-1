using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainStart : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGame");


    }
    public void CreditsGame()
    {
        SceneManager.LoadScene("CreditsScene");


    }
    public void MenuGame()
    {
        SceneManager.LoadScene("Menu");


    }
    public void QuitGame()
    {
        Application.Quit();


    }


}