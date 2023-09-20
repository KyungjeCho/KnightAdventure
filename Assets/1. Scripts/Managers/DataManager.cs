using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임의 모든 데이터를 관리하는 매니저
/// 데이터에 접근 하려면 데이터 매니저를 통해 접근 해야 한다.
/// </summary>
public class DataManager
{
    private static MonsterData monsterData;

    DataManager()
    {
        if (monsterData == null)
        {
            monsterData = new MonsterData();
            monsterData.LoadData();
        }
    }

    /// <summary>
    /// 몬스터 이름으로 원하는 데이터 클립을 가져오는 메서드
    /// 
    /// </summary>
    /// <param name="monsterName"></param>
    /// <returns></returns>
    public static MonsterClip GetMonsterData(string monsterName)
    {
        if (monsterData == null)
        {
            monsterData = new MonsterData();
            monsterData.LoadData();
        }

        MonsterClip temp = null;
        try
        {
            temp = monsterData.clips[monsterName];
        } catch (KeyNotFoundException e) 
        {
            Debug.LogError("몬스터의 이름이 잘못되었습니다. : " + monsterName + e);
        }

        return temp;
    }
}
