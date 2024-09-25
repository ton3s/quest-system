using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  A point in the game world that the player can interact with to start a quest.
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
	[Header("Quest")]
	[SerializeField] private QuestInfoSO questInfoForPoint;

	[Header("Config")]
	[SerializeField] private bool startPoint = false;
	[SerializeField] private bool finishPoint = false;

	private bool playerIsNear = false;
	private string questId;
	private QuestState currentQuestState;

	private QuestIcon questIcon;

	private void Awake()
	{
		questId = questInfoForPoint.id;
		questIcon = GetComponentInChildren<QuestIcon>();
	}

	private void OnEnable()
	{
		GameEventsManager.instance.questEvents.onQuestStateChanged += QuestStateChanged;
		GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
	}

	private void OnDisable()
	{
		GameEventsManager.instance.questEvents.onQuestStateChanged -= QuestStateChanged;
		GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
	}

	private void SubmitPressed()
	{
		if (!playerIsNear) return;

		// Check if the player can start the quest
		if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
		{
			GameEventsManager.instance.questEvents.StartQuest(questId);
		}
		// Check if the player can finish the quest
		else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
		{
			GameEventsManager.instance.questEvents.FinishQuest(questId);
		}
	}

	private void QuestStateChanged(Quest quest)
	{
		// Update the current quest state if the quest ID matches
		if (quest.info.id == questId)
		{
			currentQuestState = quest.state;
			questIcon.SetState(currentQuestState, startPoint, finishPoint);
		}
	}

	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (otherCollider.CompareTag("Player"))
		{
			playerIsNear = true;
		}
	}

	private void OnTriggerExit2D(Collider2D otherCollider)
	{
		if (otherCollider.CompareTag("Player"))
		{
			playerIsNear = false;
		}
	}
}
