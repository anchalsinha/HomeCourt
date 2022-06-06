using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingEndpoint : MonoBehaviour
{
    public bool starting;
    public TrajectoryTracker trajectoryTracker;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Enter");
        if (starting)
            trajectoryTracker.SwingStarted();
        else
            trajectoryTracker.SwingEnded();
    }
}
