using System;
using UniRx;
using UnityEngine;

namespace com.rpdev.input {
    
    public class MobileInputController : IInputController {
		
        private readonly ReactiveProperty<bool> _is_pressed  = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> _is_released = new ReactiveProperty<bool>(false);

        private readonly ReactiveProperty<Vector3> _input_world_position  = new ReactiveProperty<Vector3>();
        private readonly ReactiveProperty<Vector2> _input_screen_position = new ReactiveProperty<Vector2>();

        public IReactiveProperty<Vector3> InputWorldPosition  => _input_world_position;
        public IReactiveProperty<Vector2> InputScreenPosition => _input_screen_position;

        public IReactiveProperty<bool> IsPressed => _is_pressed;
        public IReactiveProperty<bool> IsRelease => _is_released;

        private IDisposable _input_stream;
        private Camera      _main_camera;
		
        public void Initialize() {
            _main_camera = Camera.main;
            _input_stream = Observable.EveryUpdate()
                                      .Subscribe(_ => {
                                          InputProcess();
                                      });
        }

        private void InputProcess() {
			
            if (Input.touches.Length > 0) {
				
                Touch current_touch = Input.GetTouch(0);
				
                switch (current_touch.phase) {
					
                    case TouchPhase.Began:
                        _input_screen_position.Value = current_touch.position;
                        _input_world_position.Value  = _main_camera.ScreenToWorldPoint(current_touch.position);
						
                        _is_pressed.Value = true;
                        break;
				
                    case TouchPhase.Moved:
                        _input_screen_position.Value = current_touch.position;
                        _input_world_position.Value  = _main_camera.ScreenToWorldPoint(current_touch.position);
                        break;
				
                    case TouchPhase.Ended:
                        _input_screen_position.Value = current_touch.position;
                        _input_world_position.Value  = _main_camera.ScreenToWorldPoint(current_touch.position);
						
                        _is_released.Value = true;
                        _is_pressed.Value  = false;
                        _is_released.Value = false;
                        break;
                }	
            }
        }

        public void Dispose() {
            _input_stream?.Dispose();
        }

    }
}