using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIListed : MonoBehaviour
{
    Quest quest;

    [Header("UI")]
    [SerializeField] Text questNameText;
    [SerializeField] Text questProgressText;
    public Button openQuestViewButton;

    public void Initialize(Quest quest)
    {
        this.quest = quest;
        questNameText.text = quest.QuestName;
        questProgressText.text = quest.GetQuestProgressString();
    }

    private void OnEnable()
    {
        if (quest == null)
            return;
        
        QuestController.OnQuestProgressUpdated += QuestProgressUpdated;
        QuestProgressUpdated(quest);
    }

    private void OnDisable()
    {
        QuestController.OnQuestProgressUpdated -= QuestProgressUpdated;
    }

    private void OnDestroy()
    {
        QuestController.OnQuestProgressUpdated -= QuestProgressUpdated;
    }

    private void QuestProgressUpdated(Quest quest)
    {
        if (this.quest != quest)
            return;

        questNameText.text = (quest.Completed) ? "<color=green>" + quest.QuestName + "</color>" : quest.QuestName;
        questProgressText.text = quest.GetQuestProgressString();
    }

    public Quest GetQuest()
    {
        return quest;
    }
}
