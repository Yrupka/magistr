using UnityEngine;
using System;

public class LMS_controller : MonoBehaviour
{
	private float startTime;
	private float currentTime;

    private void Update()
    {
		currentTime = Time.time - startTime;
    }

    private void Scorm_Commit_Complete()
    {
		// ScormManager.Terminate();
	}

    public void Add_record(string description, string answer, int score)
    {
        // StudentRecord.LearnerInteractionRecord interaction = new StudentRecord.LearnerInteractionRecord();
        // interaction.id = ScormManager.GetNextInteractionId();
        // interaction.timeStamp = DateTime.Now;

        // if (score == -1) // эссе
        // {
        //     interaction.type = StudentRecord.InteractionType.fill_in;
        //     if(string.IsNullOrEmpty(answer))
        //         interaction.result = StudentRecord.ResultType.not_set;
        //     else
        //         interaction.result = StudentRecord.ResultType.neutral;
        // }
        // else // вопрос
        // {
        //     interaction.type = StudentRecord.InteractionType.choice;
        //     if (score > 0)
        //         interaction.result = StudentRecord.ResultType.correct;
        //     else
        //     {
        //         if (string.IsNullOrEmpty(answer))
        //             interaction.result = StudentRecord.ResultType.not_set;
        //         else
        //             interaction.result = StudentRecord.ResultType.incorrect;
        //     }
                
        // }

        // interaction.description = description;
        // interaction.response = answer;
        // interaction.weighting = score;
        
        
        // ScormManager.AddInteraction(interaction);
    }

    public void Set_score(int score)
    {
        // ScormManager.SetScoreRaw(score);
    }

    public void Exit()
    {
		// ScormManager.SetSessionTime(currentTime);
        // ScormManager.SetCompletionStatus(StudentRecord.CompletionStatusType.completed);
		// ScormManager.SetExit(StudentRecord.ExitType.normal);
		// ScormManager.Commit();
	}
}
