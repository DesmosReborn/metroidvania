using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaScript : MonoBehaviour
{
    private SpriteRenderer sprite;

    private Color color;
    [SerializeField]
    private float cycleTime = 5;
    private float alpha;
    [SerializeField]
    private float minAlpha = 0.1f;
    [SerializeField]
    private float maxAlpha = 0.3f;
    private float alphaMid;
    private float amplitude;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        color = sprite.color;
        alpha = color.a;
        alphaMid = (maxAlpha + minAlpha) / 2;
        amplitude = maxAlpha - alphaMid;
    }

    // Update is called once per frame
    void Update()
    {
        alpha = alphaMid + amplitude * Mathf.Sin(Time.time * Mathf.PI / cycleTime);
        color.a = alpha;
        sprite.color = color;
    }
}
