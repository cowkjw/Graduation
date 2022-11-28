using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Button : MonoBehaviour
{
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void CloseButton()
    {
        transform.GetChild(0).Find("CloseButton").parent.gameObject.SetActive(false);
    }
}
