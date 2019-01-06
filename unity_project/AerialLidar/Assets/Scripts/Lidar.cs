using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Lidar scanner class
/// </summary>
public class Lidar : MonoBehaviour
{
    private float lastUpdate = 0;

    private List<Laser> lasers;
    private float horizontalAngle = 0;
    private float horizontalDelta = 0.3f;
    private GameObject parent;
    private LidarStorage lidarStorage;
    private bool onRotation = false;
    private float deltaAngle = 1;
    private float currentRotationAngle = 0;
    public float currentDistanceX = 0;
    public float currentDistanceY = 0;
    private float direction = 1f;
    public string sceneName = "lidar_demo";


    private float deltaMeters = 0.25f;
    public float width = 500;
    public float height = 500;
    public int verticalFov = 15;
    public float thetta = 30;
    private int numberOfLasers = 10;
    public float rotationSpeedHz = 1.0f;
    public float rotationAnglePerStep =4f;
    private float rayDistance = 1000f;
    public static event NewPoints OnScanned;
    public delegate void NewPoints(float time, LinkedList<SphericalPoint> data);
    LinkedList<SphericalPoint> hits;

    public string saveDirectory = "";
    public String filename = "scene";
    private float lapTime = 0;

    private bool isPlaying = false;

    private float previousUpdate;

    private float lastLapTime;

    public GameObject lineDrawerPrefab;

    // Use this for initialization
    private void Start()
    {
        lastLapTime = 0;
        hits = new LinkedList<SphericalPoint>();
        parent = transform.parent.gameObject;
        lidarStorage = GetComponent<LidarStorage>();
        if (saveDirectory=="")
        {
            saveDirectory = Application.dataPath;
        }
        InitiateLasers();
    }

    void OnDestroy()
    {
        
    }

    public void UpdateSettings(float speed, int verticalFov, float thetta, int numberOfLasers, float rotationSpeedHz, float rotationAnglePerStep, float rayDistance)
    {
        this.deltaMeters = speed;
        this.numberOfLasers = numberOfLasers;
        this.rotationSpeedHz = rotationSpeedHz;
        this.rotationAnglePerStep = rotationAnglePerStep;
        this.rayDistance = rayDistance;
        this.verticalFov = verticalFov;
        InitiateLasers();
    }

    private void InitiateLasers()
    {
        // Initialize number of lasers, based on user selection.
        if (lasers != null)
        {
            foreach (Laser l in lasers)
            {
                Destroy(l.GetRenderLine().gameObject);
            }
        }

        lasers = new List<Laser>();
        float angle = -verticalFov;
        float delta = (verticalFov*2f)/ (numberOfLasers-1);
 
        for (int i = 0; i < numberOfLasers; i++)
        {
            GameObject lineDrawer = Instantiate(lineDrawerPrefab);
            lineDrawer.transform.parent = gameObject.transform; // Set parent of drawer to this gameObject.
            lasers.Add(new Laser(gameObject, angle, rayDistance, 0, lineDrawer, i));

            angle += delta;
        }

    }

    public void PauseSensor(bool simulationModeOn)
    {
        if (!simulationModeOn)
        {
            isPlaying = simulationModeOn;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            SaveManager.SaveToXYZ(lidarStorage.GetData(), Application.dataPath+"/"+ filename+".pcd");
            print("saved to " + Application.dataPath + "/" + filename + ".pcd");
        }
        // For debugging, shows visible ray in real time.

        foreach (Laser laser in lasers)
        {
            laser.DebugDrawRay();
        }

    }

    

    private void FixedUpdate()
    {
        hits = new LinkedList<SphericalPoint>();

        // Check if number of steps is greater than possible calculations by unity.
        float numberOfStepsNeededInOneLap = (2 * thetta) / Mathf.Abs(rotationAnglePerStep);
        float numberOfStepsPossible = 1 / Time.fixedDeltaTime / 5;
        float precalculateIterations = 1;
        // Check if we need to precalculate steps.
        if (numberOfStepsNeededInOneLap > numberOfStepsPossible)
        {
            precalculateIterations = (int)(numberOfStepsNeededInOneLap / numberOfStepsPossible);
            if (360 % precalculateIterations != 0)
            {
                precalculateIterations += (2 * thetta) % precalculateIterations;
            }
        }

        
        // Check if it is time to step. Example: 2hz = 2 rotations in a second.
        if (Time.fixedTime - lastUpdate > (1 / (numberOfStepsNeededInOneLap) / rotationSpeedHz) * precalculateIterations)
        {
            // Update current execution time.
            lastUpdate = Time.fixedTime;
            parent.transform.position += direction * Vector3.forward * deltaMeters;
            currentDistanceX += deltaMeters;

            if (currentDistanceX >= width)
            {
                onRotation = true;
            }

            if (onRotation)
            {
                parent.transform.Rotate(0, 0, direction * deltaAngle);
                parent.transform.position += Vector3.left * deltaMeters/2f;
                currentDistanceY += deltaMeters / 2f;
                currentRotationAngle += deltaAngle;

                if (currentRotationAngle >= 180)
                {
                    direction = -direction;
                    currentRotationAngle = 0;
                    onRotation = false;
                    currentDistanceX = 0;


                }
            }

            for (int i = 0; i < precalculateIterations; i++)
            {
                // Perform rotation.
                transform.Rotate(0, rotationAnglePerStep, 0);

                horizontalAngle += rotationAnglePerStep; // Keep track of our current rotation.
                if (Mathf.Abs(horizontalAngle) >= thetta)
                {
                    rotationAnglePerStep = -rotationAnglePerStep;
                    lastLapTime = Time.fixedTime;

                }



                if (currentDistanceY >= width)
                {
                    print("Done");
                    SaveManager.SaveToXYZ(lidarStorage.GetData(), Application.dataPath + "/"+filename+".pcd");
                    print("saved to " + Application.dataPath + "/" + filename + ".pcd");
                    Application.Quit();

                }
                // Execute lasers.
                foreach (Laser laser in lasers)
                {
                    RaycastHit hit = laser.ShootRay();
                    float distance = hit.distance;
                    Color color = Color.white;
                    
                    if (distance != 0) // Didn't hit anything, don't add to list.
                    {
                        Renderer renderer = hit.transform.GetComponent<MeshRenderer>();// getting color
                        if (renderer != null)
                        {
                            Texture2D texture = (Texture2D)renderer.material.mainTexture;
                            Vector2 uv = hit.textureCoord;
                            
                            color = renderer.material.color * ((texture == null) ? Color.white : texture.GetPixel((int)uv.x, (int)uv.y));
                        }

                        float verticalAngle = laser.GetVerticalAngle();

                        GameObject hitObject = hit.collider.gameObject;
                        //print("start search :" +hitObject.name);
                        while (!hitObject.name.Contains(sceneName))
                        {
                            hitObject = hitObject.transform.parent.gameObject;
                        }
                        string objectName = hitObject.name;
                        if (objectName.Substring(objectName.Length - 1)=="n")
                        {
                            print("!" + objectName);
                        }
                        //print("after: "+objectName);
                        hits.AddLast(new SphericalPoint(distance, verticalAngle, horizontalAngle, hit.point, color, objectName.Substring(objectName.Length-1)));

                        
                    }
                }
            }


            // Notify listeners that the lidar sensor have scanned points. 
            OnScanned(lastLapTime, hits);

        }
    }
}
