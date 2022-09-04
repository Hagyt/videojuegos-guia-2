using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultitouchBehavior : MonoBehaviour
{
    public TextMeshProUGUI text;
    private int tapCount = 0;
    private string mtInfo;
    private Touch touch;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mtInfo = string.Format("Numero de dedos: {0}\n", tapCount);

        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                touch = Input.GetTouch(i);
                string info = "Touch: " + i.ToString() + " - " + touch.phase.ToString() + "\n";

                mtInfo += info;
            }
        }

        tapCount = Input.touchCount;
        text.SetText(mtInfo);

    }
}
