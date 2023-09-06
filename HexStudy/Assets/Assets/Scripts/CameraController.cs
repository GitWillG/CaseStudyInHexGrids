using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    //main camera
    Camera m_Camera;
    //a point at which the camera will move towards
    private Vector3 cameraFollow;
    //the Field of view of the camera (allows for zooming)
    private float currentFOV;
    private float zoomSpeed = 3f;
    GameObject objectToPan;
    bool isPanning = false;
    bool isTracking = false;

    // Position to which camera will pan to
    Vector3 panPosition;

    // References to camera movement objects
    // They are parented
    // Local position references instead of world positions
    Transform rightAnchor;
    Transform forwardAnchor;
    Transform backAnchor;
    Transform leftAnchor;
    public float moveAmount = 10f;
    public float edgeSize = 5f;


    // Camera rotation speed
    [Range(0, 100)]
    public float rotationSpeed = 60;

    void Start()
    {
        //Get the scenes main camera
        m_Camera = Camera.main;
        //set the follow position to the main cameras position to prevent issues with scene instantiation.
        cameraFollow = m_Camera.transform.position;
        //set the current FOV to the camera's default FOV
        currentFOV = m_Camera.fieldOfView;

        // Fill the variables with references under camera's parents
        rightAnchor = transform.Find("D");
        forwardAnchor = transform.Find("W");
        backAnchor = transform.Find("S");
        leftAnchor = transform.Find("A");

    }



    // Update is called once per frame
    void FixedUpdate()
    {

        RotateCamera();

        GetCameraInputs();

        ZoomCamera();
    }

    /// <summary>
    /// Pans camera along x/z axis based on mouse position or WASD controls
    /// </summary>
    private void GetCameraInputs()
    {
        //Touching an edge of the screen will also pan the camera in that direction
        //Forward
        if ((Input.GetKey(KeyCode.W)) || Input.mousePosition.y > Screen.height - edgeSize)
        {
            cameraFollow = Vector3.MoveTowards(cameraFollow, forwardAnchor.position, 1);
        }
        //Backward
        if ((Input.GetKey(KeyCode.S)) || Input.mousePosition.y < edgeSize)
        {
            cameraFollow = Vector3.MoveTowards(cameraFollow, backAnchor.position, 1);
        }

        //Left
        if ((Input.GetKey(KeyCode.A)) || Input.mousePosition.x < edgeSize)
        {
            cameraFollow = Vector3.MoveTowards(cameraFollow, leftAnchor.position, 1);
        }
        //Right
        if ((Input.GetKey(KeyCode.D)) || Input.mousePosition.x > Screen.width - edgeSize)
        {
            cameraFollow = Vector3.MoveTowards(cameraFollow, rightAnchor.position, 1);
        }
        
    }

    /// <summary>
    /// Controls the FOW to simulate zooming in/out
    /// </summary>
    private void ZoomCamera()
    {
        m_Camera.transform.position = cameraFollow;

        //get the Scroll wheel input
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");
        //Only zoom if you are actually scrolling 
        if (scrollData != 0.0)
        {
            //zoom in the direction scrolled
            currentFOV += Mathf.Sign(scrollData) * zoomSpeed;
        }
        //limit zoom in and outwards
        currentFOV = Mathf.Clamp(currentFOV, 20, 80);
        //move the camera in the clamped limits
        m_Camera.fieldOfView = currentFOV;
    }



    /// <summary>
    /// Rotates the camera to right (right arrow key) or to left (left arrow key)
    /// </summary>
    void RotateCamera()
    {
        Vector3 rotation = transform.eulerAngles;

        if (Input.GetKey(KeyCode.E))
        {
            rotation.y += rotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            rotation.y -= rotationSpeed * Time.deltaTime;
        }
        transform.eulerAngles = rotation;
    }


}
