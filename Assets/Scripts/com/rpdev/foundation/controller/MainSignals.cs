using com.rpdev.foundation.view.unit;

namespace com.rpdev.foundation.controller {

	public class SpawnCreatureFromCrateSignal {
		public IUnitView crate_view;
	}

	public class SpawnCreatureFromMergeSignal {
		public ICreatureView first_creature;
		public ICreatureView second_creature;
	}

	public class CollectCoinSignal {
		public ICoinView coin_view;
	}
}