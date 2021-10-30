using System.Collections;
using System.Collections.Generic;
using com.rpdev.foundation.controller;
using UnityEngine;
using Zenject;

namespace com.rpdev.foundation.view.unit {

	public class CrateView : UnitView {

		public override void Interact() {
		
			CreateRandomAnimation();
			
			SignalBus.Fire(new SpawnCreatureFromCrateSignal {
				crate_view = this
			});
		}
	}
}