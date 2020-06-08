using System.Collections;
using System.Collections.Generic;
using com.rpdev.foundation.model;
using com.rpdev.foundation.view.unit;
using UnityEngine;
	
namespace com.rpdev.foundation.controller.commands {

	public class SpawnCreatureFromCrateCommand : ICustomCommand<IUnitView> {

		protected readonly LocationController location_controller;
		protected readonly LocationModel location_model;
		
		public SpawnCreatureFromCrateCommand(LocationController location_controller,
		                                     LocationModel location_model) {

			this.location_model = location_model;
			this.location_controller = location_controller;
		}
		
		public void Execute(IUnitView crate) {
			location_controller.SpawnCreature(crate.Position);
			location_model.RemoveUnit(crate);
		}
	}
}