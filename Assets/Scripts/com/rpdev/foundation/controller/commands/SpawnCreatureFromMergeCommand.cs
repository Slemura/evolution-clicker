using System.Collections;
using System.Collections.Generic;
using com.rpdev.foundation.model;
using UnityEngine;
	
namespace com.rpdev.foundation.controller.commands {

	public class SpawnCreatureFromMergeCommand : ICustomCommand<SpawnCreatureFromMergeSignal> {

		private readonly LocationModel _location_model;
		private readonly LocationController _location_controller;
		
		public SpawnCreatureFromMergeCommand(LocationModel location_model, LocationController location_controller) {
			_location_controller = location_controller;
			_location_model = location_model;
		}
		
		public void Execute(SpawnCreatureFromMergeSignal data) {
			if (data.first_creature.Level != data.second_creature.Level) return;
			
			Vector3 lastPos = data.first_creature.Position;
			int     level   = data.first_creature.Level;
			
			_location_model.RemoveUnit(data.first_creature);
			_location_model.RemoveUnit(data.second_creature);

			_location_controller.SpawnCreature(lastPos, level + 1);
		}
	}
}