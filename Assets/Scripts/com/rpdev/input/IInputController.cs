using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace com.rpdev.input {

	public interface IInputController : IInitializable, IDisposable {
		IReactiveProperty<Vector3> InputWorldPosition { get; }
		IReactiveProperty<Vector2> InputScreenPosition { get; }
		
		IReactiveProperty<bool> IsPressed { get; }
		IReactiveProperty<bool> IsRelease { get; }
	}
}