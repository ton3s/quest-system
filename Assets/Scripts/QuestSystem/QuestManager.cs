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

		// Example of how to get a Quest object by ID
		Quest quest = GetQuestByID("CollectCoinsQuest");
		Debug.Log(quest.info.displayName);
		Debug.Log(quest.info.levelRequirement);
		Debug.Log(quest.state);
		Debug.Log(quest.CurrentStepExists());
	}

	/// <summary>
	/// Create a map of Quest objects using the QuestInfoSOs in the Resources/Quests folder
	/// </summary>
	/// <returns></returns>
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
