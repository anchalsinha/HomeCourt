using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class GameController : MonoBehaviour
{
    public GameObject guideHand;
    public string trajectoryFilename;

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

        List<Snapshot> recording = JsonConvert.DeserializeObject<List<Snapshot>>(Encoding.UTF8.GetString(loadRequest.downloadHandler.data));
        guidePlayer.Load(recording);
    }
}
