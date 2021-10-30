using com.rpdev.foundation.model;
using com.rpdev.foundation.view.unit;
using com.rpdev.module.common.commands;

namespace com.rpdev.foundation.controller.commands {

	public class CollectCoinsCommand : ICustomCommand<ICoinView> {
		
		private readonly PlayerModel   _player_model;
		private readonly LocationModel _location_model;
		
		public CollectCoinsCommand(PlayerModel   player_model,
		                           LocationModel location_model) {

			this._player_model   = player_model;
			this._location_model = location_model;
		}
		
		public void Execute(ICoinView data) {
			_player_model.AddCoins(data.CoinCount);
			_location_model.RemoveUnit(data);
		}
	}
}