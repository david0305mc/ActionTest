using Unity.Mathematics;
using UnityEngine;

public class UnitHUD : MonoBehaviour
{

    [SerializeField] private GameObject devilHitedIcon;
    [SerializeField] private GameObject selectedObj;
    [SerializeField] private GameObject hpProgress;

    //private Camera mainCamera;
    private quaternion orgRotation;

    public GameObject SelectedObj => selectedObj;

    public GameObject DevilHitedIcon => devilHitedIcon;

    public void Init()
    {
        //mainCamera = Camera.main;
        orgRotation = transform.rotation;
        DevilHitedIcon.SetActive(false);
        SelectedObj.SetActive(false);
        SetProgressBar(1f);
    }
    public void SetProgressBar(float _progress)
    {
        hpProgress.transform.localScale = new Vector2(_progress, 1);
    }
    private void LateUpdate()
    {
        //transform.rotation = mainCamera.transform.rotation * orgRotation;
        transform.rotation = orgRotation;
    }
}
