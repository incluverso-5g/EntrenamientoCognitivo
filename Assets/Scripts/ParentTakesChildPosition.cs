using UnityEngine;

public class ParentTakesChildPosition : MonoBehaviour
{
    public GameObject parentObject;
    public GameObject childObject;

    void Update()
    {
        if (parentObject != null && childObject != null)
        {
            parentObject.transform.position = childObject.transform.position;
        }
    }
}
