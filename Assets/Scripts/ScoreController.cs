using UnityEngine;
using TMPro;

public class ScoreController: MonoBehaviour
{
    public int score = 0;
    public TextMeshProUGUI scoreText;
    public bool is_visible = false;
    private int num_stabs = 0;
    private int num_slices = 0;
    private int num_bombs = 0;

    void Start()
    {
        UpdateScoreText();
    }
    
    void Update()
    {
        UpdateScoreText();
    }

    public void AddPoints(int points)
    {
        score += points;
        if (score < 0) {
            score = 0;
        }
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (!is_visible)
        {
            scoreText.text = "";
            return;
        }
        scoreText.text = "Score: " + score;
    }

    public void ResetStats()
    {
        num_stabs = 0;
        num_slices = 0;
        num_bombs = 0;
    }

    public void IncrStabs() { num_stabs++; }

    public void IncrSlices() { num_slices++;}

    public void IncrBombs() { num_bombs++; }

    public int GetStabs() { return num_stabs; }

    public int GetSlices() { return num_slices; }
    
    public int GetBombs() { return num_bombs; }


}