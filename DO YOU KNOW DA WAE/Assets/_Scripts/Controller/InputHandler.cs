using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public StatesManager states;
    private CameraManager camManager;
    private Transform camHolder;

    float horizontal;
    float vertical;
    float altitude; //ADDED THIS

    bool sprintInput;
    bool shootInput;
    bool reloadInput;
    bool switchInput;
    bool pivotInput;

    bool isInit;

    float delta;

    void Start() {
        camManager = CameraManager.singleton;
        InitInGame();
    }

    public void InitInGame() {
        states.Init();
        camManager.Init(transform);

        camHolder = camManager.cameraTransform;

        isInit = true;
    }

    private void FixedUpdate() {
        if (!isInit) return;

        delta = Time.fixedDeltaTime;
        InGame_UpdateStates_FixedUpdate();

        states.FixedTick(delta);
        camManager.FixedTick(delta);

    }

    void Update() {
        if (!isInit) return;

        delta = Time.deltaTime;

        states.Tick(delta);

        GetInput();


        states.controllerStates.IsFliping = Input.GetButton("Jump");
    }

    void GetInput() {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");
        altitude = Input.GetAxis("Ascend"); //ADDED THIS
    }

    void InGame_UpdateStates_FixedUpdate() {
        //Record input values - handled every frame by StatesManager
        states.inp.horizontal = horizontal;
        states.inp.vertical = vertical;
        // states.inp.altitude = altitude; //ADDED THIS
     
        states.inp.moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical) + Mathf.Abs(altitude));


        Vector3 moveDir = camHolder.forward * vertical;
        moveDir.y = 0;
        moveDir += camHolder.right * horizontal;
        moveDir += Vector3.up * altitude; // ADDED THIS
        moveDir.Normalize();

        states.inp.moveDirection = moveDir;

    
    }
}

public enum GamePhase
{
    inGame, inMenu
}
