using UnityEngine;

public class Popup : MonoBehaviour
{
    [HideInInspector] public string popupName;
    [HideInInspector] public int stateNum;

    public virtual void OpenPopup(string pName, int sNum = 0)
    {
        popupName = pName;
        stateNum = sNum;

        gameObject.SetActive(true);
    }

    public virtual void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnClickButton(int bNum)
    {

    }
}
