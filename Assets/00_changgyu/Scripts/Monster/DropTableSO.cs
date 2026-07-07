using UnityEngine;

[CreateAssetMenu(fileName = "DropTableSO", menuName = "Scriptable Objects/DropTableSO")]
public class DropTableSO : ScriptableObject
{
    public ItemSO[] dropItems;
    public float[] dropChance;
   

}
