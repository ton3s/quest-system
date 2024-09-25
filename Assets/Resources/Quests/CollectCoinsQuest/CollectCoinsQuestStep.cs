using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCoinsQuestStep : QuestStep
{
	private int coinsCollected = 0;
	private int coinsToComplete = 5;

	private void OnEnable()
	{
		GameEventsManager.instance.miscEvents.onCoinCollected += CoinCollected;
	}

	private void OnDisable()
	{
		GameEventsManager.instance.miscEvents.onCoinCollected -= CoinCollected;
	}

	private void CoinCollected()
	{
		coinsCollected++;

		if (coinsCollected >= coinsToComplete)
		{
			FinishQuestStep();
		}
	}
}
