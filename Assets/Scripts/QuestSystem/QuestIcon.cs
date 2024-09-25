using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestIcon : MonoBehaviour
{
	[Header("Icons")]
	[SerializeField] private GameObject requirementsNotMetToStartIcon;
	[SerializeField] private GameObject canStartIcon;
	[SerializeField] private GameObject requirementsNotMetToFinishIcon;
	[SerializeField] private GameObject canFinishIcon;

	public void SetState(QuestState newState, bool startPoint, bool finishPoint)
	{
		// Set all icons to inactive
		requirementsNotMetToStartIcon.SetActive(false);
		canStartIcon.SetActive(false);
		requirementsNotMetToFinishIcon.SetActive(false);
		canFinishIcon.SetActive(false);

		// Set the correct icon active
		switch (newState)
		{
			case QuestState.REQUIREMENTS_NOT_MET:
				if (startPoint)
				{
					requirementsNotMetToStartIcon.SetActive(true);
				}
				break;
			case QuestState.CAN_START:
				if (startPoint)
				{
					canStartIcon.SetActive(true);
				}
				break;
			case QuestState.IN_PROGRESS:
				if (finishPoint)
				{
					requirementsNotMetToFinishIcon.SetActive(true);
				}
				break;
			case QuestState.CAN_FINISH:
				if (finishPoint)
				{
					canFinishIcon.SetActive(true);
				}
				break;
			case QuestState.FINISHED:
				break;
			default:
				Debug.LogWarning("Quest state not recognized for switch icon: " + newState);
				break;
		}
	}
}
