using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Oculus.Interaction.Input;

public class TrajectoryTracker : MonoBehaviour
{
    public GameObject rightHand, leftHand;

    public GameObject playbackHand;

    private Hand gestureHand;

    private bool isRecording;

    private List<Transform> recordedTransforms = new List<Transform>();
    private List<Snapshot> recording = new List<Snapshot>();
    private int count = 0;

    void Start()
    {
        gestureHand = leftHand.GetComponent<Hand>();
        recordedTransforms = rightHand.GetComponentsInChildren<Transform>().Where(t => t.tag == "Trackable").ToList();
        Debug.Log($"Recorded transforms: {recordedTransforms.Count}");
    }

    void Update()
    {
        if (gestureHand.GetFingerIsPinching(HandFinger.Index))
        {
            if (!isRecording)
            {
                Debug.Log("Start Recording");
                isRecording = true;
            }
            TakeSnapshot();
        }
        else
        {
            if (isRecording)
            {
                isRecording = false;
                count++;
                Debug.Log("Stop Recording");
                // SaveClip();
                var guide = Instantiate(playbackHand, Vector3.zero, Quaternion.identity);
                TrajectoryPlayer guidePlayer = guide.GetComponent<TrajectoryPlayer>();
                guidePlayer.Load(recording, guide.GetComponentsInChildren<Transform>().Where(t => t.tag == "Trackable").ToList());
            }
        }
    }

    void TakeSnapshot() 
    {
        var states = new State[recordedTransforms.Count];
        Debug.Log("Initialized States");
        for (var i = 0; i < states.Length; i++)
        {
            var t = recordedTransforms[i];
            Debug.Log("Get Transform");
            var state = new State();
            state.Position = t.position;
            state.Rotation = t.rotation.eulerAngles;
            state.Scale = t.lossyScale;
            Debug.Log("Update State");
            states[i] = state;
            Debug.Log("Assign State");
        }
        Debug.Log("Done getting states");
        var snapshot = new Snapshot();
        snapshot.Time = Time.time;
        snapshot.States = states;
        Debug.Log("Update snapshot");

        recording.Add(snapshot);
        Debug.Log("Added snapshot");
    }

    void SaveClip()
    {
        Debug.Log("Save Clip");
    }
}
