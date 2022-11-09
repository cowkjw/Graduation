using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownScene : MonoBehaviour
{
    
    
    void Start()
    {
        Init();
    }


    void Init()
    {
        Managers.Clear(); // �����ߴ��� nulló��
        
        GameObject player = Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
        player.transform.position = new Vector3(0, 0, 0);
        if (player != null)
        {
            Camera.main.gameObject.GetComponent<CameraController>().SetPlayer(player);

        }
        else
            Debug.Log("Error");
    }
}
