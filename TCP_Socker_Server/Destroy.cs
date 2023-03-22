using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject signManager;
    void Start()
    {

        Destroy(signManager.transform.GetChild(0), 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
