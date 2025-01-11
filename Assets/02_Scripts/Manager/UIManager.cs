using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct PopupVar
{
    public string popupName;
    public int stateNum;
}

public class UIManager : Singletone<UIManager>
{
    [SerializeField] private Transform canvasTrn;

    private List<Popup> makePopups = new List<Popup>(1000);

    private Stack<Popup> openPopups = new Stack<Popup>(100);
    private Queue<PopupVar> pendingPopupVars = new Queue<PopupVar>(100);

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
        //else if (Input.GetKeyDown(KeyCode.A))
        //{
        //    OpenPopup(Define.PopupOk);
        //}
        //else if (Input.GetKeyDown(KeyCode.S))
        //{
        //    OpenPopup(Define.PopupYesNo);
        //}
    }

    /// <summary>
    /// pName = 호출할 팝업이름
    /// sNum = 팝업의 상태 (기본값은 0)
    /// isNow = 팝업이 뜬 상태에서 그 위에 호출할 팝업인경우 (기본값은 flase)
    /// </summary>
    /// <param name="pName"></param>
    /// <param name="sNum"></param>
    /// <param name="isNow"></param>
    public void OpenPopup(string pName, int sNum = 0, bool isNow = false)
    {
        if (isNow || (openPopups.Count <= 0 && pendingPopupVars.Count <= 0))
            SetNowPopup(pName, sNum);
        else
            SetPendingPopup(pName, sNum);
    }

    public void SetNowPopup(string pName, int sNum = 0)
    {
        Popup pop;
        Popup makePop;

        if (makePopups.Any(r => r.popupName == pName))
        {
            pop = makePopups.SingleOrDefault(r => r.popupName == pName);
            if (pop == null)
            {
                Debug.LogError(pName + " is null.");
                return;
            }

            openPopups.Push(pop);
            pop.OpenPopup(pName, sNum);
        }
        else
        {
            makePop = (Popup)ObjectManager.Instance.GetGameObject(Define.PathPopupPrefab, pName, canvasTrn)?.GetComponent(typeof(Popup));
            if (makePop == null)
            {
                Debug.LogError(pName + " is null.");
                return;
            }
            makePop.gameObject.SetActive(false);
            makePopups.Add(makePop);

            openPopups.Push(makePop);
            makePop.OpenPopup(pName, sNum);
        }
    }

    public void SetPendingPopup(string pName, int sNum = 0)
    {
        PopupVar popupVar = new PopupVar();
        popupVar.popupName = pName;
        popupVar.stateNum = sNum;

        pendingPopupVars.Enqueue(popupVar);
    }

    public void ClosePopup()
    {
        if (openPopups.Count <= 0)
        {
            Debug.LogError("openPopups.Count is Zero");
            return;
        }

        openPopups.Pop().ClosePopup();
        if (openPopups.Count <= 0 && pendingPopupVars.Count > 0)
        {
            SetNowPopup(pendingPopupVars.First().popupName, pendingPopupVars.First().stateNum);
            pendingPopupVars.Dequeue();
        }
    }
}
