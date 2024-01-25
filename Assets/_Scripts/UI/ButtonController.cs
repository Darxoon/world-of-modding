using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public Button data;
    private SpriteRenderer sprite;
    private BoxCollider2D col;
    private void Start(){
        gameObject.layer = LayerMask.NameToLayer("UI");
        sprite = gameObject.AddComponent<SpriteRenderer>();
        SpriteData thing = null;
        GameManager.imageFiles.TryGetValue(data.up, out thing);
        sprite.color = new Color(data.colorize.r, data.colorize.g, data.colorize.b, data.alpha);
        transform.localPosition = data.pos.ToVector3() + new Vector3(0, 0, -data.depth);
        transform.localScale = new Vector2(data.scaleX, data.scaleY);
        transform.rotation = Quaternion.Euler(0, 0, data.rotation);
        sprite.sortingOrder = (int)data.depth;
        if(thing.sprite2x != null){
            transform.localScale /= 2;
            sprite.sprite = thing.sprite2x;
        } else
            sprite.sprite = thing.sprite;
        col = gameObject.AddComponent<BoxCollider2D>();
    }
    private void OnMouseDown(){
        GameManager.instance.RunEvent(data.onclick);
    }
}
