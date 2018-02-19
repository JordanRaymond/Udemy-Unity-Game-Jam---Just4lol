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
        GetInput_FixedUpdate();
        InGame_UpdateStates_FixedUpdate();

        states.FixedTick(delta);
        camManager.FixedTick(delta);

    }

    void Update() {
        if (!isInit) return;

        delta = Time.deltaTime;

        states.Tick(delta);

        states.controllerStates.IsFliping = Input.GetButton("Jump");
    }

    void GetInput_FixedUpdate() {
        vertical = Input.GetAxis("Vertical");
        horizontal =  Input.GetAxis("Horizontal");

    }

    void InGame_UpdateStates_FixedUpdate() {
        states.inp.horizontal = horizontal;
        states.inp.vertical = vertical;
        states.inp.moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

        Vector3 moveDir = camHolder.forward * vertical;
        moveDir.y = 0;
        moveDir += camHolder.right * horizontal;
        moveDir.Normalize();
        states.inp.moveDirection = moveDir;

    
    }
}

public enum GamePhase
{
    inGame, inMenu
}
