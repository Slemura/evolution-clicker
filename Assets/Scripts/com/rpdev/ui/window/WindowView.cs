using UnityEngine;
using Zenject;

namespace com.rpdev.ui.window {

	public class WindowView : MonoBehaviour{

	    protected WindowMediator      mediator;
	    protected RectTransform       rect_transform;
	    
	    [Inject]
	    protected IWindowsManager window_manager;

	    public virtual void Init(WindowMediator mediator) {
	        this.rect_transform      = GetComponent<RectTransform>();
	        this.mediator            = mediator;
            
            StartDisable();
        }

        public virtual void EnableInstance() {
	        gameObject.SetActive(true);
	    }

        public virtual void DisableInstance() {
	        gameObject.SetActive(false);
        }

	    protected virtual void StartDisable() {

	        gameObject.SetActive(false);
        }

	    protected T Mediator<T>() where T : WindowMediator {
	        return mediator as T;
	    }
	}
}