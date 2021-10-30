using UnityEngine;

namespace com.rpdev.ui {

	public interface IUIContextFacade {
		public Vector3 CoinPanelPosition { get; }
	}
	
	public class UIContextFacade : CachedMonoBehaviour, IUIContextFacade {
		
		[SerializeField]
		private RectTransform _coin_panel;
		
		public Vector3 CoinPanelPosition => _coin_panel.position;
	}
}