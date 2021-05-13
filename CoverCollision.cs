using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverCollision : MonoBehaviour {
    public string player1Name;
    public string player2Name;
    public string bullet1Name;
    public string bullet2Name;
    public int coverDestructionValue;

    void Start() {
    }

    void Update() {
    }

    void OnCollisionEnter(Collision collider) {
        string colliderName = collider.gameObject.name;
        GameObject colliderObject = GameObject.Find(colliderName);

        if (colliderName == player1Name || colliderName == player2Name) {
            CharacterMovement colliderObjectScript = (CharacterMovement)colliderObject.GetComponent(typeof(CharacterMovement));

            if (colliderObjectScript.positionChange.x > coverDestructionValue || colliderObjectScript.positionChange.x < -coverDestructionValue ||
                    colliderObjectScript.positionChange.z > coverDestructionValue || colliderObjectScript.positionChange.z < -coverDestructionValue) {
                Destroy(gameObject);
            }
        } else if (colliderName == bullet1Name || colliderName == bullet2Name) {
            BulletMovement colliderObjectScript = (BulletMovement)colliderObject.GetComponent(typeof(BulletMovement));

            if (colliderObjectScript.positionChange.x > coverDestructionValue || colliderObjectScript.positionChange.x < -coverDestructionValue ||
                    colliderObjectScript.positionChange.z > coverDestructionValue || colliderObjectScript.positionChange.z < -coverDestructionValue) {
                Destroy(gameObject);
            }
        }
    }
}
