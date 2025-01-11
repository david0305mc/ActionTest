using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class ObjectManager : Singletone<ObjectManager>
{
    private StringBuilder sb = new StringBuilder();
    private Dictionary<string, GameObject> loadObjectList;

    public ObjectManager()
    {
        loadObjectList = new Dictionary<string, GameObject>();
    }

    public void ClearLoadedObject()
    {
        foreach (KeyValuePair<string, GameObject> data in loadObjectList)
            Addressables.ReleaseInstance(data.Value);
        loadObjectList.Clear();
    }

    public void InsertGameObject(string keystr, GameObject obj)
    {
        if (loadObjectList.ContainsKey(keystr))
            return;

        if (obj != null)
            loadObjectList.Add(keystr, obj);
    }

    public GameObject GetGameObject(string bundlename, string keystr, Transform trans)
    {
        GameObject retObj = null;

        sb.Length = 0;
        sb.Append(bundlename).Append(keystr).Append(Define.Prefab);
        if (loadObjectList.ContainsKey(sb.ToString()))
            retObj = loadObjectList[sb.ToString()];

        if (retObj == null)
        {
            Debug.LogError("ERROR " + bundlename + "/" + keystr);

            var op = Addressables.LoadAssetAsync<GameObject>(sb.ToString());
            retObj = op.WaitForCompletion();
            if (retObj != null)
                loadObjectList.Add(sb.ToString(), retObj);
        }

        return Instantiate(retObj, trans);
    }
}
