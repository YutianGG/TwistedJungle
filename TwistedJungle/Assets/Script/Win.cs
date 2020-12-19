using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    private void Update()
    {
        BakeToMenu();
        QuitGame();
    }
    private void BakeToMenu()
    {
        if ( Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene("Menu");
    }

    private void QuitGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }
}
