using UnityEngine;



[System.Serializable]
public class BattleUnitData
{
    public long uid;
    public int tid;
    public bool IsEnemy;
    public int hp;
    public int grade;
    public bool isDead;
    public int maxHP;
    //public DataManager.Unitinfo refData;
    //public DataManager.UnitGradeInfo refUnitGradeData;
    //public bool IsMaxGrade => grade >= refData.maxgrade;

    public static BattleUnitData Create(long _uid, int _tid, int _grade, bool _isEnemy)
    {
        BattleUnitData data = new BattleUnitData()
        {
            uid = _uid,
            tid = _tid,
            grade = _grade,
            IsEnemy = _isEnemy,
            isDead = false,
        };
        data.UpdateRefData();
        data.maxHP = 3;
        data.hp = data.maxHP;
        //data.hp = data.refUnitGradeData.hp;

        return data;
    }
    public void UpdateRefData()
    {
        //refData = DataManager.Instance.GetUnitinfoData(tid);
        //refUnitGradeData = DataManager.Instance.GetUnitGrade(tid, grade);
    }
}

public struct AttackMessage
{
    public GameObject attackerObj;
    public int attackerUID;
    public int attackerTID;
    public int damage;
    public int grade;
    public bool attackToEnemy;

    public AttackMessage(GameObject _attackerObj, int _uid, int _tid, int _damage, int _grade, bool _attackToEnemy)
    {
        attackerObj = _attackerObj;
        attackerUID = _uid;
        attackerTID = _tid;
        damage = _damage;
        grade = _grade;
        attackToEnemy = _attackToEnemy;
    }
}

public class RewardData
{
    public ITEM_TYPE rewardtype;
    public int rewardid;
    public int rewardcount;
}
