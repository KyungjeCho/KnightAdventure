using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ��� �����͸� �����ϴ� �Ŵ���
/// �����Ϳ� ���� �Ϸ��� ������ �Ŵ����� ���� ���� �ؾ� �Ѵ�.
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
    /// ���� �̸����� ���ϴ� ������ Ŭ���� �������� �޼���
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
            Debug.LogError("������ �̸��� �߸��Ǿ����ϴ�. : " + monsterName + e);
        }

        return temp;
    }
}
