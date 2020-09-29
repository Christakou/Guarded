using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveSpeed = 7;
    public float smoothMoveTime = .1f;
    public float turnSpeed = 8;
    public static event System.Action OnPlayerBeatLevel;
    public bool detected = false;

    float angle;
    float smoothInputMagnitude;
    float smoothMoveVelocity;


    Vector3 velocity;
    public GameObject endPoint;
    
    Rigidbody rigidbody;
    bool disabled = false;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Guard.OnGuardHasSpottedPlayer += Disable;
        Guard.OnPlayerEnterGuardFOV += StartDetection;
        Guard.OnPlayerLeftGuardFOV += EndDetection;
    }

    void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        if (disabled == false)
        {
            inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        }
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);
        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
    }

    void FixedUpdate()
    {
        rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EndPoint")
        {
            OnPlayerBeatLevel();
        }
    }

    void Disable()
    {
        FindObjectOfType<AudioManager>().Play("SadTrombone");
        disabled = true;
    }

    private void OnDestroy()
    {
        Guard.OnGuardHasSpottedPlayer -= Disable;
        Guard.OnPlayerEnterGuardFOV -= StartDetection;
        Guard.OnPlayerLeftGuardFOV -= EndDetection;
    }

    private void StartDetection()
    {
        if (!detected)
        {
            detected = true;
            Debug.Log("Start detection");
            FindObjectOfType<AudioManager>().Play("Clock");
        }
    }
    private void EndDetection()
    {
        if (detected)
        {
            detected = false;
            Debug.Log("End detection");
            FindObjectOfType<AudioManager>().Stop("Clock");
        }
    }
}
