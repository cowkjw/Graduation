using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    GameObject _player =null;

    [SerializeField]
    Vector3 _delta = new Vector3(-9f, 9f, 3f); // 더 해줄 vector

    Renderer _obstacleRenderer;
    Material _material;
    Color matColor;

    List<Renderer> _obsList;
   
     public void SetPlayer(GameObject player) { _player = player; } // 씬 이동시에 player set


    private void Start()
    {
        _obsList = new List<Renderer>();
    }

    void Update()
    {
        RaycastHit hit;
      
        if (Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("Wall")))
        {
            
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
            

            foreach (var i in _obsList)
            {
                _material = i.material;
                matColor = _material.color;
                if(matColor.a == 0f)
                {
                matColor.a = 1f;
                _material.color = matColor;

                }
            }
            _obsList.Clear();
        }
    }

    void LateUpdate() // Update 후에 카메라가 이동하도록
    {
        transform.position = _player.transform.position + _delta;
        transform.LookAt(_player.transform);
    }
}
