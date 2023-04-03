using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    public Shader circleShader;
    public Color color;
    [Range(0f, 1f)]
    public float alpha = 1f;
    public float size = 3f;

    private Projector projector;

    void Start()
    {
        projector = GetComponent<Projector>();
        projector.material = new Material(circleShader);
        projector.material.SetColor("_Color", color);
        projector.material.SetFloat("_Alpha", alpha);
        projector.material.SetFloat("_Size", size);

    }
}
