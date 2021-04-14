using UnityEngine;

public class Scale : MonoBehaviour
{
    private TextMesh value_text;
    private float weight;
    private float zero_weight;
    private bool power_state;

    private void Awake()
    {
        zero_weight = 0f;
        weight = 0f;
        value_text = transform.Find("Value_text").GetComponent<TextMesh>();
        value_text.text = "";
        transform.Find("Button_power").GetComponent<Item_button>().Add_listener(Power_state);
        transform.Find("Button_zero").GetComponent<Item_button>().Add_listener(Weight_zero);
        power_state = false;
    }

    private void Power_state()
    {
        power_state = !power_state;
        zero_weight = 0f;
        Weight_set(weight);
    }

    private void Weight_zero()
    {
        zero_weight = weight;
        Weight_set(weight);
    }

    public void Weight_set(float value)
    {
        weight = value;
        if (power_state)
            value_text.text = (weight - zero_weight).ToString("0.00");
        else
            value_text.text = "";
    }

    public string Get_weight()
    {
        return value_text.text;
    }
}
