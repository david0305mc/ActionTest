using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class AddressableTestManager : MonoBehaviour
{
    [SerializeField] private Button buttonA;
    [SerializeField] private Button buttonB;

    [SerializeField] private AssetReferenceGameObject cubeObj;
    private List<GameObject> gameObjs = new List<GameObject>();

    private void Awake()
    {
        buttonA.onClick.AddListener(() =>
        {
            cubeObj.InstantiateAsync().Completed += (obj) =>
            {
                gameObjs.Add(obj.Result);
            };
        });
        buttonB.onClick.AddListener(() =>
        {
            if (gameObjs.Count == 0)
            {
                return;
            }

            for (int i = gameObjs.Count - 1; i >= 0; i--)
            {
                Addressables.ReleaseInstance(gameObjs[i]);
                gameObjs.RemoveAt(i);
            }
        });
    }
}
