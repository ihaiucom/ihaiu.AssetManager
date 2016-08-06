using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ListUtil 
{
    public static List<T> Merge<T>(this List<T> src, List<T> b)
    {
        int count = b.Count;
        for(int i = 0; i < count; i ++)
        {
            src.Add(b[i]);
        }

        return src;
    }
}