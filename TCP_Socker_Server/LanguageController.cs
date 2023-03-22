using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LanguageController : MonoBehaviour
{
    private RectTransform rectTransform;
    private int speed = 0;

    public void setSpeed(int speed)
    {
        this.speed = speed;
    }
    private void FixedUpdate()
    {
        Vector2 pos = rectTransform.position;
        pos.y-= speed;
        rectTransform.position = pos;
        if (pos.y <-500)
        {
            Destroy(this.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
