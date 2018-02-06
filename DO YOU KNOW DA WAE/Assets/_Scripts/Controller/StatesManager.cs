using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StatesManager : MonoBehaviour
{

    // TODO Check if not null or get the ControllerStats in init
    public ControllerStats stats;
    public ControllerStates controllerStates;

    public InputVariables inp;

    [System.Serializable]
    public class InputVariables
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public Vector3 moveDirection;
    }

    [System.Serializable]
    public class ControllerStates
    {
        public bool onGround;
        public bool isAiming;
        public bool IsFliping;
        public bool isRunning; // Disk cant run x)
    }

    // public Animator anim;
    // public GameObject activeModel;
    [HideInInspector] public Rigidbody rigid;
    [HideInInspector] public Collider controllerCollider;

    public Transform tTransform;
    public CharState currentState;

    public LayerMask ignoreLayers;
    public LayerMask ignoreForGround;

    public float delta;

    void Start() {

    }

    public void Init() {
        tTransform = transform;

        // SetupAnimator();
        rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = false;

        rigid.drag = stats.drag;
        rigid.angularDrag = stats.angularDrag;
        // rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        controllerCollider = GetComponent<Collider>();

        ignoreLayers = ~(1 << 9);
        ignoreForGround = ~(1 << 9 | 1 << 10);
    }

    public Vector3 rotateForcePoint;

    public void FixedTick(float p_delta) {
        delta = p_delta;

        currentState = controllerStates.IsFliping ? CharState.rolling : CharState.normal;

        //if (controllerStates.IsFliping) {
        //    ToRollState();
        //}

        switch (currentState) {
            case CharState.normal:
                controllerStates.onGround = OnGround();
                if (controllerStates.isAiming) {

                }
                else {
                    RotationNormal();
                    MovementNormal();
                }

                break;
            case CharState.onAir:
                rigid.drag = 0;
                controllerStates.onGround = OnGround();
                break;
            case CharState.rolling:
                MovementNormal();
                rigid.AddForce(Vector3.down * stats.downForce, ForceMode.Force);

                break;
            default:
                break;
        }

    }

    public void Tick(float p_delta) {
        delta = p_delta;

        switch (currentState) {
            case CharState.normal:
                controllerStates.onGround = OnGround();

                break;
            case CharState.onAir:
                rigid.drag = 0;
                controllerStates.onGround = OnGround();
                break;
            default:
                break;
        }
    }

    void ToRollState() {
        // rigid.AddForce(Vector3.up * stats.jumpTrust, ForceMode.Impulse);
    }

    void MovementNormal() {

        float speed = stats.moveSpeed;
        if (controllerStates.isRunning) {
            speed = stats.boostSpeed;
        }

        Vector3 dir = Vector3.zero;
        dir = inp.moveDirection * (speed * inp.moveAmount);
        // rigid.AddForce(dir, ForceMode.Force);
        rigid.velocity = dir;

    }

    private float accelerationFactor = 0;

    void RotationNormal() {
        rigid.maxAngularVelocity = stats.maxAngularVelocity;
        rigid.angularDrag = stats.angularDrag;

        // To simulate acceleration
        if (Input.GetAxis("HorizontalR") != 0) {
            accelerationFactor += stats.rotationAccelerationSpeed;

            rigid.AddTorque(Vector3.up * Mathf.Lerp(0, stats.torque, accelerationFactor) * Mathf.Sign(Input.GetAxis("HorizontalR")), ForceMode.Force);
            rigid.AddForce(Vector3.up * Mathf.Lerp(0, stats.rotationUpForce, accelerationFactor), ForceMode.Force);
        }
        else {
            accelerationFactor -= stats.rotationAccelerationSpeed;
        }

        accelerationFactor = Mathf.Clamp01(accelerationFactor);
    }



    void MovementAiming() {

    }

    bool OnGround() {
        Vector3 origin = tTransform.position;
        origin.y += 0.6f;
        Vector3 dir = Vector3.down;
        float dis = 0.7f;
        RaycastHit hit;

        if (Physics.Raycast(origin, dir, out hit, dis, ignoreForGround)) {
            Vector3 targetPosition = hit.point;
            // tTransform.position = targetPosition;

            return true;
        }

        return false;
    }

    public enum CharState
    {
        normal, onAir, shield, charging, rolling
    }

    //void SetupAnimator() {
    //    if (activeModel == null) {
    //        anim = GetComponentInChildren<Animator>();
    //        activeModel = anim.gameObject;
    //    }

    //    if (anim == null) {
    //        anim = activeModel.GetComponent<Animator>();
    //    }

    //    anim.applyRootMotion = false;
    //}

    //void HandleAnimationNormal() {
    //    float anim_v = inp.moveAmount;
    //    anim.SetFloat("vertical", anim_v, 0.15f, delta);
    //}
}

