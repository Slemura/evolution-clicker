using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
	
namespace com.rpdev.foundation.model {

	public class PlayerModel {
		
		protected readonly ReactiveProperty<int> coins = new ReactiveProperty<int>(0);

		public ReactiveProperty<int> Coins => coins;
		
		public void AddCoins(int count) {
			coins.Value += count;
		}
	}
	
}