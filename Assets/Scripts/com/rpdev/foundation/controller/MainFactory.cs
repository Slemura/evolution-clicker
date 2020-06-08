using System.Collections;
using System.Collections.Generic;
using com.rpdev.foundation.view.unit;
using UnityEngine;
using Zenject;

namespace com.rpdev.foundation.controller {

	public class MainFactory<T, V> : IFactory<T, V> where T : Component {

		protected readonly DiContainer container;
		
		public MainFactory(DiContainer container) {
			this.container = container;
		}
		
		public V Create(T param) {
			return container.InstantiatePrefabForComponent<V>(param);
		}
	}
}