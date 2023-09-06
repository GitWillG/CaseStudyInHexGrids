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
    void Update()
    {

        rotateCamera();

        //  WASD Camera steer
        if (!isPanning || !isTracking)
        {

            if ((Input.GetKey(KeyCode.W)))
            {
                // anchor.transform.position += cameraRight * Time.deltaTime;
                cameraFollow = Vector3.MoveTowards(cameraFollow, forwardAnchor.position, 1);
            }
            if ((Input.GetKey(KeyCode.S)))
            {

                cameraFollow = Vector3.MoveTowards(cameraFollow, backAnchor.position, 1);
            }
            if ((Input.GetKey(KeyCode.A)))
            {
                cameraFollow = Vector3.MoveTowards(cameraFollow, leftAnchor.position, 1);
            }
            // left control is for debug tools
            if (!Input.GetKey(KeyCode.LeftControl) && (Input.GetKey(KeyCode.D)))
            {
                cameraFollow = Vector3.MoveTowards(cameraFollow, rightAnchor.position, 1);
            }

            m_Camera.transform.position = cameraFollow;


            // TODO - Prevent steering when mouse is outside the game screen in unity editor
            #region Edge steering
            // Edge steering

            //Touching an edge of the screen will also pan the camera in that direction
            float edgeSize = 3f;
            if (Input.mousePosition.x < Screen.width && Input.mousePosition.y < Screen.height)
            {
                if (Input.mousePosition.x > Screen.width - edgeSize)
                {
                    cameraFollow.x += moveAmount * Time.deltaTime;
                }
            if (Input.mousePosition.x < edgeSize)
            {
                cameraFollow.x -= moveAmount * Time.deltaTime;
            }
            if (Input.mousePosition.y > Screen.height - edgeSize)
            {
                cameraFollow.z += moveAmount * Time.deltaTime;
            }
            if (Input.mousePosition.y < edgeSize)
            {
                cameraFollow.z -= moveAmount * Time.deltaTime;
            }
            //Once we have moved the camera target to the appropriate position we move the actual camera there

        }
        #endregion


        // Uses camera's Field of view to simulate zoom
        // TODO - Use actual transforms, as FOV distorts the view
        #region Scroll Wheel Zoom
        // Scroll wheel zoom

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
            #endregion
        }


        // PANNING LOGIC
        if (isPanning)
        {
            if (objectToPan == null) return;

            panPosition = new Vector3
            (
                objectToPan.transform.position.x - 5,
                cameraFollow.y,
                objectToPan.transform.position.z - 5
            );
            cameraFollow = Vector3.MoveTowards(cameraFollow, panPosition, Mathf.Infinity);

            if (Vector3.Distance(cameraFollow, panPosition) <= 3)
            {
                StartCoroutine("resetPan", 1 / 4);
            }
        }

        // TRACKING LOGIC
        if (isTracking)
        {

            panPosition = new Vector3
            (
                objectToPan.transform.position.x - 5,
                cameraFollow.y,
                objectToPan.transform.position.z - 5
            );
            cameraFollow = Vector3.MoveTowards(cameraFollow, panPosition, Mathf.Infinity);
        }


    }

    /// <summary>
    /// Updates the object reference for camera to pan. Actual panning logic is in cameraControl's update. This just changes the target reference.
    /// </summary>
    /// <param name="targetObject"></param>
    public void panToObject(GameObject targetObject)
    {
        isPanning = true;
        objectToPan = targetObject;
    }

    public IEnumerator panToObjectWithDelay(GameObject target, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        panToObject(target);
    }

    /// <summary>
    /// Updates the object reference for camera to track. Actual tracking logic is in cameraControl's update. This just changes the target reference.
    /// </summary>
    /// <param name="targetObject"></param>
    public void trackObject(GameObject targetObject)
    {
        isTracking = true;
        objectToPan = targetObject;

    }

    /// <summary>
    /// Nullifies the target reference for camera and stops camera tracking.
    /// </summary>
    public void finishTracking()
    {
        isTracking = false;
        objectToPan = null;

    }

    /// <summary>
    /// Rotates the camera to right (right arrow key) or to left (left arrow key)
    /// </summary>
    void rotateCamera()
    {
        Vector3 rotation = transform.eulerAngles;

        if (Input.GetKey(KeyCode.E))
        {
            rotation.y += rotationSpeed * Time.deltaTime;
            Debug.Log("Right arrow key");
        }
        if (Input.GetKey(KeyCode.Q))
        {
            rotation.y -= rotationSpeed * Time.deltaTime;
            Debug.Log("Left arrow key");
        }
        transform.eulerAngles = rotation;
    }


    /// <summary>
    /// Nullifies the panning target after the time specified.
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    IEnumerator resetPan(float time)
    {
        yield return new WaitForSeconds(time);
        isPanning = false;
        objectToPan = null;

    }
}
