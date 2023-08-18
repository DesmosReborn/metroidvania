using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScript : MonoBehaviour
{
    public GameObject light1;
    public GameObject light2;
    public GameObject structure1;
    public GameObject structure2;
    public GameObject structure3;

    // Start is called before the first frame update
    void Start()
    {
        light1.SetActive(Random.Range(0, 5) < 1);
        light2.SetActive(Random.Range(0, 5) < 1);
        structure1.SetActive(Random.Range(0, 20) < 1);
        structure2.SetActive(Random.Range(0, 20) < 1);
        structure3.SetActive(Random.Range(0, 20) < 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
