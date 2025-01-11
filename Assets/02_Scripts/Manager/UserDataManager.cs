using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;

public class UserDataManager : Singleton<UserDataManager>
{
    private long flashUID = 10000;
    public long GenerateFlashUID() { return flashUID++; }
    public long GenerateUID() { return BaseData.GenerateUID(); }

    private static readonly string LocalFilePath = Path.Combine(Application.persistentDataPath, "SaveData");
    private static readonly string LocalVersionPath = Path.Combine(Application.persistentDataPath, "VersionData");
    public BaseData BaseData { get; set; }
    public InventoryData InventoryData { get; set; } = new InventoryData();


    public BattleUnitData PlayerBattleData { get; private set; }
    private Dictionary<long, BattleUnitData> EnemyDic;


    public void SaveLocalData(bool _updateVersion = true)
    {
        if (_updateVersion)
        {
            BaseData.dbVersion = GameTime.Get();
        }
        var saveData = JsonUtility.ToJson(BaseData);
        //saveData = Utill.EncryptXOR(saveData);
        Utill.SaveFile(LocalFilePath, saveData);
    }

    public void InitData()
    {
        BaseData = new BaseData();
        EnemyDic = new Dictionary<long, BattleUnitData>();
        PlayerBattleData = BattleUnitData.Create(GenerateFlashUID(), 0, 0, false);
        PlayerBattleData.hp = 100;
        PlayerBattleData.maxHP = 100;
    }
    public void LoadLocalData(string _uid)
    {
        InitData();
        if (File.Exists(LocalFilePath))
        {
            var localData = Utill.LoadFromFile(LocalFilePath);
            //localData = Utill.EncryptXOR(localData);
            BaseData loadedData = JsonUtility.FromJson<BaseData>(localData);

            if (loadedData.userUID != _uid)
            {
                CreateNewUser(_uid);
            }
            else
            {
                BaseData = Utill.MergeFields(BaseData, loadedData);
            }
            //LocalData.UpdateRefData();
        }
        else
        {
            CreateNewUser(_uid);
        }
        Debug.Log($"load Local {BaseData.userUID}");
    }

    public void CreateNewUser(string _uid)
    {
        // NewGame
        //LocalData.UpdateRefData();
        //LoadDefaultData();
        BaseData.userUID = _uid;
        SaveLocalData(false);
    }

    public void KillEnemy(long _uid)
    {
        var enmeyData = GetEnemyData(_uid);
        enmeyData.isDead = true;
    }

    public void RemoveEnemyData(long _uid)
    {
        if (EnemyDic.ContainsKey(_uid))
        {
            EnemyDic.Remove(_uid);
        }
        else
        {
            Debug.LogError($"Not Contain Enemy {_uid}");
        }
    }

    public bool IsEnemyAlive(long _uid)
    {
        var enemyData = GetEnemyData(_uid);
        if (enemyData != default)
        {
            return enemyData.hp > 0;
        }
        return false;
    }
    public BattleUnitData GetEnemyData(long _uid)
    {
        if (EnemyDic.TryGetValue(_uid, out BattleUnitData data))
        {
            return data;
        }
        return default;
    }
    public BattleUnitData AddEnemyData(int _tid)
    {
        var battleUnitData = BattleUnitData.Create(GenerateFlashUID(), _tid, 0, false);
        EnemyDic.Add(battleUnitData.uid, battleUnitData);
        return battleUnitData;
    }

    public BattleUnitData HitToEnemy(long enemyUID)
    {
        if (EnemyDic.TryGetValue(enemyUID, out var battleUnitData))
        {
            battleUnitData.hp --;
            if (battleUnitData.hp <= 0)
            {
                battleUnitData.hp = 0;
                battleUnitData.isDead = true;
            }
            return battleUnitData;
        }
        return null;
    }

    public BattleUnitData HitToPlayer()
    {
        PlayerBattleData.hp--;
        
        if (PlayerBattleData.hp <= 0)
        {
            PlayerBattleData.hp = 0;
            PlayerBattleData.isDead = true;
        }
        return PlayerBattleData;
    }

}


public class SData
{ 
    public void GetClassName()
    {
        Debug.Log(GetType().Name);
    }
    public T ConvertToObject<T>(string _saveData)
    {
        return JsonUtility.FromJson<T>(_saveData);
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}

public class BaseData : SData
{
    private long uidSeed;
    public long dbVersion;
    public string userUID;
    public IntReactiveProperty gold = new IntReactiveProperty();
    public int level;
    
    public SerializableDictionary<int, int> dicTest = new SerializableDictionary<int, int>();

    public long GenerateUID()
    {
        return uidSeed++;
    }

    public BaseData()     
    {
        uidSeed = 1000;
        level = 1;
        dbVersion = 0;
    }
    public void AddGold()
    {
        gold.Value++;
    }
    public void AddDicTest(int add)
    {
        GetClassName();
        if (dicTest.ContainsKey(1))
        {
            dicTest[1] = dicTest[1] + add;
        }
        else
        {
            dicTest[1] = add;
        }
        
    }
}

public class InventoryData : SData
{
    public List<int> itemList = new List<int>();
    public void AddItem()
    {
        GetClassName();
        itemList.Add(Random.Range(0, 1000));
    }
}
