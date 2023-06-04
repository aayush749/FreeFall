using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreUpdater : MonoBehaviour
{
    TMP_Text scoreText, highScoreText;

    int initialScore = 0, score = 0;
    int highScore = 0;

    bool isGameOver = false;

    // Bam Sprites
    [SerializeField]
    List<GameObject> bamSprites;

    private static string saveFileName = "HighScore.txt";

    // Start is called before the first frame update
    void Start()
    {
        // initialize score
        initialScore = (int)Time.realtimeSinceStartup;

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

        score = (int)(Time.realtimeSinceStartup) - initialScore;
        scoreText.text = string.Format("Score: {0}", score);
    }

    private static int GetLatestHighScore()
    {
        return LoadHighScoreFromDisk();
    }

    private static int LoadHighScoreFromDisk()
    {
        var persistentDataPath = Application.persistentDataPath;
        Debug.LogFormat($"Game is saving data at: {persistentDataPath}");

        string saveFilePath = Path.Combine(persistentDataPath, saveFileName);

        if (File.Exists(saveFilePath))
        {
            // Save file exists, read the high score from it
            string highScoreTxt = File.ReadAllText(saveFilePath);
            return int.Parse(highScoreTxt);
        }
        else
        {
            // Save file doesn't exist, initialize with a high score of 0
            SaveHighScoreToDisk(0);
        }

        // in all cases other than a successful read from the save file, return 0 from this function
        return 0;
    }

    private static void SaveHighScoreToDisk(int scoreToSave)
    {
        var saveFilePath = Application.persistentDataPath;
        saveFilePath = Path.Combine(saveFilePath, saveFileName);

        // Overwrite the file if its exists/ create a new file if it doesn't exist
        File.WriteAllText(saveFilePath , Convert.ToString(scoreToSave));
    }

    private void OnGameEnd(object sender, PlayerMovementController.PlayerDeadEventArgs args)
    {
        // Save the high score to the disk, if it has changed
        if (score > highScore)
            SaveHighScoreToDisk(score);

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
