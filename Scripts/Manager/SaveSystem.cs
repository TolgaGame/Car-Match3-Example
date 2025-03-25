using UnityEngine;
using System;
using System.IO;
using System.Text;

public class SaveSystem : MonoBehaviour {
    private static SaveSystem _instance;
    private static bool _instanceSet;

    public static SaveSystem Instance {
        get {
            if (!_instanceSet) {
                _instance = FindObjectOfType<SaveSystem>();
                if (_instance == null) {
                    Debug.LogError("SaveSystem is not present in the scene!");
                    return null;
                }
                _instanceSet = true;
            }
            return _instance;
        }
    }

    private GameData _gameData;
    private const string SAVE_FILE_NAME = "gameData.json";

    public GameData GameData => _gameData;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGame();
        }
    }

    private void OnDestroy() {
        if (_instance == this) {
            _instanceSet = false;
        }
    }

    private void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus) {
            SaveGame();
        }
    }

    private void OnApplicationQuit() {
        SaveGame();
    }

    public void SaveGame() {
        try {
            string path = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            string jsonData = JsonUtility.ToJson(_gameData, true); // true for pretty print
            File.WriteAllText(path, jsonData, Encoding.UTF8);
            Debug.Log("Game saved successfully!");
        } catch (Exception e) {
            Debug.LogError($"Error saving game: {e.Message}");
        }
    }

    public void LoadGame() {
        try {
            string path = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            if (File.Exists(path)) {
                string jsonData = File.ReadAllText(path, Encoding.UTF8);
                _gameData = JsonUtility.FromJson<GameData>(jsonData);
                Debug.Log("Game loaded successfully!");
            } else {
                _gameData = new GameData();
                Debug.Log("No save file found. Created new game data.");
            }
        } catch (Exception e) {
            Debug.LogError($"Error loading game: {e.Message}");
            _gameData = new GameData();
        }
    }

    public void UpdateGameData(Action<GameData> updateAction) {
        updateAction?.Invoke(_gameData);
        SaveGame();
    }

    public void ResetGameData() {
        _gameData = new GameData();
        SaveGame();
    }

    // Helper methods for common operations
    public void AddCoins(int amount) {
        UpdateGameData(data => data.totalCoins += amount);
    }

    public void UpdateHighScore(int score) {
        UpdateGameData(data => {
            if (score > data.highScore) {
                data.highScore = score;
            }
        });
    }

    public void UpdateLevel(int level) {
        UpdateGameData(data => {
            if (level > data.currentLevel) {
                data.currentLevel = level;
            }
        });
    }

    public void ToggleSound(bool enabled) {
        UpdateGameData(data => data.soundEnabled = enabled);
    }

    public void ToggleMusic(bool enabled) {
        UpdateGameData(data => data.musicEnabled = enabled);
    }

    public void UpdateGameStats(int matches, int specialMatches, float gameTime) {
        UpdateGameData(data => {
            data.totalGamesPlayed++;
            data.totalMatches += matches;
            data.totalSpecialMatches += specialMatches;
            
            // Fix average game time calculation
            if (data.totalGamesPlayed == 1) {
                data.averageGameTime = gameTime;
            } else {
                data.averageGameTime = (data.averageGameTime * (data.totalGamesPlayed - 1) + gameTime) / data.totalGamesPlayed;
            }
            
            data.SetLastPlayedDate(DateTime.Now);
        });
    }

    public void UpdateStreak(int currentStreak) {
        UpdateGameData(data => {
            data.currentStreak = currentStreak;
            if (currentStreak > data.longestStreak) {
                data.longestStreak = currentStreak;
            }
        });
    }

    // Debug method to print save file location
    public void PrintSaveFileLocation() {
        string path = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        Debug.Log($"Save file location: {path}");
    }
} 