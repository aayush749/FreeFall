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

    bool isGameOver = false;

    // Bam Sprites
    [SerializeField]
    List<GameObject> bamSprites;

    // Start is called before the first frame update
    void Start()
    {
        // Grab references to the text mesh pro objects from the scene
        scoreText = GameObject.Find("Score Text").GetComponent<TMP_Text>();
        highScoreText = GameObject.Find("High Score Text").GetComponent<TMP_Text>();

        // update the high score
        highScore = GetLatestHighScore();
        highScoreText.text = string.Format("High Score: {0}", highScore);

        // Grab the reference of the player object
        PlayerMovementController player = GameObject.Find("Player").GetComponent<PlayerMovementController>();
        player.OnPlayerDead += OnGameEnd;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScores();
    }

    private void UpdateScores()
    {
        if (isGameOver) return;

        score = (int)(Time.realtimeSinceStartup);
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

    private void OnGameEnd(object sender, PlayerMovementController.PlayerDeadEventArgs args)
    {
        // stop the game's execution
        Time.timeScale = 0;
        isGameOver = true;
        Debug.Log("Game Over");

        if (highScore < score)
        {
            Debug.Log("You made a new high score!!");
        }

        Vector3 collisionPoint = args.pointOfCollision.point;
        Debug.Log("Collision occurred at " + collisionPoint);

        // Instantiate a BAM sprite at point of contact (currently not doing this)
        //InstatiateBamSpriteAtCollisionPoint(collisionPoint);

        // Bring up Game Over board by triggering its animation
        Animator animator = GameObject.Find("Game Over Text Background").GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("GameOverTrigger");
            Debug.Log("Set game over trigger");
        }
    }

    private void InstatiateBamSpriteAtCollisionPoint(Vector3 point)
    {
        int spriteIdx = UnityEngine.Random.Range(0, bamSprites.Count - 1);

        GameObject.Instantiate(bamSprites[spriteIdx], point, Quaternion.identity);
    }

}
