using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] ButtonHitbox hitbox;
    [SerializeField] Vector3 normalScale;
    [SerializeField] float hoveringMultiplicator;
    [SerializeField] float interpolationCoefficient;

    private void Update()
    {
        if (hitbox.hovering)
            transform.localScale = Vector3.Lerp(transform.localScale, normalScale * hoveringMultiplicator, interpolationCoefficient);
        else
            transform.localScale = Vector3.Lerp(transform.localScale, normalScale, interpolationCoefficient);
    }
}
