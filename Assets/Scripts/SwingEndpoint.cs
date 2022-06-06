using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingEndpoint : MonoBehaviour
{
    public bool starting;
    public GameController gameController;

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Collision Enter");
        if (starting)
            gameController.EnableStartSwing();
        else
            gameController.EnableEndSwing();
    }
}
