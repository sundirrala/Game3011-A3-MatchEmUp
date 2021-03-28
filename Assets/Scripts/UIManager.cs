using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text matchText;



    [SerializeField]
    private Image starMeter;

    // ----------- for the win panel -------------- //
    [Header("Win Panel")]
    public GameObject winPanel;
    public GameObject starOne;
    public GameObject starTwo;
    public GameObject starThree;
    public Text finalScoreText;

    private void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        winPanel.SetActive(false);
        starOne.SetActive(false);
        starTwo.SetActive(false);
        starThree.SetActive(false);
    }
    public void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + score.ToString("D9"); // -------- D9 is 9 decimals
        UpdateStarMeter(score);
    }

    public void UpdateMatchText(int matchScore)
    {
        matchText.text = matchScore.ToString("D2"); // -------- D2 is 2 decimals
    }

    void UpdateStarMeter(int score)
    {
        float currentScore = (float)score / (float)GameManager.instance.maxScore; // type cast the ints into floats
        starMeter.rectTransform.localScale = new Vector3(starMeter.rectTransform.localScale.x, currentScore, starMeter.rectTransform.localScale.z); // ------- rescaling it according to the current score
    }

    public void ActivateWinPanel()
    {
        BoardManager.instance.StopGame();
        winPanel.SetActive(true);
        finalScoreText.text = "Score: " + GameManager.instance.ReadScore().ToString("D1");
        StartCoroutine(ActivateStar());
    }
    
    IEnumerator ActivateStar()
    {
        yield return new WaitForSeconds(0.25f);
        // ---- if score is higher than 25% ------------ //
        if(GameManager.instance.ReadScore() > (float)(GameManager.instance.maxScore * 0.25f) )
        {
            starOne.SetActive(true);
        }

        yield return new WaitForSeconds(0.25f);
        if (GameManager.instance.ReadScore() > (float)(GameManager.instance.maxScore * 0.65f))
        {
            starTwo.SetActive(true);
        }

        yield return new WaitForSeconds(0.25f);
        if (GameManager.instance.ReadScore() > (float)(GameManager.instance.maxScore * 0.90f))
        {
            starThree.SetActive(true);
        }
    }
}
