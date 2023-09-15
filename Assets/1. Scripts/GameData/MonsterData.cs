using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터 클립 리스트를 가지고 있다.
/// CSV 파일을 읽는 기능을 가지고 있다.
/// </summary>
public class MonsterData : BaseData
{
    public Dictionary<string, MonsterClip> clips;

    private string csvFilePath = "Data/";
    private string csvFileName = "MonsterTable";

    public MonsterData() { }

    public void LoadData()
    {
        var list = CSVReader.Read(csvFilePath + csvFileName);
        clips = new Dictionary<string, MonsterClip>();
        for (int i = 0; i < list.Count; i++)
        {
            string monsterName = list[i]["name"].ToString();

            clips.Add(monsterName, new MonsterClip());

            clips[monsterName].monsterName    = monsterName;
            clips[monsterName].hp             = float.Parse(list[i]["hp"].ToString());
            clips[monsterName].damage         = float.Parse(list[i]["damage"].ToString());
            clips[monsterName].defence        = float.Parse(list[i]["defence"].ToString());
            clips[monsterName].speed          = float.Parse(list[i]["speed"].ToString());
            clips[monsterName].cooldown       = float.Parse(list[i]["cooldown"].ToString());
        }
    }
}
