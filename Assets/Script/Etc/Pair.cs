using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pair<T,U>
{

    public T First { get; set; }
    public U Second { get; set; }

    public Pair(T first, U second)
    {
        First = first;
        Second = second;
    }
}
