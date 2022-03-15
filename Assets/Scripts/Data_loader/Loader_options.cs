using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class Loader_options : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string Get_file_data();

    public GameObject window;
    public Stand_controller_lab_1 stand_controller_lab_1;
    public Stand_controller_lab_2 stand_controller_lab_2;
    public Stand_controller_lab_3 stand_controller_lab_3;
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
        //заглушка
        //1 string data = "1\n{\"engine_name\":\"д\",\"car_name\":\"м\",\"hints\":[\"\",\"\",\"\",\"\"],\"fuel_amount\":10,\"heat_time\":44,\"interpolation\":5,\"lever_length\":5.0,\"max_moment\":9.0,\"rpms\":[{\"rpm\":0.0,\"moment\":0.0,\"consumption\":0.0},{\"rpm\":1000.0,\"moment\":10.0,\"consumption\":11.0},{\"rpm\":1000.0,\"moment\":20.0,\"consumption\":12.0},{\"rpm\":1000.0,\"moment\":30.0,\"consumption\":13.0},{\"rpm\":2000.0,\"moment\":10.0,\"consumption\":14.0},{\"rpm\":2000.0,\"moment\":20.0,\"consumption\":15.0},{\"rpm\":2000.0,\"moment\":30.0,\"consumption\":16.0},{\"rpm\":3000.0,\"moment\":7.0,\"consumption\":17.0},{\"rpm\":3000.0,\"moment\":8.0,\"consumption\":18.0},{\"rpm\":3000.0,\"moment\":9.0,\"consumption\":19.0}],\"questions\":[{\"score\":11,\"num\":0,\"text\":\"привет\",\"answers\":[\"аыр\",\"юа\",\"дд\"]},{\"score\":22,\"num\":0,\"text\":\"fffff\",\"answers\":[\"aa\"]},{\"score\":33,\"num\":-1,\"text\":\"zzzzz\",\"answers\":[]}]}";
        //2 string data = "2\n{\"engine_name\":\"д\",\"car_name\":\"м\",\"hints\":[\"\",\"\",\"\",\"\"],\"fuel_amount\":10,\"heat_time\":44,\"interpolation\":0,\"lever_length\":1.0,\"max_moment\":0.0,\"rpms\":[{\"rpm\":1000.0,\"moment\":10.0,\"consumption\":11.0,\"deg\":10.0},{\"rpm\":1000.0,\"moment\":20.0,\"consumption\":12.0,\"deg\":20.0},{\"rpm\":1000.0,\"moment\":30.0,\"consumption\":13.0,\"deg\":15.0},{\"rpm\":2000.0,\"moment\":10.0,\"consumption\":14.0,\"deg\":10.0},{\"rpm\":2000.0,\"moment\":20.0,\"consumption\":15.0,\"deg\":15.0},{\"rpm\":2000.0,\"moment\":30.0,\"consumption\":16.0,\"deg\":20.0},{\"rpm\":3000.0,\"moment\":7.0,\"consumption\":17.0,\"deg\":13.0},{\"rpm\":3000.0,\"moment\":8.0,\"consumption\":18.0,\"deg\":30.0},{\"rpm\":3000.0,\"moment\":36.0,\"consumption\":19.0,\"deg\":25.0}],\"questions\":[{\"score\":11,\"num\":0,\"text\":\"аааа\",\"answers\":[\"a\",\"b\",\"c\"]},{\"score\":22,\"num\":0,\"text\":\"fffff\",\"answers\":[\"aa\"]},{\"score\":33,\"num\":-1,\"text\":\"zzzzz\",\"answers\":[]}]}";
        //3 string data = "{\"engine_name\":\"д\",\"car_name\":\"м\",\"hints\":[\"\",\"\",\"\",\"\"],\"fuel_amount\":10,\"heat_time\":44,\"interpolation\":0,\"lever_length\":1.0,\"max_moment\":9.0,\"rpms\":[{\"rpm\":1000.0,\"moment\":10.0,\"consumption\":11.0,\"air\":10.0},{\"rpm\":2000.0,\"moment\":20.0,\"consumption\":12.0,\"air\":20.0},{\"rpm\":3000.0,\"moment\":30.0,\"consumption\":13.0,\"air\":30.0},{\"rpm\":4000.0,\"moment\":10.0,\"consumption\":14.0,\"air\":40.0},{\"rpm\":5000.0,\"moment\":20.0,\"consumption\":15.0,\"air\":55.0},{\"rpm\":5500.0,\"moment\":30.0,\"consumption\":16.0,\"air\":40.0}],\"questions\":[{\"score\":11,\"num\":0,\"text\":\"аааа\",\"answers\":[\"a\",\"b\",\"c\"]},{\"score\":22,\"num\":0,\"text\":\"fffff\",\"answers\":[\"aa\"]},{\"score\":33,\"num\":-1,\"text\":\"zzzzz\",\"answers\":[]}]}";

        // получение списка файлов с разрешением json
        if (data == "error")
        {
            Window("Файл сохранения не найден");
            return;
        }

        // получение номера лаб. работы
        int lab_num = int.Parse(data.Substring(0, 1));
        // разделение данных на массив вопросов и данные профиля
        data = data.Remove(0, 2);
        data = data.Remove(data.Length - 1);
        string[] data_raw = data.Split(new string[] { ",\"questions\":" }, System.StringSplitOptions.None);
        Engine_options_lab_1 options_lab_1 = new Engine_options_lab_1();
        Engine_options_lab_2 options_lab_2 = new Engine_options_lab_2();
        Engine_options_lab_3 options_lab_3 = new Engine_options_lab_3();

        switch (lab_num)
        {
            case 1:
                try
                {
                    options_lab_1 = JsonUtility.FromJson<Engine_options_lab_1>(data_raw[0] + "}");
                    questions = JsonHelper.FromJson<Questions_data>(data_raw[1]);
                }
                catch (System.Exception)
                {
                    Window("Ошибка в файле сохранения");
                    return;
                }

                stand_controller_lab_1.Load_options(options_lab_1);
                fuel_controller.Load_options(options_lab_1.fuel_amount);
                menu.Load_options(options_lab_1.hints, options_lab_1.car_name, options_lab_1.engine_name);
                break;
            case 2:
                try
                {
                    options_lab_2 = JsonUtility.FromJson<Engine_options_lab_2>(data_raw[0] + "}");
                    questions = JsonHelper.FromJson<Questions_data>(data_raw[1]);
                }
                catch (System.Exception)
                {
                    Window("Ошибка в файле сохранения");
                    return;
                }

                stand_controller_lab_2.Load_options(options_lab_2);
                fuel_controller.Load_options(options_lab_2.fuel_amount);
                menu.Load_options(options_lab_2.hints, options_lab_2.car_name, options_lab_2.engine_name);
                break;
            case 3:
                try
                {
                    options_lab_3 = JsonUtility.FromJson<Engine_options_lab_3>(data_raw[0] + "}");
                    questions = JsonHelper.FromJson<Questions_data>(data_raw[1]);
                }
                catch (System.Exception)
                {
                    Window("Ошибка в файле сохранения");
                    return;
                }

                stand_controller_lab_3.Load_options(options_lab_3);
                fuel_controller.Load_options(options_lab_3.fuel_amount);
                menu.Load_options(options_lab_3.hints, options_lab_3.car_name, options_lab_3.engine_name);
                break;
        }

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
