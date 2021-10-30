using System;
using com.rpdev.foundation.model;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace com.rpdev.ui.view {

	public class UITopPanelView : MonoBehaviour, IInitializable, IDisposable {

		[SerializeField]
		protected TMP_Text units_count_txt;
		
		[SerializeField]
		protected TMP_Text coins_count_txt;

		private LocationModel _location_model;
		private PlayerModel   _player_model;

		private CompositeDisposable _process_flow;
		
		[Inject]
		protected void Construct(LocationModel location_model, PlayerModel player_model) {
			_location_model = location_model;
			_player_model   = player_model;
		}

		[Inject]
		public void Initialize() {
			_process_flow = new CompositeDisposable();
			
			_player_model.Coins
						 .Subscribe(coins => {
							 coins_count_txt.text = coins.ToString();
						 })
						 .AddTo(_process_flow);

			_location_model.TotalUnitCount
						   .Subscribe(units_count => {
							   units_count_txt.text = units_count.ToString();
						   })
						   .AddTo(_process_flow);
		}

		public void Dispose() {
			_process_flow?.Dispose();
		}
	}
}