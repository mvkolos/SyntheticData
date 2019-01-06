using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The data structure for the lidar data.
/// </summary>
public class LidarStorage : MonoBehaviour {

    public delegate void Filled();
    public static event Filled HaveData;


	private Dictionary<float, List<LinkedList<SphericalPoint>>> dataStorage;

	public LidarStorage()
	{
		this.dataStorage = new Dictionary<float, List<LinkedList<SphericalPoint>>>();
        Lidar.OnScanned += Save;
	}

    void OnDestroy()
    {
        Lidar.OnScanned -= Save;
    }
    

    /// <summary>
    /// Saves the current collected points on the given timestamp. 
    /// </summary>
    /// <param name="newTime"></param>
    public void Save(float time, LinkedList<SphericalPoint> hits)
	{
        if (hits.Count != 0) {
            if (!dataStorage.ContainsKey(time))
            {
                List<LinkedList<SphericalPoint>> keyList = new List<LinkedList<SphericalPoint>>();
                keyList.Add(hits);
                dataStorage.Add(time, keyList);
            } else
            {
                dataStorage[time].Add(hits);
            }
        }		
	}


    public Dictionary<float, List<LinkedList<SphericalPoint>>> GetData()
    {
        return dataStorage;
    }

    public void SetData(Dictionary<float,List<LinkedList<SphericalPoint>>> data )
    {
        this.dataStorage = data;
        if(HaveData != null && data != null)
        {
            HaveData();
        }
    }
 


}
