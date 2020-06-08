using System.Collections;
using System.Collections.Generic;
using com.rpdev.foundation.controller;
using com.rpdev.foundation.controller.commands;
using com.rpdev.foundation.model;
using com.rpdev.foundation.view.unit;
using com.rpdev.ui;
using com.rpdev.ui.view;
using UnityEngine;
using Zenject;

namespace com.rpdev.foundation {

	public class MainInstaller : MonoInstaller {

		[SerializeField]
		protected UIContextFacade ui_context;
		
		public override void InstallBindings() {
			
			SignalBusInstaller.Install(Container);
			
			Container.DeclareSignal<SpawnCreatureFromCrateSignal>();
			Container.DeclareSignal<SpawnCreatureFromMergeSignal>();
			Container.DeclareSignal<CollectCoinSignal>();
			
			Container.BindInterfacesAndSelfTo<LocationController>().AsSingle();
			
		#if UNITY_STANDALONE || UNITY_EDITOR
			Container.BindInterfacesTo<MouseInputController>().AsSingle();
		#elif UNITY_ANDROID
			Container.BindInterfacesTo<TouchInputController>().AsSingle();
		#endif
			
			Container.Bind<LocationModel>().AsSingle();
			Container.Bind<UIContextFacade>().FromInstance(ui_context);
			Container.Bind<PlayerModel>().AsSingle();
			
			
			Container.BindFactory<UnitView, IUnitView, UnitView.Factory>()
			         .FromFactory<MainFactory<UnitView, IUnitView>>();
			
			Container.BindSignal<SpawnCreatureFromCrateSignal>()
			         .ToMethod<SpawnCreatureFromCrateCommand>((command, param) => command.Execute(param.crate_view))
			         .From(x => x.AsSingle());
			
			Container.BindSignal<CollectCoinSignal>()
			         .ToMethod<CollectCoinsCommand>((command, param) => command.Execute(param.coin_view))
			         .From(x => x.AsSingle());
			
			Container.BindSignal<SpawnCreatureFromMergeSignal>()
			         .ToMethod<SpawnCreatureFromMergeCommand>((command, param) => command.Execute(param))
			         .From(x => x.AsSingle());
		}
	}
}