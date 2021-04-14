using UnityEngine;
using UnityEngine.UI;

public class UI_timer : UI_grab
{
    private Text text_value;
    private bool state;
    private float time;

    private void Awake()
    {
        text_value = transform.Find("Value").GetComponent<Text>();
        transform.Find("Start_stop").GetComponent<Button>().onClick.AddListener(Timer_start_stop);
        transform.Find("Discard").GetComponent<Button>().onClick.AddListener(Timer_discard);
        
        Timer_discard();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            Timer_start_stop();
        if (Input.GetKeyDown(KeyCode.B))
            Timer_discard();
        if (state)
        {
            time += Time.deltaTime;
            text_value.text = time.ToString("00.00");
        }
    }

    private void Timer_start_stop()
    {
        state = !state;
    }

    private void Timer_discard()
    {
        time = 0f;
        text_value.text = "0";
        state = false;
    }
}
