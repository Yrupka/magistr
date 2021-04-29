using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class Loader_options_lab_1 : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string Get_file_data();

    public GameObject window;
    public Stand_controller_lab_1 stand_controller;
    public Fuel_controller fuel_controller;
    public Menu_interactive menu;

    private Questions_data[] questions;

    private void Start()
    {
        Collect();
        DontDestroyOnLoad(this.gameObject);
    }

    private void Collect()
    {
        string data = Get_file_data();
        // получение списка файлов с разрешением json
        
        if (data == "error")
        {
            Window("Файл сохранения не найден");
            return;
        }

        // разделение данных на массив вопросов и данные профиля
        data = data.Remove(data.Length - 1);
        string[] data_raw = data.Split(new string[] {",\"questions\":"}, System.StringSplitOptions.None);
        Engine_options_lab_1 options = new Engine_options_lab_1();

        try
        {
            options = JsonUtility.FromJson<Engine_options_lab_1>(data_raw[0] + "}");
            questions = JsonHelper.FromJson<Questions_data>(data_raw[1]);
        }
        catch (System.Exception)
        {
            Window("Ошибка в файле сохранения");
            return;
        }
        
        options.max_moment = Mathf.Max(options.Get_list_moment().ToArray());

        stand_controller.Load_options(options);
        fuel_controller.Load_options(options.fuel_amount);
        menu.Load_options(options.hints, options.car_name, options.engine_name);
        Window("");
    }

    private void Window(string message)
    {
        if (string.IsNullOrEmpty(message))
            window.SetActive(false);
        else
            window.transform.Find("Info").Find("Info").GetComponent<Text>().text = message;
    }

    public Questions_data[] Get_questions()
    {
        return questions;
    }
}
