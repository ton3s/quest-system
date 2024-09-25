using UnityEngine;
using System;

public class QuestEvents
{
	// Event for starting a quest
	public event Action<string> onStartQuest;

	// Start a quest by ID
	public void StartQuest(string questID)
	{
		// Check if the event has any subscribers
		if (onStartQuest != null)
		{
			onStartQuest(questID);
		}
	}

	// Event for advancing a quest
	public event Action<string> onAdvanceQuest;

	// Advance a quest by ID
	public void AdvanceQuest(string questID)
	{
		// Check if the event has any subscribers
		if (onAdvanceQuest != null)
		{
			onAdvanceQuest(questID);
		}
	}

	// Event for finishing a quest
	public event Action<string> onFinishQuest;

	// Finish a quest by ID
	public void FinishQuest(string questID)
	{
		// Check if the event has any subscribers
		if (onFinishQuest != null)
		{
			onFinishQuest(questID);
		}
	}

	// Event for quest changing state
	public event Action<Quest> onQuestStateChanged;

	// Change the state of a quest
	public void QuestStateChanged(Quest quest)
	{
		// Check if the event has any subscribers
		if (onQuestStateChanged != null)
		{
			onQuestStateChanged(quest);
		}
	}
}