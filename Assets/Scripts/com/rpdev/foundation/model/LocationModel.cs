using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.rpdev.foundation.view.unit;
using UniRx;
using UnityEngine;
	
namespace com.rpdev.foundation.model {

	public class LocationModel {
		
	    protected readonly List<IUnitView> all_units_on_location = new List<IUnitView>();
	    protected readonly List<ICreatureView> all_creatures_on_location = new List<ICreatureView>();
	    protected readonly List<ICoinView> all_coin_on_location = new List<ICoinView>();
	    protected readonly ReactiveProperty<int> total_units_count = new ReactiveProperty<int>(0);
	    
	    public IReactiveProperty<int> TotalUnitCount => total_units_count;
	    
	    public void AddUnit(IUnitView unit) {
		    
		    all_units_on_location.Add(unit);
		    
		    switch (unit) {
			    case ICreatureView view:
				    all_creatures_on_location.Add(view);
				    break;
			    case ICoinView coinView:
				    all_coin_on_location.Add(coinView);
				    break;
		    }

		    total_units_count.Value = all_units_on_location.Count(un => un is CrateView || un is ICreatureView);
	    }

	    public void RemoveUnit(IUnitView unit) {
		    
		    GameObject.Destroy(unit.GameObject);
		    
		    if (all_units_on_location.Contains(unit)) {
			    all_units_on_location.Remove(unit);
		    }

		    switch (unit) {
			    case ICreatureView view:
				    if(all_creatures_on_location.Contains(view))
					    all_creatures_on_location.Remove(view);
				    break;
			    case ICoinView coinView:
				    if(all_coin_on_location.Contains(coinView))
					    all_coin_on_location.Remove(coinView);
				    break;
		    }
		    
		    total_units_count.Value = all_units_on_location.Count(un => un is CrateView || un is ICreatureView);
	    }
	    
	    public ICreatureView GetIntersectInputPositionCreature(ICreatureView creature) {
		    return all_creatures_on_location.FirstOrDefault(unit => unit != creature &&
		                                                            unit.Bounds.Intersects(creature.Bounds));

	    }
	    
	    public ICoinView GetIntersectInputPositionCoin(Vector3 input_world_position) {
		    
		    List<ICoinView> intersects = all_coin_on_location.Where(unit => input_world_position.x >= unit.Bounds.center.x - unit.Bounds.extents.x &&
		                                                                     input_world_position.x <= unit.Bounds.center.x + unit.Bounds.extents.x &&
		                                                                     input_world_position.y > unit.Bounds.center.y - unit.Bounds.extents.y &&
		                                                                     input_world_position.y < unit.Bounds.center.y + unit.Bounds.extents.y).ToList();
		    
		    intersects?.Sort();

		    return intersects.Count > 0 ? intersects[0] : null;
	    }
	    
	    
	    public IUnitView GetIntersectInputPositionUnit(Vector3 input_world_position) {
		    
		    List<IUnitView> intersects = all_units_on_location.Where(unit => input_world_position.x >= unit.Bounds.center.x - unit.Bounds.extents.x &&
		                                                                     input_world_position.x <= unit.Bounds.center.x + unit.Bounds.extents.x &&
		                                                                     input_world_position.y > unit.Bounds.center.y - unit.Bounds.extents.y &&
		                                                                     input_world_position.y < unit.Bounds.center.y + unit.Bounds.extents.y).ToList();
		    
		    intersects?.Sort();

		    return intersects.Count > 0 ? intersects[0] : null;
	    }

	}
}