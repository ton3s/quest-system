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

	private bool playerIsNear = false;
	private string questId;
	private QuestState currentQuestState;

	private void Awake()
	{
		questId = questInfoForPoint.id;
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
		if (playerIsNear)
		{

		}
	}

	private void QuestStateChanged(Quest quest)
	{
		// Update the current quest state if the quest ID matches
		if (quest.info.id == questId)
		{
			currentQuestState = quest.state;
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
