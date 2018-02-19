using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StatesManager : MonoBehaviour
{
    // TODO : Check if not null or get the ControllerStats in init ( I hate feading a var through the inspector)
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

    private Material extMatRing;

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

        extMatRing = GetExternalRingMaterial();
    }

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
                    Hovering();

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

        ColorLerp();
    }

    // Need a rework so it can work with the wipout controlls
    void ToRollState() {
        // rigid.AddForce(Vector3.up * stats.jumpTrust, ForceMode.Impulse);
    }

    void MovementNormal() {
        rigid.drag = stats.drag;

        // TODO : Acceleration?
        float speed = stats.moveSpeed;
        if (controllerStates.isRunning) {
            speed = stats.boostSpeed;
        }

        Vector3 dir = Vector3.zero;
        dir = inp.moveDirection * (speed * inp.moveAmount);
        rigid.AddForce(dir, ForceMode.Force);
    }

    // TODO : MOVE VAR
    // TODO : If you accel in one direction and try to spin the other direction, the acceleration factor is still at 
    private float accelerationFactor = 0;
    private int sign = 1;
    void RotationNormal() {
        rigid.maxAngularVelocity = stats.maxAngularVelocity;
        rigid.angularDrag = stats.angularDrag;

        // To simulate acceleration
        if (Input.GetButton("FireL") || Input.GetButton("FireR")) {
            sign = Input.GetButton("FireL") ? 1 : -1;

            accelerationFactor += (stats.rotationAccelerationSpeed * sign);
            Debug.Log(accelerationFactor);
            // Rotation
            rigid.AddTorque(Vector3.up * Mathf.Lerp(0, stats.torque, Mathf.Abs(accelerationFactor)) * sign, ForceMode.Force);
            // Upward force
            rigid.AddForce(Vector3.up * Mathf.Lerp(0, stats.rotationUpForce, Mathf.Abs(accelerationFactor)), ForceMode.Force);
        }
        else {
            float deceleration = stats.decelerationFactor * (accelerationFactor >= 0 ? -1 : 1);
            accelerationFactor += stats.rotationAccelerationSpeed * deceleration;
        }

        accelerationFactor = Mathf.Clamp(accelerationFactor, -1, 1);
    }


    public Transform[] raysPositions;

    void Hovering() {
        Vector3 origin = tTransform.position;
        Vector3 dir = Vector3.down;
        RaycastHit hit;

        for (int i = 0; i < raysPositions.Length; i++) {
            Debug.DrawRay(raysPositions[i].position, dir * stats.hoverRayDistance, Color.red);

            if (Physics.Raycast(raysPositions[i].position, dir, out hit, stats.hoverRayDistance, ignoreForGround)) {
                float hoverDistance = hit.distance;

                float hoverForce = ((hoverDistance - stats.restingHeight) / Time.deltaTime) * (-0.2f * (stats.restingHeight - hoverDistance));
                hoverForce -= stats.hoverDamping * rigid.velocity.y;

                rigid.AddForceAtPosition(Vector3.up * hoverForce, raysPositions[i].position, ForceMode.Force);
            }
        }
    }

    private Material GetExternalRingMaterial() {
        Renderer rend = GetComponentInChildren<Renderer>();
        Material[] mats = rend.materials;

        for (int i = 0; i < mats.Length; i++) {
            if (mats[i].name == "Light 2 (Instance)") {
                return mats[i];
            }
        }

        return rend.material;
    }

    public Color color1;
    public Color color2;
    void ColorLerp() {
      extMatRing.SetColor("_EmissionColor", Color.Lerp(color1, color2, Mathf.PingPong(Time.time, 1)));
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
