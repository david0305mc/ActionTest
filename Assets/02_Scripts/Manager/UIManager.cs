using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public readonly struct PopupData
{
    public string PopupName { get; }
    public int StateNum { get; }

    public PopupData(string popupName, int stateNum)
    {
        PopupName = popupName ?? throw new ArgumentNullException(nameof(popupName));
        StateNum = stateNum;
    }
}

public class UIManager : Singletone<UIManager>
{
    [SerializeField] private Transform canvasTransform;

    private readonly Dictionary<string, Popup> popupCache = new Dictionary<string, Popup>();
    private readonly Stack<Popup> activePopups = new Stack<Popup>();
    private readonly Queue<PopupData> pendingPopups = new Queue<PopupData>();

    private const string POPUP_PREFAB_PATH = Define.PathPopupPrefab;

    #region Unity Lifecycle
    private void Update()
    {
        HandleInput();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 팝업을 열기 위한 메서드
    /// </summary>
    /// <param name="popupName">팝업 이름</param>
    /// <param name="stateNum">팝업 상태 (기본값: 0)</param>
    /// <param name="immediate">즉시 표시 여부 (기본값: false)</param>
    /// <exception cref="ArgumentNullException">popupName이 null인 경우</exception>
    public void OpenPopup(string popupName, int stateNum = 0, bool immediate = false)
    {
        if (string.IsNullOrEmpty(popupName))
            throw new ArgumentNullException(nameof(popupName));

        if (immediate || CanShowPopupImmediately())
        {
            ShowPopup(popupName, stateNum);
        }
        else
        {
            QueuePopup(popupName, stateNum);
        }
    }

    /// <summary>
    /// 현재 활성화된 최상위 팝업을 닫습니다.
    /// </summary>
    public void ClosePopup()
    {
        if (!activePopups.Any())
        {
            Debug.LogWarning("No active popups to close.");
            return;
        }

        CloseTopPopup();
        ProcessPendingPopups();
    }
    #endregion

    #region Private Methods
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }

    private bool CanShowPopupImmediately()
    {
        return !activePopups.Any() && !pendingPopups.Any();
    }

    private void ShowPopup(string popupName, int stateNum)
    {
        var popup = GetOrCreatePopup(popupName);
        if (popup != null)
        {
            activePopups.Push(popup);
            popup.OpenPopup(popupName, stateNum);
        }
    }

    private void QueuePopup(string popupName, int stateNum)
    {
        pendingPopups.Enqueue(new PopupData(popupName, stateNum));
    }

    private Popup GetOrCreatePopup(string popupName)
    {
        if (popupCache.TryGetValue(popupName, out var existingPopup))
        {
            return existingPopup;
        }

        var newPopup = CreatePopup(popupName);
        if (newPopup != null)
        {
            popupCache[popupName] = newPopup;
        }
        return newPopup;
    }

    private Popup CreatePopup(string popupName)
    {
        var popupObject = ObjectManager.Instance.GetGameObject(POPUP_PREFAB_PATH, popupName, canvasTransform);
        if (popupObject == null)
        {
            Debug.LogError($"Failed to create popup: {popupName}");
            return null;
        }

        var popup = popupObject.GetComponent<Popup>();
        if (popup == null)
        {
            Debug.LogError($"Popup component not found on: {popupName}");
            return null;
        }

        popup.gameObject.SetActive(false);
        return popup;
    }

    private void CloseTopPopup()
    {
        var popup = activePopups.Pop();
        popup.ClosePopup();
    }

    private void ProcessPendingPopups()
    {
        if (!activePopups.Any() && pendingPopups.Any())
        {
            var nextPopup = pendingPopups.Dequeue();
            ShowPopup(nextPopup.PopupName, nextPopup.StateNum);
        }
    }
    #endregion
}
