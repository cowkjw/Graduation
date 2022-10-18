using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    GameObject _player = null;

    [SerializeField]
    Vector3 _delta = new Vector3(0.0f, 6.0f, -5.0f); // 더 해줄 vector

    Renderer _obstacleRenderer;
    Material _material;
    Color matColor;

    // public void SetPlayer(GameObject player) { _player = player; }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("Wall")))
        {
            _obstacleRenderer = hit.transform.gameObject.GetComponentInChildren<Renderer>();
            _material = _obstacleRenderer.material;
            matColor = _material.color;
            

            if (_obstacleRenderer != null)
            {
             
                matColor.a = 0f;
                _material.color = matColor;
 
            }
        }
        else
        {
            if (_obstacleRenderer != null)
            {
                matColor.a = 1f;
                _material.color = matColor;
            }
        }
    }

    void LateUpdate() // Update 후에 카메라가 이동하도록
    {
        transform.position = _player.transform.position + _delta;
        transform.LookAt(_player.transform);
    }
}
