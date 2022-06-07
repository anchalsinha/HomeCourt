using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using Oculus;

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

    public Text mainText;
    public Text scoreText;
    public Text feedbackText;

    public Transform startSwing, endSwing;
    public GameObject tennisBall;

    private TrajectoryPlayer guidePlayer;
    private TrajectoryTracker tracker;

    private MeshRenderer startEndpointRenderer, endEndpointRenderer;

    private bool inEvaluatingState = false;
    private char grade = 'x';

    void Start()
    {
        OVRManager.display.RecenterPose();

        state = GameState.START;
        Debug.Log(state);

        guidePlayer = guideHand.GetComponent<TrajectoryPlayer>();
        tracker = GetComponent<TrajectoryTracker>();
        
        StartCoroutine(LoadGuideRecording());

        startEndpointRenderer = startSwing.gameObject.GetComponent<MeshRenderer>();
        endEndpointRenderer = endSwing.gameObject.GetComponent<MeshRenderer>();
        // ResetSwingEndpoints();

        Instantiate(tennisBall);

    }

    void Update()
    {
        if (state == GameState.START) {
            // placeholder / initialization state
            UpdateText(mainText, TextConstants.START_GAME);
        } else if (state == GameState.WAIT_TRAJ) {
            UpdateText(mainText, TextConstants.AWAITING_SWING);
            UpdateText(feedbackText, GetScoreFeedback());

            guidePlayer.PlayGuideTrajectory();
            if (!tracker.TrackSwing())
                changeState();
        } else if (state == GameState.EVAL_TRAJ) {
            // discretize recording data to match user data length
            // evaluate using metric
            // display score/grade based on metric results
            // reset back to WAIT_TRAJ state via changeState();
            if (!inEvaluatingState)
                StartCoroutine(EvaluateState());
            
        }
    }

    IEnumerator EvaluateState()
    {
        inEvaluatingState = true;
        var trajectory = tracker.GetTrajectoryRecording();
        var guide = guidePlayer.smoothTrajectory;
        yield return trajectory;
        float metric = EvaluateTrajectory(trajectory, guide);
        Debug.Log($"Eval result: {metric}");

        grade = getGrade(metric);

        UpdateText(scoreText, grade.ToString());
        UpdateText(feedbackText, GetScoreFeedback());

        yield return new WaitForSeconds(0.5f);
        Instantiate(tennisBall);

        changeState();
        inEvaluatingState = false;
    }

    void UpdateText(Text t, string s) {
        t.text = s;

        if (t == feedbackText) {
            switch (grade) {
                case 'A': case 'a':
                    t.color = Color.green;
                    break;
                case 'B': case 'b':
                case 'C': case 'c':
                    t.color = Color.yellow;
                    break;
                case 'D': case 'd':
                case 'F': case 'f':
                    t.color = Color.red;
                    break;
                default:
                    t.color = Color.white;
                    break;
            }
        }
    }

    private string GetScoreFeedback() {
        switch (grade) {
            case 'X': case 'x': // no swings performed yet
                return "";
            case 'A': case 'a':
                return TextConstants.SWING_FEEDBACK_A;
            case 'B': case 'b':
                return TextConstants.SWING_FEEDBACK_B;
            case 'C': case 'c':
                return TextConstants.SWING_FEEDBACK_C;
            case 'D': case 'd':
                return TextConstants.SWING_FEEDBACK_D;
            case 'F': case 'f':
                return TextConstants.SWING_FEEDBACK_F;
            default:
                return "";
        }
    }

    private char getGrade(float score) {
        if (score <= ScoreConstants.SCORE_THRESH_A)
            return 'A';
        else if (score > ScoreConstants.SCORE_THRESH_A && score <= ScoreConstants.SCORE_THRESH_B)
            return 'B';
        else if (score > ScoreConstants.SCORE_THRESH_B && score <= ScoreConstants.SCORE_THRESH_C)
            return 'C';
        else if (score > ScoreConstants.SCORE_THRESH_C && score <= ScoreConstants.SCORE_THRESH_D)
            return 'D';
        else
            return 'F';
    }

    void changeState() {
        if      (state == GameState.START)      state = GameState.WAIT_TRAJ;
        else if (state == GameState.WAIT_TRAJ)  state = GameState.EVAL_TRAJ;
        else if (state == GameState.EVAL_TRAJ)  state = GameState.WAIT_TRAJ;

        Debug.Log(state);
    }

    IEnumerator LoadGuideRecording()
    {
        var loadRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, $"{trajectoryFilename}.json"));
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
        
        return 10 * total / n;
    }
}
