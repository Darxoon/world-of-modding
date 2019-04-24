using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPop : MonoBehaviour
{
    #region Singleton 

    public static ButtonPop instance;
    private void Awake()
    {
        if (instance)
            Destroy(gameObject);
        else
            instance = this;
    }

    #endregion

    [SerializeField] private AudioSource[] sounds;

    public void Play()
    {
        float random = Random.value;
        int index = (int) Mathf.Round(random * (sounds.Length - 1));

        sounds[index].Play();
    }
}
