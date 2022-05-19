using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GameController : MonoBehaviour
{
    public GameObject guideHand;
    string trajectoryFilename;

    private TrajectoryPlayer guidePlayer;
    void Start()
    {
        guidePlayer = guideHand.GetComponent<TrajectoryPlayer>();
        StartCoroutine(LoadGuideRecording());
    }

    void Update()
    {
        
    }

    IEnumerator LoadGuideRecording()
    {
        var loadRequest = UnityWebRequest.Get(Path.Combine(Application.streamingAssetsPath, trajectoryFilename));
        yield return loadRequest.SendWebRequest();

        var guide = Instantiate(guideHand, Vector3.zero, Quaternion.identity);
        List<Snapshot> recording = JsonUtility.FromJson<List<Snapshot>>(Encoding.UTF8.GetString(loadRequest.downloadHandler.data));
        guidePlayer.Load(recording);
    }
}
