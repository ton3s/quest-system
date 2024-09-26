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
		this.questStepStates = new QuestStepState[info.questStepPrefabs.Length];
		for (int i = 0; i < questStepStates.Length; i++)
		{
			questStepStates[i] = new QuestStepState();
		}
	}

	public Quest(QuestInfoSO questInfo, QuestState questState, int currentQuestStepIndex, QuestStepState[] questStepStates)
	{
		this.info = questInfo;
		this.state = questState;
		this.currentQuestStepIndex = currentQuestStepIndex;
		this.questStepStates = questStepStates;

		// If the quest step states and prefabs are different lengths
		// something has changed during development and the saved data is out of sync
		if (questStepStates.Length != this.info.questStepPrefabs.Length)
		{
			Debug.LogWarning("Quest step states and prefabs are different lengths for quest: " + info.id);
		}
	}

	public void MoveToNextStep()
	{
		currentQuestStepIndex++;
	}

	public bool CurrentStepExists()
	{
		// Ensure the current step index is within the bounds of the quest steps
		return currentQuestStepIndex < info.questStepPrefabs.Length;
	}

	public void InstantiateCurrentQuestStep(Transform parentTransform)
	{
		GameObject questStepPrefab = GetCurrentQuestStepPrefab();
		if (questStepPrefab != null)
		{
			Debug.Log("Instantiating quest step: " + questStepPrefab.name);
			QuestStep questStep = Object.Instantiate(questStepPrefab, parentTransform).GetComponent<QuestStep>();

			// Initialize the quest step with the quest ID, step index, and state
			questStep.InitializeQuestStep(info.id, currentQuestStepIndex, questStepStates[currentQuestStepIndex].state);
		}
	}

	private GameObject GetCurrentQuestStepPrefab()
	{
		GameObject questStepPrefab = null;
		if (CurrentStepExists())
		{
			questStepPrefab = info.questStepPrefabs[currentQuestStepIndex];
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
