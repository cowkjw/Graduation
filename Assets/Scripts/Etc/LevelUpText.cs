using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpText : MonoBehaviour
{
    float _offset = 1.8f;
    float _maxScale = 2.0f;
    float _minScale = 1.0f;
    float _startTime;
    Transform _playerHead;

    private void Start()
    {
        _playerHead = Managers.Game.GetPlayer().transform; // 플레이어 위치
        _startTime = Time.time;
        transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(_playerHead.position + Vector3.up * _offset); // 플레이어 포지션 + (0,1,0) * offset
        transform.position = Camera.main.ScreenToWorldPoint(screenPos);
        transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);

        float t = (Time.time - _startTime) / 1f;
        float scale = Mathf.SmoothStep(_minScale, _maxScale, t); // 자연스럽게 변환
        transform.localScale = new Vector3(scale, scale, scale);

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
