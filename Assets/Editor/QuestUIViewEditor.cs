using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(QuestUIView))]
public class QuestUIViewEditor : Editor
{
    QuestUIView questUIView;

    private void OnEnable()
    {
        questUIView = (QuestUIView)target;
    }

    public override void OnInspectorGUI()
    {
        questUIView.statusType = (QuestStatusType)EditorGUILayout.EnumPopup("Quest Status Type", questUIView.statusType);

        EditorGUILayout.LabelField("General UI", EditorStyles.boldLabel);
        questUIView.questNameText = (Text)EditorGUILayout.ObjectField("Quest Name Text", questUIView.questNameText, typeof(Text), true);
        questUIView.questDescriptionText = (Text)EditorGUILayout.ObjectField("Quest Description Text", questUIView.questDescriptionText, typeof(Text), true);
        questUIView.rewardText = (Text)EditorGUILayout.ObjectField("Quest Reward Text", questUIView.rewardText, typeof(Text), true);
        questUIView.progressText = (Text)EditorGUILayout.ObjectField("Quest Progress Text", questUIView.progressText, typeof(Text), true);

        EditorGUILayout.LabelField("StatusType-Specific UI", EditorStyles.boldLabel);
        switch (questUIView.statusType)
        {
            case QuestStatusType.TRACKED:
            case QuestStatusType.UNTRACKED:
                questUIView.questOptionsDropdown = (Dropdown)EditorGUILayout.ObjectField("Quest Options Dropdown", questUIView.questOptionsDropdown, typeof(Dropdown), true);
                break;
            case QuestStatusType.PROPOSED:
                questUIView.acceptButton = (Button)EditorGUILayout.ObjectField("Accept Button", questUIView.acceptButton, typeof(Button), true);
                questUIView.declineButton = (Button)EditorGUILayout.ObjectField("Decline Button", questUIView.declineButton, typeof(Button), true);
                break;
            default:
                break;
        }
    }
}
