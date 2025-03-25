using System;
using UnityEngine;

[Serializable]
public class GameData {
    public int currentLevel;
    public int totalCoins;
    public int highScore;
    public bool soundEnabled;
    public bool musicEnabled;
    public string lastPlayedDate;
    public int totalGamesPlayed;
    public int totalMatches;
    public int totalSpecialMatches;
    public float averageGameTime;
    public int longestStreak;
    public int currentStreak;

    public GameData() {
        currentLevel = 1;
        totalCoins = 0;
        highScore = 0;
        soundEnabled = true;
        musicEnabled = true;
        lastPlayedDate = DateTime.Now.ToString("O");
        totalGamesPlayed = 0;
        totalMatches = 0;
        totalSpecialMatches = 0;
        averageGameTime = 0f;
        longestStreak = 0;
        currentStreak = 0;
    }

    public DateTime GetLastPlayedDate() {
        return DateTime.Parse(lastPlayedDate);
    }

    public void SetLastPlayedDate(DateTime date) {
        lastPlayedDate = date.ToString("O");
    }
} 