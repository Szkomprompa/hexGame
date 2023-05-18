using UnityEngine;

public class HexUnit : MonoBehaviour
{
    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            location = value;
            transform.localPosition = new Vector3();//value.Position;
        }
    }

    HexCell location;
}
