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

    private float time = 0;
    private float minTime;
    private float maxTime;

    internal List<Vector3> smoothTrajectory;

    private LineRenderer lineRenderer;
    public float lineWidth;


    public void Start()
    {
        transforms = GetComponentsInChildren<Transform>().Where(t => t.tag == "Trackable").ToList();
    }

    public void Load(List<Snapshot> recording)
    {
        minTime = recording[0].Time;
        maxTime = recording[recording.Count - 1].Time;
        snapshots = new List<Snapshot>(recording);

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < snapshots.Count; i++)
            positions.Add(snapshots[i].States[Constants.PALM_CENTER_MARKER_ID].Position);
        positions = Utilities.Discretize(positions);
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
        smoothTrajectory = positions;
    }

    private void Update()
    {
        // PlayGuideTrajectory();
    }

    private (Snapshot prev, Snapshot next) GetSnapshots()
    {
        while (currSnapshot < snapshots.Count - 2)
        {
            var s = snapshots[currSnapshot];
            if (s.Time >= time)
                break;
            currSnapshot++;
        }

        var prev = snapshots[currSnapshot];
        var next = snapshots[currSnapshot + 1];
        return (prev, next);
    }

    public bool PlayGuideTrajectory() {
        if (snapshots.Count == 0)
            return false;
        
        if (currSnapshot >= snapshots.Count - 2)
        {
            currSnapshot = 0;
            time = 0;
        }

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
        // currSnapshot++;

        return true;
    }
}
