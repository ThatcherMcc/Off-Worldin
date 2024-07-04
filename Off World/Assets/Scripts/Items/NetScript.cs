using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetScript : MonoBehaviour
{
    Rigidbody rb;
    private ObjectGrabbable objectGrabbable;

    // Start is called before the first frame update
    private void Awake()
    {
        objectGrabbable = GetComponent<ObjectGrabbable>();
    }

    public void Capture()
    {

    }
   
}
