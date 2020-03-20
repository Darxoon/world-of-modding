using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        sprite = gameObject.AddComponent<SpriteRenderer>();
        Sprite thing = null;
        GameManager.imageFiles.TryGetValue(data.image, out thing);
        sprite.sprite = thing;
        sprite.color = new Color(data.colorize.r, data.colorize.g, data.colorize.b, data.alpha);
        gameObject.name = data.image;
        transform.localPosition = data.pos.ToVector2();
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
#if DEBUG
        if (StaticData.levelLoader.visualdebug)
        {
            Vector3 topRight = new Vector3(sprite.bounds.extents.x + transform.position.x, sprite.bounds.extents.y + transform.position.y, 0);
            Vector3 topLeft = new Vector3(-sprite.bounds.extents.x + transform.position.x, sprite.bounds.extents.y + transform.position.y, 0);
            Vector3 bottomRight = new Vector3(sprite.bounds.extents.x + transform.position.x, -sprite.bounds.extents.y + transform.position.y, 0);
            Vector3 bottomLeft = new Vector3(-sprite.bounds.extents.x + transform.position.x, -sprite.bounds.extents.y + transform.position.y, 0);
            Debug.DrawLine(topRight, topLeft,  Color.red, 0, false); //top
            Debug.DrawLine(topRight, bottomRight, Color.red, 0, false); //right
            Debug.DrawLine(topLeft, bottomLeft, Color.red, 0, false); //left
            Debug.DrawLine(bottomLeft, bottomRight, Color.red, 0, false); //bottom    
        }
#endif
    }
}
