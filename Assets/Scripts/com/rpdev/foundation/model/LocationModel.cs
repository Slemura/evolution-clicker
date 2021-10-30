using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.rpdev.foundation.view.unit;
using UniRx;
using UnityEngine;
	
namespace com.rpdev.foundation.model {

	public class LocationModel {
		
		private readonly List<IUnitView>       _all_units_on_location     = new List<IUnitView>();
		private readonly List<ICreatureView>   _all_creatures_on_location = new List<ICreatureView>();
		private readonly List<ICoinView>       _all_coin_on_location      = new List<ICoinView>();
		private readonly ReactiveProperty<int> _total_units_count         = new ReactiveProperty<int>(0);
	    
	    public IReactiveProperty<int> TotalUnitCount => _total_units_count;
	    
	    public void AddUnit(IUnitView unit) {
		    
		    _all_units_on_location.Add(unit);
		    
		    switch (unit) {
			    case ICreatureView view:
				    _all_creatures_on_location.Add(view);
				    break;
			    case ICoinView coin_view:
				    _all_coin_on_location.Add(coin_view);
				    break;
		    }

		    _total_units_count.Value = _all_units_on_location.Count(un => un is CrateView || un is ICreatureView);
	    }

	    public void RemoveUnit(IUnitView unit) {
		    
		    GameObject.Destroy(unit.GameObject);
		    
		    if (_all_units_on_location.Contains(unit)) {
			    _all_units_on_location.Remove(unit);
		    }

		    switch (unit) {
			    case ICreatureView view:
				    if(_all_creatures_on_location.Contains(view))
					    _all_creatures_on_location.Remove(view);
				    break;
			    case ICoinView coin_view:
				    if(_all_coin_on_location.Contains(coin_view))
					    _all_coin_on_location.Remove(coin_view);
				    break;
		    }
		    
		    _total_units_count.Value = _all_units_on_location.Count(un => un is CrateView || un is ICreatureView);
	    }
	    
	    public ICreatureView GetIntersectInputPositionCreature(ICreatureView creature) {
		    return _all_creatures_on_location.FirstOrDefault(unit => unit != creature &&
		                                                            unit.Bounds.Intersects(creature.Bounds));

	    }
	    
	    public ICreatureView GetIntersectInputPositionCreature(Vector3 input_world_position) {
		    
		    List<ICreatureView> intersects = _all_creatures_on_location.Where(unit => input_world_position.x >= unit.Bounds.center.x - unit.Bounds.extents.x &&
		                                                                     input_world_position.x <= unit.Bounds.center.x + unit.Bounds.extents.x &&
		                                                                     input_world_position.y >= unit.Bounds.center.y - unit.Bounds.extents.y &&
		                                                                     input_world_position.y <= unit.Bounds.center.y + unit.Bounds.extents.y)
		                                                              .ToList();
		    
		    intersects?.Sort();

		    return intersects.Count > 0 ? intersects[0] : null;
	    }
	    
	    public ICoinView[] GetIntersectInputPositionCoin(Vector3 input_world_position) {
		    
		    return _all_coin_on_location.Where(unit => input_world_position.x >= unit.Bounds.center.x - unit.Bounds.size.x &&
		                                              input_world_position.x <= unit.Bounds.center.x + unit.Bounds.size.x &&
		                                              input_world_position.y >= unit.Bounds.center.y - unit.Bounds.size.y &&
		                                              input_world_position.y <= unit.Bounds.center.y + unit.Bounds.size.y).ToArray();
	    }

	    
	    
	    public IUnitView GetIntersectInputPositionUnit(Vector3 input_world_position) {
		    
		    List<IUnitView> intersects = _all_units_on_location.Where(unit => input_world_position.x >= unit.Bounds.center.x - unit.Bounds.extents.x &&
		                                                                     input_world_position.x <= unit.Bounds.center.x + unit.Bounds.extents.x &&
		                                                                     input_world_position.y >= unit.Bounds.center.y - unit.Bounds.extents.y &&
		                                                                     input_world_position.y <= unit.Bounds.center.y + unit.Bounds.extents.y).ToList();
		    
		    intersects?.Sort();

		    return intersects.Count > 0 ? intersects[0] : null;
	    }
	}
}