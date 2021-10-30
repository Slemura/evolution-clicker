using System;
using UniRx;
using UnityEngine;

namespace com.rpdev.input {
    public class PCInputController : IInputController {
		
        private bool _from_press = false;

        private readonly ReactiveProperty<bool> _is_pressed  = new ReactiveProperty<bool>();
        private readonly ReactiveProperty<bool> _is_released = new ReactiveProperty<bool>(false);

        private readonly ReactiveProperty<Vector3> _input_world_position  = new ReactiveProperty<Vector3>();
        private readonly ReactiveProperty<Vector2> _input_screen_position = new ReactiveProperty<Vector2>();
		
        private IDisposable _input_flow;
        private Camera      _main_camera;
		
        public IReactiveProperty<Vector3> InputWorldPosition  => _input_world_position;
        public IReactiveProperty<Vector2> InputScreenPosition => _input_screen_position;

        public IReactiveProperty<bool> IsPressed => _is_pressed;
        public IReactiveProperty<bool> IsRelease => _is_released;

		
		
        public void Initialize() {
            _main_camera = Camera.main;
            _input_flow = Observable.EveryUpdate()
                                    .Subscribe(_ => {
                                        InputProcess();
                                    });
        }

        protected virtual void InputProcess() {
			
            if (Input.GetMouseButton(0)) {
				
                _is_pressed.Value = true;
                _from_press       = true;
				
            } else if(_from_press) {
                _from_press        = false;
                _is_released.Value = true;
                _is_pressed.Value  = false;
                _is_released.Value = false;
            }
			
            _input_screen_position.Value = Input.mousePosition;
            _input_world_position.Value  = _main_camera.ScreenToWorldPoint(Input.mousePosition);
        }

        public void Dispose() {
            _input_flow?.Dispose();
        }
    }
}