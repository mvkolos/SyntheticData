using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class SaveManager : MonoBehaviour
{
    public static void SaveToXYZ(Dictionary<float, List<LinkedList<SphericalPoint>>> data, String filename)
    {
        try
        {
            ///datatable for rows to be added
            List<string[]> dataTable = new List<string[]>();
            /// object list
            /// 
            /// header in csv file
            string[] header = new string[10];
            LidarManager lidarManager = FindObjectOfType<LidarManager>();
            List<string> classesIds = lidarManager.classesIds;

            int pointCount = 0;

            foreach (var coordinatePair in data)
            {
                foreach (LinkedList<SphericalPoint> keyList in coordinatePair.Value)
                {
                    pointCount += keyList.Count;
                }
            }
            
            foreach (var coordinatePair in data)
            {
                float time = coordinatePair.Key;
                foreach (LinkedList<SphericalPoint> keyList in coordinatePair.Value){
                    foreach (SphericalPoint point in keyList)
                    {
                        Vector3 worldCoordinate = point.GetWorldCoordinate();
                        string[] rows = new string[5];
               
                        rows[0] = worldCoordinate.x.ToString();
                        rows[1] = worldCoordinate.z.ToString();
                        rows[2] = worldCoordinate.y.ToString();
                        Color color = point.GetColor();
                        rows[3] = (int.Parse(ColorUtility.ToHtmlStringRGB(color), System.Globalization.NumberStyles.HexNumber)).ToString();
                        rows[4] = point.GetClassId();
                        if (classesIds.Contains(rows[4]) || classesIds.Count == 0 || rows[4] == "n")
                        {
                            dataTable.Add(rows);
                        }
                    }
                }

            }
            print("data " + dataTable.Count);
            ///put each row in data table to an array
            string[][] output = new string[dataTable.Count][];
            
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = dataTable[i];
            }
            print("data " + output.Length);
            ///add delimiter between strings in each row in the data table
            StringBuilder sb = new StringBuilder();

            int length = output.GetLength(0);
            string delimiter = " ";
            for (int r = 0; r < length; r++)
            {

                sb.AppendLine(string.Join(delimiter, output[r]));
            }
            print("saving to " + filename);
            ///write lines to output file
            StreamWriter outputstream = System.IO.File.CreateText(filename);
            /// write separator as the first  line in the file för CSV file to be oppened correctly
            /// 
            print("points " + pointCount);
            outputstream.WriteLine("VERSION .7");
            outputstream.WriteLine("FIELDS x y z rgb class");
            outputstream.WriteLine("SIZE 4 4 4 4 4");
            outputstream.WriteLine("TYPE F F F U S");
            outputstream.WriteLine("COUNT 1 1 1 1 1");
            outputstream.WriteLine(String.Format("WIDTH {0}", pointCount));
            outputstream.WriteLine("HEIGHT 1");
            outputstream.WriteLine("VIEWPOINT 0 0 0 1 0 0 0");
            outputstream.WriteLine(String.Format("POINTS {0}", pointCount));
            outputstream.WriteLine("DATA ascii");

            outputstream.WriteLine(sb);
            outputstream.Close();
    
        }
        catch (IOException e)
        {
            Debug.Log("Access violation, printing file.");
        }
    }
}
