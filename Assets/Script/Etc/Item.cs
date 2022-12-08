using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{

    public Sprite itemSprite;
    string itemName;
    public bool isDrop = true;

    public string ItemName { get { return itemName; } }

    private void Awake()
    {
        itemName = transform.name;
        itemSprite = Resources.Load<Sprite>($"Items/{itemName}");
    }
}
