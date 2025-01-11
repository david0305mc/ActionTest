using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
public class UIIntro : MonoBehaviour
{
    [SerializeField] private Button startBtn;

    private void Awake()
    {
        Application.targetFrameRate = 120;
        DataManager.Instance.LoadLocalization();
        startBtn.onClick.AddListener(() =>
        {
            StartGame().Forget();
        });
    }

    private async UniTask<bool> StartGame()
    {
        LocalizeManager.Instance.Initialize();
        await DataManager.Instance.LoadDataAsync();
        await DataManager.Instance.LoadConfigTable();
        await GameTime.InitGameTime();
        DataManager.Instance.MakeClientDT();
        
        int uid = PlayerPrefs.GetInt("uid", 1000);

        UserDataManager.Instance.LoadLocalData(uid.ToString());
        PlayerPrefs.SetInt("uid", uid);

        var mainSceneAsync = SceneManager.LoadSceneAsync("Main");
        await mainSceneAsync;
        return true;
    }

}
