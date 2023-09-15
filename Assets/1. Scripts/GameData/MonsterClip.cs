using UnityEngine;

/// <summary>
/// 몬스터 클립은 CSV File을 통해 얻은 데이터를 런타임에 저장하고 있을 클래스이다.
/// 몬스터 오브젝트는 원하는 데이터가 있으면 DataManager 클래스를 통해 각각 MonsterClip의 내부 Attribute에 접근할 수 있다.
/// </summary>
public class MonsterClip
{
    #region Variables
    public string monsterName;
    public float hp;
    public float damage;
    public float defence;
    public float speed;
    public float cooldown;
    #endregion Variables
}
