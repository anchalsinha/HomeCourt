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
    private List<Snapshot> recording = new List<Snapshot>();

    private GameController gameController;

    void Start()
    {
        recordedTransforms = rightHand.GetComponentsInChildren<Transform>().Where(t => t.tag == "Trackable").ToList();
        gameController = GetComponent<GameController>();
    }

    public void SwingStarted()
    {
        swingStarted = true;
        gameController.EnableStartSwing();
    }

    public void SwingEnded()
    {
        gameController.EnableEndSwing();
    }

    public bool TrackSwing() {
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
            SerializeRecording();
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
