using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static int PALM_CENTER_MARKER_ID = 61;
}

[Serializable]
public struct State 
{
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
}

[Serializable]
public struct Snapshot
{
    public float Time;
    public State[] States;
}