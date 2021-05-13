using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player2Controller : MonoBehaviour
{
    float moveSpeed = 1f / 16;
    Vector3 oldDirection = new Vector3(0, 0, 0);
    Vector3 oldHorizontal = new Vector3(0, 0, 0);
    Vector3 oldVertical = new Vector3(0, 0, 0);
    Vector3 horizontalAugment = new Vector3(0, 0, 0);
    Vector3 verticalAugment = new Vector3(0, 0, 0);
    public static Vector3 horizontalMovement1 = new Vector3(0, 0, 0);
    public static Vector3 verticalMovement1 = new Vector3(0, 0, 0);
    Vector3 collisionVertMovement = new Vector3(0, 0, 0);
    Vector3 collisionHorizMovement = new Vector3(0, 0, 0);
    Vector3 vertical, horizontal;
    public Transform Bullet2;
    public Transform Cover;
    public int impulseIndex = 0;
    public static List<Transform> bulletList1 = new List<Transform>();
    public static List<Vector3> bulletImpulse1 = new List<Vector3>();
    public static List<Vector3> colVerMov1 = new List<Vector3>();
    public static List<Vector3> colHorMov1 = new List<Vector3>();
    Vector3 jump = new Vector3(0.0f, 1.5f, 0.0f);
    float jumpForce = 2.0f;
    float augment = 1f / 5;
    bool isGrounded;
    float timeStamp1 = 0;
    float chargeAmount = 1f;
    void Start()
    {
        moveSpeed = 1f / 16;
        oldDirection = new Vector3(0, 0, 0);
        oldHorizontal = new Vector3(0, 0, 0);
        oldVertical = new Vector3(0, 0, 0);
        horizontalAugment = new Vector3(0, 0, 0);
        verticalAugment = new Vector3(0, 0, 0);
        horizontalMovement1 = new Vector3(0, 0, 0);
        verticalMovement1 = new Vector3(0, 0, 0);
        collisionVertMovement = new Vector3(0, 0, 0);
        collisionHorizMovement = new Vector3(0, 0, 0);
        impulseIndex = 0;
        bulletList1 = new List<Transform>();
        bulletImpulse1 = new List<Vector3>();
        colVerMov1 = new List<Vector3>();
        colHorMov1 = new List<Vector3>();
        jump = new Vector3(0.0f, 1.5f, 0.0f);
        jumpForce = 2.0f;
        augment = 1f / 5;
        timeStamp1 = 0;
        chargeAmount = 1f;
        vertical = Camera.main.transform.forward;
        vertical.y = 0;
        vertical = Vector3.Normalize(vertical);
        horizontal = Quaternion.Euler(new Vector3(0, 90, 0)) * vertical;
    }

    void Update()
    {
        Jump();
        Move();
        Fire();
        BulletMove();
        if (transform.position.y < -10)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    void Move()
    {
        Vector3 impulseSumVer = new Vector3(0, 0, 0);
        Vector3 impulseSumHor = new Vector3(0, 0, 0);
        for (; impulseIndex < bulletImpulse1.Count; impulseIndex++)
        {
            impulseSumHor += new Vector3(bulletImpulse1[impulseIndex].x, 0, 0);
            impulseSumVer += new Vector3(0, 0, bulletImpulse1[impulseIndex].z);
        }
        if (isGrounded)
        {
            Vector3 newDirection = new Vector3(Input.GetAxis("joy_1_axis_0"), 0, Input.GetAxis("joy_1_axis_1"));
            Vector3 direction = new Vector3(oldDirection.x + newDirection.x, 0, oldDirection.z + newDirection.z);
            if (isGrounded)
            {
                horizontalAugment = horizontal * moveSpeed * Time.deltaTime * Input.GetAxis("joy_1_axis_0") + oldHorizontal;
                horizontalAugment = Vector3.Scale(horizontalAugment, new Vector3(augment, augment, augment));
                horizontalAugment = Vector3.Scale(horizontalAugment, new Vector3(Mathf.Abs(horizontalAugment.x), Mathf.Abs(horizontalAugment.y), Mathf.Abs(horizontalAugment.z)));
            }
            else
            {
                horizontalAugment = new Vector3(0, 0, 0);
            }
            if (isGrounded)
            {
                verticalAugment = vertical * moveSpeed * Time.deltaTime * Input.GetAxis("joy_1_axis_1") + oldVertical;
                verticalAugment = Vector3.Scale(verticalAugment, new Vector3(augment, augment, augment));
                verticalAugment = Vector3.Scale(verticalAugment, new Vector3(Mathf.Abs(verticalAugment.x), Mathf.Abs(verticalAugment.y), Mathf.Abs(verticalAugment.z)));
            }
            else
            {
                verticalAugment = new Vector3(0, 0, 0);
            }
            horizontalMovement1 = horizontal * moveSpeed * Time.deltaTime * Input.GetAxis("joy_1_axis_0") + oldHorizontal - horizontalAugment + (.001f * (collisionHorizMovement));// - impulseSumHor));
            verticalMovement1 = vertical * moveSpeed * Time.deltaTime * Input.GetAxis("joy_1_axis_1") + oldVertical - verticalAugment + (.001f * (collisionVertMovement));// - impulseSumVer));
            transform.position += horizontalMovement1;
            transform.position += verticalMovement1;
            Vector3 heading = Vector3.Normalize(horizontalMovement1 + verticalMovement1);
            transform.forward = heading;
            transform.position += horizontalMovement1;
            transform.position += verticalMovement1;
            oldDirection = direction;
            oldHorizontal = horizontalMovement1;
            oldVertical = verticalMovement1;
        }
        else
        {
            Vector3 heading = Vector3.Normalize(horizontalMovement1 + verticalMovement1);
            transform.forward = heading;
            transform.position += 3 * (horizontalMovement1); //- impulseSumHor);
            transform.position += 3 * (verticalMovement1); //- impulseSumVer);
        }
    }

    void OnCollisionStay()
    {
        isGrounded = true;
        impulseIndex = 0;
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Terrain")
        {
            isGrounded = false;
            impulseIndex = bulletImpulse1.Count - 1;
        }
    }
    void Jump()
    {
        if (isGrounded && (Input.GetAxis("joy_1_axis_5") >= .1 || Input.GetAxis("joy_1_axis_5") <= -.1))
        {
            GetComponent<Rigidbody>().AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
    void Fire()
    {
        if (Time.time >= timeStamp1)
        {
            if (Input.GetAxis("joy_1_axis_6") >= .1)
            {
                chargeAmount += 0.2f;
                if (chargeAmount > 20f)
                {
                    chargeAmount = 20f;
                }
            }
            else if (chargeAmount > 1f)
            {
                Transform bullet = Instantiate(Bullet2, transform.position, transform.rotation);
                // bullet.GetComponent<Rigidbody>().AddForce(new Vector3(Input.GetAxis("joy_0_axis_2"), 0, Input.GetAxis("joy_0_axis_3")) * 20f, ForceMode.Impulse);
                bulletList1.Add(bullet);
                bulletImpulse1.Add((new Vector3(Input.GetAxis("joy_1_axis_0"), 0, Input.GetAxis("joy_1_axis_1")) * chargeAmount) * 2);
                colVerMov1.Add(new Vector3(0, 0, 0));
                colHorMov1.Add(new Vector3(0, 0, 0));
                timeStamp1 = Time.time + .5f;
                chargeAmount = 1f;
            }
        }
    }
    void BulletMove()
    {
        for (int i = 0; i < bulletList1.Count; i++)
        {
            Transform bullet = bulletList1[i];
            if (bullet != null)
            {
                Vector3 horizontalBullet = horizontalMovement1;
                Vector3 verticalBullet = verticalMovement1;
                bullet.transform.position += horizontalBullet;
                bullet.transform.position += verticalBullet;
                Vector3 heading = Vector3.Normalize(horizontalBullet + verticalBullet + bulletImpulse1[i]);
                bullet.transform.forward = heading;
                colVerMov1[i] = verticalBullet + new Vector3(bulletImpulse1[i].x, 0, 0);
                colHorMov1[i] = horizontalBullet + new Vector3(0, 0, bulletImpulse1[i].z);
                bullet.transform.position += 1.5f * horizontalBullet;//+ new Vector3(bulletImpulse[i].x, 0, 0);
                bullet.transform.position += 1.5f * verticalBullet;//+ new Vector3(0, bulletImpulse[i].y, 0);
                bullet.transform.position = new Vector3(bullet.transform.position.x, .5f, bullet.transform.position.z);
                // bullet.transform.position.Set(bullet.transform.position.x, 0.1f, bullet.transform.position.z);
            }
        }
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Bullet1(Clone)")
        {
            //int index = Player2Controller.bulletList1.IndexOf(col.transform);
            Vector3 heading = Vector3.Normalize(CHarController.horizontalMovement1 + CHarController.verticalMovement1);
            transform.forward = heading;
            collisionHorizMovement += 8 * CHarController.horizontalMovement1;//colHorMov1[index];
            collisionVertMovement += 8 * CHarController.verticalMovement1;//colVerMov1[index];
            Destroy(col.gameObject);
        }
        if (col.gameObject.name == "Bullet2(Clone)")
        {
            /*
            int index = bulletList1.IndexOf(col.transform);
            collisionHorizMovement += colHorMov1[index];
            collisionVertMovement += colVerMov1[index];
            */
        }
        if (col.gameObject.name == "Cover(Clone)")
        {
            Vector3 colNorm = col.contacts[0].normal;
            horizontalMovement1 = Vector3.Reflect(horizontalMovement1, colNorm);
            verticalMovement1 = Vector3.Reflect(verticalMovement1, colNorm);
            transform.position += horizontalMovement1;
            transform.position += verticalMovement1;
            Vector3 heading = Vector3.Normalize(horizontalMovement1 + verticalMovement1);
            transform.forward = heading;
            oldDirection = new Vector3(0, 0, 0);
            transform.position += horizontalMovement1;
            transform.position += verticalMovement1;
            oldHorizontal = horizontalMovement1;
            oldVertical = verticalMovement1;
            if (horizontalMovement1.x > 0.1f || verticalMovement1.z > 0.1f || horizontalMovement1.x < -0.1f || verticalMovement1.z < -0.1f)
            {
                Destroy(col.gameObject);
            }
        }
        if (col.gameObject.name == "Player1")
        {
            transform.position += .5f * CHarController.verticalMovement1;
            transform.position += .5f * CHarController.horizontalMovement1;
            oldVertical = .5f * (oldVertical + CHarController.verticalMovement1);
            oldHorizontal = .5f * (oldHorizontal + CHarController.horizontalMovement1);
        }
    }
}
