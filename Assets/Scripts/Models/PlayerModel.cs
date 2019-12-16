using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour 
{

    private bool isGrounded;
    private bool isSprinting;
    private Transform cam;
    private ChunksController chunksController;
    private float walkSpeed = 3f;
    private float sprintSpeed = 6f;
    private float jumpForce = 5f;
    private float gravity = -9.8f;
    private float width = 0.15f;    
    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;    
    private GameSettingModel gameSettings;
    private Dictionary<KeyCode, Action> inputActions = new Dictionary<KeyCode, Action>();
    private bool inUI = false;

    private void Awake()
    {
        gameSettings = new GameSettingModel();
        MakeInputActions();
    }

    private void OnGUI()
    {
        GetPlayerInputs();
    }

    private void Start() 
    {
        cam = GameObject.Find("Main Camera").transform;
        chunksController = GameObject.Find("Chunks").GetComponent<ChunksController>();
    }

    private void FixedUpdate()
    {        
        CalculateVelocity();        
        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.right * -mouseVertical);
        transform.Translate(velocity, Space.World);
    }

    private void Update() 
    {
        if (!InUI) {            
            UpdateAxis();
            ExecuteMouseAction();
            CheckIfPlayerIsSprinting();          
        }
    }

    private void ExecuteMouseAction()
    {        
        LeftClick();
        RightClick();        
    }

    private void CheckIfPlayerIsSprinting()
    {
        //todo generate key cuz it always remove it?
        if (Input.GetButtonDown("Sprint")) {
            isSprinting = true;
        }
        if (Input.GetButtonUp("Sprint")) {
            isSprinting = false;
        }
    }

    private void UpdateAxis()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");
    }

    private void MakeInputActions()
    {
        inputActions.Add(KeyCode.Space, Jump);
        inputActions.Add(KeyCode.I, HandleMenu);
        inputActions.Add(KeyCode.Escape, Quit);
        inputActions.Add(KeyCode.F3, HandleDebugScreen);
    }

    private void LeftClick()
    {
        if (Input.GetMouseButtonDown(0)) {
            //world.DestroyHighlightBlock(highlightBlock);
        }
    }

    private void RightClick()
    {
        if (Input.GetMouseButtonDown(1)) {
            //PlaceItemInToolbar();
        }
    }

    private void Jump()
    {
        if (isGrounded) {
            verticalMomentum = jumpForce;
            isGrounded = false;
        }
    }

    private void HandleMenu()
    {
        if (Input.GetKeyDown(KeyCode.I)) {
            InUI = !InUI;
        }
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void HandleDebugScreen()
    {
        if (Input.GetKeyDown(KeyCode.F3)) {
            //debugScreen.SetActive(!debugScreen.activeSelf);
        }
    }

    private void CalculateVelocity() 
    {
        if (verticalMomentum > gravity) {
            verticalMomentum += Time.fixedDeltaTime * gravity;
        }
        if (isSprinting) {
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        } else {
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;
        }            
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;
        if ((velocity.z > 0 && Front) || (velocity.z < 0 && Back)) {
            velocity.z = 0;
        }
        if ((velocity.x > 0 && Right) || (velocity.x < 0 && Left)) {
            velocity.x = 0;
        }
        if (velocity.y < 0) {
            velocity.y = CheckDownSpeed(velocity.y);
        } else if (velocity.y > 0) {
            velocity.y = CheckUpSpeed(velocity.y);
        }            
    }

    private void GetPlayerInputs()
    {
        Event currentEvent = Event.current;
        if (currentEvent != null) {
            GetKeyboardInputs(currentEvent);
        }
    }

    private void GetKeyboardInputs(Event currentEvent)
    {
        if (currentEvent.isKey) {
            ExecuteKeyCodeAction(currentEvent.keyCode);
        }
    }

    private void ExecuteKeyCodeAction(KeyCode keyCode)
    {
        if (inputActions.ContainsKey(keyCode)) {
            inputActions[keyCode].Invoke();
        }
    }

    private float CheckDownSpeed(float downSpeed)
    {
        var playerWidth = width;
        bool CheckForVexel1 = chunksController.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth);
        bool CheckForVexel2 = chunksController.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth);
        bool CheckForVexel3 = chunksController.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth);
        bool CheckForVexel4 = chunksController.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth);
        if (CheckForVexel1 || CheckForVexel2 || CheckForVexel3 || CheckForVexel4) {
            isGrounded = true;
            return 0;
        } else {
            isGrounded = false;
            return downSpeed;
        }
    }

    private float CheckUpSpeed(float upSpeed)
    {
        var playerWidth = width;
        bool CheckForVexel1 = chunksController.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth);
        bool CheckForVexel2 = chunksController.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z - playerWidth);
        bool CheckForVexel3 = chunksController.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth);
        bool CheckForVexel4 = chunksController.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 2f + upSpeed, transform.position.z + playerWidth);
        return (CheckForVexel1 || CheckForVexel2 || CheckForVexel3 || CheckForVexel4) ? 0 : upSpeed;
    }

    public bool Front {

        get {
            return
                chunksController.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z + width)) ||
                chunksController.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + width));
        }

    }
    public bool Back {

        get {
            return
                chunksController.CheckForVoxel(new Vector3(transform.position.x, transform.position.y, transform.position.z - width)) ||
                chunksController.CheckForVoxel(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - width));
        }

    }
    public bool Left {

        get {
            return
                chunksController.CheckForVoxel(new Vector3(transform.position.x - width, transform.position.y, transform.position.z)) ||
                chunksController.CheckForVoxel(new Vector3(transform.position.x - width, transform.position.y + 1f, transform.position.z));
        }

    }
    public bool Right {

        get {
            return
                chunksController.CheckForVoxel(new Vector3(transform.position.x + width, transform.position.y, transform.position.z)) ||
                chunksController.CheckForVoxel(new Vector3(transform.position.x + width, transform.position.y + 1f, transform.position.z));
        }

    }
    public bool InUI
    {
        get {
            return inUI;
        }
        set {
            inUI = value;
            if (inUI) {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            } else {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
