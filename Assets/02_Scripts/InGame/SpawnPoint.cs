using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnPoint : EventPoint
{
    [HideInInspector] public SPAWN_TYPE spawnType;
    [HideInInspector] public int spawnTypeValue;
    public SPAWN_TARGET spawnTarget;
    public float delayTime;
    public int eventLinkNum;

    public List<DataManager.GridStageMSGroup> gridStageMSGroups;

    public void SpawnSet(int idx, DataManager.GridStageInfo _gridStageInfo)
    {
        if (spawnTarget == SPAWN_TARGET.USER)
            return;
        else if (spawnTarget == SPAWN_TARGET.ENEMY)
        {
            switch (idx)
            {
                case 0:
                    spawnType = (SPAWN_TYPE)_gridStageInfo.msconditiontype1;
                    spawnTypeValue = _gridStageInfo.msconditionvalue1;
                    gridStageMSGroups = DataManager.Instance.GetGridStageMSGroups(_gridStageInfo.msgroupid1);
                    break;
                case 1:
                    spawnType = (SPAWN_TYPE)_gridStageInfo.msconditiontype2;
                    spawnTypeValue = _gridStageInfo.msconditionvalue2;
                    gridStageMSGroups = DataManager.Instance.GetGridStageMSGroups(_gridStageInfo.msgroupid2);
                    break;
                case 2:
                    spawnType = (SPAWN_TYPE)_gridStageInfo.msconditiontype3;
                    spawnTypeValue = _gridStageInfo.msconditionvalue3;
                    gridStageMSGroups = DataManager.Instance.GetGridStageMSGroups(_gridStageInfo.msgroupid3);
                    break;
                case 3:
                    spawnType = (SPAWN_TYPE)_gridStageInfo.msconditiontype4;
                    spawnTypeValue = _gridStageInfo.msconditionvalue4;
                    gridStageMSGroups = DataManager.Instance.GetGridStageMSGroups(_gridStageInfo.msgroupid4);
                    break;
                default:
                    spawnType = (SPAWN_TYPE)_gridStageInfo.msconditiontype1;
                    spawnTypeValue = _gridStageInfo.msconditionvalue1;
                    gridStageMSGroups = DataManager.Instance.GetGridStageMSGroups(_gridStageInfo.msgroupid1);
                    break;
            }
        }
        else
            return;
    }
}
