using UnityEngine;

namespace com.rpdev.ui {

	public interface IUIContextFacade {
	}
	
	public class UIContextFacade : CachedMonoBehaviour, IUIContextFacade {
		
		public Vector3 CoinPanelPosition => coin_panel.position;

		[SerializeField]
		protected RectTransform coin_panel;
		
		public void SpawnCoin(int count, Vector3 position) {
			
			Vector2 viewportPosition = Camera.main.WorldToViewportPoint(position);
			Vector2 worldObjectScreenPosition = new Vector2(
			                                                 ((viewportPosition.x * RectTransform.sizeDelta.x) -
			                                                  (RectTransform.sizeDelta.x * 0.5f)),
			                                                 
			                                                 ((viewportPosition.y * RectTransform.sizeDelta.y) -
			                                                  (RectTransform.sizeDelta.y * 0.5f)));
			
		}
	}
}