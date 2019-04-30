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
    public static void RetreiveGameDirectories(Action<string[]> callback)
    {
        string[] gameDirectories = Directory.GetDirectories(savedGamesPath);

        //string[] formattedDirectories = new string[gameDirectories.Length];

        //for (int i = 0; i < gameDirectories.Length; i++)
        //{
        //    string formattedGamePath = Directory.GetParent(gameDirectories[i]).FullName + "\\" + GetGameNameFromPath(gameDirectories[i]);
        //    formattedGamePath = formattedGamePath.Replace('\\', '/');
        //    Debug.Log(formattedGamePath);
        //    formattedDirectories[i] = formattedGamePath;
        //}

        callback(gameDirectories);
    }

    /// <summary>
    /// Checks if a game gameName already exists.
    /// </summary>
    /// <param name="gameName">Name of new game.</param>
    /// <returns>True if game exists, false if not.</returns>
    private static bool GameExists(string gameName)
    {
        return Directory.Exists(savedGamesPath + "/" + gameName);
    }

    /// <summary>
    /// Checks if a game path gamePath exists.
    /// </summary>
    /// <param name="gamePath"></param>
    /// <returns></returns>
    private static bool GamePathExists(string gamePath)
    {
        return Directory.Exists(gamePath);
    }

    /// <summary>
    /// Attempts to create a new game directory.
    /// </summary>
    /// <param name="gameName">Name of new game.</param>
    /// <param name="statusCallback">0 = New game succuessful. 1 = Invalid game name. 2 = Game already exists. string = New game directory.</param>
    public static void CreateNewGameDirectory(string gameName, Action<int, string> statusCallback)
    {
        if (string.IsNullOrEmpty(gameName) || string.IsNullOrWhiteSpace(gameName))
        {
            statusCallback(1, null);
            return;
        }

        if (!GameExists(gameName))
        {
            SetCurrentGamePath(savedGamesPath + "/" + gameName);
            DirectoryInfo di = Directory.CreateDirectory(CurrentGamePath);
            statusCallback(0, di.FullName);
        }
        else
        {
            statusCallback(2, null);
        }
    }

    /// <summary>
    /// Sets the path of the current game.
    /// </summary>
    /// <param name="gamePath">Name of game.</param>
    public static void SetCurrentGamePath(string gamePath)
    {
        CurrentGamePath = gamePath;

        //if (GamePathExists(gamePath))
        //    CurrentGamePath = gamePath;
        //else
        //{
        //    Debug.LogError("Game path [" + gamePath + "] does not exist.");
        //    Debug.Break();
        //}
    }

    /// <summary>
    /// Retreives the path of the current game.
    /// </summary>
    /// <returns>Current game path.</returns>
    public static string GetCurrentGamePath()
    {
        return CurrentGamePath;
    }

    /// <summary>
    /// Gets the name of a game from gamePath.
    /// </summary>
    /// <param name="gamePath">The game directory.</param>
    /// <returns></returns>
    public static string GetGameNameFromPath(string gamePath)
    {
        gamePath = Reverse(gamePath);

        string[] result = gamePath.Split('/','\\');

        result[0] = Reverse(result[0]);
        
        return result[0];
    }

    /// <summary>
    /// Helper function that returns string s reversed.
    /// </summary>
    /// <param name="s">String to be reversed.</param>
    /// <returns></returns>
    private static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}
