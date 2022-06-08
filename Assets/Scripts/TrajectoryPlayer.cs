using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrajectoryPlayer : MonoBehaviour
{
    [SerializeField] private float timeScale = 1;
    public bool autoPlay = false;

    private List<Snapshot> snapshots;
    private int currSnapshot = 0;
    private List<Transform> transforms;

    private float time = 0;
    private float minTime;
    private float maxTime;

    internal List<Vector3> smoothTrajectory;

    private LineRenderer lineRenderer;
    public float lineWidth;

    public GameObject head;
    private float minHeight;
    private float headPos;
    public float heightOffset = 0.10f;

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

        // find min height in guide recording (for camera adjustment)
        minHeight = positions[0].y;
        for (int i = 1; i < positions.Count; i++) {
            if (positions[i].y < minHeight) {
                minHeight = positions[i].y;
            }
        }
    }

    private void Update()
    {
        if (autoPlay)
            PlayGuideTrajectory();
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

        if (currSnapshot == 0) {
            headPos = head.transform.position.y;
        }

        time = Mathf.Clamp(time + Time.deltaTime * timeScale, minTime, maxTime);
                
        var (prev, next) = GetSnapshots();
        var snapshotDelta = (time - prev.Time) / (next.Time - prev.Time);

        for (var i = 0; i < transforms.Count; i++)
        {
            var transform = transforms[i];
            var prevState = prev.States[i];
            var nextState = next.States[i];
            
            // adjust guide height based on user head position
            prevState.Position.y = (prevState.Position.y - minHeight) + (headPos - heightOffset);
            nextState.Position.y = (nextState.Position.y - minHeight) + (headPos - heightOffset);
            
            transform.position = Vector3.Lerp(prevState.Position, nextState.Position, snapshotDelta);
            transform.eulerAngles = Quaternion.Lerp(Quaternion.Euler(prevState.Rotation), Quaternion.Euler(nextState.Rotation), snapshotDelta).eulerAngles;
            transform.localScale = Vector3.Lerp(prevState.Scale, nextState.Scale, snapshotDelta);
        }
        // currSnapshot++;

        return true;
    }
}
