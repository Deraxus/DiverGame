using System.Collections.Generic;
using UnityEngine;

public class PuzirLogic : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    public float speed = 1;
    void Start()
    {
        int randIndex =  Random.Range(0, sprites.Count);
        GetComponent<SpriteRenderer>().sprite = sprites[randIndex];
        speed = Random.Range(0.1f, 1.5f) + speed;
        GetComponent<Rigidbody2D>().linearVelocityY = Vector2.up.y * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
