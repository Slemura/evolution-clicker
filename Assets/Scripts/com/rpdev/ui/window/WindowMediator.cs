using System;
using com.rpdev.ui.view.window.model;
using UniRx;
using UnityEngine;
using Zenject;

namespace com.rpdev.ui.window {

    public interface IWindowMediator {
        IObservable<WindowMediator.WindowsCloseAdditionalData> CloseObservable { get; }
        WindowsManager.Window WindowType { get; }
        void InitArgs(params object[] args);
        void InitAdditionlData(AdditionalWindowData data);
        void ShowElements();
        void HideElements();
        void Show();
        void Hide();
    }

	public class WindowMediator : MonoBehaviour, IWindowMediator, IDisposable {

        [SerializeField]
	    protected WindowsManager.Window window_type;
        [SerializeField]
	    protected WindowView view;

	    protected CompositeDisposable input_stream;
	    protected SignalBus signal_bus;
	    protected IWindowsManager windows_manager;
	    protected IUIContextFacade ui_context;
	    protected Subject<WindowsCloseAdditionalData> close_observable;
        protected AdditionalWindowData additional_data;

	    public IObservable<WindowsCloseAdditionalData> CloseObservable => close_observable;
	    public WindowsManager.Window WindowType => window_type;

	    [Inject]
	    public virtual void Construct(SignalBus            signal_bus, 
                                      IWindowsManager      windows_manager,
                                      IUIContextFacade     ui_context) {

            this.close_observable = new Subject<WindowsCloseAdditionalData>();
	        this.signal_bus       = signal_bus;
	        this.windows_manager  = windows_manager;
            this.ui_context       = ui_context;

            view.Init(this);
	    }

	    public virtual void InitAdditionlData(AdditionalWindowData data) {
	        this.additional_data = data;
	    }

	    public void ShowElements() {
		    gameObject.SetActive(true);
	    }

	    public void HideElements() {
		    gameObject.SetActive(false);
	    }

	    protected virtual void InitInputs() {
		    input_stream?.Dispose();
		    input_stream = new CompositeDisposable();
	    }

	    public virtual void Show() {
		    InitInputs();
            view.EnableInstance();
	    }

	    public virtual void Hide() {
		    
		    input_stream?.Dispose();
            view.DisableInstance();
            
            close_observable.OnNext(new WindowsCloseAdditionalData {
	            mediator = this
            });
        }

	    protected T GetAdditionalData<T>() where T : AdditionalWindowData {
	        return additional_data as T;
	    }

	    protected T View<T>() where T : WindowView {
	        return view as T;
	    }


	    public virtual void InitArgs(params object[] args) {

	    }
	    
	    public class WindowsCloseAdditionalData {
		    public IWindowMediator mediator;
	    }

	    public void Dispose() {
		    close_observable?.Dispose();
		    input_stream?.Dispose();
	    }
	}
}