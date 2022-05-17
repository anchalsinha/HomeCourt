using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct State 
{
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
}

public struct Snapshot
{
    public float Time;
    public State[] States;
}