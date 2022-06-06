using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
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

        ResetSwingEndpoints();

        startEndpointRenderer = startSwing.gameObject.GetComponent<MeshRenderer>();
        endEndpointRenderer = endSwing.gameObject.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (state == GameState.START) {
            //
        } else if (state == GameState.WAIT_TRAJ) {
            guidePlayer.PlayGuideTrajectory();
            // if (!TrajectoryTracker.TrackSwing())
            //     changeState();
            
            // choose when to display/hide start/end points
            // add line render to user's swing while recording
        } else if (state == GameState.EVAL_TRAJ) {
            // discretize recording data to match user data length
            // evaluate using metric
            // display score/grade based on metric results
            // reset back to WAIT_TRAJ state via changeState();
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
        startEndpointRenderer.gameObject.SetActive(false);
        endEndpointRenderer.gameObject.SetActive(false);
    }

    public void EnableStartSwing()
    {
        startEndpointRenderer.gameObject.SetActive(true);
    }
    public void EnableEndSwing()
    {
        endEndpointRenderer.gameObject.SetActive(true);
    }
}
