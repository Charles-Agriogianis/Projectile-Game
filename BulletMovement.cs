using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BulletMovement : MonoBehaviour {
    public string playerName;
    public string otherPlayerName;
    public string otherBulletName;
    public int collisionAugment;
    public int maxBounds;
    public Vector3 positionChange;
    public float timeStamp;
    Vector3 emptyVector;

    void Start() {
        emptyVector = new Vector3(0, 0, 0);
        timeStamp = 0;
        positionChange = emptyVector;
    }

    void Update() {
        if (transform.position.x < -maxBounds || transform.position.x < maxBounds ||
                transform.position.z < -maxBounds || transform.position.z < maxBounds) {
            SceneManager.LoadScene("SampleScene");
        }
    }

    void OnCollisionEnter(Collision collider) {
        string colliderName = collider.gameObject.name;

        if (colliderName == playerName || colliderName == otherPlayerName) {
            Destroy(gameObject);
        } else if (colliderName == otherBulletName) {
            Destroy(gameObject);
        } else {
            GetComponent<Rigidbody>().AddForce(-positionChange * collisionAugment, ForceMode.Impulse);
        }
    }
}
