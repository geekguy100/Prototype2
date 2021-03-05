using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sin : MonoBehaviour
{
    [Tooltip("the speed of horizontal movement")]
    public float speed = 1;

    [Tooltip("the amount of time that the sin lives")]
    public float lifeTime = 1;

    [Tooltip("when to start disolving the image in unity units(distance from parent)")]
    public float distanceToDissolve = 6.5f;

    [Tooltip("the width of the srpite in unity units")]
    public float widthOfSprite;

    RectTransform rt;
    Image img;

    // Start is called before the first frame update
    void Awake()
    {
        img = GetComponent<Image>();
        rt = GetComponent<RectTransform>();

        widthOfSprite = img.sprite.bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float xPos = rt.transform.position.x;
        float yPos = rt.transform.position.y;
        float moveDistance = speed * Time.deltaTime;
        rt.transform.position = new Vector3(xPos+moveDistance,yPos,0);

        if(rt.transform.position.x > distanceToDissolve)
        {
            img.fillAmount -= moveDistance;

        }

        if (img.fillAmount <= 0)
        {
            //reset position and start doing the opposite of shrinking
        }
    }
}
