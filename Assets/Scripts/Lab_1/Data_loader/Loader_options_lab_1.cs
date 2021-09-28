using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class Loader_options_lab_1 : MonoBehaviour
{
    // [DllImport("__Internal")]
    // private static extern string Get_file_data();

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
        //string data = Get_file_data();
        //заглушка
        string data = "{\"engine_name\":\"д\",\"car_name\":\"м\",\"hints\":[\"\",\"\",\"\",\"\"],\"fuel_amount\":10,\"heat_time\":44,\"interpolation\":0,\"lever_length\":1.0,\"max_moment\":9.0,\"rpms\":[{\"rpm\":1000.0,\"moment\":10.0,\"consumption\":11.0,\"deg\":10.0,\"load\":10.0},{\"rpm\":1000.0,\"moment\":20.0,\"consumption\":12.0,\"deg\":20.0,\"load\":20.0},{\"rpm\":1000.0,\"moment\":30.0,\"consumption\":13.0,\"deg\":15.0,\"load\":30.0},{\"rpm\":2000.0,\"moment\":10.0,\"consumption\":14.0,\"deg\":10.0,\"load\":10.0},{\"rpm\":2000.0,\"moment\":20.0,\"consumption\":15.0,\"deg\":15.0,\"load\":20.0},{\"rpm\":2000.0,\"moment\":30.0,\"consumption\":16.0,\"deg\":20.0,\"load\":30.0},{\"rpm\":3000.0,\"moment\":7.0,\"consumption\":17.0,\"deg\":13.0,\"load\":10.0},{\"rpm\":3000.0,\"moment\":8.0,\"consumption\":18.0,\"deg\":30.0,\"load\":20.0},{\"rpm\":3000.0,\"moment\":9.0,\"consumption\":19.0,\"deg\":25.0,\"load\":30.0}],\"questions\":[{\"score\":11,\"num\":0,\"text\":\"аааа\",\"answers\":[\"a\",\"b\",\"c\"]},{\"score\":22,\"num\":0,\"text\":\"fffff\",\"answers\":[\"aa\"]},{\"score\":33,\"num\":-1,\"text\":\"zzzzz\",\"answers\":[]}]}";
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
