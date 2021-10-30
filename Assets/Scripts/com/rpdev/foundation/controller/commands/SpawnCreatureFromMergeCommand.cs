using com.rpdev.foundation.model;
using com.rpdev.foundation.view.unit;
using com.rpdev.module.common.commands;
using UnityEngine;
	
namespace com.rpdev.foundation.controller.commands {

	public class SpawnCreatureFromMergeCommand : ICustomCommand<ICreatureView, ICreatureView> {

		private readonly LocationModel _location_model;
		private readonly LocationController _location_controller;
		
		public SpawnCreatureFromMergeCommand(LocationModel location_model, LocationController location_controller) {
			_location_controller = location_controller;
			_location_model = location_model;
		}
		
		public void Execute(ICreatureView first_creature, ICreatureView second_creature) {
			if (first_creature.Level != second_creature.Level) return;
			
			Vector3 last_pos = first_creature.Position;
			int     level    = first_creature.Level;
			
			_location_model.RemoveUnit(first_creature);
			_location_model.RemoveUnit(second_creature);

			_location_controller.SpawnCreature(last_pos, level + 1);
		}
	}
}