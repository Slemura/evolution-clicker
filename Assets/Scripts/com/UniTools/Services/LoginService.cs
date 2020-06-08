using UnityEngine;
using System.Collections;
using System;
using SimpleJSON;

namespace UniTools
{
	public delegate void ResultCallback(bool error, string data);

	public interface ILoginService
	{
		void Register(string username, string password, ResultCallback callback);
		void GetData(string username, string password, ResultCallback callback);
		void SaveData(string username, string password, string data, ResultCallback callback);
	}

	public class LoginService : ILoginService
	{
		private const string BASE_URL = "https://rocky-gorge-84073.herokuapp.com/api/";
		private const float WAIT_TIMEOUT = 10.0f;

		private readonly ICoroutineExecuter _coroutineExecuter;

		public LoginService(ICoroutineExecuter ce)
		{
			_coroutineExecuter = ce;
		}

		public void Register(string username, string password, ResultCallback callback)
		{
			var form = new WWWForm();

			form.AddField("username", username);
			form.AddField("password", password);

			var www = new WWW(BASE_URL + "register", form);

			SendRequest(www, callback);
		}

		public void GetData(string username, string password, ResultCallback callback)
		{
			var form = new WWWForm();

			form.AddField("username", username);
			form.AddField("password", password);

			var www = new WWW(BASE_URL + "getData", form);

			SendRequest(www, callback);
		}

		public void SaveData(string username, string password, string data, ResultCallback callback)
		{
			var form = new WWWForm();

			form.AddField("username", username);
			form.AddField("password", password);
			form.AddField("data", data);

			var www = new WWW(BASE_URL + "saveData", form);

			SendRequest(www, callback);
		}

		private void SendRequest(WWW www, ResultCallback callback) {
			
			Debug.Log("[LoginService] sendRequest:" + www.url);

			_coroutineExecuter.Execute(ExecuteRequest(www, callback));
		}

		private IEnumerator ExecuteRequest(WWW www, ResultCallback callback) {
			
			float elapsedTime = 0.0f;

			while (!www.isDone)
			{
				elapsedTime += Time.deltaTime;

				if (elapsedTime >= WAIT_TIMEOUT)
				{
					if (callback != null)
						callback(true, "{\"status\":400,\"message\":\"local timeout!\"}");

					yield break;
				}

				yield return null;
			}

			if (!www.isDone || !string.IsNullOrEmpty(www.error) || string.IsNullOrEmpty(www.text))
			{
				if (callback != null)
					callback(true, "{\"status\":400,\"message\":\"" + www.error + "\"}");

				yield break;
			}

			var response = www.text;

			try {
				JSONNode json = SimpleJSON.JSON.Parse(response);

				if (json["status"] != null && json["status"].AsInt != 200) {
					
					callback?.Invoke(true, response);
					yield break;
				}
			} catch (Exception ex) {
				Debug.LogException(ex);
			}

			if (callback != null)
				callback(false, response);
		}
	}
}