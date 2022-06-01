using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballObject;
    
    public float spawnInterval;

    void Start()
    {
        StartCoroutine(Spawn());
    }

    public IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            Instantiate(ballObject, transform, false);
        }
    }

    void Update()
    {
        
    }
}
