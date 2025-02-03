using System;
using System.Threading;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    // Static instance of GameManager which allows it to be accessed from any other script
    public static GameManager Instance { get; private set; }
    public static int timeForGame = 90;
    public static float bombProbability = 0.2f;

    // g factor
    public static float gStartFactor = 0.2f;
    public static float gEndFactor = 3.5f;
    public static float gJump = 0.2f;
    private static float increaseGInterval = 8f;
    public float currentGFactor = gStartFactor;


    // number of spawn fruits
    public static int numSpawnFruitStart = 5;
    public static int numSpawnFruitEnd = 30;
    public static int numSpawnFruitJump = 5;
    private static float increaseNumSpawnFruitInterval = 10f;


    // Variables to hold game-wide parameters (e.g., scores, level, etc.)
    public int currentScore { get; private set; }
    public int highScore { get; private set; }


    public Timer timerController;
    public SoundController soundController;
    public ScoreController scoreController;
    public FruitSpawner fruitSpawner;
    public SwordController swordController;
    public LivesController livesController;
    public GameObject mainmenu;
    public GameObject gameTitle;
    public GameObject gameOverTitle;
    public GameObject endmenu;
    public TextMeshProUGUI statsText;
    public bool gameIsRunning = false;
    public GameObject rightController;
    public GameObject leftController;
    public GameObject rightRay1Controller;
    public GameObject leftRay1Controller;
    public GameObject rightRay2Controller;
    public GameObject leftRay2Controller;

    private float lastTimeIncrBombPercentage = timeForGame;
    private float increaseBombInterval = 20f;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Check if there is already an instance of GameManager
        if (Instance == null)
        {
            // If not, set it to this.
            Instance = this;
            // Make sure this instance is not destroyed when loading a new scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If an instance already exists, destroy this one (singleton pattern)
            Destroy(gameObject);
        }
    }

    // Initialize any other variables or states before the game starts
    private void Start()
    {
        ResetGame();
    }

    public void Update()
    {
        setControllersVisible(!gameIsRunning);
        if (gameIsRunning && (timerController.timeRemaining == 0 || livesController.lives == 0))
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        CancelInvoke("IncreaseBombPercentage");
        CancelInvoke("IncreaseGFactor");
        CancelInvoke("increaseNumSpawnFruit");
        gameIsRunning = false;
        fruitSpawner.StopSpawning();
        UpdateStatistic();
        endmenu.SetActive(true);
        timerController.timeText.text = "";
        gameOverTitle.SetActive(true);
        scoreController.is_visible = false;
        livesController.isLiveMode = false;
        soundController.StartMenuMusic();
        currentGFactor = gStartFactor;
        UpdateGFactor();
        setControllersVisible(true);
        fruitSpawner.maxNumberOfSpawnFruitsAtATime = numSpawnFruitStart;

    }

    public void UpdateStatistic()
    {
        int stabs = scoreController.GetStabs();
        int slices = scoreController.GetSlices();
        int bombs = scoreController.GetBombs();
        int score = scoreController.score;

        string statistics_string = $"Game Statistics:\nScore: {score}\nFruit Stabs: {stabs}\nFruit Slices: {slices}\nBombs Exploded: {bombs}";
        statsText.text = statistics_string;
    }

    // Reset all game parameters
    public void ResetGame()
    {
        timerController.timeText.text = "";
        // statsText.text = "";
        scoreController.is_visible = false;
        endmenu.SetActive(false);
        gameOverTitle.SetActive(false);
        mainmenu.SetActive(true);
        gameTitle.SetActive(true);
        scoreController.score = 0;
        scoreController.ResetStats();
        livesController.lives = 3;
        livesController.isLiveMode = false;
        gameIsRunning = false;
        currentGFactor = gStartFactor;
        UpdateGFactor();
        setControllersVisible(true);
        fruitSpawner.maxNumberOfSpawnFruitsAtATime = numSpawnFruitStart;
    }

    void StartGameCommon()
    {
        timerController.timeRemaining = timeForGame;
        scoreController.is_visible = true;
        gameIsRunning = true;
        scoreController.score = 0;
        fruitSpawner.StartSpawning();
        fruitSpawner.StartGameInPool();
        mainmenu.SetActive(false);
        gameTitle.SetActive(false);
        soundController.StartGameMusic();
        currentGFactor = gStartFactor;
        UpdateGFactor();
        fruitSpawner.maxNumberOfSpawnFruitsAtATime = numSpawnFruitStart;
        setControllersVisible(false);
        InvokeRepeating("IncreaseBombPercentage", 0f, increaseBombInterval);
        InvokeRepeating("IncreaseGFactor", 0f, increaseGInterval);
        InvokeRepeating("increaseNumSpawnFruit", 0f, increaseNumSpawnFruitInterval);
    }
    public void StartZenMode()
    {
        fruitSpawner.bomb_prob = 0.0f;
        timerController.timerIsRunning = true;
        livesController.isLiveMode = false;
        StartGameCommon();
    }

    public void StartTimerMode()
    {
        fruitSpawner.bomb_prob = bombProbability;
        timerController.timerIsRunning = true;
        livesController.isLiveMode = false;
        StartGameCommon();
    }

    public void StartClassicMode()
    {
        fruitSpawner.bomb_prob = bombProbability;
        timerController.timerIsRunning = false;
        livesController.isLiveMode = true;
        StartGameCommon();
    }

    public void IncreaseBombPercentage()
    {
        if (fruitSpawner.bomb_prob == 0.0) {
            return; // Zen Mode
        }
        if (fruitSpawner.bomb_prob >= 0.25f) {
            return; // Max prob reached
        }
        fruitSpawner.bomb_prob = fruitSpawner.bomb_prob += 0.05f;
    }

    public void IncreaseGFactor()
    {
        if (currentGFactor >= gEndFactor) {
            return; // Max g reached
        }
        currentGFactor = currentGFactor + gJump;
        UpdateGFactor();
    }

    public void UpdateGFactor()
    {
        //todo update g with currentGFactor
        Physics.gravity = new Vector3(0, -currentGFactor, 0);
    }

    public void increaseNumSpawnFruit()
    {
        if (fruitSpawner.maxNumberOfSpawnFruitsAtATime >= numSpawnFruitEnd) {
            return; // Max g reached
        }
        fruitSpawner.maxNumberOfSpawnFruitsAtATime += numSpawnFruitJump;
    }

    public void setControllersVisible(bool is_visible)
    {
        rightController.GetComponent<SkinnedMeshRenderer>().enabled = is_visible;
        leftController.GetComponent<SkinnedMeshRenderer>().enabled = is_visible;
        // rightRay1Controller.GetComponent<MeshRenderer>().enabled = is_visible;
        // leftRay1Controller.GetComponent<MeshRenderer>().enabled = is_visible;
        // rightRay2Controller.GetComponent<LineRenderer>().enabled = is_visible;
        // leftRay2Controller.GetComponent<LineRenderer>().enabled = is_visible;
        rightRay1Controller.SetActive(is_visible);
        leftRay1Controller.SetActive(is_visible);
        rightRay2Controller.SetActive(is_visible);
        leftRay2Controller.SetActive(is_visible);
    }
}
