using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    GameObject _player = null;
    [SerializeField]
    Vector3 _delta = new Vector3(-9f, 9f, 3f);
    Material _material;
    HashSet<Renderer> _obsList;
    Color _matColor;

    public void SetPlayer(GameObject player) { _player = player; }

    private void Start()
    {
        _obsList = new HashSet<Renderer>();
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("Wall")))
        {

            if (!_obsList.Contains(hit.transform.gameObject.GetComponentInChildren<Renderer>()))
                _obsList.Add(hit.transform.gameObject.GetComponentInChildren<Renderer>());
            foreach (Renderer i in _obsList)
            {
                _material = i.material;
                _matColor = _material.color;
                _matColor.a = 0f;
                _material.color = _matColor;
            }


        }
        else
        {
            foreach (Renderer renderer in _obsList)
            {
                _material = renderer.material;
                _matColor = _material.color;
                if (_matColor.a == 0f)
                {
                    _matColor.a = 1f;
                    _material.color = _matColor;
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
