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
		
		protected Vector3 next_jump_pos;
		protected Vector3 start_drag_pos;
		
		protected int current_level = 1;
		protected Bounds camera_bounds;
		protected float CurrentScale => settings.start_scale + current_level * settings.scale_from_level;
		protected Settings settings;
		protected LocationController location_controller;

		
		public int Level => current_level;

		[Inject]
		protected void Construct(Settings           settings, 
		                         LocationController location_controller) {
		    
			this.settings            = settings;
			this.location_controller = location_controller;
		}
	    
		[Inject]
		public override void Initialize() {
			camera_bounds = Camera.main.OrthographicBounds();
		}
		
	    public override void Interact() {
		    
		    if (start_drag_pos == transform.position) {
				DropCoin();   
		    }
	    }

	    protected void DropCoin() {
		    if (Level == 1) {
			    location_controller.SpawnCoin(settings.base_coin_value, transform.position);
		    } else {
			    location_controller.SpawnCoin(settings.base_coin_value * Mathf.RoundToInt(Mathf.Pow(2, Level - 1)), transform.position);
		    }
	    }

	    public void SetLevel(int level) {
		    
		    current_level        = level;
		    transform.localScale = new Vector3(CurrentScale, CurrentScale, CurrentScale);

		    float calcLevel = current_level;
		    Color color;
		    
		    if (calcLevel % 4 == 0) {
			    color = Color.white;
		    } else if (calcLevel % 3 == 0) {
			    color = Color.HSVToRGB(0.3f, 0.5f,  0.8f);
		    } else if (calcLevel % 2 == 0) {
			    color = Color.HSVToRGB(0.5f, 0.5f,  1f);
		    } else {
			    color = Color.HSVToRGB(0.8f, 1f,  1f);
		    }
		    
		    SpriteRenderer.color = color;
		    
		    CreateAnimationStream();
	    }

	    public void SetDrag(bool is_drag) {
		    
		    start_drag_pos = transform.position;
		    
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

	    protected override void CreateAnimationStream(bool is_immidiate_play = true) {
		    
		    animation_stream?.Dispose();
		    
		    animation_stream = Observable.Timer(TimeSpan.FromSeconds(1f))
		                                 .Repeat()
		                                 .Subscribe(_ => {
			                                  DropCoin();
			                                  CreateRandomAnimation();
		                                  });
		    
		    if(is_immidiate_play)
				CreateRandomAnimation(false);
	    }

	    protected void CreateRandomAnimation(bool is_move = true) {
		    
		    DOTween.Kill(transform, false);
			sequence?.Kill();
			
			transform.DOScale(new Vector3(CurrentScale * 0.9f, CurrentScale * 1.1f, 1), 0.2f).SetEase(Ease.InCirc)
			         .OnComplete(() => {
				          transform.DOScale(new Vector3(CurrentScale * 1.1f, CurrentScale * 0.9f, 1), 0.1f)
				                   .SetEase(Ease.InCirc)
				                   .OnComplete(() => {
					                    transform.DOScale(new Vector3(CurrentScale, CurrentScale, CurrentScale), 0.1f)
					                             .SetEase(Ease.InCirc);
				                    });
			          });
				
		    transform.DOJump(is_move ? next_jump_pos : transform.position, 0.3f, 1, 0.3f)
		             .SetEase(Ease.InCirc)
		             .OnComplete(() => {

			              next_jump_pos = transform.position +
			                              new Vector3(Random.Range(-settings.move_speed_range, settings.move_speed_range), Random.Range(-0.2f, 0.2f));

			              if (Bounds.min.x < camera_bounds.min.x) {
				              next_jump_pos = next_jump_pos.SetX(camera_bounds.min.x + 0.1f);
			              } else if (Bounds.max.x > camera_bounds.max.x) {
				              next_jump_pos = next_jump_pos.SetX(camera_bounds.max.x - camera_bounds.max.x - 0.1f);
			              }

			              if (Bounds.min.y < camera_bounds.min.y) {
				              next_jump_pos = next_jump_pos.SetY(camera_bounds.min.y + 0.1f);
			              } else if (Bounds.max.y > camera_bounds.max.y) {
				              next_jump_pos = next_jump_pos.SetY(camera_bounds.max.y - camera_bounds.max.y - 0.1f);
			              }
		              });
	    }

	    public override void SetPosition(Vector3 position) {
		    next_jump_pos = position;
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