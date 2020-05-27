using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;


enum WeaponTypes
{
    laser = 0, missile = 1
}
public class PlayerMovement : MonoBehaviour
{
    private Transform playerModel;
    private Rigidbody rigidbody;
    private float boostMult = 1;

    float h;

    private float fireSpeed = 0.1F;
    private float nextLaserFire = 0;

	private GameObject lockedTarget = null;
	private float targetLockTime = 0;
	private float targetShootDelay = 1f;

    [Header("Settings")]
    public bool joystick = true;
    public Transform leftBlaster;
    public Transform rightBlaster;
    public Transform AimReticle;

    [Space]

    [Header("Parameters")]
    public float xySpeed = 18;
    public float lookSpeed = 340;
    public float forwardSpeed = 6;
    public float maxLeanAngle = 10;

    [Space]

    [Header("Public References")]
    public Transform aimTarget;
    public CinemachineDollyCart dolly;
	public CinemachineVirtualCamera cinemachineVirtualCamera;
    public Transform cameraParent;

    [Space]

    [Header("Particles")]
    public ParticleSystem trail;
    public ParticleSystem circle;
    // public ParticleSystem barrel;
    public ParticleSystem stars;
    
    [Header("Weapons")]
    public GameObject laserPrefab;
    private WeaponTypes activeWeapon = WeaponTypes.laser;
    private float weaponChangeDelay = 1f;
    private float lastWeaponChange = 0;


	[Space]
	public Canvas ui;
	[Space]
    public Vector3 velocity;
    public Vector3 velocityVector;
    private Vector3 prevPos = Vector3.zero;

    private bool barrelRolling = false;

    void Start()
    {
        playerModel = transform.GetChild(0);
        rigidbody = transform.GetComponent<Rigidbody>();
        Physics.IgnoreCollision(laserPrefab.GetComponent<Collider>(),GetComponent<Collider>());
        // SetSpeed(forwardSpeed);
    }

    void FixedUpdate()
    {
        velocity = (transform.position - prevPos);// / Time.fixedDeltaTime;
        prevPos = transform.position;
    }

    private float barrelRollTime = 0;
    void Update()
    {

        float fwdDotProduct = Vector3.Dot(transform.forward, velocity);
        float upDotProduct = Vector3.Dot(transform.up, velocity);
        float rightDotProduct = Vector3.Dot(transform.right, velocity);
        velocityVector = new Vector3(rightDotProduct, upDotProduct, fwdDotProduct);


        h = joystick ? Input.GetAxis("Horizontal") : Input.GetAxis("Mouse X");
        float v = joystick ? Input.GetAxis("Vertical") : Input.GetAxis("Mouse Y");


        if(!DOTween.IsTweening(playerModel) && boostMult != 1){
            boostMult = 1;
        }
        if(h!=0 || v!=0)LocalMove(h, v, xySpeed);
        RotationLook(h,v, lookSpeed);
        HorizontalLean(playerModel, h, maxLeanAngle, .1f);

        // if (Input.GetButtonDown("Action"))
        //     Boost(true);

        // if (Input.GetButtonUp("Action"))
        //     Boost(false);

        // if (Input.GetButtonDown("Fire3"))
        //     Break(true);

        // if (Input.GetButtonUp("Fire3"))
        //     Break(false);

        if (Input.GetButton("SwapWeapon"))
        {
            if (Time.time - lastWeaponChange >= weaponChangeDelay)
            {
				activeWeapon += 1;
				if (Enum.GetNames(typeof(WeaponTypes)).Length == (int)activeWeapon) { activeWeapon = 0; }
				Debug.Log("Activeweapon: " + activeWeapon);
				lastWeaponChange = Time.time;
				ui.GetComponent<UiHandlerScript>().ResetReticle();
				lockedTarget = null;
            }
            
		}

		if (barrelRolling && (Time.time - barrelRollTime >= 0.5f))
        {
            barrelRolling = false;
            barrelRollTime = 0;
        }


		if( Input.GetButton("Fire1")){
			switch (activeWeapon)
			{
				case WeaponTypes.laser:
					if(Time.time > nextLaserFire){
						ShootLaser();
						nextLaserFire = Time.time + fireSpeed;
					}
					break;
				case WeaponTypes.missile:
					if (lockedTarget != null)
					{
						
						ui.GetComponent<UiHandlerScript>().ResetReticle();
						lockedTarget = null;
					}
					break;
				default:
					break;


			}
		}

		if (Input.GetButton("Fire2") && activeWeapon == WeaponTypes.missile)
		{
			Debug.Log("Searching");
			GameObject newTarget = SearchMissileTarget();
			if ((lockedTarget != null && newTarget != null) || lockedTarget == null)
			{
				lockedTarget = newTarget;
			}
		}

		

        // if( Input.GetButton("Fire1") && Time.time > nextLaserFire){
        //     ShootLaser();
        //     nextLaserFire = Time.time + fireSpeed;
        // }

        if (Input.GetButtonDown("Roll"))
        {
            
            int dir = Input.GetButtonDown("Roll") ? -1 : 1;
            BarrelRoll((int)Input.GetAxis("Roll"));
        } else if(Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E)){
            int dir = 0;
            if(Input.GetKeyDown(KeyCode.Q)) dir = -1;
            else if(Input.GetKeyDown(KeyCode.E)) dir = 1;
            BarrelRoll(dir);
        }
        
