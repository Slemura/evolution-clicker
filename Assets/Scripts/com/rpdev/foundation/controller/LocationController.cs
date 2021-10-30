using System;
using com.mikecann.scripts;
using com.rpdev.foundation.model;
using com.rpdev.foundation.view.unit;
using com.rpdev.input;
using UniRx;
using UniTools;
using UnityEngine;
using Zenject;

namespace com.rpdev.foundation.controller {

	public class LocationController : IInitializable, IDisposable {
		
		private readonly UnitView.Factory _unit_factory;
		private readonly Settings         _settings;
		private readonly IInputController _input_controller;
		private readonly LocationModel    _location_model;
		private readonly SignalBus        _signal_bus;

		private Bounds              _camera_bounds;
		private ICreatureView       _current_drag_creature;
		private IDisposable         _crate_stream;
		private CompositeDisposable _input_stream;
		
		public LocationController(UnitView.Factory unit_factory,
								  Settings settings,
								  LocationModel location_model,
								  SignalBus signal_bus,
								  IInputController input_controller) {


			this._signal_bus       = signal_bus;
			this._location_model   = location_model;
			this._input_controller = input_controller;
			this._unit_factory     = unit_factory;
			this._settings         = settings;
			this._camera_bounds    = Camera.main.OrthographicBounds();
		}

		public void Initialize() {
			
			StartCrateSpawner();
			
			_input_stream = new CompositeDisposable();
			
			//Input release logic
			_input_controller.IsRelease
							 .Where(release => release == true)
							 .Subscribe(_ => {
								 if (_current_drag_creature == null) {

									 IUnitView found_unit = _location_model.GetIntersectInputPositionUnit(_input_controller.InputWorldPosition.Value);
									 found_unit?.Interact();

								 } else {

									 ICreatureView collide_creature = _location_model.GetIntersectInputPositionCreature(_current_drag_creature);

									 if (collide_creature != null) {
										 if (collide_creature.Level == _current_drag_creature.Level) {
											 _signal_bus.Fire(new SpawnCreatureFromMergeSignal {
												 first_creature  = _current_drag_creature,
												 second_creature = collide_creature
											 });
										 }
									 } else {
										 _current_drag_creature.Interact();
									 }

									 _current_drag_creature.SetDrag(false);
									 _current_drag_creature = null;
								 }
							 })
							 .AddTo(_input_stream);

			//Input press logic
			_input_controller.IsPressed
							 .Where(press => press)
							 .Subscribe(_ => {

								 if (_current_drag_creature == null) {

									 CrateView possible_crate =
										 _location_model.GetIntersectInputPositionUnit(_input_controller.InputWorldPosition.Value) as CrateView;
									 
									 if (possible_crate == null) {

										 ICreatureView unit = _location_model.GetIntersectInputPositionCreature(_input_controller.InputWorldPosition.Value);

										 if (unit != null) {
											 _current_drag_creature = unit;
											 _current_drag_creature?.SetDrag(true);
										 }
									 }
								 }
							 })
							 .AddTo(_input_stream);

			//Input move logic
			_input_controller.InputWorldPosition
							 .Where(pos => _input_controller.IsPressed.Value)
							 .Subscribe(pos => {

								 if (_current_drag_creature == null) {
									 ICoinView[] coin_view = _location_model.GetIntersectInputPositionCoin(pos);

									 if (coin_view.Length > 0) {
										 coin_view.ForEach(coin => {
											 if (!coin.IsCollected) {
												 coin.Interact();
											 }
										 });
									 }
								 } else {
									 _current_drag_creature.SetPosition(pos.VectorBoundInBound(_current_drag_creature.Bounds, _camera_bounds).SetZ(_current_drag_creature.Position.z));
								 }
							 })
							 .AddTo(_input_stream);
			
			SpawnCrate();
		}

		private void StartCrateSpawner() {
			
			_crate_stream = Observable.Timer(TimeSpan.FromSeconds(_settings.crate_creation_interval))
			                         .Repeat()
			                         .Where(_ => _location_model.TotalUnitCount.Value < _settings.max_creatures)
			                         .Subscribe(_ => {
				                         SpawnCrate(); 
			                          });
		}

		private void SpawnCrate() {
			SpawnUnit(_settings.crate_view, new Vector3 (UnityEngine.Random.Range(_camera_bounds.min.x + _settings.crate_view.Bounds.max.x, _camera_bounds.max.x - _settings.crate_view.Bounds.max.x),
			                                                    UnityEngine.Random.Range(_camera_bounds.min.y + _settings.crate_view.Bounds.max.y, _camera_bounds.max.y - _settings.crate_view.Bounds.max.y),
																0));
			
		}

		public void SpawnCoin(int coin_count, Vector3 origin_pos) {
			ICoinView coin = SpawnUnit(_settings.coin_view, origin_pos) as ICoinView;
			coin?.InitCoin(coin_count, origin_pos);
		}

		public ICreatureView SpawnCreature(Vector3 position, int level = 1) {
			ICreatureView creature = SpawnUnit(_settings.creature_view, position) as ICreatureView;
			creature?.SetLevel(level);
			return creature;
		}

		private IUnitView SpawnUnit(UnitView view, Vector3 position) {
			
			IUnitView unit = _unit_factory.Create(view);
			unit.SetPosition(position);
			unit.Initialize();
			_location_model.AddUnit(unit);
			
			return unit;
		}

		public void Dispose() {
			_crate_stream?.Dispose();
			_input_stream?.Dispose();
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