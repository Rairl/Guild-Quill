using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{

    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnExitButtonClicked()
    {
        Application.Quit();
    }

}
