using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Questions : MonoBehaviour
{
    private GameObject list;
    private GameObject content;
    private List<Question> questions_list;
    private List<Toggle> toggles_list;

    private int current;

    public Question question;
    public LMS_controller controller;
    public GameObject end_screen;

    private void Awake()
    {
        list = transform.Find("List").gameObject;
        transform.Find("Next").GetComponent<Button>().onClick.AddListener(Next);
        transform.Find("Prev").GetComponent<Button>().onClick.AddListener(Prev);
        transform.Find("End").GetComponent<Button>().onClick.AddListener(End);
        current = 0;
    }
    
    private void Start()
    {
        // получить вопросы из объекта с прошлой сцены
        Questions_data[] questions;
        questions = GameObject.Find("Loader").GetComponent<Loader_options>().Get_questions();

        questions_list = new List<Question>();
        content = transform.Find("Questions_list").Find("Viewport").Find("Content").gameObject;

        for (int i = 0; i < questions.Length; i++)
        {
            Question new_question = Instantiate<Question>(question, content.transform);
            if (questions[i].num == -1) // эссе
            {
                new_question.Init(questions[i].text);
                questions_list.Add(new_question);
            }
            else // вопрос
            {
                new_question.Init(questions[i].text, questions[i].answers, questions[i].score);
                questions_list.Add(new_question);
            }
        }
        
        toggles_list = new List<Toggle>();
        Toggle toggle = list.transform.Find("Toggle").GetComponent<Toggle>();
        for (int i = 0; i < questions_list.Count; i++)
        {
            Toggle prefab_toggle = Instantiate(toggle, list.transform);
            prefab_toggle.group = list.GetComponent<ToggleGroup>();
            prefab_toggle.transform.Find("Label").GetComponent<Text>().text = "Вопрос " + (i + 1).ToString();
            prefab_toggle.gameObject.SetActive(true);
            prefab_toggle.onValueChanged.AddListener(Toggle_clicked);
            toggles_list.Add(prefab_toggle);
        }
        questions_list[0].gameObject.SetActive(true); // начало на 1 вопросе
        toggles_list[0].isOn = true; 
    }

    private void Next()
    {
        if (current < toggles_list.Count - 1)
        {
            current++;
            toggles_list[current].isOn = true;
            questions_list[current].gameObject.SetActive(true);
            questions_list[current - 1].gameObject.SetActive(false);
        }   
    }

    private void Prev()
    {
        if (current > 0)
        {
            current--;
            toggles_list[current].isOn = true;
            questions_list[current].gameObject.SetActive(true);
            questions_list[current + 1].gameObject.SetActive(false);
        }  
    }

    private void Toggle_clicked(bool state)
    {
        if (state)
        {
            questions_list[current].gameObject.SetActive(false);

            current = toggles_list.FindIndex(x => x.isOn);
            questions_list[current].gameObject.SetActive(true);
        }
    }

    private void End()
    {
        int score = 0;
        for (int i = 0; i < questions_list.Count; i++)
        {
            controller.Add_record(questions_list[i].Text(), questions_list[i].Answer(), questions_list[i].Result());
            if (questions_list[i].Result() != -1)
                score += questions_list[i].Result();
        }
        controller.Set_score(score);
        controller.Exit();
        end_screen.SetActive(true);
    }
}
