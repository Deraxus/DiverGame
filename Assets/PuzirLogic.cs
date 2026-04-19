using System.Collections.Generic;
using UnityEngine;

public class PuzirLogic : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    public float speed = 1;
    public PuzirSpawner.Direction directionType = PuzirSpawner.Direction.DownUp;
    void Start()
    {
        int randIndex =  Random.Range(0, sprites.Count);
        GetComponent<SpriteRenderer>().sprite = sprites[randIndex];
        speed = Random.Range(0.1f, 1.5f) + speed;
        if (directionType == PuzirSpawner.Direction.DownUp)
        {
            GetComponent<Rigidbody2D>().linearVelocityY = Vector2.up.y * speed;   
        }

        if (directionType == PuzirSpawner.Direction.LeftRight)
        {
            GetComponent<Rigidbody2D>().linearVelocityX = Vector2.right.x * speed;   
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
