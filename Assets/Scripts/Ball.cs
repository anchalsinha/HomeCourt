using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float lifespan = 5.0f;
    public float force;
    public Vector3 direction;

    private bool hit = false;
    private Rigidbody rigidBody;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.collider.name == "Racquet Collider" && !hit)
        {
            hit = true;
            rigidBody.useGravity = true;
            rigidBody.AddForce(direction* force, ForceMode.Impulse);
            StartCoroutine(Kill());
        }
        else if (collisionInfo.collider.name == "TennisBall")
        {
            Destroy(gameObject);
        }
    }


    public IEnumerator Kill()
    {
        yield return new WaitForSeconds(lifespan);
        Destroy(gameObject);
    }

}
