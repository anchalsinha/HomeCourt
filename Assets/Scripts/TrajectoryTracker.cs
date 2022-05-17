using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class TrajectoryTracker : MonoBehaviour
{
    public AnimationClip clip;
    public GameObject rightHand;

    private GameObjectRecorder recorder;

    void Start()
    {
        recorder = new GameObjectRecorder(rightHand);
        recorder.BindComponentsOfType<Transform>(rightHand, true);
    }

    void LateUpdate()
    {
        if (clip == null)
            return;

        recorder.TakeSnapshot(Time.deltaTime);
    }

    void OnDisable()
    {
        if (clip == null)
            return;

        if (recorder.isRecording)
        {
            recorder.SaveToClip(clip);
        }
    }
}
