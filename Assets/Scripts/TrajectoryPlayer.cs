using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrajectoryPlayer : MonoBehaviour
{
    [SerializeField] private float timeScale = 1;
    private List<Snapshot> snapshots;
    private int currSnapshot = 0;
    private List<Transform> transforms;

    private int frame = 1;
    private float time = 0;
    private float minTime;
    private float maxTime;

    public void Start()
    {
        transforms = GetComponentsInChildren<Transform>().Where(t => t.tag == "Trackable").ToList();
    }

    public void Load(List<Snapshot> recording)
    {
        minTime = recording[0].Time;
        maxTime = recording[recording.Count - 1].Time;
        snapshots = new List<Snapshot>(recording);
    }

    private void Update()
    {
        // if (snapshots.Count == 0)
        //     return;

        if (frame == snapshots.Count - 1)
            frame = 0;

        time = Mathf.Clamp(time + Time.deltaTime * timeScale, minTime, maxTime);
                
        // var (prev, next) = GetSnapshots();

        // var snapshotDelta = (time - prev.Time) / (next.Time - prev.Time);

        // for (var i = 0; i < transforms.Count; i++)
        // {
        //     var transform = transforms[i];
        //     var prevState = prev.States[i];
        //     var nextState = next.States[i];
        //     transform.position = Vector3.Lerp(prevState.Position, nextState.Position, snapshotDelta);
        //     transform.eulerAngles = Quaternion.Lerp(Quaternion.Euler(prevState.Rotation), Quaternion.Euler(nextState.Rotation), snapshotDelta).eulerAngles;
        //     transform.localScale = Vector3.Lerp(prevState.Scale, nextState.Scale, snapshotDelta);
        // }
    }

    private (Snapshot prev, Snapshot next) GetSnapshots()
    {
        while (true)
        {
            var s = snapshots[frame];
            if (s.Time >= time)
                break;
            frame++;
        }

        var prev = snapshots[frame - 1];
        var next = snapshots[frame];
        return (prev, next);
    }

    public bool PlayGuideTrajectory() {
        if (snapshots.Count == 0)
            return false;
        
        if (currSnapshot == snapshots.Count - 1)
            currSnapshot = 0;

        time = Mathf.Clamp(time + Time.deltaTime * timeScale, minTime, maxTime);
                
        var (prev, next) = GetSnapshots();
        var snapshotDelta = (time - prev.Time) / (next.Time - prev.Time);

        for (var i = 0; i < transforms.Count; i++)
        {
            var transform = transforms[i];
            var prevState = prev.States[i];
            var nextState = next.States[i];
            transform.position = Vector3.Lerp(prevState.Position, nextState.Position, snapshotDelta);
            transform.eulerAngles = Quaternion.Lerp(Quaternion.Euler(prevState.Rotation), Quaternion.Euler(nextState.Rotation), snapshotDelta).eulerAngles;
            transform.localScale = Vector3.Lerp(prevState.Scale, nextState.Scale, snapshotDelta);
        }
        currSnapshot++;

        return true;
    }
}
