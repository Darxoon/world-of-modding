using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelExitController : MonoBehaviour
{

    public bool isActive = false;
    public GameObject pipeCap;
    public List<GameObject> matches = new List<GameObject>();
    public List<GameObject> normalmatches = new List<GameObject>();
    public int required = 0;
    public int current = 0;
    public Loader loader;
    public int previous;
    //int currentMatches = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (matches.Count > 0)
        {
            pipeCap.GetComponent<pipeParts>().ActivateSuckoMode();
            for (int i = 0; i<matches.Count; i++)
            {
                if (Vector3.Distance(transform.position, normalmatches[i].transform.position) < 2)
                    succ(normalmatches[i]);
            }
        }
        previous = matches.Capacity;
        matches.Capacity = previous;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<GooBallController>().isTower == true) { 
        if (!matches.Contains(collision.gameObject))
            {
                matches.Add(collision.gameObject);
            }
        }

        if (collision.gameObject.GetComponent<GooBallController>().isTower == false)
        {
            if (!normalmatches.Contains(collision.gameObject))
            {
                normalmatches.Add(collision.gameObject);
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<GooBallController>().isTower == true)
            matches.Remove(collision.gameObject);
            pipeCap.GetComponent<pipeParts>().DeactiveSuckoMode();
    }

    void succ(GameObject gooball)
    {
        if (gooball.active == true)
        {
            gooball.SetActive(false);
            current = current + 1;
            bool isStarted = true;
            bool isStartedOnce = false;
            if (current >= required && isStarted == true)
                isStarted = false;
                isStartedOnce = true;
            current = 0;
            finishlevel();
                
        }
    }
    void finishlevel()
    {
        loader.LoadNewLevel("", true);
    }

}
