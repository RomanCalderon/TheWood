using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestStatusType
{
    TRACKED,
    UNTRACKED,
    PROPOSED
}

public class QuestController : MonoBehaviour
{
    public delegate void QuestMenuHandler(bool state);
    public static event QuestMenuHandler OnQuestMenuStateChanged;
    public delegate void QuestProposalHandler(Quest quest);
    public static event QuestProposalHandler OnQuestProposed;
    public static event QuestProposalHandler OnQuestAccepted;
    public static event QuestProposalHandler OnQuestDeclined;
    public delegate void QuestOptionsHandler(Quest quest);
    public static event QuestOptionsHandler OnQuestTracked;
    public static event QuestOptionsHandler OnQuestUntracked;
    public static event QuestOptionsHandler OnQuestAbandoned;
    public delegate void QuestProgressHandler(Quest quest);
    public static event QuestProgressHandler OnQuestProgressUpdated;

    private bool QuestMenuDisplayed = false;

    public List<Quest> trackedQuests = new List<Quest>();
    public List<Quest> untrackedQuests = new List<Quest>();
    public List<Quest> proposedQuests = new List<Quest>();

    [Header("Listed Quest Holders")]
    [SerializeField] Transform trackedQuestsHolder;
    [SerializeField] Transform untrackedQuestsHolder;
    [SerializeField] Transform proposedQuestsHolder;

    [Space]
    [SerializeField] GameObject listedQuestPrefab;

    [Header("Focused Quest Views")]
    [SerializeField] QuestUIView trackedQuestView;
    [SerializeField] QuestUIView untrackedQuestView;
    [SerializeField] QuestUIView proposedQuestView;

    // Start is called before the first frame update
    void Start()
    {
        //OnQuestMenuStateChanged += QuestMenuState;

        OnQuestProposed += ReceiveQuestProposal;
        OnQuestDeclined += RemoveQuest;
        OnQuestAccepted += AcceptQuest;
        OnQuestTracked += TrackQuest;
        OnQuestUntracked += UntrackQuest;
        OnQuestAbandoned += AbandonQuest;
    }

    // Events
    public static void ChangeQuestMenuState(bool state)
    {
        OnQuestMenuStateChanged?.Invoke(state);
    }

    public static void QuestProposed(Quest quest)
    {
        OnQuestProposed?.Invoke(quest);
        OnQuestMenuStateChanged(true);
    }

    public static void QuestAccepted(Quest quest)
    {
        OnQuestAccepted?.Invoke(quest);
    }

    public static void QuestDeclined(Quest quest)
    {
        OnQuestDeclined?.Invoke(quest);
    }

    public static void QuestTracked(Quest quest)
    {
        OnQuestTracked?.Invoke(quest);
    }

    public static void QuestUntracked(Quest quest)
    {
        OnQuestUntracked?.Invoke(quest);
    }

    public static void QuestAbandoned(Quest quest)
    {
        OnQuestAbandoned?.Invoke(quest);
    }

    public static void UpdateQuestProgress(Quest quest)
    {
        OnQuestProgressUpdated?.Invoke(quest);
    }
    

    // Received Quest proposal and adds to proposedQuests list
    public void ReceiveQuestProposal(Quest quest)
    {
        Debug.Log("New Quest proposal! (" + quest.QuestName + ")");
        CreateListedQuest(quest, QuestStatusType.PROPOSED);

        // Open the Proposed Quest View
        proposedQuestView.Initialize(quest);
    }

    // Declines Quest proposal and removes from porposedQuests list
    private void RemoveQuest(Quest quest)
    {
        Debug.Log("Quest declined! (" + quest.QuestName + ")");
        DestroyListedQuest(quest, QuestStatusType.PROPOSED);
    }

    // Accepts Quest proposal and adds to trackedQuests list
    private void AcceptQuest(Quest quest)
    {
        Debug.Log("Quest accepted! (" + quest.QuestName + ")");
        MoveListedQuest(quest, QuestStatusType.PROPOSED, QuestStatusType.TRACKED);
    }

    private void TrackQuest(Quest quest)
    {
        MoveListedQuest(quest, QuestStatusType.UNTRACKED, QuestStatusType.TRACKED);
    }

    private void UntrackQuest(Quest quest)
    {
        MoveListedQuest(quest, QuestStatusType.TRACKED, QuestStatusType.UNTRACKED);
    }

    private void AbandonQuest(Quest quest)
    {
        Debug.Log("Abandoned (" + quest.QuestName + ")");
        DestroyListedQuest(quest, (quest.IsTracked) ? QuestStatusType.TRACKED : QuestStatusType.UNTRACKED);
    }

    #region Listed Quest Functions

