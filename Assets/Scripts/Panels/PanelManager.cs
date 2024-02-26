using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PanelManager : Singleton<PanelManager>
{
    [SerializeField] List<Panel> panels = new List<Panel>();
    [SerializeField] Panel startPanel;

    void Awake()
    {
        Init();
    }

    public void Init()
    {
        InitAllPanels();
        if (startPanel == null && panels.Count > 0)
        {
            startPanel = panels[0];
        }
        startPanel?.Show();
    }

    public T GetPanel<T>() where T : Panel
    {
        return panels.Find(panel => panel is T) as T;
    }

    public void ShowPanel<T>() where T : Panel
    {
        T panel = GetPanel<T>();
        if (panel != null)
        {
            panel.Show();
        }
    }

    public void HidePanel<T>() where T : Panel
    {
        T panel = GetPanel<T>();
        if (panel != null)
        {
            panel.Hide();
        }
    }

    public void ShowPanel(Panel panel)
    {
        if (panel != null)
        {
            panel.Show();
        }
    }

    public void HidePanel(Panel panel)
    {
        if (panel != null)
        {
            panel.Hide();
        }
    }

    void InitAllPanels()
    {
        foreach (Panel panel in panels)
        {
            panel.Init();
        }
    }
}
