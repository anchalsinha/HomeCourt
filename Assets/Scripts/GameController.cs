using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using Newtonsoft.Json;

public enum GameState {
    START,
    WAIT_TRAJ,
    EVAL_TRAJ,
}

public class GameController : MonoBehaviour
{
    public GameObject guideHand;
    public string trajectoryFilename;
    private GameState state;

    public Transform startSwing, endSwing;

    private TrajectoryPlayer guidePlayer;
    private TrajectoryTracker tracker;

    private MeshRenderer startEndpointRenderer, endEndpointRenderer;

    void Start()
    {
        state = GameState.START;
        Debug.Log(state);

        guidePlayer = guideHand.GetComponent<TrajectoryPlayer>();
        tracker = GetComponent<TrajectoryTracker>();
        
        StartCoroutine(LoadGuideRecording());

        startEndpointRenderer = startSwing.gameObject.GetComponent<MeshRenderer>();
        endEndpointRenderer = endSwing.gameObject.GetComponent<MeshRenderer>();
        // ResetSwingEndpoints();
    }

    void Update()
    {
        if (state == GameState.START) {
            //
        } else if (state == GameState.WAIT_TRAJ) {
            guidePlayer.PlayGuideTrajectory();
            if (!tracker.TrackSwing())
                changeState();
            
            // choose when to display/hide start/end points
            // add line render to user's swing while recording
        } else if (state == GameState.EVAL_TRAJ) {
            // discretize recording data to match user data length
            // evaluate using metric
            // display score/grade based on metric results
            // reset back to WAIT_TRAJ state via changeState();

            var trajectory = tracker.GetTrajectoryRecording();
            var guide = guidePlayer.smoothTrajectory;
            float metric = EvaluateTrajectory(trajectory, guide);
            Debug.Log($"Eval result: {metric}");
            changeState();
        }
    }

    void changeState() {
        if      (state == GameState.START)      state = GameState.WAIT_TRAJ;
        else if (state == GameState.WAIT_TRAJ)  state = GameState.EVAL_TRAJ;
        else if (state == GameState.EVAL_TRAJ)  state = GameState.WAIT_TRAJ;

        Debug.Log(state);
    }

    IEnumerator LoadGuideRecording()
    {
        var loadRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, trajectoryFilename));
        yield return loadRequest.SendWebRequest();

        List<Snapshot> recording = JsonConvert.DeserializeObject<List<Snapshot>>(Encoding.UTF8.GetString(loadRequest.downloadHandler.data));
        guidePlayer.Load(recording);

        startSwing.position = recording[0].States[Constants.PALM_CENTER_MARKER_ID].Position;
        endSwing.position = recording[recording.Count - 1].States[Constants.PALM_CENTER_MARKER_ID].Position;

        if (state == GameState.START)
            changeState();
    }

    public void ResetSwingEndpoints()
    {
        startEndpointRenderer.enabled = false;
        endEndpointRenderer.enabled = false;
    }

    public void EnableStartSwing()
    {
        startEndpointRenderer.enabled = true;
        tracker.SwingStarted();
    }
    public void EnableEndSwing()
    {
        endEndpointRenderer.enabled = true;
        tracker.SwingEnded();
    }

    private float EvaluateTrajectory(List<Vector3> trajectory, List<Vector3> guide)
    {
        float total = 0.0f;
        Assert.IsTrue(trajectory.Count == guide.Count);

        int n = trajectory.Count;
        for (int i = 0; i < n; i++)
        {
            total += (trajectory[i] - guide[i]).sqrMagnitude;
        }
        
        return total / n;
    }
}
