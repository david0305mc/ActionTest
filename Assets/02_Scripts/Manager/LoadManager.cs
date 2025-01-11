using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadManager : Singletone<LoadManager>
{
    List<string> labels = new List<string>() { "cdn" };

    private void Awake()
    {
        StartLoad().Forget();
    }

    private async UniTask<bool> StartLoad()
    {
        await StartDownLoad();
        await LoadAddressable();

        return true;
    }

    private async UniTask StartDownLoad()
    {
        foreach (string label in labels)
        {
            Addressables.DownloadDependenciesAsync(label).Completed += (AsyncOperationHandle handle) => 
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Addressables.Release(handle);
                }
            };
        }
    }

    //private IEnumerator CoStartDownLoad(string label)
    //{
    //    var handle = Addressables.DownloadDependenciesAsync(label);

    //    while (!handle.IsDone)
    //    {
    //        yield return Define.WaitForEndOfFrame;
    //    }
    //    if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Failed)
    //    {
    //        yield break;
    //    }

    //    Addressables.Release(handle);

    //    LoadAddressable();
    //}

    private async UniTask LoadAddressable()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Define.PathPopupPrefab).Append(Define.PopupOk).Append(Define.Prefab);
        var op = Addressables.LoadAssetAsync<GameObject>(sb.ToString());
        await UniTask.WaitUntil(() => op.IsDone);
        if (op.Status == AsyncOperationStatus.Succeeded)
            ObjectManager.Instance.InsertGameObject(sb.ToString(), op.Result);

        sb.Length = 0;
        sb.Append(Define.PathPopupPrefab).Append(Define.PopupYesNo).Append(Define.Prefab);
        op = Addressables.LoadAssetAsync<GameObject>(sb.ToString());
        await UniTask.WaitUntil(() => op.IsDone);
        if (op.Status == AsyncOperationStatus.Succeeded)
            ObjectManager.Instance.InsertGameObject(sb.ToString(), op.Result);
    }

    //private IEnumerator CoLoadAddressable(string keystr)
    //{
    //    var op = Addressables.LoadAssetAsync<GameObject>(keystr);
    //    yield return op;
    //    if (op.IsDone)
    //        ObjectManager.Instance.InsertGameObject(keystr, op.Result);
    //}
}
