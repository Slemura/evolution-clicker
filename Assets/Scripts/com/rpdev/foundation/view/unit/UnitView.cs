using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace com.rpdev.foundation.view.unit {

	public interface IUnitView : IInitializable, IDisposable, IComparable<IUnitView> {
		SpriteRenderer SpriteRenderer { get; }
		GameObject GameObject { get; }
		Bounds Bounds { get; }
		Vector3 Position       { get; }
		Vector2 ScreenPosition { get; }
		
		void Interact();
		void SetPosition(Vector3 position);
	}

	[RequireComponent(typeof(SpriteRenderer))]
	public class UnitView : CachedMonoBehaviour, IUnitView {

		private SignalBus _signal_bus;
		
		private IDisposable    _animation_disposable;
		private Sequence       _animation_sequence;
		
		private SpriteRenderer _sprite_renderer;
		private Vector3        _started_scale;

		protected SignalBus   SignalBus           => _signal_bus;
		
		protected IDisposable AnimationDisposable {
			get => _animation_disposable;
			set => _animation_disposable = value;
		}
		
		protected Sequence AnimationSequence {
			get => _animation_sequence;
			set => _animation_sequence = value;
		}

		public    GameObject  GameObject          => gameObject;
		public    Bounds      Bounds              => SpriteRenderer.bounds;
		

		public SpriteRenderer SpriteRenderer {
			get {
				if (_sprite_renderer == null)
					_sprite_renderer = GetComponent<SpriteRenderer>();

				return _sprite_renderer;
			}
		}

		public Vector3 Position => transform.position;
		public Vector2 ScreenPosition => Camera.main.WorldToScreenPoint(transform.position);

		
		[Inject]
		private void Construct(SignalBus signal_bus) {
			_signal_bus = signal_bus;
		}

		protected void Awake() {
			SpriteRenderer.sortingOrder = Random.Range(0, 20);
			_started_scale = transform.localScale;
		}
		
		public virtual void Interact() {}
		
		public virtual void Initialize() {
			CreateAnimationStream();
		}

		protected virtual void CreateAnimationStream(bool immediately_play = true) {
			
			_animation_disposable?.Dispose();
			_animation_disposable = Observable.Timer(TimeSpan.FromSeconds(3f))
			                             .Repeat()
			                             .Subscribe(_ => { CreateRandomAnimation(); });

			if(immediately_play)
				CreateRandomAnimation();
		}

		protected void StopAnimation() {
			_animation_disposable?.Dispose();
			DOTween.Kill(transform, false);
			_animation_sequence?.Kill(false);
		}

		protected virtual void CreateRandomAnimation() {
			
			DOTween.Kill(transform, false);
			_animation_sequence?.Kill(false);
			
			_animation_sequence = DOTween.Sequence();
			
			_animation_sequence.Append(transform.DOScale(new Vector3(0.9f, 1.1f, 1), 0.2f).SetEase(Ease.InCirc));
			_animation_sequence.Append(transform.DOScale(new Vector3(1.1f, 0.9f, 1), 0.1f).SetEase(Ease.InCirc));
			_animation_sequence.Append(transform.DOScale(_started_scale, 0.1f).SetEase(Ease.InCirc));
			
			transform.DOJump(transform.position, 0.3f, 1, 0.3f).SetEase(Ease.InCirc);
		}
		
		public virtual void SetPosition(Vector3 position) {
			transform.position = position;
		}

		public virtual void Dispose() {
			_animation_disposable?.Dispose();
		}

		protected void OnDestroy() {
			DOTween.Kill(transform, false);
			_animation_sequence?.Kill(false);
			Dispose();
		}


		public int CompareTo(IUnitView other) {
			if (SpriteRenderer.sortingLayerID > other.SpriteRenderer.sortingLayerID) {
				
				return 1;
			} else if (SpriteRenderer.sortingLayerID == other.SpriteRenderer.sortingLayerID) {
				
				if (SpriteRenderer.sortingOrder > other.SpriteRenderer.sortingOrder) {
					return -1;
				} else if (SpriteRenderer.sortingOrder < other.SpriteRenderer.sortingOrder) {
					return 1;
				}

				return 0;
			} else {
				return -1;
			}
		}
		
		public class Factory : PlaceholderFactory<UnitView, IUnitView> { }
	}
}