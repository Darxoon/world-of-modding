using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        sprite = gameObject.AddComponent<SpriteRenderer>();
        sprite.sprite = StaticData.RetrieveImage(data.image);
        sprite.color = new Color(data.colorize.r, data.colorize.g, data.colorize.b, data.alpha);
        gameObject.name = data.image;
        transform.position = data.pos.ToVector2();
        transform.localScale = new Vector2(data.scaleX, data.scaleY);
        transform.rotation = Quaternion.Euler(0, 0, data.rotation);
        sprite.sortingOrder = (int)data.depth;
        //TODO: ADD TILING
    }

    SpriteRenderer sprite;
    //public string image;
    public Scenelayer data;

    // Update is called once per frame
    void Update()
    {
        
    }
}
