using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
	[Header("Config")]
	[SerializeField] private bool loadQuestState = true;

	private Dictionary<string, Quest> questMap;

	// Quest start requirements
	private int currentPlayerLevel;

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

		GameEventsManager.instance.questEvents.onQuestStepStateChanged += QuestStepStateChanged;

		// Subscribe to the PlayerEvents
		GameEventsManager.instance.playerEvents.onPlayerLevelChange += PlayerLevelChange;
	}

	private void OnDisable()
	{
		// Unsubscribe from the QuestEvents
		GameEventsManager.instance.questEvents.onStartQuest -= StartQuest;
		GameEventsManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
		GameEventsManager.instance.questEvents.onFinishQuest -= FinishQuest;

		GameEventsManager.instance.questEvents.onQuestStepStateChanged -= QuestStepStateChanged;

		GameEventsManager.instance.playerEvents.onPlayerLevelChange -= PlayerLevelChange;
	}

	private void PlayerLevelChange(int newLevel)
	{
		currentPlayerLevel = newLevel;
	}

	private bool CheckRequirementsMet(Quest quest)
	{
		bool meetsRequirements = true;

		// Check if the player level is high enough to start the quest
		if (currentPlayerLevel < quest.info.levelRequirement)
		{
			// Debug.LogWarning("Player level is too low to start quest: " + quest.info.id);
			meetsRequirements = false;
		}

		// Check if all quest prerequisites are completed
		foreach (QuestInfoSO prerequisite in quest.info.questPrerequisites)
		{
			Quest prerequisiteQuest = GetQuestByID(prerequisite.id);
			if (prerequisiteQuest.state != QuestState.FINISHED)
			{
				// Debug.LogWarning("Prerequisite quest not completed: " + prerequisite.id);
				meetsRequirements = false;
				break;
			}
		}
		return meetsRequirements;
	}

	private void Start()
	{
		// Broadcast the inital state or all quests on startup
		foreach (Quest quest in questMap.Values)
		{
			// Initalize any loaded quest steps
			if (quest.state == QuestState.IN_PROGRESS)
			{
				quest.InstantiateCurrentQuestStep(transform);
			}

			// Notify the GameEventsManager that the quest state has changed (initial state) for all quests
			GameEventsManager.instance.questEvents.QuestStateChanged(quest);
		}
	}

	private void Update()
	{
		// Loop through all quests and check if they can be started, advanced or finished
		foreach (Quest quest in questMap.Values)
		{
			// Check if the quest is in a state where it can be started
			if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
			{
				ChangeQuestState(quest.info.id, QuestState.CAN_START);
			}
		}
	}

	private void ChangeQuestState(string id, QuestState state)
	{
		Quest quest = GetQuestByID(id);
		if (quest != null)
		{
			quest.state = state;
			GameEventsManager.instance.questEvents.QuestStateChanged(quest);
		}
	}

	private void StartQuest(string questID)
	{
		// TODO: Implement quest starting logic
		Debug.Log("Starting quest with ID: " + questID);

		Quest quest = GetQuestByID(questID);
		quest.InstantiateCurrentQuestStep(transform);
		ChangeQuestState(questID, QuestState.IN_PROGRESS);
	}

	private void AdvanceQuest(string questID)
	{
		// TODO: Implement quest advancing logic
		Debug.Log("Advancing quest with ID: " + questID);

		Quest quest = GetQuestByID(questID);
		quest.MoveToNextStep();

		// If there are more steps, instantiate the next step
		if (quest.CurrentStepExists())
		{
			Debug.Log("Advancing to next step for quest with ID: " + questID);
			quest.InstantiateCurrentQuestStep(transform);
		}
		// If there are no more steps, finish the quest
		else
		{
			Debug.Log("No more steps for quest with ID: " + questID);
			ChangeQuestState(questID, QuestState.CAN_FINISH);
		}
	}

	private void FinishQuest(string questID)
	{
		// TODO: Implement quest finishing logic
		Debug.Log("Finishing quest with ID: " + questID);

		Quest quest = GetQuestByID(questID);
		ClaimReward(quest);
		ChangeQuestState(questID, QuestState.FINISHED);
	}

	private void ClaimReward(Quest quest)
	{
		GameEventsManager.instance.goldEvents.GoldGained(quest.info.goldReward);
		GameEventsManager.instance.playerEvents.ExperienceGained(quest.info.experienceReward);
	}

	private void QuestStepStateChanged(string id, int stepIndex, QuestStepState questStepState)
	{
		Quest quest = GetQuestByID(id);
		quest.StoreQuestStepState(questStepState, stepIndex);
		ChangeQuestState(id, quest.state);
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

			// Load the quest and add it to the map
			idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
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

	private void OnApplicationQuit()
	{
		// Save the state of all quests when the application quits
		foreach (Quest quest in questMap.Values)
		{
			Debug.Log("Saving quest: " + quest.info.id);
			SaveQuest(quest);
		}
	}

	private void SaveQuest(Quest quest)
	{
		try
		{
			QuestData questData = quest.GetQuestData();
			string serializedData = JsonUtility.ToJson(questData);
			PlayerPrefs.SetString(quest.info.id, serializedData);

			Debug.Log(serializedData);
		}
		catch (System.Exception e)
		{
			Debug.LogError("Failed to save quest with id " + quest.info.id + ": " + e);
		}
	}

	private Quest LoadQuest(QuestInfoSO questInfo)
	{
		Quest quest = null;
		try
		{
			// Load quest from saved data
			if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState)
			{
				string serializedData = PlayerPrefs.GetString(questInfo.id);
				QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
				quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
			}
			// Create a new quest if no saved data exists
			else
			{
				quest = new Quest(questInfo);
			}
		}
		catch (System.Exception e)
		{
			Debug.LogError("Failed to load quest with id " + questInfo.id + ": " + e);
		}
		return quest;
	}
}
