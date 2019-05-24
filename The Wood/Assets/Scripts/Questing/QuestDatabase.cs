using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class QuestDatabase : MonoBehaviour
{
    public static QuestDatabase instance;
    
    [SerializeField] public List<Quest> quests = new List<Quest>();
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
            instance = this;
        DontDestroyOnLoad(gameObject);

        ReadQuestData();
    }

    private void ReadQuestData()
    {
        quests = JsonConvert.DeserializeObject<List<Quest>>(Resources.Load<TextAsset>("JSON/Quests").ToString());
    }

    /// <summary>
    /// Adds a Quest to the database.
    /// </summary>
    /// <param name="quest">New Quest to be added to the database.</param>
    private void AddQuest(Quest quest)
    {
        // Check if the Quest is null
        if (quest == null)
        {
            Debug.LogError("New Quest is null.");
            return;
        }

        // Add the new Quest to the database
        quests.Add(quest);
    }

    public Quest GetQuestByName(string questName)
    {
        return quests.Find(q => q.QuestName == questName);
    }

    public List<Quest> GetAllQuests()
    {
        return quests;
    }

    public string[] GetQuestNames()
    {
        string[] result = new string[quests.Count];

        for (int i = 0; i < quests.Count; i++)
            result[i] = quests[i].QuestName;
        
        return result;
    }
}
