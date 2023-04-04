using System;
using UnityEngine;


[Serializable]
public class VectorConverter // 몬스터 위치 변환
{
    public float x;
    public float y;
    public float z;

    public VectorConverter(Vector3 vector)
    {
        this.x = vector.x;
        this.y = vector.y;
        this.z = vector.z;
    }

    public Vector3 ToVecotr3()
    {
        return new Vector3(this.x, this.y, this.z);
    }
}
