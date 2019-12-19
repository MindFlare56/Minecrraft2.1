using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using Assets.Scripts.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Controllers.Factories.BlockTypesFactory;

[Serializable]
public class PlayerModel : MonoBehaviour 
{

    [SerializeField] private GameObject debugScreen;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject menu;
    [SerializeField] private ToolbarModel toolbar;
    [SerializeField] private Transform destroyCursorBlock;
    [SerializeField] private Transform placeCursorBlock;
    [SerializeField] private KeyCode menuKeyCode = KeyCode.F1;
    [SerializeField] private KeyCode debugScreenKeyCode = KeyCode.F3;
    [SerializeField] private KeyCode inventoryKeyCode = KeyCode.I;
    [SerializeField] private GameObject saveZone;
    [SerializeField] private GameObject loadZone;
    private Transform cam;
    private ChunksController chunksController;
    private GameSettingModel gameSettings;
    private Dictionary<KeyCode, Action> inputActions = new Dictionary<KeyCode, Action>();    
    private Vector3 velocity;    
    private bool isGrounded;
    private bool isSprinting;    
    private float walkSpeed = 3f;
    private float sprintSpeed = 6f;
    private float jumpForce = 5f;
    private float gravity = -9.8f;
    private float width = 0.15f;    
    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;    
    private float verticalMomentum = 0;        
    private bool inUI = false;    

    private void Awake()
    {
        gameSettings = new GameSettingModel();
        Cursor.lockState = CursorLockMode.Locked;
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
        if (!InUI && chunksController.IsReady) {
            CalculateVelocity();
            transform.Rotate(Vector3.up * mouseHorizontal);
            cam.Rotate(Vector3.right * -mouseVertical);
            transform.Translate(velocity, Space.World);
        }        
    }

    private void Update() 
    {
        if (!InUI && chunksController.IsReady) {            
            UpdateAxis();
            RightClick();
            CheckIfPlayerIsSprinting();
            UpdateCursorBlocksPosition();
        }
        LeftClick();
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
        inputActions.Add(inventoryKeyCode, HandleInventory);
        inputActions.Add(menuKeyCode, HandleMenu);        
        inputActions.Add(debugScreenKeyCode, HandleDebugScreen);
        inputActions.Add(KeyCode.F12, Quit);
    }    

    private void LeftClick()
    {
        if (Input.GetMouseButtonDown(0)) {
            if (!InUI) {
                HandleBlockDestruction();
            }
        }
    }    

    private void HandleBlockDestruction()
    {        
            var blockType = chunksController.TryDestroyingBlock(Vector.ToInt(destroyCursorBlock.position));
            if (blockType != BlockTypeKey.Air) {
                AddBlockTopPlayer(blockType);
            }        
    }

    private void RightClick()
    {
        if (Input.GetMouseButtonDown(1)) {
            if (toolbar.Slots[toolbar.slotIndex].HasItem) {
                var blockType = toolbar.Slots[toolbar.slotIndex].itemSlot.stack.blockType;
                chunksController.TryAddingBlock(Vector.FloorToInt(placeCursorBlock.position), blockType);                
                toolbar.Slots[toolbar.slotIndex].itemSlot.Take(1);
            }
        }
    }   

    private void AddBlockTopPlayer(BlockTypeKey blockType)
    {
        //todo handle this logic somewhere else                
        //todo change this logic to support multi stack
        if (toolbar.HasEmptyOrAddableSameItem(blockType)) {
            toolbar.AddBlock(blockType);
        } else {
            throw new NotImplementedException();
            //todo handle on full inventory drop animation
        }
    }

    //todo refactor
    private void UpdateCursorBlocksPosition()
    {        
        float checkIncrement = 0.1f;
        float reach = 8f;
        float step = checkIncrement;
        Vector3 lastPosition = new Vector3();
        while (step < reach) {
            Vector3 position = cam.position + (cam.forward * step);
            if (chunksController.CheckForVoxel(position)) {                
                destroyCursorBlock.position = Vector.FloorToInt(position);
                placeCursorBlock.position = lastPosition;
                destroyCursorBlock.gameObject.SetActive(true);
                placeCursorBlock.gameObject.SetActive(true);
                return;
            }
            lastPosition = Vector.FloorToInt(position);
            step += checkIncrement;
        }
        destroyCursorBlock.gameObject.SetActive(false);
        placeCursorBlock.gameObject.SetActive(false);
    }

    private void SwitchInUI()
    {
        InUI = !InUI;
        if (!Cursor.visible && InUI) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        if (!InUI) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
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
        //todo handle this better
        if (inventory.activeSelf) {
            inventory.SetActive(false);
        }        
        if (Input.GetKeyDown(menuKeyCode)) {
            SwitchInUI();
            menu.SetActive(!menu.activeSelf);            
        }
    }

    private void HandleInventory()
    {
        if (inventory.activeSelf) {
            menu.SetActive(false);
        }
        if (Input.GetKeyDown(inventoryKeyCode)) {
            SwitchInUI();
            //todo
            //inventory.SetActive(!inventory.activeSelf);
        }
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void HandleDebugScreen()
    {
        if (Input.GetKeyDown(debugScreenKeyCode)) {
            debugScreen.SetActive(!debugScreen.activeSelf);
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
