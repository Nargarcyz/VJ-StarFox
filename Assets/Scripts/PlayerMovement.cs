using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerMovement : MonoBehaviour
{
    private Transform playerModel;
    private Rigidbody rigidbody;
    private float boostMult = 1;

    float h;

    private float fireSpeed = 0.1F;
    private float nextFire = 0;

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
    public Transform cameraParent;

    [Space]

    [Header("Particles")]
    public ParticleSystem trail;
    public ParticleSystem circle;
    // public ParticleSystem barrel;
    public ParticleSystem stars;
    
    [Header("Weapons")]
    public GameObject laserPrefab;


    public Vector3 velocity;
    public Vector3 velocityVector;
    private Vector3 prevPos = Vector3.zero;

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
        LocalMove(h, v, xySpeed);
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

        if( Input.GetButton("Fire1") && Time.time > nextFire){
            Shoot();
            nextFire = Time.time + fireSpeed;
        }

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
        
        


    }

    void Shoot(){

        GameObject laser = Instantiate(laserPrefab);
        float blasterOffset = AimReticle.transform.GetComponent<SpriteRenderer>().bounds.extents.x;
        blasterOffset -= blasterOffset/4;

        laser.transform.position = leftBlaster.transform.position;

        // Vector3 aimLocationLeft = transform.TransformPoint(AimReticle.localPosition - new Vector3(blasterOffset,0,0)) - leftBlaster.position ;
        // Vector3 aimLocationRight = transform.TransformPoint(AimReticle.localPosition + new Vector3(blasterOffset,0,0)) - rightBlaster.position ;
        Vector3 aimLocationLeft = AimReticle.TransformPoint(-new Vector3(.05f,0,0));
        Vector3 aimLocationRight = AimReticle.TransformPoint(new Vector3(0.05f,0,0));

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

    public void BarrelRoll(int dir)
    {
        if (!DOTween.IsTweening(playerModel))
        {
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
