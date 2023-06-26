using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    public GameObject NpcUI = null;
    public GameObject Inventory = null;
    public  Slider PlayerMpBar;
    public  Slider PlayerHpBar;

    protected GameObject _uiPrefab = null;


    public void Init()
    {
        _uiPrefab = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UI_Prefab/UI"));
        _uiPrefab.name = "UI";
        Object.DontDestroyOnLoad(_uiPrefab); // _uiPrefab 파괴되지 않도록 설정
        Inventory = GameObject.Find("UI").transform.Find("Inventory").gameObject;
        NpcUI = GameObject.Find("UI").transform.Find("NPC").gameObject;
        GameObject playerUI = GameObject.FindGameObjectWithTag("PlayerUI");

        if (playerUI != null)
        {
            PlayerHpBar = playerUI.transform.GetChild(1).GetComponent<Slider>();
            PlayerMpBar = playerUI.transform.GetChild(2).GetComponent<Slider>();
        }
        else
        {
            Debug.LogWarning("PlayerUI를 찾을 수 없습니다.");
        }
    }
    public void BindUI()
    {
        GameObject playerUI = GameObject.FindGameObjectWithTag("PlayerUI");

        if (playerUI != null)
        {
            PlayerHpBar = playerUI.transform.GetChild(1).GetComponent<Slider>();
            PlayerMpBar = playerUI.transform.GetChild(2).GetComponent<Slider>();
        }
        else
        {
            Debug.LogWarning("PlayerUI를 찾을 수 없습니다.");
        }
    }
    public void Clear()
    {
        UI_Button uiButton = GameObject.FindObjectOfType<UI_Button>();
        if (uiButton == null)
        {
            return;
        }
        Managers.Input.KeyboardAction -= uiButton.UIKeyEvent;
        Managers.Input.KeyboardAction += uiButton.UIKeyEvent;
    }
}
