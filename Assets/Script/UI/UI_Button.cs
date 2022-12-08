using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Button : MonoBehaviour
{

    public void CloseButton()
    {
        transform.GetChild(0).GetChild(0).Find("CloseButton").parent.parent.gameObject.SetActive(false);
    }
}
