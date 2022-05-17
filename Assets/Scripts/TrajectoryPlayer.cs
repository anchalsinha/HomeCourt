using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPlayer : MonoBehaviour
{
    [SerializeField] private float timeScale = 1;
    private List<Snapshot> snapshots;
    private List<Transform> transforms;

    public float TimeScale
    {
        get => timeScale;
        set => timeScale = value;
    }

    public void Start()
    {

    }

    public void Load(List<Snapshot> snapshots, List<Transform> transforms)
    {
        time = 0;
        frame = 1;
        minTime = snapshots[0].Time;
        maxTime = snapshots[snapshots.Count - 1].Time;
        this.snapshots = snapshots;
        this.transforms = transforms;
    }

    private int frame = 1;
    private float time = 0;
    private float minTime;
    private float maxTime;

    private void Update()
    {
        if (snapshots == null || snapshots.Count == 0)
        {
            return;
        }
        
        time = Mathf.Clamp(time + UnityEngine.Time.deltaTime * TimeScale, minTime, maxTime);
                
        var (prev, next) = GetSnapshots();

        var snapshotDelta = (time - prev.Time) / (next.Time - prev.Time);

        for (var i = 0; i < transforms.Count; i++)
        {
            var transform = transforms[i];
            var prevState = prev.States[i];
            var nextState = next.States[i];
            transform.position = Vector3.Lerp(prevState.Position, nextState.Position, snapshotDelta);
            transform.eulerAngles = Quaternion.Lerp(Quaternion.Euler(prevState.Rotation), Quaternion.Euler(nextState.Rotation), snapshotDelta)
                .eulerAngles;
            transform.localScale = Vector3.Lerp(prevState.Scale, nextState.Scale, snapshotDelta);
        }
    }
    private (Snapshot prev, Snapshot next) GetSnapshots()
    {
        while (true)
        {
            if (TimeScale > 0)
            {
                var sample = snapshots[frame];
                if (sample.Time >= time)
                {
                    break;
                }

                frame++;
            }
            else
            {
                var sample = snapshots[frame];
                if (sample.Time <= time)
                {
                    break;
                }

                frame--;
            }
        }

        if (TimeScale > 0)
        {
            var prev = snapshots[frame - 1];
            var next = snapshots[frame];
            return (prev, next);
        }
        else
        {
            var prev = snapshots[frame + 1];
            var next = snapshots[frame];
            return (prev, next);
        }
    }
}
