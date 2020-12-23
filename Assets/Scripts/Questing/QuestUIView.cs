using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIView : MonoBehaviour
{
    Quest quest;
    public QuestStatusType statusType;

    [Header("General UI")]
    public Text questNameText;
    public Text questDescriptionText;
    public Text rewardText;
    public Text progressText;

    [Header("StatusType-Specific UI")]
    public Dropdown questOptionsDropdown;
    public Button acceptButton;
    public Button declineButton;
    

    public void Initialize(Quest quest)
    {
        if (quest == null)
        {
            Debug.LogError("QuestUIView: Initialize(quest) is null");
            return;
        }

        // Add progress update listener
        QuestController.OnQuestProgressUpdated += QuestProgressUpdated;
        QuestProgressUpdated(quest);

        // General initializations
        this.quest = quest;
        questNameText.text = quest.QuestName;
        questDescriptionText.text = quest.Description;
        rewardText.text = quest.GetQuestRewardString();
        progressText.text = quest.GetQuestProgressString();

        // StatusType-specific initializations
        Dropdown.OptionDataList trackedOptionDataList = new Dropdown.OptionDataList();
        trackedOptionDataList.options.Add(new Dropdown.OptionData("Track Quest"));
        trackedOptionDataList.options.Add(new Dropdown.OptionData("Untrack Quest"));
        Dropdown.OptionDataList untrackedOptionDataList = new Dropdown.OptionDataList();
        untrackedOptionDataList.options.Add(new Dropdown.OptionData("Track Quest"));
        untrackedOptionDataList.options.Add(new Dropdown.OptionData("Untrack Quest"));
        untrackedOptionDataList.options.Add(new Dropdown.OptionData("Abandon Quest"));
        
        switch (statusType)
        {
            case QuestStatusType.TRACKED:
                // Set dropdown options
                questOptionsDropdown.AddOptions(trackedOptionDataList.options);
                break;
            case QuestStatusType.UNTRACKED:
                // Set dropdown options
                questOptionsDropdown.AddOptions(untrackedOptionDataList.options);
                break;
            case QuestStatusType.PROPOSED:
                // Set "Accept" button listener
                acceptButton.onClick.AddListener(delegate { AcceptQuest(quest); });
                // Set "Decline" button listener
                declineButton.onClick.AddListener(delegate { DeclineQuest(quest); });
                break;
            default:
                break;
        }
        if (statusType != QuestStatusType.PROPOSED && statusType != QuestStatusType.COMPLETED)
        {
            // Remove old listener for onValueChange
            questOptionsDropdown.onValueChanged.RemoveAllListeners();

            // Add listener for onValueChange
            questOptionsDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(questOptionsDropdown); });

            // Set default value
            questOptionsDropdown.value = (quest.IsTracked) ? 0 : 1;
        }

        // Open this QuestView
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        QuestController.OnQuestProgressUpdated -= QuestProgressUpdated;
    }

    private void AcceptQuest(Quest quest)
    {
        quest.IsTracked = true;
        QuestController.QuestAccepted(quest);
        CloseQuestUIView();
    }

    private void DeclineQuest(Quest quest)
    {
        QuestController.QuestDeclined(quest);
        CloseQuestUIView();
    }

    void DropdownValueChanged(Dropdown change)
    {
        //Debug.Log("dropdown value changed to: " + change.value);

        // 0 = Quest Tracked
        if (change.value == 0 && !quest.IsTracked)
        {
            Destroy(questOptionsDropdown.GetComponentInChildren<Canvas>().gameObject);
            QuestController.QuestTracked(quest);
        }
        // 1 = Quest Untracked
        else if (change.value == 1 && quest.IsTracked)
        {
            Destroy(questOptionsDropdown.GetComponentInChildren<Canvas>().gameObject);
            QuestController.QuestUntracked(quest);
        }
        // 2 = Abandon Quest
        else if (change.value == 2)
        {
            Destroy(questOptionsDropdown.GetComponentInChildren<Canvas>().gameObject);
            QuestController.QuestAbandoned(quest);
            CloseQuestUIView();
        }

        //CloseQuestUIView();
    }

    public void CloseQuestUIView()
    {
        gameObject.SetActive(false);

        //quest = null;
        questNameText.text = string.Empty;
        questDescriptionText.text = string.Empty;
        rewardText.text = string.Empty;
        progressText.text = string.Empty;

        switch (statusType)
        {
            case QuestStatusType.TRACKED:
            case QuestStatusType.UNTRACKED:
                questOptionsDropdown.ClearOptions();
                questOptionsDropdown.onValueChanged.RemoveAllListeners();
                break;
            case QuestStatusType.PROPOSED:
                acceptButton.onClick.RemoveAllListeners();
                declineButton.onClick.RemoveAllListeners();
                break;
            default:
                break;
        }
    }

    private void QuestProgressUpdated(Quest quest)
    {
        if (this.quest != quest)
            return;

        questNameText.text = (quest.Completed) ? "<color=green>" + quest.QuestName + "</color>" : quest.QuestName;
        progressText.text = quest.GetQuestProgressString();
    }
}
