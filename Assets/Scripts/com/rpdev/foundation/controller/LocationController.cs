using System;
using com.mikecann.scripts;
using com.rpdev.foundation.model;
using com.rpdev.foundation.view.unit;
using UniRx;
using UniTools;
using UnityEngine;
using Zenject;

namespace com.rpdev.foundation.controller {

	public class LocationController : IInitializable, IDisposable {

		protected readonly UnitView.Factory unit_factory;
		protected readonly Settings settings;
		protected readonly IInputController input_controller;
		protected readonly LocationModel location_model;
		protected readonly SignalBus signal_bus;
		
		protected Bounds camera_bounds;
		protected ICreatureView current_drag_creature;
		protected IDisposable crate_stream;
		protected CompositeDisposable input_stream;
		
		public LocationController(UnitView.Factory unit_factory,
								  Settings settings,
								  LocationModel location_model,
								  SignalBus signal_bus,
								  IInputController input_controller) {


			this.signal_bus       = signal_bus;
			this.location_model   = location_model;
			this.input_controller = input_controller;
			this.unit_factory     = unit_factory;
			this.settings         = settings;
			this.camera_bounds    = Camera.main.OrthographicBounds();
		}

		public void Initialize() {
			StartCrateSpawner();
			
			input_stream = new CompositeDisposable();
			
			//Input release logic
			input_controller.IsRelease
			                .Where(release => release == true)
			                .Subscribe(_ => {
				                 if (current_drag_creature == null) {
					                 
					                 IUnitView foundUnit = location_model.GetIntersectInputPositionUnit(input_controller.InputWorldPosition.Value);
					                 foundUnit?.Interact();
					                 
				                 } else {
					                 
					                 ICreatureView collideCreature = location_model.GetIntersectInputPositionCreature(current_drag_creature);

					                 if (collideCreature != null) {
						                 if (collideCreature.Level == current_drag_creature.Level) {
							                 signal_bus.Fire(new SpawnCreatureFromMergeSignal {
								                 first_creature  = current_drag_creature,
								                 second_creature = collideCreature
							                 });
						                 }
					                 } else {
						                 current_drag_creature.Interact();
					                 }
					                 
					                 current_drag_creature.SetDrag(false);
					                 current_drag_creature = null;
				                 }
			                 })
			                .AddTo(input_stream);

			//Input press logic
			input_controller.IsPressed
			                .Where(press => press)
			                .Subscribe(_ => {
				                 
				                 if (current_drag_creature == null) {
					                 IUnitView unit = location_model.GetIntersectInputPositionUnit(input_controller.InputWorldPosition.Value);
					                 if (unit is ICreatureView view) {
						                 current_drag_creature = view;
						                 current_drag_creature.SetDrag(true);
					                 }
					                 
				                 }
			                 })
			                .AddTo(input_stream);

			//Input move logic
			input_controller.InputWorldPosition
			                .Where(pos => input_controller.IsPressed.Value)
			                .Subscribe(pos => {
				                 
				                 if (current_drag_creature == null) {
					                 ICoinView[] coinView = location_model.GetIntersectInputPositionCoin(pos);

					                 if (coinView.Length > 0) {
						                 coinView.ForEach(coin => {
							                 if (!coin.IsCollected) {
								                 coin.Interact();
							                 }
						                 });
					                 }
					                 
				                 } else {
					                 current_drag_creature.SetPosition(pos.VectorBoundInBound(current_drag_creature.Bounds, camera_bounds).SetZ(current_drag_creature.Position.z));    
				                 }
			                 })
			                .AddTo(input_stream);
			
			SpawnCrate();
		}

		protected void StartCrateSpawner() {
			
			crate_stream = Observable.Timer(TimeSpan.FromSeconds(settings.crate_creation_interval))
			                         .Repeat()
			                         .Where(_ => location_model.TotalUnitCount.Value < settings.max_creatures)
			                         .Subscribe(_ => {
				                         SpawnCrate(); 
			                          });
		}

		protected void SpawnCrate() {
			IUnitView crate  = SpawnUnit(settings.crate_view, new Vector3 (UnityEngine.Random.Range(camera_bounds.min.x + settings.crate_view.Bounds.max.x, camera_bounds.max.x - settings.crate_view.Bounds.max.x),
			                                                               UnityEngine.Random.Range(camera_bounds.min.y + settings.crate_view.Bounds.max.y, camera_bounds.max.y - settings.crate_view.Bounds.max.y),
			                                                               0
			                                                              ));
			
		}

		public void SpawnCoin(int coin_count, Vector3 origin_pos) {
			ICoinView coin = SpawnUnit(settings.coin_view, origin_pos) as ICoinView;
			coin.InitCoin(coin_count, origin_pos);
		}

		public ICreatureView SpawnCreature(Vector3 position, int level = 1) {
			ICreatureView creature = SpawnUnit(settings.creature_view, position) as ICreatureView;
			creature.SetLevel(level);
			return creature;
		}

		public IUnitView SpawnUnit(UnitView view, Vector3 position) {
			
			IUnitView unit = unit_factory.Create(view);
			unit.SetPosition(position);
			unit.Initialize();
			location_model.AddUnit(unit);
			
			return unit;
		}

		public void Dispose() {
			crate_stream?.Dispose();
			input_stream?.Dispose();
		}
		
		[Serializable]
		public struct Settings {
			public CrateView crate_view;
			public CreatureView creature_view;
			public CoinView coin_view;
			public int   max_creatures;
			public float crate_creation_interval;
		}
	}
}