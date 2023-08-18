using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlatformScript : MonoBehaviour
{
    public GameObject structure1;
    public GameObject structure2;

    // Start is called before the first frame update
    void Start()
    {
        int i = Random.Range(0, 2);
        if (i < 1)
        {
            structure1.SetActive(true);
            structure2.SetActive(false);
        } else
        {
            structure1.SetActive(false);
            structure2.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
