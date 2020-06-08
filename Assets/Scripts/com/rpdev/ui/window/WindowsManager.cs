using System;
using System.Collections.Generic;
using System.Linq;
using com.rpdev.ui.view.window.model;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Zenject;

namespace com.rpdev.ui.window {
    public class WindowsManager : MonoBehaviour, IWindowsManager {
        
    #region Serialized

        [SerializeField]
        protected WindowMediator[] windows;

        [SerializeField]
        protected CanvasGroup background;

    #endregion

    #region Protected

        protected IWindowMediator          current_open_window;
        protected List<IWindowMediator>    active_windows          = new List<IWindowMediator>();
        protected Subject<IWindowMediator> current_window_observer = new Subject<IWindowMediator>();

    #endregion

    #region Injected
        [Inject]
        protected IUIContextFacade ui_context;
    #endregion

    #region Public

        public IWindowMediator              CurrentWindow         => current_open_window;
        public bool                         IsAnyWindowShowed     => active_windows != null && active_windows.Count > 0;
        public IObservable<IWindowMediator> CurrentWindowObserver => current_window_observer;

    #endregion

    #region Init

        public void Initialize() {
            background.gameObject.SetActive(false);
        }

    #endregion


    #region Show Window Methods

        public void ShowWindow(Window window) {
            
            current_open_window = windows.FirstOrDefault(wnd => wnd.WindowType == window);
            
            if (current_open_window == null || active_windows.Contains(current_open_window)) return;
            
            active_windows.Add(current_open_window);
            current_window_observer.OnNext(current_open_window);
            current_open_window.Show();

            //DisableActiveGameEntities();
        }

        public void ShowWindow(Window window, AdditionalWindowData data) {
            
            current_open_window = windows.FirstOrDefault(wnd => wnd.WindowType == window);

            if (current_open_window == null || active_windows.Contains(current_open_window)) return;
            
            current_open_window.InitAdditionlData(data);
            active_windows.Add(current_open_window);
            current_window_observer.OnNext(current_open_window);
            current_open_window.Show();

            //DisableActiveGameEntities();
        }

    #endregion

    #region Hide Window Methods

        public void HideLastWindow(bool from_window = false) {
            if (current_open_window != null && !from_window) {
                current_open_window.Hide();
            }

            active_windows.Remove(current_open_window);

            current_open_window = active_windows.Count > 0 ? active_windows.First() : null;

            if (active_windows.Count != 0) return;
            current_window_observer.OnNext(null);
            //EnableActiveGameEntities();
        }

        public void HideWindow(WindowsManager.Window window) {
            IWindowMediator current_window = active_windows.FirstOrDefault(wnd => wnd.WindowType == window);

            if (current_window == null) return;

            current_window.Hide();

            if (active_windows.IndexOf(current_window) > -1)
                active_windows.RemoveAt(active_windows.IndexOf(current_window));

            current_open_window = active_windows.FirstOrDefault();

            if (active_windows.Count != 0) return;

            current_window_observer.OnNext(null);
            //EnableActiveGameEntities();
        }

    #endregion

    #region Utils

        public bool WindowOpened(WindowsManager.Window window) {
            return active_windows.Any(wnd => wnd.WindowType == window);
        }

        public IWindowMediator GetCurrentWindow(WindowsManager.Window window) {
            return active_windows.FirstOrDefault(wnd => wnd.WindowType == window);
        }

        public string ListWindows() {
            return string.Join("; ", active_windows.Select(window => window.WindowType.ToString()).ToArray());
        }

    #endregion

        public void ShowPreWindowElements() {
            windows.First(window => window.WindowType == Window.INVENTORY_WINDOW).ShowElements();
        }

        public void HidePreWindowElements() {
            windows.First(window => window.WindowType == Window.INVENTORY_WINDOW).HideElements();
        }

        public void HideBackground() {
            background.DOComplete();           
            background.DOFade(0, 1).SetEase(Ease.OutExpo);
            background.gameObject.SetActive(false);
        }

        public void ShowBackground() {
            background.alpha = 0;
            background.gameObject.SetActive(true);
            background.DOFade(1, 0.8f).SetEase(Ease.InExpo);
        }
        
        public void ShowBackground(Action on_fade_callback) {
            background.alpha = 0;
            background.gameObject.SetActive(true);
            background.DOFade(1, 0.8f).SetEase(Ease.InExpo).OnComplete(() => on_fade_callback?.Invoke());
        }

        public enum Window {
            MAIN_MENU_WINDOW,
            DIALOG_WINDOW,
            TEXT_EVENT_WINDOW,
            LOSE_WINDOW,
            WIN_WINDOW,
            WAVE_WIN_WINDOW,
            PAUSE_WINDOW,
            SERVICE_WINDOW,
            BAG_WINDOW,
            INVENTORY_WINDOW,
            ITEM_INFO_PANEL_WINDOW
        }
    }

    public interface IWindowsManager : IInitializable {
        IObservable<IWindowMediator> CurrentWindowObserver { get; }
        IWindowMediator              CurrentWindow         { get; }
        bool                         IsAnyWindowShowed     { get; }
        IWindowMediator              GetCurrentWindow(WindowsManager.Window window);
        void                         ShowPreWindowElements();
        void                         HidePreWindowElements();
        void                         ShowWindow(WindowsManager.Window window);
        void                         ShowWindow(WindowsManager.Window window, AdditionalWindowData data);
        void                         HideLastWindow(bool              from_window = false);
        void                         HideWindow(WindowsManager.Window window);
        void                         HideBackground();
        void                         ShowBackground();
        void                         ShowBackground(Action on_fade_callback);
        bool                         WindowOpened(WindowsManager.Window window);
        string                       ListWindows();
    }
}