using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sTransformController : MonoBehaviour
{

    public GameObject connectedBall;
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = transform.localScale * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 direction = connectedBall.transform.position - transform.position;

        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //transform.rotation = Quaternion.AngleAxis(angle - 90, -Vector3.forward);



        GetComponent<SpriteRenderer>().size = new Vector2(GetComponent<SpriteRenderer>().size.x, Vector2.Distance(transform.position, connectedBall.transform.position));
        Vector2 direction = connectedBall.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        float distance = Vector3.Distance(transform.parent.position, connectedBall.transform.position);

        transform.localScale = new Vector3(transform.localScale.x, distance / 3, transform.localScale.z);

        Vector3 center = ((transform.parent.position + connectedBall.transform.position) * 0.5f);
        transform.position = center;

    }
}
