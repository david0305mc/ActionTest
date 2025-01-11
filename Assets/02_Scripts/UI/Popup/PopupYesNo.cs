using UnityEngine;

public class PopupYesNo : Popup
{
    public override void OpenPopup(string pName, int sNum = 0)
    {
        base.OpenPopup(pName, sNum);


    }

    public override void ClosePopup()
    {
        base.ClosePopup();
    }

    public override void OnClickButton(int bNum)
    {
        switch (bNum)
        {
            case 0:
                Debug.Log("Click_NO");
                UIManager.Instance.ClosePopup();
                break;
            case 1:
                Debug.Log("Click_YES");
                UIManager.Instance.ClosePopup();
                break;
        }
    }
}
