using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int currentScore;

    [SerializeField]
    private int matches;

    public int maxScore;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UIManager.instance.UpdateMatchText(matches);
        UIManager.instance.UpdateScoreText(currentScore);
    }

    public void UpdateScore(int amount)
    {
        currentScore += amount;
        UIManager.instance.UpdateScoreText(currentScore);
    }

    public void UpdateMatches()
    {
        matches--;
        UIManager.instance.UpdateMatchText(matches);

        // -------- the WIN or LOSE condition ------------- //
        if(matches <= 0)
        {
            // ---------- show a score GUI ------------ //
            UIManager.instance.ActivateWinPanel();

            // ----------- play particles (?) --------- //

            // --------- load another scene --------- //
            Debug.Log("NO MATCHES LEFT!!");
        }
    }

    public int ReadScore()
    {
        return currentScore;
    }
}
