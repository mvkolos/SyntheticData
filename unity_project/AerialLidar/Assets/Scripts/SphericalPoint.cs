using System;
using UnityEngine;

/// <summary>
/// A class representing spherical coordinates. These are created by the lidar sensor.
/// Includes spherical coordinates and point metadata for segmentation
/// </summary>
public class SphericalPoint
{
    private Vector3 globalWorldCoordinate; // Useful for some things. The other coordinates are local.
    private float radius;
    private float inclination;
    private float azimuth;
    private Color color;
    private string classId;

    public SphericalPoint(float radius, float inclination, float azimuth, Vector3 globalWorldCoordinate, Color color, string classId)
    {
        this.radius = radius;
        this.inclination = (90 + inclination) * (2 * Mathf.PI / 360);
        this.azimuth = azimuth * (2 * Mathf.PI / 360);
        this.globalWorldCoordinate = globalWorldCoordinate;
        this.color = color;
        this.classId = classId;
    }


    public SphericalPoint(float radius, float inclination, float azimuth)
    {
        this.radius = radius;
        this.inclination = (90 + inclination) * (2 * Mathf.PI / 360);
        this.azimuth = azimuth / (2 * Mathf.PI / 360);
    }

    // Constructor based on cartesian coordinates
    /// <summary>
    /// Initializes a new instance of the <see cref="SphericalPoint"/> class using cartesian coordinates.
    /// </summary>
    /// <param name="coordinates">Coordinates.</param>
    public SphericalPoint(Vector3 coordinates)
    {
        globalWorldCoordinate = coordinates;

    }

    /// <summary>
    /// Converts a spherical coordinate to a cartesian equivalent. 
    /// </summary>
    /// <returns></returns>
    public Vector3 ToCartesian()
    {
        Vector3 cartesian = new Vector3
        {
            z = radius * Mathf.Sin(inclination) * Mathf.Cos(azimuth),
            x = radius * Mathf.Sin(inclination) * Mathf.Sin(azimuth),
            y = radius * Mathf.Cos(inclination)
        };
        return cartesian;
    }

    /// <summary>
    /// Gets the radius.
    /// </summary>
    /// <returns>The radius.</returns>
    public float GetRadius()
    {
        return this.radius;
    }
    /// <summary>
    /// Gets the inclination.
    /// </summary>
    /// <returns>The inclination.</returns>
    public float GetInclination()
    {
        return this.radius;
    }
    /// <summary>
    /// Gets the azimuth.
    /// </summary>
    /// <returns>The azimuth.</returns>
    public float GetAzimuth()
    {
        return this.azimuth;
    }

    public Vector3 GetWorldCoordinate()
    {
        return this.globalWorldCoordinate;
    }

    public Color GetColor()
    {
        return this.color;
    }

    public string GetClassId()
    {
        return this.classId;
    }


    /// <summary>
    /// Clones this instance of the class
    /// </summary>
    /// <returns></returns>
    public SphericalPoint Clone()
    {
        return new SphericalPoint(this.radius, this.inclination, this.azimuth,
            new Vector3(globalWorldCoordinate.x, globalWorldCoordinate.y, globalWorldCoordinate.z), this.color, this.classId);
    }

    /// <summary>
    /// Overriding the equals method to be able to avoid float pooint errors.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        double eps = 0.01;
        SphericalPoint other = (SphericalPoint)obj;
        return (Math.Abs(this.azimuth - other.azimuth) < eps
            && Math.Abs(this.inclination - other.inclination) < eps
            && Math.Abs(this.radius - other.radius) < eps);
    }

    /// <summary>
    /// Override hash code
    /// </summary>
    /// <returns></returns>
    /*public String ToString() {
		return radius.ToString () + ";" + inclination.ToString () + ";" + azimuth.ToString () + ":::";
	}*/

    public override int GetHashCode()
    {
        return (int)Math.Floor(azimuth * 3 + inclination * 13 + radius * 11);
    }


    public String ToString()
    {
        return "Radius: " + radius.ToString() + " Inclination: " + inclination.ToString() + " Azimuth: " + azimuth.ToString() +
        " World coordinates: " + globalWorldCoordinate.ToString() + " Color: " + color.ToString() + " ::: ";
    }
}