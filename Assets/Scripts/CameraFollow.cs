using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Offset")]
    public Vector3 offset;
    [HideInInspector] public float originalOffset;

    
    [Header("Smoothing Damp Time")]
    [Range(0,1)]
    public float dampingTime = 0.5f;
    [Header("Camera Limits")]
    public Vector3 cameraLimits;
    [Header("Public References")]
    public Transform target;

    Vector3 velocity = Vector3.zero;

    private void Start() {
        originalOffset = offset.z;
    }
    void Update()
    {
        if (!Application.isPlaying)
        {
            transform.localPosition = offset;
        }
        FollowTarget(target);
    }

    void LateUpdate(){
        ClampCamera();
    }


    //follow player object with velocity matching
    void FollowTarget(Transform targetObject){
        Vector3 localPos = transform.localPosition;
        Vector3 targetPos = targetObject.localPosition;

        transform.localPosition = Vector3.SmoothDamp(localPos, new Vector3(targetPos.x + offset.x, targetPos.y + offset.y, targetPos.z + (-offset.z)), ref velocity, dampingTime);
    }

    void ClampCamera(){
        Vector3 localPosition = transform.localPosition;
        transform.localPosition = new Vector3(Mathf.Clamp(localPosition.x, -cameraLimits.x, cameraLimits.x), Mathf.Clamp(localPosition.y, -cameraLimits.y, cameraLimits.y), localPosition.z);
    }

    public void SetOffset(float zoom){
        transform.DOLocalMove(new Vector3(0f, 0f, zoom), 0.4f);
    }
}
