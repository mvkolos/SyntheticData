using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scene manager class for colliders and target classes 
/// </summary>
public class LidarManager : MonoBehaviour {

    public GameObject scene;
    public List<string> classesIds;

	// Use this for initialization
	void Start () {
        SetUpColliders();
    }

    void SetUpColliders()
    {
        Transform[] allChildren = scene.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.gameObject.GetComponent<MeshRenderer>() != null)
            {
                child.gameObject.AddComponent<MeshCollider>();
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
