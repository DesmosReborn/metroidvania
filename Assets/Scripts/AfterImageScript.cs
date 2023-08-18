using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterImageScript : MonoBehaviour
{
    private PlayerScript player;
    private SpriteRenderer sprite;
    private SpriteRenderer playerSprite;

    private Color color;
    [SerializeField]
    private float activeTime = 0.1f;
    private float timeActivated;
    private float alpha;
    private float alphaSet = 0.8f;
    [SerializeField]
    private float alphaMultiplier;

    private void OnEnable()
    {
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindObjectOfType<PlayerScript>();
        playerSprite = player.transform.GetChild(0).GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        sprite.sprite = playerSprite.sprite;
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
        timeActivated = Time.time;
    }

    private void Update()
    {
        alpha *= alphaMultiplier;
        color = new Color(1, 1, 1, alpha);
        sprite.color = color;

        if (Time.time > timeActivated + activeTime)
        {
            AfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
