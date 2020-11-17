using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Instantiate an new empty object, add this script to it as a component, use Initialization() function to make it work.
public class AfterImage : MonoBehaviour
{
    SpriteRenderer copy;

    float totalTime, timeElapsed, originAlpha;

    public void Initialization(Transform origin, float time, float initiateAlpha)
    {
        // copy the sprite information from the original
        copy = gameObject.AddComponent<SpriteRenderer>();
        var original = origin.GetComponent<SpriteRenderer>();

        copy.sprite = original.sprite;
        copy.color = original.color;
        copy.flipX = original.flipX;
        copy.flipY = original.flipY;
        copy.drawMode = original.drawMode;
        copy.maskInteraction = original.maskInteraction;
        copy.spriteSortPoint = original.spriteSortPoint;
        copy.material = original.material;
        copy.sortingLayerID = original.sortingLayerID;
        copy.sortingOrder = original.sortingOrder - 1;

        originAlpha = original.color.a * initiateAlpha;

        // copy the transform information
        transform.position = origin.position;
        transform.rotation = origin.rotation;
        transform.localScale = origin.localScale;

        // initiate the time clock
        totalTime = time;
        timeElapsed = 0.0f;

        // start updating this script
        this.enabled = true;
    }

    private void Update()
    {
        timeElapsed = Mathf.Clamp(timeElapsed + Time.deltaTime, 0.0f, totalTime);
        float newAlpha = Mathf.Lerp(originAlpha, 0.0f, timeElapsed / totalTime);

        copy.color = new Color(copy.color.r, copy.color.g, copy.color.b, newAlpha);

        if (timeElapsed >= totalTime)
        {
            Destroy(gameObject);
        }
    }
}
