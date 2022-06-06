using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Oculus.Interaction.Input;
using Newtonsoft.Json;

public class TrajectoryTracker : MonoBehaviour
{
    public GameObject rightHand, leftHand;
    public GameObject guideHand;
    public GameObject startSwing, endSwing;

    private Hand gestureHand;

    private bool isRecording = false;

    private List<Transform> recordedTransforms = new List<Transform>();
    private List<Snapshot> recording = new List<Snapshot>();

    private GameObject playbackHandInstance;


    void Start()
    {
        gestureHand = leftHand.GetComponent<Hand>();
        recordedTransforms = rightHand.GetComponentsInChildren<Transform>().Where(t => t.tag == "Trackable").ToList();
        Debug.Log($"Recorded transforms: {recordedTransforms.Count}");

    }

    void Update()
    {
        // if (gestureHand.GetFingerIsPinching(HandFinger.Index))
        // {
        //     if (!isRecording)
        //     {
        //         Debug.Log("Start Recording");
        //         isRecording = true;
        //     }
        //     TakeSnapshot();
        // }
        // else
        // {
        //     if (isRecording)
        //     {
        //         isRecording = false;
        //         count++;
        //         Debug.Log("Stop Recording");

        //         // show playback
        //         if (playbackHandInstance)
        //             Destroy(playbackHandInstance);
        //         playbackHandInstance = Instantiate(guideHand, Vector3.zero, Quaternion.identity);
        //         var guidePlayer = playbackHandInstance.GetComponent<TrajectoryPlayer>();
        //         guidePlayer.Load(recording);

        //         SerializeRecording();
        //     }
        // }
    }

    public bool TrackSwing() {
        // if (!isRecording && COLLIDE_POINT_A) {
        //     isRecording = true;
        //     Debug.Log("Start Recording");
        // }

        if (isRecording)
            TakeSnapshot();
        
        // if (isRecording && COLLIDE_POINT_B) {
        //     isRecording = false;
        //     Debug.Log("Stop Recording");
        //     SerializeRecording();
        //     return false;
        // }

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
        var snapshot = new Snapshot();
        snapshot.Time = Time.time;
        snapshot.States = states;

        recording.Add(snapshot);
    }

    void SerializeRecording()
    {
        string recording_string = JsonConvert.SerializeObject(recording);
        recording.Clear();

        string fname = System.DateTime.Now.ToString("HH-mm-ss") + ".json";
        string path = Path.Combine(Application.persistentDataPath, fname);
        Debug.Log($"Recording saved: {path}");
        StreamWriter writer = new StreamWriter(path);
        writer.WriteLine(recording_string);
        writer.Close();
    }
}
