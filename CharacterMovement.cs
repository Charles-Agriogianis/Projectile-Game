using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterMovement : MonoBehaviour {
    public string otherPlayerName;
    public string otherBulletName;
    public string bulletName;
    public float moveSpeed;
    public Transform bullet;
    public float jumpForce;
    public float jumpY;
    public float chargeAugment;
    public int frictionAugment;
    public int playerNumber;
    public int dashAugment;
    public int collisionAugment;
    public Vector3 positionChange;
    public bool keyboard;
    bool grounded;
    Vector3 jump;
    Vector3 analogAim;
    Vector3 analogMove;
    Vector3 previousPosition;
    Vector3 emptyVector;
    float timeStamp;
    float chargeAmount;
    List<Transform> bulletList = new List<Transform>();
    string horizontalMove;
    string horizontalShoot;
    string verticalMove;
    string verticalShoot;

    void Start() {
        if (keyboard) {
            horizontalMove = "Horizontal";
            verticalMove = "Vertical";
            horizontalShoot = "Horizontal_shoot";
            verticalShoot = "Vertical_shoot";
        } else {
            horizontalMove = "joy_" + playerNumber + "_axis_0";
            verticalMove = "joy_" + playerNumber + "_axis_1";
            horizontalShoot = "joy_" + playerNumber + "_axis_2";
            verticalShoot = "joy_" + playerNumber + "_axis_3";
        } 

        emptyVector = new Vector3(0, 0, 0);
        analogMove = emptyVector;
        analogAim = emptyVector;
        grounded = false;
        jump = new Vector3(0, jumpY, 0);
        timeStamp = 0;
        chargeAmount = 1;
        previousPosition = transform.position;
        positionChange = emptyVector;
    }

    void Update() {
        if (grounded) {
            analogMove = new Vector3(Input.GetAxis(horizontalMove), 0, Input.GetAxis(verticalMove));
            print(analogMove.x);
        }

        analogAim = new Vector3(Input.GetAxis(horizontalShoot), 0, Input.GetAxis(verticalShoot));
        positionChange = previousPosition - transform.position;
        Jump();
        Move();
        Fire();
        Dash();
        BulletMove();
        previousPosition = transform.position;

        if (transform.position.y < -10) {
            SceneManager.LoadScene("SampleScene");
        }
    }

    private void Move() {
        GetComponent<Rigidbody>().AddForce(moveSpeed * analogMove);

        if (grounded) {
            Vector3 friction = new Vector3(positionChange.x * Mathf.Abs(positionChange.x), 0, positionChange.z * Mathf.Abs(positionChange.z)) / frictionAugment;
            GetComponent<Rigidbody>().AddForce(-friction);
        }
    }

    void Jump() {
        if (grounded && (Input.GetAxis("joy_" + playerNumber + "_axis_5") >= .1 || Input.GetAxis("joy_" + playerNumber + "_axis_5") <= -.1)) {
            GetComponent<Rigidbody>().AddForce(jump * jumpForce);
            grounded = false;
        }
    }

    void Fire() {
        if (Time.time >= timeStamp) {
            if (Input.GetAxis("joy_" + playerNumber + "_axis_6") >= .1) {
                chargeAmount += 0.2f;

                if (chargeAmount > 20) {
                    chargeAmount = 20;
                }
            } else if (chargeAmount > 1) {
                Transform newBullet = Instantiate(bullet, transform.position, transform.rotation);
                BulletMovement newBulletScript = (BulletMovement)newBullet.GetComponent(typeof(BulletMovement));
                newBulletScript.timeStamp = Time.deltaTime;
                newBullet.GetComponent<Rigidbody>().AddForce(analogAim * chargeAugment * chargeAmount);
                GetComponent<Rigidbody>().AddForce(-(analogAim * chargeAugment * chargeAmount));
                bulletList.Add(newBullet);
                timeStamp = Time.time + 0.5f;
                chargeAmount = 1;
            }
        }
    }

    void BulletMove() {
        foreach (Transform bullet in bulletList) {
            bullet.GetComponent<Rigidbody>().AddForce(analogMove);
        }
    }

    void Dash() {
        if (Input.GetAxis("joy_" + playerNumber + "_axis_4") >= 0.1) {
            GetComponent<Rigidbody>().AddForce(analogMove * dashAugment);
        }

        foreach (Transform bullet in bulletList) {
            bullet.GetComponent<Rigidbody>().AddForce(analogMove * dashAugment);
        }
    }

    void OnCollisionStay() {
        grounded = true;
    }

    void OnCollisionEnter(Collision collider) {
        string colliderName = collider.gameObject.name;
        Vector3 positionDifference = emptyVector;
        GameObject colliderObject = GameObject.Find(colliderName);

        if (colliderName == bulletName || colliderName == otherBulletName) {
            BulletMovement colliderObjectScript = (BulletMovement)colliderObject.GetComponent(typeof(BulletMovement));

            if (colliderName == bulletName && Time.deltaTime >= (colliderObjectScript.timeStamp + 0.1)) {
            } else {
                positionDifference = colliderObjectScript.positionChange - positionChange;
            }
        } else if (colliderName == otherPlayerName) {
            CharacterMovement colliderObjectScript = (CharacterMovement)colliderObject.GetComponent(typeof(CharacterMovement));
            positionDifference = colliderObjectScript.positionChange - positionChange;
        } else {
            positionDifference = -positionChange;
        }

        GetComponent<Rigidbody>().AddForce(positionDifference * collisionAugment);
    }
}
