using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTarget : MonoBehaviour
{
    float inputX;
    float inputY;

    Vector3 moveToPosition;
    [Header("Aim Cursor Offset")]
    public float aimOffset = 10f;
    float planarSpeed;
    
    Vector3 velocity = Vector3.zero;
    
    [Header("Smoothing Damp Time")]
    [Range(0,1)]
    public float dampingTime = 0.5f;
    [HideInInspector]
    public bool isMovingInPlane;

    [Header("Public References")]
    public PlayerMovementController thisPlayerMovementController;
    public Transform thisPlayer;
    void Start()
    {
        planarSpeed = thisPlayerMovementController.planarSpeed * 1.7f;
    }

    void Update()
    {
         GetInput();
    }

    void GetInput(){
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        moveToPosition = new Vector3(inputX, inputY, 0f);
        if(moveToPosition.magnitude != 0)
            isMovingInPlane = true;
        else{
            isMovingInPlane = false;
            FixToPlayerAxis();
        } 
        
        moveToPosition *= planarSpeed * Time.deltaTime;

        transform.localPosition += moveToPosition * Time.deltaTime;
        ClampToCameraPlane();
        
    }
    void LateUpdate(){
        Vector3 currentPosition = transform.localPosition;
        transform.localPosition = new Vector3(currentPosition.x, currentPosition.y, thisPlayer.localPosition.z + aimOffset);
    }

    void ClampToCameraPlane(){
        Vector3 playerPositionInPlane = Camera.main.WorldToViewportPoint(transform.position);
        playerPositionInPlane.x = Mathf.Clamp01(playerPositionInPlane.x);
        playerPositionInPlane.y = Mathf.Clamp01(playerPositionInPlane.y);
        transform.position = Camera.main.ViewportToWorldPoint(playerPositionInPlane);
    }

    public void FixToPlayerAxis(){
        Vector3 targetPosition = new Vector3(thisPlayer.localPosition.x, thisPlayer.localPosition.y, transform.localPosition.z);
        Vector3 currentPosition = transform.localPosition;
        transform.localPosition = Vector3.SmoothDamp(currentPosition, targetPosition, ref velocity, dampingTime);
    }
}
