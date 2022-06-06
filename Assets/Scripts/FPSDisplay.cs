using UnityEngine;
using System.Collections;
 
public class FPSDisplay : MonoBehaviour {
 
    private int FramesPerSec;
    public float frequency = 1.0f;
    private string fps;
 
 
 
    void Start(){
        StartCoroutine(FPS());
    }
 
    private IEnumerator FPS() {
        for(;;){
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;
           
            // Display it
 
            Debug.Log(string.Format("FPS: {0}" , Mathf.RoundToInt(frameCount / timeSpan)));
        }
    }
}