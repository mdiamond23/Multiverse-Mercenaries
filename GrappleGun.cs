using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: add github source link
public class GrappleGun : MonoBehaviour
{
    [Header("Scripts Ref:")]
    public GrappleRope grappleRope;

    [Header("Layers Settings:")]
    public LayerMask grappleLayers = new LayerMask();

    [Header("Main Camera:")]
    public Camera m_camera;

    [Header("Transform Ref:")]
    public Transform gunHolder;
    public Transform gunPivot;
    public Transform firePoint;

    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    [Header("Rotation:")]
    [SerializeField] private bool rotateOverTime = true;
    [Range(0, 60)] [SerializeField] private float rotationSpeed = 4;

    [Header("Distance:")]
    [SerializeField] private bool hasMaxDistance = false;
    [SerializeField] private float maxDistnace = 20;

    private enum LaunchType
    {
        Transform_Launch,
        Physics_Launch
    }

    [Header("Launching:")]
    [SerializeField] private bool launchToPoint = true;
    [SerializeField] private LaunchType launchType = LaunchType.Physics_Launch;
    [SerializeField] private float launchSpeed = 1;

    [Header("No Launch To Point")]
    [SerializeField] private bool autoConfigureDistance = false;
    [SerializeField] private float targetDistance = 3;
    [SerializeField] private float targetFrequncy = 1;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    // New settings
    public float grappleGunCooldown = 2f;
    public int grappleGunAmmo = 3;
    public int maxAmmo = 3;
    public float lastShootTime = 0f;
    public Image utilityCooldownImage;

    public PlayerController player;

    private void Awake()
    {
        grappleRope.enabled = false;
        m_springJoint2D.enabled = false;
    }

    private void Start(){
        player = GameObject.FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        if(Time.time >= lastShootTime + grappleGunCooldown * player.cooldownModier && grappleGunAmmo < maxAmmo) {
            grappleGunAmmo += 1;
            lastShootTime = Time.time;
            utilityCooldownImage.fillAmount = (float) grappleGunAmmo / maxAmmo;
        }

        if (Input.GetKeyDown(KeyCode.G) && grappleGunAmmo > 0)
        {
            grappleGunAmmo -= 1;
            lastShootTime = Time.time;
            utilityCooldownImage.fillAmount = (float) grappleGunAmmo / maxAmmo;

            SetGrapplePoint();
        }
        else if (Input.GetKey(KeyCode.G))
        {
            if (grappleRope.enabled)
            {
                RotateGun(grapplePoint, false);
            }
            else
            {
                Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
                RotateGun(mousePos, true);
            }

            if (launchToPoint && grappleRope.isGrappling)
            {
                if (launchType == LaunchType.Transform_Launch)
                {
                    Vector2 firePointDistnace = firePoint.position - gunHolder.localPosition;
                    Vector2 targetPos = grapplePoint - firePointDistnace;
                    float distance = Vector2.Distance(targetPos, gunHolder.position);
                    if (distance < .5f) return;
                    gunHolder.position = Vector2.Lerp(gunHolder.position, 
                        targetPos, Time.deltaTime * launchSpeed * 1f / distance);
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.G))
        {
            grappleRope.enabled = false;
            m_springJoint2D.enabled = false;
            m_rigidbody.gravityScale = 5;
        }
        else
        {
            Vector2 mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            RotateGun(mousePos, true);
        }
    }

    void RotateGun(Vector3 lookPoint, bool allowRotationOverTime)
    {
        Vector3 distanceVector = lookPoint - gunPivot.position;

        float angle = Mathf.Atan2(distanceVector.y, distanceVector.x) * Mathf.Rad2Deg;
        if (rotateOverTime && allowRotationOverTime)
        {
            gunPivot.rotation = Quaternion.Lerp(gunPivot.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * rotationSpeed);
        }
        else
        {
            gunPivot.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void SetGrapplePoint()
    {
        Vector2 distanceVector = m_camera.ScreenToWorldPoint(Input.mousePosition) - gunPivot.position;
        RaycastHit2D _hit = Physics2D.Raycast(firePoint.position, distanceVector.normalized,
            Mathf.Infinity, grappleLayers);
        if (_hit)
        {
            if (Vector2.Distance(_hit.point, firePoint.position) <= maxDistnace || !hasMaxDistance)
            {
                grapplePoint = _hit.point;
                grappleDistanceVector = grapplePoint - (Vector2)gunPivot.position;
                grappleRope.enabled = true;
            }
        }
    }

    public void Grapple()
    {
        m_springJoint2D.autoConfigureDistance = false;
        if (!launchToPoint && !autoConfigureDistance)
        {
            m_springJoint2D.distance = targetDistance;
            m_springJoint2D.frequency = targetFrequncy;
        }
        if (!launchToPoint)
        {
            if (autoConfigureDistance)
            {
                m_springJoint2D.autoConfigureDistance = true;
                m_springJoint2D.frequency = 0;
            }

            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.enabled = true;
        }
        else
        {
            switch (launchType)
            {
                case LaunchType.Physics_Launch:
                    m_springJoint2D.connectedAnchor = grapplePoint;

                    Vector2 distanceVector = firePoint.position - gunHolder.position;

                    m_springJoint2D.distance = distanceVector.magnitude;
                    m_springJoint2D.frequency = launchSpeed;
                    m_springJoint2D.enabled = true;
                    break;
                case LaunchType.Transform_Launch:
                    m_rigidbody.gravityScale = 0;
                    m_rigidbody.velocity = Vector2.zero;
                    break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (firePoint != null && hasMaxDistance)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(firePoint.position, maxDistnace);
        }
    }
}
