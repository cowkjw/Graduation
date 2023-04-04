using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownScene : BaseScene
{

    Vector3 npcPos; // NPC 위치
    Vector3 dis; // NPC와 나의 거리

    public GameObject NPCUI { get { return npcUI; } }

    public override void Init()
    {
        base.Init();

        playerPos = new Vector3(0, -0.7f, 0);
        _player = Managers.Game.SpawnPlayer(playerPos);

        Camera.main.gameObject.GetComponent<CameraController>().SetPlayer(_player);
        npcUI = GameObject.Find("UI").transform.Find("NPC").gameObject;
        Managers.Input.MouseAction -= ClickNPC;
        Managers.Input.MouseAction += ClickNPC;
    }

    protected override void ClickNPC(Define.MouseState evt)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool raycastHit = Physics.Raycast(ray, out hit, 100f, 1 << 12 | 1 << 6);


        if (evt == Define.MouseState.LButtonDown ||
            evt == Define.MouseState.Click)
        {
            if (hit.collider == null)
                return;

            if (hit.collider.gameObject.layer == 12) //NPC 클릭
            {
                npcPos = hit.collider.gameObject.transform.position; // NPC 위치 할당
                dis = _player.transform.position - npcPos; // 나와 NPC 거리
                if (dis.magnitude <= 1)
                {
                    if (npcUI.activeSelf == false)
                    {
                        npcUI.SetActive(true);
                        inventory.SetActive(true);
                    }
                }
            }
            else // 땅을 찍고 이동할 때
            {
                dis = _player.transform.position - npcPos; // NPC와 나와의 거리 
                if (dis.magnitude >= 3)
                {

                    if (npcUI.activeSelf == true)
                    {
                        npcUI.SetActive(false);
                        inventory.SetActive(false);
                    }
                }
            }
        }
    }
}
