using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField]
    GameObject levelUpText;

    public void InstantiateLevelUpText()
    {
        GameObject tempLevelText = Instantiate(levelUpText);
        tempLevelText.transform.SetParent(this.transform);
    }
}
