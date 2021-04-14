using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Question : MonoBehaviour
{
    private string _text;
    private int _score;
    
    private Toggle toggle_prefab;
    private InputField input_prefab;
    private Transform toggle_group;
    private Text question_text;
    private List<Toggle> toggle_list;

    private void Awake()
    {
        question_text = transform.Find("Text").GetComponent<Text>();
        input_prefab = transform.Find("Input").GetComponent<InputField>();
        toggle_group = transform.Find("Answers");
        toggle_prefab = toggle_group.Find("Toggle").GetComponent<Toggle>();
        toggle_list = new List<Toggle>();
        
    }

    public void Init(string text, List<string> answers, int score) // создать вопрос с ответом
    {   
        question_text.text = text;
        toggle_group.gameObject.SetActive(true);

        for (int i = 0; i < answers.Count; i++)
        {
            Toggle toggle = Instantiate(toggle_prefab, toggle_group);
            toggle.group = toggle_group.GetComponent<ToggleGroup>();
            toggle.transform.Find("Label").GetComponent<Text>().text = answers[i];
            toggle.gameObject.SetActive(true);
            toggle_list.Add(toggle);
        }

        this.gameObject.SetActive(false);
        _text = text;
        _score = score;
    }

    public void Init(string text) // создать вопрос эссе
    {
        question_text.text = text;
        this.GetComponent<VerticalLayoutGroup>().childControlHeight = false;
        input_prefab.gameObject.SetActive(true);
        input_prefab.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 200);
        this.gameObject.SetActive(false);
        _text = text;
        _score = -1;
    }

    public int Result()
    {
        if (toggle_list.Count == 0)
            return _score;
        else
        {
            if (toggle_list.FindIndex(x => x.isOn == true) == 0) // на 0 позиции стоит ответ
                return _score;
            else
                return 0;
        }
    }

    public string Text()
    {
        return _text;
    }

    public string Answer()
    {
        if (toggle_list.Count > 0)
        {
            int index = toggle_list.FindIndex(x => x.isOn == true);
            if (index == -1)
            {
                return "";
            }
            return toggle_list[index].transform.Find("Label").GetComponent<Text>().text;
        }
        else
        {
            return input_prefab.text;
        }
    }

}
