using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class play1Bullet : MonoBehaviour
{
    Vector3 collisionHorizontal = new Vector3(0, 0, 0);
    Vector3 collisionVertical = new Vector3(0, 0, 0);
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        collisionMove();
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Bullet2(Clone)")
        {
            Destroy(gameObject);
        }
        if (col.gameObject.name == "Cover(Clone)")
        {
            int index = CHarController.bulletList1.IndexOf(transform);
            Vector3 colNorm = col.contacts[0].normal;
            Vector3 first = Vector3.Reflect(CHarController.horizontalMovement1 + new Vector3(CHarController.bulletImpulse1[index].x, 0, 0), colNorm);
            Vector3 second = Vector3.Reflect(CHarController.verticalMovement1 + new Vector3(0, 0, CHarController.bulletImpulse1[index].z), colNorm);
            transform.position += .005f * first;
            transform.position += .005f * second;
            Vector3 heading = Vector3.Normalize(first + second);
            transform.forward = heading;
            transform.position += .005f * first;
            transform.position += .005f * second;
            collisionHorizontal = .005f * first;
            collisionVertical = .005f * second;
            if (first.x > 5 || second.z > 5 || first.x < -5 || second.z < -5)
            {
                Destroy(col.gameObject);
            }
        }
    }
    void collisionMove()
    {
        transform.position += collisionVertical;
        transform.position += collisionHorizontal;
    }
}
