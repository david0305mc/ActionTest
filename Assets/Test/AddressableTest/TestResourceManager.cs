using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TestResourceManager : SingletonMono<TestResourceManager>
{
    public Dictionary<string, GameObject> prefabDic = new Dictionary<string, GameObject>();

    public Dictionary<string, GameObject> objDic = new Dictionary<string, GameObject>();
    public List<string> pathList = new List<string>()
    {
        "Assets/Test/AddressableTest/Res/A.prefab",
        "Assets/Test/AddressableTest/Res/B.prefab",
        "Assets/Test/AddressableTest/Res/C.prefab"
    };


    public async UniTask LoadAssets(string _pathName)
    {
        if (!prefabDic.ContainsKey(_pathName))
        {
            prefabDic[_pathName] = await Addressables.LoadAssetAsync<GameObject>(_pathName);
        }
    }


}
