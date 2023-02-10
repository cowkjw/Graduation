using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    GameObject _player =null;

    [SerializeField]
    Vector3 _delta = new Vector3(-9f, 9f, 3f);

    Material _material;
    Color matColor; 

    public List<Renderer> _obsList;
   
     public void SetPlayer(GameObject player) { _player = player; } 


    private void Start()
    {
        _obsList = new List<Renderer>();
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("Wall")))
        {

            if (!_obsList.Contains(hit.transform.gameObject.GetComponentInChildren<Renderer>()))
            _obsList.Add(hit.transform.gameObject.GetComponentInChildren<Renderer>());
                foreach (var i in _obsList)
            { 
                _material = i.material;
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
