using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Vpad : MonoBehaviour
{
    public TextMeshProUGUI touchCount;
    public TextMeshProUGUI dirText;
    public TextMeshProUGUI value;

    private Touch touch;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private string direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);


            if (touch.phase == TouchPhase.Began)
            {
                startPoint = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended)
            {
                endPoint = touch.position;

                float x = endPoint.x - startPoint.x;
                float y = endPoint.y - startPoint.y;

                if (Mathf.Abs(x) == 0 && Mathf.Abs(y) == 0) {
                    direction = "Tapped";
                }
                else if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    direction = x > 0 ? "Right" : "Left";
                }
                else
                {
                    direction = y > 0 ? "Up" : "Down";
                }

                value.SetText("x: " + x.ToString() + "y: " + y.ToString());
            }

        }

        dirText.SetText(direction);
        touchCount.SetText(Input.touchCount.ToString());

    }

}
