using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDataManager
{
    public static string CurrentGamePath { get; private set; }
    private static string savedGamesPath;

    /// <summary>
    /// Verifies if a SavedGames directory exists.
    /// If not, create a SavedGames directory to store all saved games and their data.
    /// </summary>
    public static void VerifySavedGamesDirectory()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/SavedGames/"))
            Directory.CreateDirectory(Application.persistentDataPath + "/SavedGames/");

        savedGamesPath = Application.persistentDataPath + "/SavedGames";
    }

    /// <summary>
    /// Retreives all saved games from Application.persistentDataPath.
    /// </summary>
    /// <param name="callback">Callback when games have been found.</param>
    public static void RetreiveSavedGames(Action<string[]> callback)
    {
        string[] gameDirectories = Directory.GetDirectories(savedGamesPath);

        callback(gameDirectories);
    }

    /// <summary>
    /// Checks if a game gameName already exists.
    /// </summary>
    /// <param name="gameName">Name of new game.</param>
    /// <returns>True if game exists, false if not.</returns>
    private static bool GameExists(string gameName)
    {
        // A game with same name exists, validate
        return Directory.Exists(savedGamesPath + "/" + gameName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="statusCallback">0 = New game succuessful. 1 = Invalid game name. 2 = Game already exists.</param>
    public static void CreateNewGameDirectory(string gameName, Action<int> statusCallback)
    {
        if (string.IsNullOrEmpty(gameName) || string.IsNullOrWhiteSpace(gameName))
        {
            Debug.LogError("Please choose a valid game name.");
            statusCallback(1);
            return;
        }

        if (!GameExists(gameName))
        {
            SetCurrentGame(gameName);
            Directory.CreateDirectory(CurrentGamePath);
            statusCallback(0);
        }
        else
        {
            Debug.LogError("Game with name [" + gameName + "] already exists. Please choose a unique game name.");
            statusCallback(2);
        }
    }

    /// <summary>
    /// Sets the path of the current game.
    /// </summary>
    /// <param name="gameName">Name of game.</param>
    public static void SetCurrentGame(string gameName)
    {
        if (GameExists(gameName))
        {
            CurrentGamePath = savedGamesPath + "/" + gameName;
        }
    }

    /// <summary>
    /// Retreives the path of the current game.
    /// </summary>
    /// <returns>Current game path.</returns>
    public static string GetCurrentGamePath()
    {
        return CurrentGamePath;
    }
}
