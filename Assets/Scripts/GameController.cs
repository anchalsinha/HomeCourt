using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public enum GameState {
    START,
    PLAY_GUIDE,
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
        } else if (state == GameState.PLAY_GUIDE) {
            if (!guidePlayer.PlayGuideTrajectory())
                changeState();
        } else if (state == GameState.WAIT_TRAJ) {
            // if (!TrajectoryTracker.TrackSwing())
            //     changeState();
        } else if (state == GameState.EVAL_TRAJ) {
            //
        }
    }

    void changeState() {
        state++;
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
