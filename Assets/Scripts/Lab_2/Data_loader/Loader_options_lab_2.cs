using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Loader_options_lab_2 : MonoBehaviour
{
    public GameObject window;
    public Stand_controller_lab_2 stand_controller;
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
        // получение списка файлов с разрешением json
        DirectoryInfo info = new DirectoryInfo(Application.dataPath);
        FileInfo[] fileInfo = info.GetFiles("*.json");

        if (fileInfo.Length == 0)
        {
            Window("Файл сохранения не найден");
            return;
        }
            
        // чтение первого файла из списка
        string data = File.ReadAllText(fileInfo[0].FullName);

        // разделение данных на массив вопросов и данные профиля
        data = data.Remove(data.Length - 1);
        string[] data_raw = data.Split(new string[] {",\"questions\":"}, System.StringSplitOptions.None);
        Engine_options_lab_2 options = new Engine_options_lab_2();

        try
        {
            options = JsonUtility.FromJson<Engine_options_lab_2>(data_raw[0] + "}");
            questions = JsonHelper.FromJson<Questions_data>(data_raw[1]);
        }
        catch (System.Exception)
        {
            Window("Ошибка в файле сохранения");
            return;
        }
        
        options.Calculate();

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
