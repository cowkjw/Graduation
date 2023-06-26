using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownScene : BaseScene
{

    Vector3 npcPos; // NPC ��ġ
    Vector3 dis; // NPC�� ���� �Ÿ�

    public override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Town;

        PlayerPos = new Vector3(0, -0.7f, 0);
        Player = Managers.Game.SpawnPlayer(PlayerPos);

        Camera.main.gameObject.GetComponent<CameraController>().SetPlayer(Player);
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

            if (hit.collider.gameObject.layer == 12) //NPC Ŭ��
            {
                npcPos = hit.collider.gameObject.transform.position; // NPC ��ġ �Ҵ�
                dis = Player.transform.position - npcPos; // ���� NPC �Ÿ�
                if (dis.magnitude <= 1)
                {
                    if (Managers.UI.NpcUI.activeSelf == false)
                    {
                        Managers.UI.NpcUI.SetActive(true);
                        Managers.UI.Inventory.SetActive(true);
                    }
                }
            }
            else // ���� ��� �̵��� ��
            {
                dis = Player.transform.position - npcPos; // NPC�� ������ �Ÿ� 
                if (dis.magnitude >= 3)
                {

                    if (Managers.UI.NpcUI.activeSelf == true)
                    {
                        Managers.UI.NpcUI.SetActive(false);
                        Managers.UI.Inventory.SetActive(false);
                    }
                }
            }
        }
    }
}
