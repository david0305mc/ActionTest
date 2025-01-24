using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AddressableTestManager : MonoBehaviour
{
    [SerializeField] private Button buttonLoad;
    [SerializeField] private Button buttonInstantiage;
    [SerializeField] private Button buttonRelease;
    [SerializeField] private Button buttonGoToSecond;


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
            foreach (var name in TestResourceManager.Instance.pathList)
            {
                var itemObj = Lean.Pool.LeanPool.Spawn(TestResourceManager.Instance.prefabDic[name], transform.position + Random.insideUnitSphere * 1f, Quaternion.identity, transform);
                objDic[name] = itemObj;
            }
        });
        buttonRelease.onClick.AddListener(() =>
        {
            foreach (var name in TestResourceManager.Instance.pathList)
            {
                Lean.Pool.LeanPool.Despawn(objDic[name]);
                objDic.Remove(name);
            }
        });
        buttonGoToSecond.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("SecondScene");
        });
    }


}
