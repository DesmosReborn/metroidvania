using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerScript player;
    private float worldX;
    private float worldY;

    private Vector3 lastPlayerPosition;

    public float getWorldX()
    {
        return worldX;
    }

    public float getWorldY()
    {
        return worldY;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerScript>();
        lastPlayerPosition = player.transform.position;
        worldX = 0;
        worldY = 0;
    }

    // Update is called once per frame
    void Update()
    {
        worldX += player.transform.position.x - lastPlayerPosition.x;
        worldY += player.transform.position.y - lastPlayerPosition.y;
        lastPlayerPosition = player.transform.position;
    }
}
