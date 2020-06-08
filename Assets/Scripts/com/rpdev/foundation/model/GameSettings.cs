using System.Collections;
using System.Collections.Generic;
using com.rpdev.foundation.controller;
using com.rpdev.foundation.view.unit;
using com.rpdev.ui;
using UnityEngine;
using Zenject;

namespace com.rpdev.foundation.model {

	[CreateAssetMenu(fileName = "GameSettings", menuName = "Models/GameSettings")]
	public class GameSettings : ScriptableObjectInstaller {
		
		public LocationController.Settings location_settings;
		public CreatureView.Settings creatures_settings;
		
		public override void InstallBindings() {
			
			Container.BindInstance(location_settings).AsSingle();
			Container.BindInstance(creatures_settings).AsSingle();
		}
	}
	
}