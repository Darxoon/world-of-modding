using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExitComponent : MonoBehaviour
{
    public Levelexit data;
    private CircleCollider2D col;
    public GameObject capObj;
    public SpriteRenderer capRender;
    public SpriteData closed, open;
    private int count = 0;
    public bool pipeOpen {get; private set;} = false;
    // Start is called before the first frame update
    void Start()
    {
        col = gameObject.AddComponent<CircleCollider2D>();
        col.radius = data.radius;
        col.isTrigger = true;
        gameObject.layer = LayerMask.NameToLayer("Geometry");
    }

    // Update is called once per frame
    void Update()
    {

    }
    void ApplySprite(SpriteData sprite){
        //cardinal sin of assuming things
        if(sprite.sprite2x != null){
                capRender.sprite = sprite.sprite2x;
            } else
                capRender.sprite = sprite.sprite;
    }
    public bool GooballApproached(GameObject gooball){
        if(gooball.GetComponent<Gooball>().data.ball.suckable){
            Destroy(gooball);
            GameManager.instance.collected++;
            return true;
        }
        return false;
    }
    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.layer == LayerMask.NameToLayer("Attached Balls")){
            count++;
            if(count > 0){
                ApplySprite(open);
                pipeOpen = true;
            }
        } else if(col.gameObject.layer == LayerMask.NameToLayer("Detached Balls") && pipeOpen){
            GooballApproached(col.gameObject);
        }
    }
    void OnTriggerExit2D(Collider2D col){
        if(col.gameObject.layer == LayerMask.NameToLayer("Attached Balls")){
            count--;
            if(count < 0){
                ApplySprite(closed);
                pipeOpen = false;
            }
        }
    }
}
