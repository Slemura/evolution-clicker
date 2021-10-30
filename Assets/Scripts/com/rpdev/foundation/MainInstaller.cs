using com.rpdev.foundation.controller;
using com.rpdev.foundation.controller.commands;
using com.rpdev.foundation.model;
using com.rpdev.foundation.view.unit;
using com.rpdev.input;
using com.rpdev.module.extensions;
using com.rpdev.ui;
using UnityEngine;
using Zenject;

namespace com.rpdev.foundation {

	public class MainInstaller : MonoInstaller {

		[SerializeField]
		private UIContextFacade ui_context;
		
		public override void InstallBindings() {
			
			SignalBusInstaller.Install(Container);
			
			Container.DeclareSignal<SpawnCreatureFromCrateSignal>();
			Container.DeclareSignal<SpawnCreatureFromMergeSignal>();
			Container.DeclareSignal<CollectCoinSignal>();
			
			Container.BindInterfacesAndSelfTo<LocationController>().AsSingle();
			
		#if UNITY_STANDALONE || UNITY_EDITOR
			Container.BindInterfacesTo<PCInputController>().AsSingle();
		#elif UNITY_ANDROID
			Container.BindInterfacesTo<MobileInputController>().AsSingle();
		#endif
			
			Container.Bind<LocationModel>().AsSingle();
			Container.Bind<PlayerModel>().AsSingle();
			
			Container.BindInterfacesTo<UIContextFacade>().FromInstance(ui_context);

			Container.BindFactory<UnitView, IUnitView, UnitView.Factory>()
					 .FromFactory<MainFactory<UnitView, IUnitView>>();
			
			Container.BindComplexSignalToCommand<SpawnCreatureFromCrateSignal, SpawnCreatureFromCrateCommand, IUnitView>( "crate_view");
			Container.BindComplexSignalToCommand<SpawnCreatureFromMergeSignal, SpawnCreatureFromMergeCommand, ICreatureView, ICreatureView>(new[] { "first_creature", "second_creature" });
			Container.BindComplexSignalToCommand<CollectCoinSignal, CollectCoinsCommand, ICoinView>("coin_view");
		}
	}
}