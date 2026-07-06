using UnityEngine;

[CreateAssetMenu(fileName = "DropTableSO", menuName = "Scriptable Objects/DropTableSO")]
public class DropTableSO : ScriptableObject
{
    public ItemSO dropItem1;
    public int dropItem1Chance;
    public ItemSO dropItem2;
    public int dropItem2Chance;

}
