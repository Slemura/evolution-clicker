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
		private TMP_Text text;
		
		private IUIContextFacade _ui_context;
		private int              _coin_count;

		public int CoinCount => _coin_count;
		public bool IsCollected { get; private set; }

		[Inject]
		private void Construct(IUIContextFacade ui_context) {
			_ui_context = ui_context;
		}

		public override void Initialize() {
			
		}

		public override void Interact() {
			StopAnimation();
			
			IsCollected = true;
			
			transform.DOMove(_ui_context.CoinPanelPosition, 0.3f)
			         .SetEase(Ease.OutCirc)
			         .OnComplete(() => {
				          SignalBus.Fire(new CollectCoinSignal {
					          coin_view = this
				          });
			          }).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}
		
		public void InitCoin(int coin_count, Vector3 position) {
			
			this._coin_count = coin_count;
			text.text        = coin_count.ToString();
			
			transform.DOJump(new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(-0.5f, 0.5f), 1), 1f, 1, 0.2f)
			         .SetEase(Ease.InCirc)
			         .OnComplete(() => CreateAnimationStream(false))
					 .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
		}
	}
}