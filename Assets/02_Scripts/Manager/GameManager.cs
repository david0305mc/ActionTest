using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using System;

public partial class GameManager : SingletonMono<GameManager>
{
    [SerializeField] private UnitObj enemyObjPrefab;
    [SerializeField] private Transform enemySpawnPoint;
    private List<SpawnPoint> enemySpawnPoints = new List<SpawnPoint>();

    [SerializeField] private PlayerObj playerObjPrefab;
    [SerializeField] private SpawnPoint userSpawnPoint;

    [SerializeField] private Transform field;
    [SerializeField] private ProjectileLinear projectileTest;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera cinemachineVirtualCamera;

    [HideInInspector] public PlayerObj PlayerObj { get; set; }
    private Dictionary<long, UnitObj> enemyObjDic;
    private Dictionary<long, ProjectileLinear> projectileDic;

    private CancellationTokenSource cts;

    [HideInInspector] public int nowStageId;

    private void Dispose()
    {
        cts?.Clear();
        foreach (var item in projectileDic)
        {
            item.Value.Dispose();
        }
        projectileDic.Clear();
    }
    protected override void OnSingletonAwake()
    {
        base.OnSingletonAwake();
        enemyObjDic = new Dictionary<long, UnitObj>();
        cts = new CancellationTokenSource();
        projectileDic = new Dictionary<long, ProjectileLinear>(0);
    }

    private void Start()
    {
        InitGame();
    }

    private void OnDestroy()
    {
        Dispose();
    }

    
    public void InitGame()
    {
        nowStageId = 1;
        SpawnEnemySet();
        SpawnStart();
    }

    private void HitToEnmey(UnitBaseObj _unitbaseObj)
    {
        AttackMessage attackMessage = new AttackMessage();
        attackMessage.attackerObj = PlayerObj.gameObject;
        attackMessage.attackerUID = PlayerObj.GetInstanceID();

        var enemyData = UserDataManager.Instance.HitToEnemy(_unitbaseObj.unitData.uid);
        if (enemyData.isDead)
        {
            // Dead
            KillEnemyObj(enemyData.uid);
        }
        else
        {
            _unitbaseObj.GetDamaged(attackMessage);
        }
    }

    private void HitToPlayer(UnitBaseObj _unitbaseObj)
    {
        AttackMessage attackMessage = new AttackMessage();
        attackMessage.attackerObj = PlayerObj.gameObject;
        attackMessage.attackerUID = PlayerObj.GetInstanceID();

        var playerData = UserDataManager.Instance.HitToPlayer();
        if (playerData.hp <= 0)
        {
            // Dead
            KillEnemyObj(playerData.uid);
        }
        else
        {
            _unitbaseObj.GetDamaged(attackMessage);
        }
    }


    public UnitObj GetEnemyObj(long _uid)
    {
        if (enemyObjDic.TryGetValue(_uid, out var unitObj))
        {
            return unitObj;
        }
        Debug.LogError($"GetEnemyObj Not Found {_uid}");
        return default;
    }
    private void KillEnemyObj(long _uid)
    {
        var enemyObj = enemyObjDic[_uid];
        Lean.Pool.LeanPool.Despawn(enemyObj);
        enemyObjDic.Remove(_uid);
        UserDataManager.Instance.KillEnemy(_uid);
    }

    private void KillPlayerObj()
    {
        Debug.LogError("KillPlayerObj");
    }

    public void SpawnEnemy()
    {
        BattleUnitData enemyData = UserDataManager.Instance.AddEnemyData(0);
        Vector3 spawnPos = enemySpawnPoint.position + new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), 0, UnityEngine.Random.Range(-1.5f, 1.5f));
        UnitObj enemyObj = Lean.Pool.LeanPool.Spawn(enemyObjPrefab, spawnPos, Quaternion.identity, field);
        enemyObj.InitObj(enemyData);
        enemyObjDic.Add(enemyData.uid, enemyObj);
        
        enemyObj.DamageEvent = (_unitbaseObj) =>
        {
            HitToPlayer(_unitbaseObj);
        };
    }

    private void SpawnEnemySet()
    {
        DataManager.GridStageInfo gridStageInfo = DataManager.Instance.GetGridStageInfo(nowStageId);
        int len = enemySpawnPoint.childCount;
        for (int i = 0; i < len; ++i)
        {
            SpawnPoint sp = enemySpawnPoint.GetChild(i).GetComponent<SpawnPoint>();
            sp.SpawnSet(i, gridStageInfo);
            enemySpawnPoints.Add(sp);
        }
    }

    public async UniTaskVoid SpawnEnemy(SpawnPoint _sp)
    {
        if (_sp.delayTime > 0)
            await UniTask.Delay(TimeSpan.FromSeconds(_sp.delayTime));

        int groupLen = _sp.gridStageMSGroups.Count;
        for (int j = 0; j < groupLen; ++j)
        {
            int enemyCount = _sp.gridStageMSGroups[j].unitcount;
            for (int k = 0; k < enemyCount; ++k)
            {
                BattleUnitData enemyData = UserDataManager.Instance.AddEnemyData(_sp.gridStageMSGroups[j].unitid);
                Vector3 spawnPos = _sp.transform.position + new Vector3(UnityEngine.Random.Range(-1.5f, 1.5f), 0, UnityEngine.Random.Range(-1.5f, 1.5f));
                UnitObj enemyObj = Lean.Pool.LeanPool.Spawn(enemyObjPrefab, spawnPos, Quaternion.identity, field);
                enemyObj.InitObj(enemyData);
                enemyObjDic.Add(enemyData.uid, enemyObj);
            }
        }
    }

    public void SpawnStart()
    {
        if (PlayerObj == null)
            PlayerObj = Lean.Pool.LeanPool.Spawn(playerObjPrefab, userSpawnPoint.transform.position, Quaternion.identity, field);
        PlayerObj.InitObj(UserDataManager.Instance.PlayerBattleData);
        PlayerObj.DamageEvent = (_unitbaseObj) =>
        {
            HitToEnmey(_unitbaseObj);
        };

        PlayerObj.ProjectileFireEvent = (_targetUnit) =>
        {
            ProjectileLinear projectile = Lean.Pool.LeanPool.Spawn(projectileTest, PlayerObj.transform.position + Vector3.up, Quaternion.identity, field);
            var uid = UserDataManager.Instance.GenerateFlashUID();
            projectile.Fire(_targetUnit).Forget();
            projectile.DamageEvent = (_unitbaseObj) =>
            {
                HitToEnmey(_unitbaseObj);
                if (projectileDic.ContainsKey(uid))
                {
                    projectile.Dispose();
                    projectileDic.Remove(uid);
                }
                else
                {
                    Debug.LogError($"projectileDic Not ContainsKey {uid}");
                }
            };
            projectileDic.Add(uid, projectile);
        };
        cinemachineVirtualCamera.Follow = PlayerObj.GetCharacterAniTrn();

        int len = enemySpawnPoints.Count;
        for (int i = 0; i < len; ++i)
        {
            SpawnEnemy(enemySpawnPoints[i]);
        }
    }

    public void DodgePlayer()
    {
        PlayerObj.Dodge();
    }
}
