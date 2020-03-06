using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pipeParts : MonoBehaviour
{
    public GameObject last;
    string type;
    public int order = 1;
    

    // Start is called before the first frame update
    void Start()
    {
        //transform.localScale = transform.localScale * 0.5f;
        if (order != 0)
        {
            if (type == null || type == "")
            {
                GameObject horiz = new GameObject();
                //Texture2D _cap = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipeCap.png");
                Texture2D _horizontal = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipe_horiz.png");
                //Texture2D _cap_closed = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipeCap_closed.png");
                Texture2D _pipe_bend_bl = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipe_bend_bl.png");
                Texture2D _pipe_bend_tl = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipe_bend_tl.png");
                Texture2D _pipe_bend_br = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipe_bend_br.png");
                Texture2D _pipe_bend_tr = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipe_bend_tr.png");

                Sprite horizontal = Sprite.Create(_horizontal, new Rect(0, 0, _horizontal.width, _horizontal.height), new Vector2(0.5f, 0.5f));
                //Sprite cap = Sprite.Create(_cap, new Rect(0, 0, _cap.width, _cap.height), new Vector2(0.5f, 0.5f));
                //Sprite cap_closed = Sprite.Create(_cap_closed, new Rect(0, 0, _cap_closed.width, _cap_closed.height), new Vector2(0.5f, 0.5f));
                Sprite pipe_bend_bl = Sprite.Create(_pipe_bend_bl, new Rect(0, 0, _pipe_bend_bl.width, _pipe_bend_bl.height), new Vector2(0.5f, 0.5f));
                Sprite pipe_bend_tl = Sprite.Create(_pipe_bend_tl, new Rect(0, 0, _pipe_bend_tl.width, _pipe_bend_tl.height), new Vector2(0.5f, 0.5f));
                Sprite pipe_bend_br = Sprite.Create(_pipe_bend_br, new Rect(0, 0, _pipe_bend_br.width, _pipe_bend_br.height), new Vector2(0.5f, 0.5f));
                Sprite pipe_bend_tr = Sprite.Create(_pipe_bend_tr, new Rect(0, 0, _pipe_bend_tr.width, _pipe_bend_tr.height), new Vector2(0.5f, 0.5f));
                horiz.transform.SetParent(transform);
                horiz.AddComponent<SpriteRenderer>().sprite = horizontal;
                horiz.transform.localPosition = Vector3.zero;
                horiz.GetComponent<SpriteRenderer>().size = new Vector2(horiz.GetComponent<SpriteRenderer>().size.x, Vector2.Distance(transform.position, last.transform.position));

                Vector2 direction = last.transform.position - horiz.transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                float distance = Vector3.Distance(horiz.transform.parent.position, last.transform.position);
                horiz.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.localScale = new Vector3(horiz.transform.localScale.x, distance, horiz.transform.localScale.z);
                Vector3 center = ((horiz.transform.parent.position + last.transform.position) * 0.5f);
                horiz.transform.position = center;
            }
        }
        if (order == 0)
        {
            if (type == null || type == "")
            {
                Texture2D _cap = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipeCap.png");
                //Texture2D _horizontal = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipe_horiz.png");
                Texture2D _cap_closed = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipeCap_closed.png");
                //Texture2D _pipe_bend_bl = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipe_bend_bl.png");
                //Texture2D _pipe_bend_tl = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipe_bend_tl.png");
                //Texture2D _pipe_bend_br = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipe_bend_br.png");
                //Texture2D _pipe_bend_tr = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipe_bend_tr.png");

                //Sprite horizontal = Sprite.Create(_horizontal, new Rect(0, 0, _horizontal.width, _horizontal.height), new Vector2(0.5f, 0.5f));
                Sprite cap = Sprite.Create(_cap, new Rect(0, 0, _cap.width, _cap.height), new Vector2(0.5f, 0.75f));
                Sprite cap_closed = Sprite.Create(_cap_closed, new Rect(0, 0, _cap_closed.width, _cap_closed.height), new Vector2(0.5f, 0.75f));
                //Sprite pipe_bend_bl = Sprite.Create(_pipe_bend_bl, new Rect(0, 0, _pipe_bend_bl.width, _pipe_bend_bl.height), new Vector2(0.5f, 0.5f));
                //Sprite pipe_bend_tl = Sprite.Create(_pipe_bend_tl, new Rect(0, 0, _pipe_bend_tl.width, _pipe_bend_tl.height), new Vector2(0.5f, 0.5f));
                //Sprite pipe_bend_br = Sprite.Create(_pipe_bend_br, new Rect(0, 0, _pipe_bend_br.width, _pipe_bend_br.height), new Vector2(0.5f, 0.5f));
                //Sprite pipe_bend_tr = Sprite.Create(_pipe_bend_tr, new Rect(0, 0, _pipe_bend_tr.width, _pipe_bend_tr.height), new Vector2(0.5f, 0.5f));
                gameObject.AddComponent<SpriteRenderer>().sprite = cap_closed;
                transform.position = new Vector3(transform.position.x, transform.position.y, -0.01f);
                transform.localScale = transform.localScale / 1.5f;
            }
        }

    }

    public void ActivateSuckoMode()
    {
        Texture2D _cap = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipeCap.png");
        Sprite cap = Sprite.Create(_cap, new Rect(0, 0, _cap.width, _cap.height), new Vector2(0.5f, 0.75f));
        gameObject.GetComponent<SpriteRenderer>().sprite = cap;
    }

    public void DeactiveSuckoMode()
    {
        Texture2D _cap_closed = Loader.LoadPNG(@"E:\Unity\Projects\World of Goo 2U 2\res\images\levelimages\pipeCap_closed.png");
        Sprite cap_closed = Sprite.Create(_cap_closed, new Rect(0, 0, _cap_closed.width, _cap_closed.height), new Vector2(0.5f, 0.75f));
        gameObject.GetComponent<SpriteRenderer>().sprite = cap_closed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
