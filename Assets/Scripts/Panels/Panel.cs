using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public event Action onShow;
    public event Action onHide;

    public bool isVisible { private set; get; }

    public virtual void Init()
    {
        Hide();
        OnInit();
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        isVisible = true;
        OnShow();
        onShow?.Invoke();
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
        isVisible = false;
        OnHide();
        onHide?.Invoke();
    }

    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
    protected virtual void OnInit() { }
}
