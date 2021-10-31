using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class InputSystem : MonoBehaviour
{
    Movement moveScript;

    [System.Serializable]
    public class InputSettings
    {
        public string forwardInput = "Vertical";
        public string strafeInput = "Horizontal";
        public string sprintInput = "Sprint";
        public string aim = "Fire2";
        public string fire = "Fire1";
    }
    [SerializeField] public InputSettings input;
    
    [Header("Camera & Character Syncing")] 
    public float lookDistance = 5;
    public float lookSpeed = 5;

    [Header("Aiming Settings")] 
    RaycastHit hit;
    public LayerMask aimLayers;
    Ray ray;
   
    [Header("Spine Settings")]
    public Transform spine;
    public Vector3 spineOffset;

    [Header("Head Rotation Settings")] public float lookAtPoint = 2.8f;

    [Header("Gravity settings")] 
    public float gravityValue = 1.2f;
    
    Transform camCenter;
    Transform mainCam;

    public Bow bowScrips;
    bool isAiming;

    public bool testAim;
    bool hitDetected;

    Animator playerAnim;
    CharacterController cc;
        
    void Start()
    {
        moveScript = GetComponent<Movement>();
        camCenter = Camera.main.transform.parent;
        mainCam = Camera.main.transform;
        playerAnim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetAxis(input.forwardInput) != 0 || Input.GetAxis(input.strafeInput) != 0)
           RotateToCamView();

        if (!cc.isGrounded)
        {
            cc.Move(new Vector3(transform.position.x, transform.position.y - gravityValue, transform.position.z));
        }
        isAiming = Input.GetButton(input.aim);
        
        if (testAim)  isAiming = true;
                
        moveScript.AnimateCharacter(Input.GetAxis(input.forwardInput), Input.GetAxis(input.strafeInput));
        moveScript.SprintCharacter(Input.GetButton(input.sprintInput));
        moveScript.CharacterAim(isAiming);

        if (isAiming)
        {
           Aim(); 
           bowScrips.EquipBow();
           moveScript.CharacterPullString(Input.GetButton(input.fire));
           if (Input.GetButtonUp(input.fire))
           {
               moveScript.CharacterFireArrow();
               if (hitDetected)
               {
                   bowScrips.Fire(hit.point);
               }
               else
               {
                   bowScrips.Fire(ray.GetPoint(300f));
               }
               moveScript.CharacterPullString(false);
           }
        }
        else
        {
            bowScrips.UnEquipBow();
            bowScrips.RemoveCrosshair();
            DisableArrow();
            Release();
        }
    }

    void LateUpdate()
    {
       if(isAiming) 
           RotateCharacterSpine();
    }

    void RotateToCamView()
    {
        Vector3 camCenterPos = camCenter.position;

        Vector3 lookPoint = camCenterPos + (camCenter.forward * lookDistance);
        Vector3 direction = lookPoint - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        lookRotation.x = 0;
        lookRotation.z = 0;

        Quaternion finalRotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * lookSpeed);
        transform.rotation = finalRotation;
    }

    void Aim()
    {
        Vector3 camPosition = mainCam.position;
        Vector3 dir = mainCam.forward;

        ray = new Ray(camPosition, dir);
        if (Physics.Raycast(ray, out hit, 500f, aimLayers))
        {
            hitDetected = true;
            Debug.DrawLine(ray.origin, hit.point, Color.green);
            bowScrips.ShowCrosshair(hit.point);
        }
        else
        {
            hitDetected = false;
            bowScrips.RemoveCrosshair();
        }
    }

    void RotateCharacterSpine()
    {
        RotateToCamView();
        spine.LookAt(ray.GetPoint(50));
        spine.Rotate(spineOffset);
    }

    public void Pull()
    {
        bowScrips.PullString();
    }

    public void EnableArrow()
    {
        bowScrips.PickArrow();
    }

    public void DisableArrow()
    {
        bowScrips.DisableArrow();
    }

    public void Release()
    {
        bowScrips.ReleaseString();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (isAiming)
        {
            playerAnim.SetLookAtWeight(1f);
            playerAnim.SetLookAtPosition(ray.GetPoint(lookAtPoint));
        }
        else
        {
           playerAnim.SetLookAtWeight(0); 
        }
    }
}