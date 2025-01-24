using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SecondSceneManager : MonoBehaviour
{
    [SerializeField] private Button gotoFirstButton;

    [SerializeField] private Button buttonLoad;
    [SerializeField] private Button buttonInstantiage;
    [SerializeField] private Button buttonRelease;

    public Dictionary<string, GameObject> objDic = new Dictionary<string, GameObject>();
    private void Awake()
    {

        buttonLoad.onClick.AddListener(() =>
        {
            foreach (var item in TestResourceManager.Instance.pathList)
            {
                TestResourceManager.Instance.LoadAssets(item);
            }
        });
        buttonInstantiage.onClick.AddListener(() =>
        {
            foreach (var item in TestResourceManager.Instance.pathList)
            {
                var itemObj = Lean.Pool.LeanPool.Spawn(TestResourceManager.Instance.prefabDic[item], transform.position + Random.insideUnitSphere * 1f, Quaternion.identity, transform);
                objDic[name] = itemObj;
            }
        });
        buttonRelease.onClick.AddListener(() =>
        {
            foreach (var item in TestResourceManager.Instance.pathList)
            {
                Lean.Pool.LeanPool.Despawn(objDic[item]);
                objDic.Remove(name);
            }
        });

        gotoFirstButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("AddressableTest");
        });
    }


}
