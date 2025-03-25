using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Variables

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI timerText;
    public GameObject gameOverPanel;

    [Header("Game Settings")]
    public float levelTime = 60f;
    public int pointsPerMatch = 100;
    public int pointsPerSpecialMatch = 200;

    [Header("GameObjects")]
    [SerializeField] private ParticleSystem confetti;

    [Header("Game State")]
    [HideInInspector] public bool IsGameStarted;
    [HideInInspector] public bool IsClickable;

    private int currentScore;
    private int currentLevel = 1;
    private float currentTime;
    private bool isGameActive;
    private int _coin;
    public int Coin => _coin;

    private int currentMatches;
    private int currentSpecialMatches;
    private float gameStartTime;

    // PlayerPrefs Data Constants
    private const string LEVEL_KEY = "level";
    private const string COIN_KEY = "coin";
    private const string SOUND_KEY = "sound";

    #endregion

    #region Events

    public static Action OnGameWin;
    public static Action OnGameLose;

    #endregion

    #region Unity Methods

    private void Awake() {
        // Register with Locator system
        Locator.Instance.RegisterGameManager(this);
        Application.targetFrameRate = 60;
        LoadGameData();
    }

    private void OnDestroy() {
        // Unregister from Locator system
        if (Locator.Instance != null) {
            Locator.Instance.UnregisterGameManager();
        }
    }

    private void Start() {
        StartNewGame();
        IsClickable = true;
    }

    private void Update() {
        if (isGameActive) {
            UpdateTimer();
        }
    }

    #endregion

    #region Game State Methods

    public void StartNewGame() {
        currentScore = 0;
        currentMatches = 0;
        currentSpecialMatches = 0;
        gameStartTime = Time.time;
        currentTime = levelTime;
        isGameActive = true;
        IsGameStarted = true;
        gameOverPanel.SetActive(false);
        UpdateUI();
    }

    private void UpdateTimer() {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0) {
            GameOver();
        }
        UpdateUI();
    }

    #endregion

    #region Game Events

    public void FinishLevel() {
        IsGameStarted = false;
        isGameActive = false;
        confetti.Play();
        Locator.Instance.SoundManagerInstance.PlaySFX("WinSound");
        AddCoin(50);
        OnGameWin?.Invoke();
        LevelUp();
        SaveGameStats();
    }

    public void GameOver() {
        IsGameStarted = false;
        Locator.Instance.SoundManagerInstance.PlaySFX("FailSound");
        OnGameLose?.Invoke();
        gameOverPanel.SetActive(true);
        SaveGameStats();
    }

    #endregion

    #region Level Methods

    private void GenerateLevel() {
        int spawnIndex = currentLevel - 1;
        // Locator.Instance.LevelGeneratorInstance.SpawnLevel(spawnIndex);
    }

    private void LevelUp() {
        currentLevel++;
        SaveSystem.Instance.UpdateLevel(currentLevel);
        currentTime = levelTime;
        UpdateUI();
    }

    public void ReloadScene() => SceneManager.LoadScene(0);

    #endregion

    #region Score Methods

    public void AddScore(int matchCount, bool isSpecial = false) {
        int points = isSpecial ? pointsPerSpecialMatch : pointsPerMatch;
        currentScore += points * matchCount;
        if (isSpecial)
        {
            currentSpecialMatches++;
        }
        else
        {
            currentMatches++;
        }
        SaveSystem.Instance.UpdateHighScore(currentScore);
        UpdateUI();
    }

    #endregion

    #region Coin Methods

    public void AddCoin(int amount) {
        _coin += amount;
        SaveSystem.Instance.AddCoins(amount);
    }

    public void DiscardCoin(int amount) {
        _coin -= amount;
        SaveSystem.Instance.AddCoins(-amount);
    }

    #endregion

    #region UI Methods

    private void UpdateUI() {
        scoreText.text = $"Score: {currentScore}";
        levelText.text = $"Level: {currentLevel}";
        timerText.text = $"Time: {Mathf.Ceil(currentTime)}s";
    }

    #endregion

    #region Data Management

    private void LoadGameData() {
        currentLevel = SaveSystem.Instance.GameData.currentLevel;
        _coin = SaveSystem.Instance.GameData.totalCoins;
    }

    private void SaveGameStats() {
        float gameTime = Time.time - gameStartTime;
        SaveSystem.Instance.UpdateGameStats(currentMatches, currentSpecialMatches, gameTime);
    }

    #endregion
}