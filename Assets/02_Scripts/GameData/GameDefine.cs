using MonsterLove.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EServerStatus
{
    /// <summary>??????</summary>
    Live,
    /// <summary>????</summary>
    Review = 5,
    /// <summary>???????? ????</summary>
    Update_Recommend,
    /// <summary>???????? ????</summary>
    Update_Essential,
    /// <summary>????</summary>
    Maintenance,
}
public class VersionData
{
    public int version;
    public OSCode os;
    public EServerStatus status;
    public string game_url;
    public string cdn_url;
    public int coupon_use;
}

public enum OSCode
{
    Unknown = -1,
    iOS = 0,
    Android = 1,
    Windows = Android,
}
public enum EServerType
{
    Dev,
    Qa,
    Review,
    Live,
}
public enum EPlatform
{
    None,
    Google,
    Apple,
    Guest,
    Webus,
    Email,
    DevWindows = 90,
    Unknown,
    Deleted,
}

public enum EStoreType
{
    None = 0,
    Android = 1,
    iOS = 2,
}
public enum EBuildType
{
    Dev,
    Release,
}

public enum UnitStates
{
    Idle,
    Approach,
    Dodge,
    Attack,
    KnockBack,
    Rolling,
    RollingExtraMove,
}

public enum AnimationEventType
{ 
    EnableWeaponFlag,
    DisableWeaponFlag,
    EndDodgeAnimation,
    EndAttack,
    DrawArrow,
}
public static class BuildSetting
{
    public static readonly EBuildType type = EBuildType.Dev;
    public static int version;

    static BuildSetting()
    {
#if DEV
        type = EBuildType.Dev;
#else
        type = EBuildType.Release;
#endif

        var versionSplit = Application.version.Split('.');
        int major = int.Parse(versionSplit[0]) * 10000;
        int minor = int.Parse(versionSplit[1]) * 100;
        int build = versionSplit.Length > 2 ? int.Parse(versionSplit[2]) : 0;
        version = major + minor + build;
    }
}

public class GameDefine
{
    public static string playerLayerName = "Player";
    public static string enemyLayerName = "Enemy";
    public static int EnemyApprochDist = 9;
    public static int EnemyAttackDist = 2;
    public static float PlayerAttackCooltime = 1f;
    public static float EnemyAttackCooltime = 3f;

}