		if (/*activeWeapon == WeaponTypes.missile && */lockedTarget != null)
		{
			if(ui.GetComponent<UiHandlerScript>().ReticleGroup.alpha == 0) ui.GetComponent<UiHandlerScript>().ReticleGroup.alpha = 1;
			Debug.Log("Target Locked");
			Camera cam = CinemachineCore.Instance.FindPotentialTargetBrain(cinemachineVirtualCamera).OutputCamera;
			ui.GetComponent<UiHandlerScript>().AimReticle(cam.WorldToScreenPoint(lockedTarget.transform.position + lockedTarget.transform.up*3));
			
		}


    }

	GameObject SearchMissileTarget(){
		LayerMask mask = (1<<11);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000f,mask))
        {
            Debug.DrawRay(transform.position, transform.forward*Vector3.Distance(transform.position,hit.transform.position),Color.red,4f);
			return hit.transform.gameObject;
        } else {
			return null;
		}
	}



    void ShootLaser(){

        GameObject laser = Instantiate(laserPrefab);
        float blasterOffset = AimReticle.transform.GetComponent<SpriteRenderer>().bounds.extents.x;
        blasterOffset -= blasterOffset/4;

        laser.transform.position = leftBlaster.transform.position;





        float distanceToReticle = Vector3.Distance(transform.position,AimReticle.position);
        Vector3 aimLocationLeft = leftBlaster.position + leftBlaster.forward * distanceToReticle;
        Vector3 aimLocationRight = rightBlaster.position + rightBlaster.forward * distanceToReticle;
        aimLocationLeft.y = AimReticle.position.y;
        aimLocationRight.y = AimReticle.position.y;


        // Vector3 aimLocationLeft = AimReticle.TransformPoint(-new Vector3(.05f,0,0));
        // Vector3 aimLocationRight = AimReticle.TransformPoint(new Vector3(0.05f,0,0));

        laser.transform.rotation = Quaternion.LookRotation(aimLocationLeft-leftBlaster.position);
        Destroy(laser,3);

        laser = Instantiate(laserPrefab);
        laser.transform.position = rightBlaster.transform.position; 
        laser.transform.rotation = Quaternion.LookRotation(aimLocationRight- rightBlaster.position);
        Destroy(laser,3);


        







    }

    void LocalMove(float x, float y, float speed)
    {
        transform.localPosition += new Vector3(x, y, 0) * speed * Time.deltaTime * boostMult;
        ClampPosition();
    }

    void ClampPosition()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }

    void RotationLook(float h, float v, float speed)
    {
        aimTarget.parent.position = Vector3.zero;
        aimTarget.localPosition = new Vector3(h, v, 1);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(aimTarget.position), Mathf.Deg2Rad * speed * Time.deltaTime);
    }

    void HorizontalLean(Transform target, float axis, float leanLimit, float lerpTime)
    {
        Vector3 targetEulerAngels = target.localEulerAngles;
        target.localEulerAngles = new Vector3(targetEulerAngels.x, targetEulerAngels.y, Mathf.LerpAngle(targetEulerAngels.z, -axis * leanLimit, lerpTime));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(aimTarget.position+transform.position, .5f);
        Gizmos.DrawSphere(aimTarget.position+transform.position, .15f);



        float blasterOffset = AimReticle.transform.GetComponent<SpriteRenderer>().bounds.extents.x;
        blasterOffset -= blasterOffset/4;

        Vector3 aimLocationLeft = AimReticle.TransformPoint(-new Vector3(.05f,0,0))  ;
        Vector3 aimLocationRight = AimReticle.TransformPoint( new Vector3(0.05f,0,0));

        Gizmos.DrawSphere(aimLocationLeft, .2f);
        Gizmos.DrawSphere(aimLocationRight , .2f);

    }

    public bool isBarrelRolling(){
        return barrelRolling;
    }

    
    public void BarrelRoll(int dir)
    {
        if (!DOTween.IsTweening(playerModel))
        {
            barrelRollTime = Time.time;
            barrelRolling = true;
            playerModel.DOLocalRotate(new Vector3(playerModel.localEulerAngles.x, playerModel.localEulerAngles.y, 360 * -dir), .5f, RotateMode.LocalAxisAdd).SetEase(Ease.OutSine);
            float angleBetweenVelocityAndRoll = Vector3.Angle(new Vector3(0,h,0),new Vector3(0,dir,0));
            Debug.Log(angleBetweenVelocityAndRoll);
            if(angleBetweenVelocityAndRoll == 0) boostMult = 1.3f;
            // barrel.Play();
        }
    }

    void SetSpeed(float x)
    {
        dolly.m_Speed = x;
    }

    void OnTriggerEnter(Collider other){
        if(other.name == "Terrain"){
            Debug.Log("Pam");
        }
    }

    // void SetCameraZoom(float zoom, float duration)
    // {
    //     cameraParent.DOLocalMove(new Vector3(0, 0, zoom), duration);
    // }

    // void DistortionAmount(float x)
    // {
    //     Camera.main.GetComponent<PostProcessVolume>().profile.GetSetting<LensDistortion>().intensity.value = x;
    // }

    // void FieldOfView(float fov)
    // {
    //     cameraParent.GetComponentInChildren<CinemachineVirtualCamera>().m_Lens.FieldOfView = fov;
    // }

    // void Chromatic(float x)
    // {
    //     Camera.main.GetComponent<PostProcessVolume>().profile.GetSetting<ChromaticAberration>().intensity.value = x;
    // }


    // void Boost(bool state)
    // {

    //     if (state)
    //     {
    //         cameraParent.GetComponentInChildren<CinemachineImpulseSource>().GenerateImpulse();
    //         trail.Play();
    //         circle.Play();
    //     }
    //     else
    //     {
    //         trail.Stop();
    //         circle.Stop();
    //     }
    //     trail.GetComponent<TrailRenderer>().emitting = state;

    //     float origFov = state ? 40 : 55;
    //     float endFov = state ? 55 : 40;
    //     float origChrom = state ? 0 : 1;
    //     float endChrom = state ? 1 : 0;
    //     float origDistortion = state ? 0 : -30;
    //     float endDistorton = state ? -30 : 0;
    //     float starsVel = state ? -20 : -1;
    //     float speed = state ? forwardSpeed * 2 : forwardSpeed;
    //     float zoom = state ? -7 : 0;

    //     DOVirtual.Float(origChrom, endChrom, .5f, Chromatic);
    //     DOVirtual.Float(origFov, endFov, .5f, FieldOfView);
    //     DOVirtual.Float(origDistortion, endDistorton, .5f, DistortionAmount);
    //     var pvel = stars.velocityOverLifetime;
    //     pvel.z = starsVel;

    //     DOVirtual.Float(dolly.m_Speed, speed, .15f, SetSpeed);
    //     SetCameraZoom(zoom, .4f);
    // }

    // void Break(bool state)
    // {
    //     float speed = state ? forwardSpeed / 3 : forwardSpeed;
    //     float zoom = state ? 3 : 0;

    //     DOVirtual.Float(dolly.m_Speed, speed, .15f, SetSpeed);
    //     SetCameraZoom(zoom, .4f);
    // }
}
