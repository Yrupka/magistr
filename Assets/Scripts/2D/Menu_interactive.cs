using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu_interactive : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Transform menu_tail;

    public UI_timer ui_timer;
    public UI_scale ui_scale;
    public UI_texts ui_texts;
    public UI_names ui_names;


    private void Start()
    {
        Transform buttons = transform.Find("Buttons");
        buttons.Find("Scale").GetComponent<Toggle>().onValueChanged.AddListener(ui_scale.gameObject.SetActive);
        buttons.Find("Timer").GetComponent<Toggle>().onValueChanged.AddListener(ui_timer.gameObject.SetActive);
        buttons.Find("Texts").GetComponent<Toggle>().onValueChanged.AddListener(ui_texts.gameObject.SetActive);
        buttons.Find("Names").GetComponent<Toggle>().onValueChanged.AddListener(ui_names.Clicked);
        Toggle profile_button = buttons.Find("Profile").GetComponent<Toggle>();
        profile_button.onValueChanged.AddListener(ui_texts.Clicked);

        menu_tail = transform.Find("Tail");
    }

    public void Load_options(string[] hints, string car_name, string engine_name)
    {
        ui_texts.Set_texts(hints, car_name, engine_name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.position += new Vector3(130,0f);
        menu_tail.gameObject.SetActive(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        transform.position += new Vector3(-130, 0f);
        menu_tail.gameObject.SetActive(true);
    }
}
