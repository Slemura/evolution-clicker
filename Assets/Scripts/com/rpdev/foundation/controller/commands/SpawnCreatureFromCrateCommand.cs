using com.rpdev.foundation.model;
using com.rpdev.foundation.view.unit;
using com.rpdev.module.common.commands;

namespace com.rpdev.foundation.controller.commands {

	public class SpawnCreatureFromCrateCommand : ICustomCommand<IUnitView> {
		
		private readonly LocationController _location_controller;
		private readonly LocationModel      _location_model;
		
		public SpawnCreatureFromCrateCommand(LocationController location_controller,
		                                     LocationModel location_model) {

			this._location_model = location_model;
			this._location_controller = location_controller;
		}
		
		public void Execute(IUnitView crate) {
			_location_controller.SpawnCreature(crate.Position);
			_location_model.RemoveUnit(crate);
		}
	}
}