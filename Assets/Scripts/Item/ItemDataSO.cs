using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataBase", menuName = "DataBase/ItemDataBase")]
public class ItemDataSO : ScriptableObject
{
    public List<ItemData> items = new List<ItemData>();
}
