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
		
		[Inject]
		protected SignalBus signal_bus;
		
		protected SpriteRenderer sprite_renderer;
		protected IDisposable animation_stream;
		protected Sequence sequence;
		protected Vector3 started_scale;
		public GameObject GameObject => gameObject;
		public Bounds Bounds => SpriteRenderer.bounds;

		public SpriteRenderer SpriteRenderer {
			get {
				if (sprite_renderer == null)
					sprite_renderer = GetComponent<SpriteRenderer>();

				return sprite_renderer;
			}
		}

		public Vector3 Position => transform.position;
		public Vector2 ScreenPosition => Camera.main.WorldToScreenPoint(transform.position);

		protected void Awake() {
			SpriteRenderer.sortingOrder = Random.Range(0, 20);
			started_scale = transform.localScale;
		}
		
		public virtual void Interact() {
		}
		
		public virtual void Initialize() {
			CreateAnimationStream();
		}

		protected virtual void CreateAnimationStream(bool is_immidiate_play = true) {
			
			animation_stream?.Dispose();
			animation_stream = Observable.Timer(TimeSpan.FromSeconds(3f))
			                             .Repeat()
			                             .Subscribe(_ => { CreateRandomAnimation(); });

			if(is_immidiate_play)
				CreateRandomAnimation();
		}

		protected void StopAnimation() {
			animation_stream?.Dispose();
			DOTween.Kill(transform, false);
			sequence?.Kill(false);
		}

		protected virtual void CreateRandomAnimation() {
			
			DOTween.Kill(transform, false);
			sequence?.Kill(false);
			
			sequence = DOTween.Sequence();
			
			sequence.Append(transform.DOScale(new Vector3(0.9f, 1.1f, 1), 0.2f).SetEase(Ease.InCirc));
			sequence.Append(transform.DOScale(new Vector3(1.1f, 0.9f, 1), 0.1f).SetEase(Ease.InCirc));
			sequence.Append(transform.DOScale(started_scale, 0.1f).SetEase(Ease.InCirc));
			
			transform.DOJump(transform.position, 0.3f, 1, 0.3f).SetEase(Ease.InCirc);
		}
		
		public virtual void SetPosition(Vector3 position) {
			transform.position = position;
		}

		public virtual void Dispose() {
			animation_stream?.Dispose();
		}

		protected void OnDestroy() {
			DOTween.Kill(transform, false);
			sequence?.Kill(false);
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