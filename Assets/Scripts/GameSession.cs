using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameSession : MonoBehaviour
{
    private int score = 0;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI healthText;
    // Start is called before the first frame update

    void Start()
    {
        //initialize score counter
        scoreText.text = "0";
    }

    public int GetScore()
    {
        return score;
    }

    public void SetHealth(int health)
    {
        healthText.text = health.ToString();
    }

    public void AddToScore(int scoreValue)
    {
        score += scoreValue;
        scoreText.text = score.ToString();
    }

    public void ResetGame()
    {
        Destroy(gameObject);
        Destroy(scoreText);
    }
}
