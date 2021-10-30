using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
	
namespace com.rpdev.foundation.model {

	public class PlayerModel {
		
		private readonly ReactiveProperty<int> _coins = new ReactiveProperty<int>(0);

		public ReactiveProperty<int> Coins => _coins;
		
		public void AddCoins(int count) {
			_coins.Value += count;
		}
	}
	
}