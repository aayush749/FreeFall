using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneOperator : MonoBehaviour 
{
    public void RestartGame()
    {
        // Load the active scene again
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        Debug.Log("Restarted Scene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
