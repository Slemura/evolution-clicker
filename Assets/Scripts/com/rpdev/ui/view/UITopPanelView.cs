using System.Collections;
using System.Collections.Generic;
using com.rpdev.foundation.model;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace com.rpdev.ui.view {

	public class UITopPanelView : MonoBehaviour {

		[SerializeField]
		protected TMP_Text units_count_txt;
		
		[SerializeField]
		protected TMP_Text coins_count_txt;

		[Inject]
		protected void Construct(LocationModel location_model, PlayerModel player_model) {
			
			player_model.Coins.Subscribe(coins => { coins_count_txt.text = coins.ToString(); });
			
			location_model.TotalUnitCount.Subscribe(units_count => {
				units_count_txt.text = units_count.ToString();
			});
		}

	}
}