using System;
using com.rpdev.foundation.controller;
using com.rpdev.ui;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace com.rpdev.foundation.view.unit {

	public interface ICoinView : IUnitView {
		int CoinCount { get; }
		bool IsCollected { get; }
		void InitCoin(int coin_count, Vector3 position);
	}
	
	public class CoinView : UnitView, ICoinView {
		
		[SerializeField]
		protected TMP_Text text;
		protected int coin_count;

		[Inject]
		protected UIContextFacade ui_context;

		public int CoinCount => coin_count;
		public bool IsCollected { get; private set; }

		
		[Inject]
		public override void Initialize() {
			
		}

		public override void Interact() {
			StopAnimation();
			
			IsCollected = true;
			
			transform.DOMove(ui_context.CoinPanelPosition, 0.3f)
			         .SetEase(Ease.OutCirc)
			         .OnComplete(() => {
				          signal_bus.Fire(new CollectCoinSignal {
					          coin_view = this
				          });
			          });
		}
		
		public void InitCoin(int coin_count, Vector3 position) {
			
			this.coin_count = coin_count;
			text.text       = coin_count.ToString();
			
			transform.DOJump(new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(-0.5f, 0.5f), 1),
			                    1f, 1, 0.2f)
			         .SetEase(Ease.InCirc)
			         .OnComplete(() => CreateAnimationStream(false));

			Observable.Timer(TimeSpan.FromSeconds(3))
			          .Subscribe(_ => Interact())
			          .AddTo(this);
		}
	}
}