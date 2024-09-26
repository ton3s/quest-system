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
	private QuestStepState[] questStepStates;

	public Quest(QuestInfoSO questInfo)
	{
		this.info = questInfo;
		this.state = QuestState.REQUIREMENTS_NOT_MET;
		this.currentQuestStepIndex = 0;
		this.questStepStates = new QuestStepState[info.questSteps.Length];
		for (int i = 0; i < questStepStates.Length; i++)
		{
			questStepStates[i] = new QuestStepState();
		}
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
			questStep.InitializeQuestStep(info.id, currentQuestStepIndex);
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

	// Store the state of a quest step at a given index 
	public void StoreQuestStepState(QuestStepState questStepState, int stepIndex)
	{
		if (stepIndex >= 0 && stepIndex < questStepStates.Length)
		{
			questStepStates[stepIndex] = questStepState;
		}
		else
		{
			Debug.LogWarning("Tried to store quest step state, but stepIndex was out of range: "
			+ "Quest ID=" + info.id + ", Step Index=" + stepIndex);
		}
	}

	public QuestData GetQuestData()
	{
		return new QuestData(state, currentQuestStepIndex, questStepStates);
	}
}
