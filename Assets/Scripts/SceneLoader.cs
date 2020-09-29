using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject gameOverScreen;
    private void Start()
    {
        gameOverScreen.SetActive(false);
        PlayerController.OnPlayerBeatLevel += NextScene;
        Guard.OnGuardHasSpottedPlayer += GameOver;
    }

    private void OnDestroy()
    {
        PlayerController.OnPlayerBeatLevel -= NextScene;
        Guard.OnGuardHasSpottedPlayer -= GameOver;
    }

    public void NextScene()
    {
        Debug.Log("Load next scene");
        if (SceneManager.GetActiveScene().buildIndex+1 < SceneManager.sceneCountInBuildSettings)
            {

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }
    public void RestartScene()
    {
        Debug.Log("Restart Scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
    }
}
