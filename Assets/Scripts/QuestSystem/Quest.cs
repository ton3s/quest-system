using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
	// Static information about the quest
	public QuestInfoSO info;

	// Current state of the quest
	public QuestState state;
	private int currentQuestStepIndex;

	public Quest(QuestInfoSO questInfo)
	{
		this.info = questInfo;
		this.state = QuestState.REQUIREMENTS_NOT_MET;
		this.currentQuestStepIndex = 0;
	}

	public void MoveToNextStep()
	{
		currentQuestStepIndex++;
	}

	public bool CurrentStepExists()
	{
		// Ensure the current step index is within the bounds of the quest steps
		return currentQuestStepIndex < info.questSteps.Length;
	}

	public void InstantiateCurrentQuestStep(Transform parentTransform)
	{
		GameObject questStepPrefab = GetCurrentQuestStepPrefab();
		if (questStepPrefab != null)
		{
			Debug.Log("Instantiating quest step: " + questStepPrefab.name);
			QuestStep questStep = Object.Instantiate(questStepPrefab, parentTransform).GetComponent<QuestStep>();
			questStep.InitializeQuestStep(info.id);
		}
	}

	private GameObject GetCurrentQuestStepPrefab()
	{
		GameObject questStepPrefab = null;
		if (CurrentStepExists())
		{
			questStepPrefab = info.questSteps[currentQuestStepIndex];
		}
		else
		{
			Debug.LogWarning("Tried to get quest step prefab, but stepIndex was out of range indicating that "
			+ "there's no current step: Quest ID=" + info.id + ", Step Index=" + currentQuestStepIndex);
		}
		return questStepPrefab;
	}
}
