using System.Collections;
using System.Collections.Generic;
using com.rpdev.foundation.model;
using com.rpdev.foundation.view.unit;
using UnityEngine;
	
namespace com.rpdev.foundation.controller.commands {

	public class CollectCoinsCommand : ICustomCommand<ICoinView> {

		protected readonly PlayerModel player_model;
		protected readonly LocationModel location_model;
		
		public CollectCoinsCommand(PlayerModel   player_model,
		                           LocationModel location_model) {

			this.player_model  = player_model;
			this.location_model = location_model;
		}
		
		public void Execute(ICoinView data) {
			player_model.AddCoins(data.CoinCount);
			location_model.RemoveUnit(data);
		}
	}
}