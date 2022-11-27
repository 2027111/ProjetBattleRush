using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUpdates
{

    public uint Tick { get; private set; }
    public Vector3 Position { get; private set; }


    public TransformUpdates(uint t, Vector3 p)
    {
        Tick = t;
        Position = p;
    }

}
