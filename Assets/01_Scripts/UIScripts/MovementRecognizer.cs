using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using PDollarGestureRecognizer;
using System.IO;
using UnityEngine.Events;

public class MovementRecognizer : MonoBehaviour
{
    public XRNode inputSource;
    public InputHelpers.Button inputButton;
    //public VRInputProcessor inputProcessor;

    public float inputThreshold = 0.1f;
    public Transform movementSource;

    public float newPositionThresholdDistance = 0.05f;
    public GameObject debugCubePrefab;
    public bool creationMode = true;
    public string newGestureName;

    public float recognitionThreshold = 0.9f;
    

    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent OnRecognized;

    public GameObject UIObject;

    private List<Gesture> trainingSet = new List<Gesture>();
    private bool isMoving = false, isPressed = false;
    private List<Vector3> positionsList = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        //creation mode only
        //string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");

        //creation mode off
        string[] gestureFiles = Directory.GetFiles(Application.dataPath + "/StreamingAssets/", "*.xml");
        foreach (var item in gestureFiles)
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(item));
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputThreshold);

        Debug.Log(isPressed);

        //start the movement
        if(!isMoving && isPressed)
        {
            StartMovement();
        }
        //ending the movement
        else if (isMoving && !isPressed){
            EndMovement();
        }
        //updating the movement
        else if (isMoving && isPressed)
        {
            UpdateMovement();
        }
    }

    void StartMovement()
    {
        isMoving = true;
        positionsList.Clear();
        positionsList.Add(movementSource.position);

        //shows movement tracking
        if(debugCubePrefab)
            Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), 3);
    }

    void EndMovement()
    {
        isMoving = false;

        //create the gesture from the position list
        Point[] pointArray = new Point[positionsList.Count];

        for (int i = 0; i < positionsList.Count; i++)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(positionsList[i]);
            pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }

        Gesture newGesture = new Gesture(pointArray);

        //add a new gesture to training set
        if (creationMode)
        {
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);

            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        }
        //recognize
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
            
            if(result.Score > recognitionThreshold)
            {
                OnRecognized.Invoke(result.GestureClass);

                if (result.GestureClass.Contains("Swipe"))
                {
                    MatchSwipeGesture(newGesture);
                }
            }
        }
    }

    void UpdateMovement()
    {
        Vector3 lastPosition = positionsList[positionsList.Count - 1];
        if (Vector3.Distance(movementSource.position, lastPosition) > newPositionThresholdDistance)
        {
            positionsList.Add(movementSource.position);

            if (debugCubePrefab)
                Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), 3);
        }
            
    }

    //check the direction of swipe gesture
    void MatchSwipeGesture(Gesture g)
    {
        Point[] gesturePoints = g.Points;
        if (gesturePoints[gesturePoints.Length - 1] == null)
            return;

        // right
        if (gesturePoints[gesturePoints.Length-1].X > gesturePoints[0].X)
        {
            UIObject.GetComponent<UILookAtCamera>().MoveRight();
            Debug.Log("move right");
        }
        else
        {
            UIObject.GetComponent<UILookAtCamera>().MoveLeft();
            Debug.Log("move left");
        }
    }
}
