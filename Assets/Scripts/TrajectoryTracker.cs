using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Oculus.Interaction.Input;
using Newtonsoft.Json;

public class TrajectoryTracker : MonoBehaviour
{
    public GameObject rightHand;

    private Hand gestureHand;

    private bool isRecording = false;
    private bool swingStarted, swingEnded = false;

    private List<Transform> recordedTransforms = new List<Transform>();
    private List<Vector3> recording = new List<Vector3>();

    public float lineWidth;
    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        recordedTransforms = rightHand.GetComponentsInChildren<Transform>().Where(t => t.tag == "Trackable").ToList();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }

    public void SwingStarted()
    {
        swingStarted = true;
        lineRenderer.positionCount = 0;
    }

    public void SwingEnded()
    {
        swingEnded = true;
    }

    public bool TrackSwing() {
        if (!isRecording && !swingStarted) {
            swingEnded = false;
        }

        if (!isRecording && swingStarted) {
            isRecording = true;
            Debug.Log("Start Recording");
        }

        if (isRecording)
            TakeSnapshot();
        
        if (isRecording && swingEnded) {
            isRecording = false;
            Debug.Log("Stop Recording");
            swingStarted = false;
            swingEnded = false;

            List<Vector3> smoothTrajectory = Utilities.Discretize(recording);
            lineRenderer.positionCount = smoothTrajectory.Count;
            lineRenderer.SetPositions(smoothTrajectory.ToArray());
            recording = smoothTrajectory;

            return false;
        }

        return true;
    }
    
    void TakeSnapshot() 
    {
        var states = new State[recordedTransforms.Count];
        for (var i = 0; i < states.Length; i++)
        {
            var t = recordedTransforms[i];
            var state = new State();
            state.Position = t.position;
            state.Rotation = t.rotation.eulerAngles;
            state.Scale = t.lossyScale;
            states[i] = state;
        }

        lineRenderer.positionCount += 1;
        Vector3 pos = states[Constants.PALM_CENTER_MARKER_ID].Position;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(pos.x, pos.y, pos.z));
        recording.Add(pos);
    }

    public List<Vector3> GetTrajectoryRecording()
    {
        var ret = new List<Vector3>(recording);
        recording.Clear();
        return ret;
    }
}
