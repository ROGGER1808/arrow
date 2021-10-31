using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Bow : MonoBehaviour
{
    
    [System.Serializable]
    public class BowSettings
    {
        [Header("Arrow Settings")] 
        public float arrowCount;
        public Rigidbody arrowPrefab;
        public Transform arrowPos;
        public Transform arrowEquipParent;
        public float arrowForce = 3;
        
        [Header("Bow Equip & UnEquip Settings")]
        public Transform EquipPos;
        public Transform UnEquipPos;

        public Transform UnEquipParent;
        public Transform EquipParent;
        

        [Header("Bow String Settings")] 
        public Transform bowString;
        public Transform stringInitialPos;
        public Transform stringHandPullPos;
        public Transform stringInitialParent;

    }
    [SerializeField] public BowSettings bowSettings;
    
    [Header("Crosshair Settings")] 
    public GameObject crossHairPrefab;
    GameObject currentCrossHair;

    Rigidbody currentArrow;

    private bool canPullString = false;
    private bool canFireArrow = false;
    
    public void PickArrow()
    {
       bowSettings.arrowPos.gameObject.SetActive(true); 
    }

    public void DisableArrow()
    {
        bowSettings.arrowPos.gameObject.SetActive(false);
    }

    public void PullString()
    {
        bowSettings.bowString.transform.position = bowSettings.stringHandPullPos.position;
        bowSettings.bowString.transform.parent = bowSettings.stringHandPullPos;
    }

    public void ReleaseString()
    {
        bowSettings.bowString.transform.position = bowSettings.stringInitialParent.position;
        bowSettings.bowString.transform.parent = bowSettings.stringInitialParent;
    }

    public void EquipBow()
    {
        this.transform.position = bowSettings.EquipPos.position;
        this.transform.rotation = bowSettings.EquipPos.rotation;
        this.transform.parent = bowSettings.EquipParent;
    }
    
    public void UnEquipBow()
    {
        this.transform.position = bowSettings.UnEquipPos.position;
        this.transform.rotation = bowSettings.UnEquipPos.rotation;
        this.transform.parent = bowSettings.UnEquipParent;
    }

    public void ShowCrosshair(Vector3 crosshairPos)
    {
        if (!currentCrossHair)
            currentCrossHair = Instantiate(crossHairPrefab) as GameObject;

        currentCrossHair.transform.position = crosshairPos;
        currentCrossHair.transform.LookAt(Camera.main.transform);
    }

    public void RemoveCrosshair()
    {
        if (currentCrossHair)
            Destroy(currentCrossHair);
    }

    public void Fire(Vector3 hitPoint)
    {
        Vector3 dir = hitPoint - bowSettings.arrowPos.position;
        currentArrow = Instantiate(bowSettings.arrowPrefab, bowSettings.arrowPos.position,
            bowSettings.arrowPos.rotation) as Rigidbody;
        
        currentArrow.AddForce(dir * bowSettings.arrowForce, ForceMode.Force);
    }
}
