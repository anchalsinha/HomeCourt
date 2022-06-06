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
    
    public GameObject startSwing, endSwing;

    private float time = 0;
    private float minTime;
    private float maxTime;

    private LineRenderer lineRenderer;
    public float lineWidth;


    public void Start()
    {
        transforms = GetComponentsInChildren<Transform>().Where(t => t.tag == "Trackable").ToList();

        startSwing = GetComponent<GameObject>();
        startSwing.transform.position = snapshots[0].States[Constants.PALM_CENTER_MARKER_ID].Position;
        endSwing = GetComponent<GameObject>();
        endSwing.transform.position = snapshots[snapshots.Count - 1].States[Constants.PALM_CENTER_MARKER_ID].Position;

        // endSwing.setActive(false);
    }

    public void Load(List<Snapshot> recording)
    {
        minTime = recording[0].Time;
        maxTime = recording[recording.Count - 1].Time;
        snapshots = new List<Snapshot>(recording);

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = snapshots.Count;

        Vector3 pos;
        for (int i = 0; i < snapshots.Count; i++)
        {
            pos = snapshots[i].States[Constants.PALM_CENTER_MARKER_ID].Position;
            lineRenderer.SetPosition(i, new Vector3(pos.x, pos.y, pos.z));
        }
    }

    private void Update()
    {
        PlayGuideTrajectory();
    }

    private (Snapshot prev, Snapshot next) GetSnapshots()
    {
        while (true)
        {
            var s = snapshots[currSnapshot];
            if (s.Time >= time)
                break;
            currSnapshot++;
        }

        var prev = snapshots[currSnapshot - 1];
        var next = snapshots[currSnapshot];
        return (prev, next);
    }

    public bool PlayGuideTrajectory() {
        if (snapshots.Count == 0)
            return false;
        
        if (currSnapshot >= snapshots.Count - 1)
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
