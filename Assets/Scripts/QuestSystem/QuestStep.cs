using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
	private bool isFinished = false;

	private string questId;
	private int stepIndex;

	public void InitializeQuestStep(string questId, int stepIndex)
	{
		this.questId = questId;
		this.stepIndex = stepIndex;
	}

	protected void FinishQuestStep()
	{
		if (!isFinished)
		{
			isFinished = true;

			// Notify the QuestManager that this quest step is finished
			GameEventsManager.instance.questEvents.AdvanceQuest(questId);

			Destroy(this.gameObject);
		}
	}

	protected void ChangeState(string newState)
	{
		GameEventsManager.instance.questEvents.QuestStepStateChanged(questId, stepIndex, new QuestStepState(newState));
	}
}
