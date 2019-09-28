using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{


    float inputX;
    float inputY;

    Vector3 moveToPosition;
    [Header("Control Variables")]
    public float planarSpeed = 5f;
    public float forwardSpeed = 10f;
    public float leanAngleLimit = 65;
    public float leanLerpTime = 1;

    float originalSpeed;
    
    [Header("Control Variables")]
    public Transform cursorTarget;
    public AimTarget aimTargetScript;
    public Transform playerModel;
    public CinemachineDollyCart dollyCart;
    public Transform cameraLocation;

    [Header("Zoom and Break variables")]
    public float zoomOut = 10f;
    public float zoomIn = 10f;

   
    

    //states
    bool boosted;
    bool breakApplied;
    void Start()
    {
        SetSpeedOfDolly();
        originalSpeed = forwardSpeed;
        boosted = false;
        breakApplied = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        LookAtCursor();
        RotateHorizontal();

        if(Input.GetButtonDown("Fire1")){
            BarrelSpin(-1);
        }
        else if(Input.GetButtonDown("Fire2"))
        {
            BarrelSpin(1);
        }

        if(!boosted && !breakApplied && Input.GetAxis("Boost") != 0f){
            BoostSpeed();
        }
        else if(boosted && !breakApplied && Input.GetAxis("Boost") == 0f){
            boosted = false;
            returnToDefault();
        }

        if(!breakApplied && !boosted && Input.GetAxis("Break") != 0f){
            BreakApplied();
        }
        else if(breakApplied && !boosted && Input.GetAxis("Break") == 0f){
            breakApplied = false;
            returnToDefault();
        }
    }
    

    void GetInput(){
        inputX = Input.GetAxis("Horizontal");
        inputY = Input.GetAxis("Vertical");

        moveToPosition = new Vector3(inputX, inputY, 0f);
        moveToPosition *= planarSpeed * Time.deltaTime;

        transform.localPosition += moveToPosition * Time.deltaTime;
       
        ClampToCameraPlane();
        
    }

    void ClampToCameraPlane(){
        Vector3 playerPositionInPlane = Camera.main.WorldToViewportPoint(transform.position);
        playerPositionInPlane.x = Mathf.Clamp01(playerPositionInPlane.x);
        playerPositionInPlane.y = Mathf.Clamp01(playerPositionInPlane.y);
        transform.position = Camera.main.ViewportToWorldPoint(playerPositionInPlane);
    }

    void LookAtCursor(){
            transform.LookAt(cursorTarget);
    }

    void RotateHorizontal(){
        Vector3 currentLocalAngles = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(currentLocalAngles.x, currentLocalAngles.y, Mathf.LerpAngle(currentLocalAngles.z, -inputX *  leanAngleLimit, leanLerpTime));
    }

    void BarrelSpin(int direction){
        if(!DOTween.IsTweening(playerModel))
            playerModel.DOLocalRotate(new Vector3(playerModel.localEulerAngles.x, playerModel.localEulerAngles.y, 360 * -direction ), .4f, RotateMode.LocalAxisAdd).SetEase(Ease.OutSine);   
    }

    void BoostSpeed(){
        boosted = true;
        forwardSpeed *= 3f;
        SetSpeedOfDolly();
        cameraLocation.DOLocalMove(new Vector3(0f, 0f, -zoomOut), 0.4f);

        cameraLocation.GetComponentInChildren<CinemachineImpulseSource>().GenerateImpulse();
    }

    void BreakApplied(){
        boosted = true;
        forwardSpeed /= 4;
        SetSpeedOfDolly();
        cameraLocation.DOLocalMove(new Vector3(0f, 0f, zoomIn), 0.4f);
    }

    void SetSpeedOfDolly(){
        dollyCart.m_Speed = forwardSpeed;
    }

    void returnToDefault(){
        forwardSpeed = originalSpeed;
        SetSpeedOfDolly();
        cameraLocation.DOLocalMove(new Vector3(0f, 0f, 0f), 0.4f);
    }
}
