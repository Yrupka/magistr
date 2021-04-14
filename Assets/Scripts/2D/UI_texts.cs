using UnityEngine.UI;

public class UI_texts : UI_grab
{
    public Fuel_controller fuel_controller;

    private Text hint_info;
    private Text profile_info;

    private string[] texts;

    private void Awake()
    {
        hint_info = transform.Find("Hint").GetComponent<Text>();
        profile_info = transform.Find("Profile").GetComponent<Text>();
        fuel_controller.Add_listener_state(Text_update);
    }

    private void Text_update()
    {
        string text = texts[fuel_controller.State()];
        if (string.IsNullOrEmpty(text))
            hint_info.text = "Нет подсказки";
        else
            hint_info.text = text;
    }

    public void Clicked(bool state)
    {
        profile_info.gameObject.SetActive(state);
    }

    public void Set_texts(string[] loaded_options, string car_name, string engine_name) // получить загруженные данные
    {
        texts = loaded_options;
        profile_info.text = "Профиль:\nАвтомобиль: " + car_name + "\nМодель двигателя: " + engine_name;
        Text_update();
    }
}
