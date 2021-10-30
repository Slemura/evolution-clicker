using System;
using com.mikecann.scripts;
using com.rpdev.foundation.controller;
using com.rpdev.ui;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace com.rpdev.foundation.view.unit {

	public interface ICreatureView : IUnitView {
		int Level { get; }
		void SetLevel(int level);
		void SetDrag(bool is_drag);
	}
	
	public class CreatureView : UnitView, ICreatureView {

		private int                _current_level = 1;
		private Vector3            _next_jump_pos;
		private Vector3            _start_drag_pos;
		private Bounds             _camera_bounds;
		private Settings           _settings;
		private LocationController _location_controller;

		
		private float CurrentScale => _settings.start_scale + _current_level * _settings.scale_from_level;
		public  int   Level        => _current_level;

		[Inject]
		protected void Construct(Settings           settings, 
		                         LocationController location_controller) {
		    
			this._settings            = settings;
			this._location_controller = location_controller;
		}
	    
		[Inject]
		public override void Initialize() {
			_camera_bounds = Camera.main.OrthographicBounds();
		}
		
	    public override void Interact() {
		    
		    if (_start_drag_pos == transform.position) {
				DropCoin();   
		    }
	    }

	    private void DropCoin() {
		    if (Level == 1) {
			    _location_controller.SpawnCoin(_settings.base_coin_value, transform.position);
		    } else {
			    _location_controller.SpawnCoin(_settings.base_coin_value * Mathf.RoundToInt(Mathf.Pow(2, Level - 1)), transform.position);
		    }
	    }

	    public void SetLevel(int level) {
		    
		    _current_level        = level;
		    transform.localScale = new Vector3(CurrentScale, CurrentScale, CurrentScale);

		    float calc_level = _current_level;
		    Color color;
		    
		    if (calc_level % 4 == 0) {
			    color = Color.white;
		    } else if (calc_level % 3 == 0) {
			    color = Color.HSVToRGB(0.3f, 0.5f,  0.8f);
		    } else if (calc_level % 2 == 0) {
			    color = Color.HSVToRGB(0.5f, 0.5f,  1f);
		    } else {
			    color = Color.HSVToRGB(0.8f, 1f,  1f);
		    }
		    
		    SpriteRenderer.color = color;
		    
		    CreateAnimationStream();
	    }

	    public void SetDrag(bool is_drag) {
		    
		    _start_drag_pos = transform.position;
		    
		    StopAnimation();
		    
		    if (is_drag) {
			    transform.DOShakeScale(0.5f, 0.8f).SetEase(Ease.InCirc).OnComplete(() => {
				    transform.DOScale(new Vector3(CurrentScale, CurrentScale, CurrentScale), 0.3f);
			    });
		    } else {
			    if (transform.localScale != new Vector3(CurrentScale, CurrentScale, CurrentScale)) {
				    transform.DOScale(new Vector3(CurrentScale, CurrentScale, CurrentScale), 0.2f)
				             .OnComplete(() => {
					              CreateAnimationStream(false);
				              });
			    } else {
				    CreateAnimationStream(false);				    
			    }
		    }
	    }

	    protected override void CreateAnimationStream(bool immediately_play = true) {
		    
		    AnimationDisposable?.Dispose();
		    
			AnimationDisposable = Observable.Timer(TimeSpan.FromSeconds(1f))
									  .Repeat()
									  .Subscribe(_ => {
										  DropCoin();
										  CreateRandomAnimation();
									  });
		    
		    if(immediately_play)
				CreateRandomAnimation(false);
	    }

		private void CreateRandomAnimation(bool is_move = true) {
		    
		    DOTween.Kill(transform, false);
			AnimationSequence?.Kill();
			
			transform.DOScale(new Vector3(CurrentScale * 0.9f, CurrentScale * 1.1f, 1), 0.2f).SetEase(Ease.InCirc)
			         .OnComplete(() => {
				          transform.DOScale(new Vector3(CurrentScale * 1.1f, CurrentScale * 0.9f, 1), 0.1f)
				                   .SetEase(Ease.InCirc)
				                   .OnComplete(() => {
					                    transform.DOScale(new Vector3(CurrentScale, CurrentScale, CurrentScale), 0.1f)
					                             .SetEase(Ease.InCirc);
				                    });
			          });

			transform.DOJump(is_move ? _next_jump_pos : transform.position, 0.3f, 1, 0.3f)
					 .SetEase(Ease.InCirc)
					 .OnComplete(() => {

						 _next_jump_pos = transform.position +
										  new Vector3(Random.Range(-_settings.move_speed_range, _settings.move_speed_range), Random.Range(-0.2f, 0.2f));

						 if (Bounds.min.x < _camera_bounds.min.x) {
							 _next_jump_pos = _next_jump_pos.SetX(_camera_bounds.min.x + 0.1f);
						 } else if (Bounds.max.x > _camera_bounds.max.x) {
							 _next_jump_pos = _next_jump_pos.SetX(_camera_bounds.max.x - _camera_bounds.max.x - 0.1f);
						 }

						 if (Bounds.min.y < _camera_bounds.min.y) {
							 _next_jump_pos = _next_jump_pos.SetY(_camera_bounds.min.y + 0.1f);
						 } else if (Bounds.max.y > _camera_bounds.max.y) {
							 _next_jump_pos = _next_jump_pos.SetY(_camera_bounds.max.y - _camera_bounds.max.y - 0.1f);
						 }
					 });
		}

	    public override void SetPosition(Vector3 position) {
		    _next_jump_pos = position;
		    base.SetPosition(position);
	    }

	    [Serializable]
	    public struct Settings {
		    public int base_coin_value;
		    public float start_scale;
		    public float scale_from_level;
		    public float move_speed_range;
	    }
	}
}