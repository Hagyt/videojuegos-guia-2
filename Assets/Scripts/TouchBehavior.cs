using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TouchBehavior : MonoBehaviour
{
    public TextMeshProUGUI text;
    private Touch touch;
    private float touchTime = .5f;
    private float displayTime = .5f;

    private TextMeshProUGUI Text { get => text; set => text = value; }

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
            Text.SetText(touch.phase.ToString());

            if (touch.phase == TouchPhase.Ended)
            {
                touchTime = Time.time;
            }


        }
        else if (Time.time - touchTime > displayTime)
        {
            Text.SetText("");
        }
    }
}
