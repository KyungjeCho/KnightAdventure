using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMonster : MonoBehaviour
{
    public string monsterName;
    public MonsterClip clip;

    // Start is called before the first frame update
    void Start()
    {
        if (clip == null)
        {
            clip = DataManager.GetMonsterData(monsterName);
        }
    }
}
