using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ButtonBehaviour : MonoBehaviour
{
    public void LoadPlayScene()
    {
        SceneManager.LoadScene("MatchEmUpGame");
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
