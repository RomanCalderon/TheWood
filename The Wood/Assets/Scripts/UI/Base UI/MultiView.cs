using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MultiView : MonoBehaviour
{
    [SerializeField] Transform tabsHolder;
    List<Button> tabs = new List<Button>();
    [SerializeField] Transform viewsHolder;
    List<Transform> views = new List<Transform>();
    
    private void Awake()
    {
        foreach (Transform t in tabsHolder)
        {
            Button tab = t.GetComponent<Button>();

            if (tab != null)
                tabs.Add(t.GetComponent<Button>());
            else
                continue;

            tab.onClick.AddListener(delegate { OnClick(tabs.IndexOf(tab)); });
        }

        foreach (Transform t in viewsHolder)
            views.Add(t);
    }

    void OnClick(int index)
    {
        tabs.ForEach(t => t.interactable = true);
        tabs[index].interactable = false;

        views.ForEach(v => v.gameObject.SetActive(false));
        views[index].gameObject.SetActive(true);
    }
}
