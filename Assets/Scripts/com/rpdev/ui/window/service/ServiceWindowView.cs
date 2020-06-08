using System.Collections;
using System.Collections.Generic;
using com.rpdev.ui.window;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.rpdev.ui.view.window.service {

	public class ServiceWindowView : WindowView {
	    
		[SerializeField]
		protected Image blured_bg;

		[SerializeField]
		protected RectTransform window_base;
		[SerializeField]
		protected Text title_txt;
		
		[SerializeField]
		protected Text message_txt;
		
		public override void EnableInstance() {
			base.EnableInstance();

			blured_bg.material.SetFloat("_Radius", 0f);
			blured_bg.material.DOFloat(2f, "_Radius", 0.5f).SetEase(Ease.OutBack).SetUpdate(true);

			window_base.sizeDelta = new Vector2(0, 0);
			window_base.DOSizeDelta(new Vector2(544, 418), 0.5f).SetEase(Ease.OutCirc).SetUpdate(true);
		}

		public void SetText(string title, string text) {
			title_txt.text   = title;
			message_txt.text = text;
		}
		
		public void Close() {
			Mediator<ServiceWindowMediator>().Hide();
		}
	}
}