using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpText : MonoBehaviour
{

    float offset = 1.8f;
    float maxScale = 2.0f;
    float minScale = 1.0f;

    Transform playerHead;
    float startTime;

    private void Start()
    {
        playerHead = Managers.Game.GetPlayer().transform; // �÷��̾� ��ġ
        startTime = Time.time;
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(playerHead.position + Vector3.up * offset); // �÷��̾� ������ + (0,1,0) * offset
        transform.position = Camera.main.ScreenToWorldPoint(screenPos);
        transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);

        float t = (Time.time - startTime) / 1f;
        float scale = Mathf.SmoothStep(minScale, maxScale, t); // �ڿ������� ��ȯ
        transform.localScale = new Vector3(scale, scale, scale);


        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
