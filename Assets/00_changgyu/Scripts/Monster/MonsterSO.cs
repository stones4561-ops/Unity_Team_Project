using UnityEngine;

[CreateAssetMenu(fileName = "MonsterSO", menuName = "Scriptable Objects/MonsterSO")]
public class MonsterSO : ScriptableObject
{
    public string monsterName;
    public int monsterHP;
    public int monsterAtk;
    public float monsterSpeed;
    
}
