using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayerController : MonoBehaviour {
    private Animator animatorController;
    public Transform rotateYTransform;
    public Transform rotateXTransform;
    public float rotateSpeed;
    public float currentRotateX = 0;
    public float MoveSpeed;
    float currentSpeed = 0;

    public Rigidbody rigidBody;

    public JumpSensor JumpSensor;
    public float JumpSpeed;
    public GunManager gunManager;
    public GameObject gun;
    public FiregunManager fireGunManager;
    public GameObject fireGun;
    private bool currentGunState = true;   //記錄目前是哪一把槍, true是gun, false是fire gun

    public GameUIManager uiManager;
    public int hp = 100;
    private AudioSource footMoveSound;

    // Use this for initialization
    void Start()
    {
        footMoveSound = this.GetComponent<AudioSource>();
        animatorController = this.GetComponent<Animator>();

        //init gun state
        if (currentGunState) {
            fireGun.SetActive(false);
        }
    }

    public void Hit(int value) {
        if (hp <= 0)
            return;

        hp -= value;
        uiManager.SetHP(hp);

        if(hp > 0) {
            uiManager.PlayHitAnimation();
        } else {
            uiManager.PlayerDiedAnimation();

            rigidBody.gameObject.GetComponent<Collider>().enabled = false;
            rigidBody.useGravity = false;
            rigidBody.velocity = Vector3.zero;
            this.enabled = false;
            rotateXTransform.transform.DOLocalRotate(new Vector3(-60, 0, 0), 0.5f);
            rotateYTransform.transform.DOLocalMoveY(-1.5f, 0.5f).SetRelative(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;
        if (Input.GetMouseButton(0)) {
            if (currentGunState) {
               gunManager.TryToTriggerGun();
            } else {
               fireGunManager.TryToTriggerGun();
            }
            
        }
        //換槍
        if (Input.GetKeyUp(KeyCode.R)) {
            if (currentGunState) {
                fireGun.SetActive(true);
                gun.SetActive(false);
                currentGunState = false;
            } else {
                gun.SetActive(true);
                fireGun.SetActive(false);
                currentGunState = true;
            }
        }


        //決定鍵盤input的結果
        Vector3 movDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) { movDirection.z += 1; }
        if (Input.GetKey(KeyCode.S)) { movDirection.z -= 1; }
        if (Input.GetKey(KeyCode.D)) { movDirection.x += 1; }
        if (Input.GetKey(KeyCode.A)) { movDirection.x -= 1; }
        movDirection = movDirection.normalized;

        //腳步聲
        if (Input.GetKeyDown(KeyCode.W)) { footMoveSound.Play(); }
        if (Input.GetKeyDown(KeyCode.S)) { footMoveSound.Play(); }
        if (Input.GetKeyDown(KeyCode.D)) { footMoveSound.Play(); }
        if (Input.GetKeyDown(KeyCode.A)) { footMoveSound.Play(); }
        if (Input.GetKeyUp(KeyCode.W)) { footMoveSound.Stop(); }
        if (Input.GetKeyUp(KeyCode.S)) { footMoveSound.Stop(); }
        if (Input.GetKeyUp(KeyCode.D)) { footMoveSound.Stop(); }
        if (Input.GetKeyUp(KeyCode.A)) { footMoveSound.Stop(); }

        //決定要給Animator的動畫參數
        if (movDirection.magnitude == 0 || !JumpSensor.IsCanJump()) {
            currentSpeed = 0;
        } 
        else {
            if (movDirection.z < 0) {
                currentSpeed = -MoveSpeed;
            } 
            else {
                currentSpeed = MoveSpeed;
            }
        }
        animatorController.SetFloat("Speed", currentSpeed);

        //轉換成世界座標的方向
        Vector3 worldSpaceDirection = movDirection.z * rotateYTransform.transform.forward +
                                      movDirection.x * rotateYTransform.transform.right;
        Vector3 velocity = rigidBody.velocity;
        velocity.x = worldSpaceDirection.x * MoveSpeed;
        velocity.z = worldSpaceDirection.z * MoveSpeed;

        if(Input.GetKey(KeyCode.Space) && JumpSensor.IsCanJump()) {
            velocity.y = JumpSpeed;
        }

        rigidBody.velocity = velocity;

        //計算滑鼠
        rotateYTransform.transform.localEulerAngles += new Vector3(0, Input.GetAxis("Horizontal"), 0) * rotateSpeed;
        currentRotateX += Input.GetAxis("Vertical") * rotateSpeed;

        if (currentRotateX > 90) {
            currentRotateX = 90;
        } else if (currentRotateX < -90) {
            currentRotateX = -90;
        }
        rotateXTransform.transform.localEulerAngles = new Vector3(-currentRotateX, 0, 0);

    }
}
