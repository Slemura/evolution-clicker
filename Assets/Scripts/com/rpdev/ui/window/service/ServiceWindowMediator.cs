using System.Collections;
using System.Collections.Generic;
using com.rpdev.ui.view.window.model;
using com.rpdev.ui.window;
using UnityEngine;
	
namespace com.rpdev.ui.view.window.service {

	public class ServiceWindowMediator : WindowMediator {
		
		public override void Show() {
			base.Show();
			
			View<ServiceWindowView>()
			   .SetText(GetAdditionalData<ServiceData>().title, GetAdditionalData<ServiceData>().text);
		}

		public class ServiceData : AdditionalWindowData {
			public string title;
			public string text;
		}
	}
}