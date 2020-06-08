using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace com.rpdev.foundation.controller {

	public interface IInputController : IInitializable, IDisposable {
		IReactiveProperty<Vector3> InputWorldPosition { get; }
		IReactiveProperty<Vector2> InputScreenPosition { get; }
		
		IReactiveProperty<bool> IsPressed { get; }
		IReactiveProperty<bool> IsRelease { get; }
	}
	
	public class MouseInputController : IInputController {

		protected bool from_press = false;
		
		protected readonly ReactiveProperty<bool> is_pressed = new ReactiveProperty<bool>();
		protected readonly ReactiveProperty<bool> is_released = new ReactiveProperty<bool>(false);
		
		protected readonly ReactiveProperty<Vector3> input_world_position = new ReactiveProperty<Vector3>();
		protected readonly ReactiveProperty<Vector2> input_screen_position = new ReactiveProperty<Vector2>();

		public IReactiveProperty<Vector3> InputWorldPosition => input_world_position;
		public IReactiveProperty<Vector2> InputScreenPosition => input_screen_position;

		public IReactiveProperty<bool> IsPressed => is_pressed;
		public IReactiveProperty<bool> IsRelease => is_released;

		protected IDisposable input_stream;
		
		protected Camera main_camera;
		
		public void Initialize() {
			main_camera = Camera.main;
			input_stream = Observable.EveryUpdate()
			                         .Subscribe(_ => {
				                          InputProcess();
			                          });
		}

		protected virtual void InputProcess() {
			
			if (Input.GetMouseButton(0)) {
				
				is_pressed.Value = true;
				from_press = true;
				
			} else if(from_press) {
				from_press = false;
				is_released.Value = true;
				is_pressed.Value  = false;
				is_released.Value = false;
			}
			
			input_screen_position.Value = Input.mousePosition;
			input_world_position.Value  = main_camera.ScreenToWorldPoint(Input.mousePosition);
		}

		public void Dispose() {
			input_stream?.Dispose();
		}
	}
	
	public class TouchInputController : IInputController {
		
		protected readonly ReactiveProperty<bool> is_pressed  = new ReactiveProperty<bool>();
		protected readonly ReactiveProperty<bool> is_released = new ReactiveProperty<bool>(false);
		
		protected readonly ReactiveProperty<Vector3> input_world_position  = new ReactiveProperty<Vector3>();
		protected readonly ReactiveProperty<Vector2> input_screen_position = new ReactiveProperty<Vector2>();

		public IReactiveProperty<Vector3> InputWorldPosition  => input_world_position;
		public IReactiveProperty<Vector2> InputScreenPosition => input_screen_position;

		public IReactiveProperty<bool> IsPressed => is_pressed;
		public IReactiveProperty<bool> IsRelease => is_released;
		
		protected IDisposable input_stream;
		protected bool from_press = false;

		protected Camera main_camera;
		
		public void Initialize() {
			main_camera = Camera.main;
			input_stream = Observable.EveryUpdate()
			                         .Subscribe(_ => {
				                          InputProcess();
			                          });
		}

		protected void InputProcess() {
			
			if (Input.touches.Length > 0) {
				
				Touch currentTouch = Input.GetTouch(0);
				
				switch (currentTouch.phase) {
					
					case TouchPhase.Began:
						input_screen_position.Value = currentTouch.position;
						input_world_position.Value  = main_camera.ScreenToWorldPoint(currentTouch.position);
						
						is_pressed.Value = true;
						from_press       = true;
						break;
				
					case TouchPhase.Moved:
						input_screen_position.Value = currentTouch.position;
						input_world_position.Value  = main_camera.ScreenToWorldPoint(currentTouch.position);
						break;
				
					case TouchPhase.Ended:
						input_screen_position.Value = currentTouch.position;
						input_world_position.Value  = main_camera.ScreenToWorldPoint(currentTouch.position);
						
						is_released.Value = true;
						is_pressed.Value  = false;
						is_released.Value = false;
						break;
				}	
			}
		}

		public void Dispose() {
			input_stream?.Dispose();
		}

	}
}