    private void CreateListedQuest(Quest quest, QuestStatusType statusType)
    {
        QuestUIListed newListedQuest = null;

        switch (statusType)
        {
            case QuestStatusType.TRACKED:
                trackedQuests.Add(quest);
                newListedQuest = Instantiate(listedQuestPrefab, trackedQuestsHolder).GetComponent<QuestUIListed>();
                newListedQuest.Initialize(quest);
                newListedQuest.openQuestViewButton.onClick.AddListener(delegate { proposedQuestView.Initialize(quest); });
                break;
            case QuestStatusType.UNTRACKED:
                untrackedQuests.Add(quest);
                newListedQuest = Instantiate(listedQuestPrefab, untrackedQuestsHolder).GetComponent<QuestUIListed>();
                newListedQuest.Initialize(quest);
                newListedQuest.openQuestViewButton.onClick.AddListener(delegate { proposedQuestView.Initialize(quest); });
                break;
            case QuestStatusType.PROPOSED:
                proposedQuests.Add(quest);
                newListedQuest = Instantiate(listedQuestPrefab, proposedQuestsHolder).GetComponent<QuestUIListed>();
                newListedQuest.Initialize(quest);
                newListedQuest.openQuestViewButton.onClick.AddListener(delegate { proposedQuestView.Initialize(quest); });
                break;
            default:
                break;
        }
    }

    private void DestroyListedQuest(Quest quest, QuestStatusType statusType)
    {
        switch (statusType)
        {
            case QuestStatusType.TRACKED:
                trackedQuests.Remove(quest);
                foreach (Transform t in trackedQuestsHolder)
                    if (t.GetComponent<QuestUIListed>().GetQuest() == quest)
                        Destroy(t.gameObject);
                break;
            case QuestStatusType.UNTRACKED:
                untrackedQuests.Remove(quest);
                foreach (Transform t in untrackedQuestsHolder)
                    if (t.GetComponent<QuestUIListed>().GetQuest() == quest)
                        Destroy(t.gameObject);
                break;
            case QuestStatusType.PROPOSED:
                proposedQuests.Remove(quest);
                foreach (Transform t in proposedQuestsHolder)
                    if (t.GetComponent<QuestUIListed>().GetQuest().QuestName == quest.QuestName)
                        Destroy(t.gameObject);
                break;
            default:
                break;
        }
    }

    private Transform GetListedQuest(Quest quest, QuestStatusType statusType)
    {
        Transform result = null;

        switch (statusType)
        {
            case QuestStatusType.TRACKED:
                foreach (Transform t in trackedQuestsHolder)
                    if (t.GetComponent<QuestUIListed>().GetQuest() == quest)
                        result = t;
                break;
            case QuestStatusType.UNTRACKED:
                foreach (Transform t in untrackedQuestsHolder)
                    if (t.GetComponent<QuestUIListed>().GetQuest() == quest)
                        result = t;
                break;
            case QuestStatusType.PROPOSED:
                foreach (Transform t in proposedQuestsHolder)
                    if (t.GetComponent<QuestUIListed>().GetQuest() == quest)
                        result = t;
                break;
            default:
                break;
        }

        if (result == null)
            Debug.LogError("Could not find " + quest + " in " + statusType.ToString());

        return result;
    }

    private void MoveListedQuest(Quest quest, QuestStatusType fromStatus, QuestStatusType toStatus)
    {
        Transform listedQuestTransform = GetListedQuest(quest, fromStatus);
        QuestUIListed questUIListed = listedQuestTransform.GetComponent<QuestUIListed>();

        // Clear old listeners
        questUIListed.openQuestViewButton.onClick.RemoveAllListeners();
        

        // Move the QuestUIListed object to new parent and relink listener
        // to new QuestUIView object
        if (listedQuestTransform != null)
        {
            // Remove Quest from old list
            // Close old QuestUIView
            switch (fromStatus)
            {
                case QuestStatusType.TRACKED:
                    trackedQuests.Remove(quest);
                    trackedQuestView.CloseQuestUIView();
                    break;
                case QuestStatusType.UNTRACKED:
                    untrackedQuests.Remove(quest);
                    untrackedQuestView.CloseQuestUIView();
                    break;
                case QuestStatusType.PROPOSED:
                    proposedQuests.Remove(quest);
                    proposedQuestView.CloseQuestUIView();
                    break;
                default:
                    break;
            }

            // Move to new status
            switch (toStatus)
            {
                case QuestStatusType.TRACKED:
                    trackedQuests.Add(quest);
                    listedQuestTransform.SetParent(trackedQuestsHolder);
                    questUIListed.openQuestViewButton.onClick.AddListener(delegate { trackedQuestView.Initialize(quest); });
                    break;
                case QuestStatusType.UNTRACKED:
                    untrackedQuests.Add(quest);
                    listedQuestTransform.SetParent(untrackedQuestsHolder);
                    questUIListed.openQuestViewButton.onClick.AddListener(delegate { untrackedQuestView.Initialize(quest); });
                    break;
                case QuestStatusType.PROPOSED:
                    proposedQuests.Add(quest);
                    listedQuestTransform.SetParent(proposedQuestsHolder);
                    questUIListed.openQuestViewButton.onClick.AddListener(delegate { proposedQuestView.Initialize(quest); });
                    break;
                default:
                    break;
            }
        }
        else
            Debug.LogError("Could not move ListedQuest because it was not found.");
    }

    #endregion
}
