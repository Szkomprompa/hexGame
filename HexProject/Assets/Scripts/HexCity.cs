using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCity : MonoBehaviour
{
    public static HexCity cityPrefab;

    public int owner;

    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            if (location)
            {
                location.Unit = null;
            }
            location = value;
            value.City = this;
            transform.localPosition = value.Position;
        }
    }
    HexCell location;

    public void Die()
    {
        location.City = null;
        Destroy(gameObject);
    }
}
