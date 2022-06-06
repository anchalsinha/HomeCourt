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

    private TrajectoryPlayer guidePlayer;
    private TrajectoryTracker tracker;

    void Start()
    {
        state = GameState.START;
        Debug.Log(state);

        guidePlayer = guideHand.GetComponent<TrajectoryPlayer>();
        // tracker = GetComponent<TrajectoryTracker>();
        
        // StartCoroutine(LoadGuideRecording());
        LoadGuideRecording();
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

    void LoadGuideRecording()
    {
        byte[] raw = File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, trajectoryFilename));

        List<Snapshot> recording = JsonConvert.DeserializeObject<List<Snapshot>>(Encoding.UTF8.GetString(raw));
        guidePlayer.Load(recording);

        if (state == GameState.START)
            changeState();
    }
}
