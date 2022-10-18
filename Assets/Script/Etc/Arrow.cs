using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    public Vector3 _target;

    void Start()
    {
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _target, 15 * Time.deltaTime);
    }

}
