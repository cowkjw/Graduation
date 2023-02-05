using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    GameObject _player =null;

    [SerializeField]
    Vector3 _delta = new Vector3(-9f, 9f, 3f); // �� ���� vector

    Renderer _obstacleRenderer;
    Material _material;
    Color matColor; 

    List<Renderer> _obsList;
   
     public void SetPlayer(GameObject player) { _player = player; } // �� �̵��ÿ� player set


    private void Start()
    {
        _obsList = new List<Renderer>();
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("Wall")))
        {
            var obs = hit.transform.gameObject.GetComponentInChildren<Renderer>();
            if (!_obsList.Contains(GetComponent<Renderer>()))
            {
                _obsList.Add(GetComponent<Renderer>());
                _material = GetComponent<Renderer>().material;
                matColor = _material.color;
                matColor.a = 0f;
                _material.color = matColor;
            }
        }
        else
        {
            foreach (var renderer in _obsList)
            {
                _material = renderer.material;
                matColor = _material.color;
                if (matColor.a == 0f)
                {
                    matColor.a = 1f;
                    _material.color = matColor;
                }
            }
            _obsList.Clear();
        }
    }

    void LateUpdate() 
    {
        transform.position = _player.transform.position + _delta;
        transform.LookAt(_player.transform);
    }
}
