using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreUpdater : MonoBehaviour
{
    TMP_Text scoreText, highScoreText;

    int score = 0;
    int highScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Grab references to the text mesh pro objects from the scene
        scoreText = GameObject.Find("Score Text").GetComponent<TMP_Text>();
        highScoreText = GameObject.Find("High Score Text").GetComponent<TMP_Text>();

        // update the high score
        highScore = GetLatestHighScore();
        highScoreText.text = string.Format("High Score: {0}", highScore);
    }

    // Update is called once per frame
    void Update()
    {
        score = (int) (Time.realtimeSinceStartup);
        scoreText.text = string.Format("Score: {0}", score);

        if (score > highScore)
        {
            // new high score is set
            // update the high score to be the same value as the score
            highScore = score;
            highScoreText.text = string.Format("High Score: {0}", highScore);
        }
    }

    private static int GetLatestHighScore()
    {
        return 1;
    }
}
