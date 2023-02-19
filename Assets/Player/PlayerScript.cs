using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Movement")]

    public WorldScript worldScript;
    public float moveSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public bool readyToJump;
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    public GameObject InventorySystem; 
    public GameObject selectionCube;
    private bool inventoryOpen = false;
    private Camera mainCamera;
    public GameObject blockPref;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        inventoryOpen = false;
        InventorySystem.SetActive(false);
        mainCamera = Camera.main;
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded && !inventoryOpen)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void FixedUpdate() 
    {
        MovePlayer();
    }
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        
        if(!inventoryOpen)
        {
            MyInput();
            SpeedControl();
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            DrawOutline(ray);
            OperationBlocks(ray);
        }
        
        if(grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        InventoryContoller();
    }
    private void MovePlayer()
    {
        if(!inventoryOpen)
        {
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput; 
            if(grounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            else if(!grounded)
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }
    private void SpeedControl()
    {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if(flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
    }
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    private void InventoryContoller()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if (!inventoryOpen)
            {
                InventorySystem.SetActive(true);
                if(!InventorySystem.GetComponentInChildren<InventoryWindow>().isInitialized)
                    InventorySystem.GetComponentInChildren<InventoryWindow>().Open();
                inventoryOpen = true;
            }
            else
            {
                InventorySystem.SetActive(false);
                inventoryOpen = false;
            }
        } 
    }
    private void DrawOutline(Ray ray)
    {
        if(Physics.Raycast(ray, out var hit, 5))
            {
                if(hit.collider.tag == "Chunk")
                {
                    Vector3 blockCenter; 
                    blockCenter = hit.point - hit.normal * 1 / 2;
                    Vector3Int blockWorldPos = Vector3Int.FloorToInt(blockCenter);
                    Vector2Int chunkPos = GetChunkContainingBlock(blockWorldPos);
                    if(worldScript.ChunkDatas.TryGetValue(chunkPos, out ChunkData chunkData))
                    {
                        selectionCube.transform.position = new Vector3(blockWorldPos.x + 0.5f, blockWorldPos.y + 0.5f, blockWorldPos.z + 0.5f);
                    }
                    selectionCube.SetActive(true);
                }
            }
            else
            {
                selectionCube.SetActive(false);
            }
    }
    private void OperationBlocks(Ray ray)
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            bool isDestroying = Input.GetMouseButtonDown(0);
            
            if(Physics.Raycast(ray, out var hit, 5))
            {
                if(hit.collider.tag == "Chunk")
                {
                    Vector3 blockCenter; 
                    if(isDestroying) 
                    {
                        blockCenter = hit.point - hit.normal * 1 / 2;
                    }
                    else
                    {
                        blockCenter = hit.point + hit.normal * 1 / 2;
                    }
                    Vector3Int blockWorldPos = Vector3Int.FloorToInt(blockCenter / 1);
                    
                    Vector2Int chunkPos = GetChunkContainingBlock(blockWorldPos);
                    if(worldScript.ChunkDatas.TryGetValue(chunkPos, out ChunkData chunkData))
                    {
                        Vector3Int chunkOrigin = new Vector3Int(chunkPos.x, 0, chunkPos.y) * ChunkRenderer.ChunkWidth;
                        if(isDestroying)
                        {
                            chunkData.renderer.DestroyBlock(blockWorldPos - chunkOrigin);
                            Instantiate(blockPref, blockCenter, Quaternion.identity);
                        }
                        else
                        {
                            chunkData.renderer.PlaceBlock(blockWorldPos - chunkOrigin);
                        }
                    }
                }
            }
        }    
    }

    private void OnCollisionEnter(Collision collision) 
    {
        if(collision.gameObject.tag == "DroppedItem")
            Destroy(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag == "DroppedItem")
            Destroy(other.gameObject);
    }

    public Vector2Int GetChunkContainingBlock(Vector3Int blockWorldPos)
    {
        return new Vector2Int(blockWorldPos.x / ChunkRenderer.ChunkWidth, blockWorldPos.z / ChunkRenderer.ChunkWidth);
    }
}
