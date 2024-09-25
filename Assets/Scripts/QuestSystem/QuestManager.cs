using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
	private Dictionary<string, Quest> questMap;

	/// <summary>
	/// Initialize the QuestManager by creating the quest map from the QuestInfoSOs
	/// </summary>
	private void Awake()
	{
		questMap = CreateQuestMap();
	}

	private void OnEnable()
	{
		// Subscribe to the QuestEvents
		GameEventsManager.instance.questEvents.onStartQuest += StartQuest;
		GameEventsManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
		GameEventsManager.instance.questEvents.onFinishQuest += FinishQuest;
	}

	private void OnDisable()
	{
		// Unsubscribe from the QuestEvents
		GameEventsManager.instance.questEvents.onStartQuest -= StartQuest;
		GameEventsManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
		GameEventsManager.instance.questEvents.onFinishQuest -= FinishQuest;
	}

	private void Start()
	{
		// Broadcast the inital state or all quests on startup
		foreach (Quest quest in questMap.Values)
		{
			// Notify the GameEventsManager that the quest state has changed (initial state) for all quests
			GameEventsManager.instance.questEvents.QuestStateChanged(quest);
		}
	}

	private void StartQuest(string questID)
	{
		// TODO: Implement quest starting logic
		Debug.Log("Starting quest with ID: " + questID);
	}

	private void AdvanceQuest(string questID)
	{
		// TODO: Implement quest advancing logic
		Debug.Log("Advancing quest with ID: " + questID);
	}

	private void FinishQuest(string questID)
	{
		// TODO: Implement quest finishing logic
		Debug.Log("Finishing quest with ID: " + questID);
	}


	/// <summary>
	/// Create a map of Quest objects using the QuestInfoSOs in the Resources/Quests folder
	/// </summary>
	private Dictionary<string, Quest> CreateQuestMap()
	{
		// Loads all QuestInfoSOs Scriptable Objects under the Assets/Resources/Quests folder
		QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");

		// Create the quest map
		Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
		foreach (QuestInfoSO questInfo in allQuests)
		{
			// Check for duplicate quest IDs
			if (idToQuestMap.ContainsKey(questInfo.id))
			{
				Debug.LogWarning("Duplicate quest ID found: " + questInfo.id);
			}
			// Create a new Quest object and add it to the map
			Quest newQuest = new Quest(questInfo);
			idToQuestMap.Add(questInfo.id, newQuest);
		}
		return idToQuestMap;
	}

	private Quest GetQuestByID(string questID)
	{
		Quest quest = questMap[questID];
		if (quest == null)
		{
			Debug.LogWarning("Quest with ID " + questID + " not found in quest map.");
		}
		return quest;
	}
}
