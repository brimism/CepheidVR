using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 10;
    public float maxDistance = .5f;


    [Space(10)]
    public bool tutorial;

    private GameObject cameraToLookAt;
    private Renderer rend;

    private bool follow = true, thumPressed = false;
    private bool moveLeft, moveRight;

    private Vector3 newPos;
    private Quaternion initialRotation;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        cameraToLookAt = GameObject.FindGameObjectWithTag("MainCamera");

    }

    void FixedUpdate()
    {
        // Rotate the camera every frame so it keeps looking at the target
        this.transform.LookAt(cameraToLookAt.transform);

        //gesture recognition to move UI
        if (moveLeft && !follow)
        {
            float distance = Vector3.Distance(newPos, transform.position);

            if (distance > maxDistance)
            {
                Debug.Log("moving left");
                transform.position = Vector3.Lerp(transform.position, newPos, smoothSpeed * Time.deltaTime);
            }
        }
        else if (moveRight && !follow)
        {
            float distance = Vector3.Distance(newPos, transform.position);

            if (distance > maxDistance)
            {
                Debug.Log("moving right");
                transform.position = Vector3.Lerp(transform.position, newPos, smoothSpeed * Time.deltaTime);
            }
        }


        if (follow)
        {
            float distance = Vector3.Distance(target.position, transform.position);
            if (distance > maxDistance)
            {
                Vector3 desiredPosition = target.position;
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
                transform.position = smoothedPosition;
                
            }
            if (transform.position.y < target.position.y)
            {
                Vector3 desiredPosition = new Vector3(transform.position.x, target.position.y + 1, transform.position.z);
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
                transform.position = smoothedPosition;
                
            }
        }


    }

    public void setFollow(bool newFollow){
        follow = newFollow;
    }


    public void MoveLeft()
    {
        //move UI to the left and stop following
        if (follow)
        {
            if (tutorial)
                this.SendMessageUpwards("teachSwipeBack");

            moveLeft = true;
            follow = false;
            newPos = new Vector3(target.position.x - 1.5f, target.position.y + 1f, target.position.z);
        }
        // UI is at the right, so move back and start following again
        else
        {
            if (tutorial)
                this.SendMessageUpwards("showObj");
            moveRight = false;
            follow = true;
        }
    }

    public void MoveRight()
    {
        //move UI to the right and stop following
        if (follow)
        {
            if (tutorial)
                this.SendMessageUpwards("teachSwipeBack");
            moveRight = true;
            follow = false;
            newPos = new Vector3(target.position.x + 1.5f, target.position.y + 1f, target.position.z);
        }
        // UI is at the right, so move back and start following again
        else
        {
            if (tutorial)
                this.SendMessageUpwards("showObj");
            moveLeft = false;
            follow = true;
        }
    }
}
