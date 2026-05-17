using UnityEngine;
using System;

[Serializable]
public class ShopItemData
{
    public string itemName;

    [TextArea]
    public string itemDescription;

    public int itemPrice;

    public Sprite itemIcon;
}
