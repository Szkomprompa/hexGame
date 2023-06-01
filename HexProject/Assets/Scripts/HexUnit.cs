using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HexUnit : MonoBehaviour
{
    public static HexUnit unitPrefab;
    List<HexCell> pathToTravel;
    const float travelSpeed = 4f;

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
            value.Unit = this;
            transform.localPosition = value.Position;
        }
    }
    HexCell location;

    public float Orientation
    {
        get
        {
            return orientation;
        }
        set
        {
            orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }
    float orientation;

    public void ValidateLocation()
    {
        transform.localPosition = location.Position;
    }

    public void Die()
    {
        location.Unit = null;
        Destroy(gameObject);
    }

    public bool IsValidDestination(HexCell cell)
    {
        return !(cell.type == HexType.WATER) && !cell.Unit;
    }

    public void Travel(List<HexCell> path)
    {
        Location = path[path.Count - 1];
        pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPath());
    }

    IEnumerator TravelPath()
    {
        //Vector3 a, b, c = pathToTravel[0].Position;
        for (int i = 1; i < pathToTravel.Count; i++)
        {
            Vector3 a = pathToTravel[i - 1].Position;
            Vector3 b = pathToTravel[i].Position;
            //a = c;
            //b = pathToTravel[i - 1].Position;
            //c = (b + pathToTravel[i].Position) * 0.5f;
            for (float t = 0f; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                transform.localPosition = Vector3.Lerp(a, b, t);
                //Vector3 d = Bezier.GetDerivative(a, b, c, t);
                //d.y = 0f;
                //transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }
        }
        orientation = transform.localRotation.eulerAngles.y;
    }

    void OnDrawGizmos()
    {
        if (pathToTravel == null || pathToTravel.Count == 0)
        {
            return;
        }

        for (int i = 1; i < pathToTravel.Count; i++)
        {
            Vector3 a = pathToTravel[i - 1].Position + new Vector3(9.673905f, 8.403134f, 4.592718f);
            Vector3 b = pathToTravel[i].Position + new Vector3(9.673905f, 8.403134f, 4.592718f);
            for (float t = 0f; t < 1f; t += 0.1f)
            {
                Gizmos.DrawSphere(Vector3.Lerp(a, b, t), 2f);
            }
            //Gizmos.DrawSphere(pathToTravel[i].Position + new Vector3(9.673905f, 8.403134f, 4.592718f), 2f);
        }
    }

    void OnEnable()
    {
        if (location)
        {
            transform.localPosition = location.Position;
        }
    }
}
