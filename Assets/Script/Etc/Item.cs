using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public bool inItem;
    string _itemName;
    

    public string ItemName { get { return _itemName; } set { _itemName = value; } }

}
