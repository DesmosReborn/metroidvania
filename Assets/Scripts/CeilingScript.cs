using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingScript : MonoBehaviour
{
    public GameObject structure1;
    public GameObject structure2;
    public GameObject structure3;
    public GameObject structure4;
    public GameObject structure5;

    // Start is called before the first frame update
    void Start()
    {
        structure1.SetActive(Random.Range(0, 5) < 1);
        structure2.SetActive(Random.Range(0, 10) < 1);
        structure3.SetActive(Random.Range(0, 10) < 1);
        if (structure4 != null)
        {
            structure4.SetActive(Random.Range(0, 5) < 1);
        }
        if (structure5 != null)
        {
            structure5.SetActive(Random.Range(0, 5) < 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
