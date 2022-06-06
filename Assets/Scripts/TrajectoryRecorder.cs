using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Oculus.Interaction.Input;
using Newtonsoft.Json;

public class TrajectoryRecorder : MonoBehaviour
{
    public GameObject rightHand, leftHand;
    public GameObject guideHand;

    private Hand gestureHand;

    private bool isRecording;

    private List<Transform> recordedTransforms = new List<Transform>();
    private List<Snapshot> recording = new List<Snapshot>();
    private int count = 0;


    private GameObject playbackHandInstance;


    void Start()
    {
        OVRManager.display.RecenterPose();
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

                // show playback
                if (playbackHandInstance)
                    Destroy(playbackHandInstance);
                playbackHandInstance = Instantiate(guideHand, Vector3.zero, Quaternion.identity);
                var guidePlayer = playbackHandInstance.GetComponent<TrajectoryPlayer>();
                guidePlayer.autoPlay = true;
                guidePlayer.Load(recording);

                SerializeRecording();
            }
        }
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
