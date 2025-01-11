using UnityEngine;

public class PopupOk : Popup
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
            case 1:
                Debug.Log("Click_OK");
                UIManager.Instance.ClosePopup();
                break;
        }
    }
}
