using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.Patterns
{
	public class Singleton<T> : MonoBehaviour where T : Component
	{
		protected static T _instance;
		public static T Instance
		{
			get
			{
				if (_instance == null)
					throw new System.NullReferenceException("An instance of " + typeof(T) + " is needed in the scene, but there is none.");

				return _instance;
			}
		}

		protected virtual void Awake()
		{
			if (_instance == null)
				_instance = this as T;
			else
				Destroy(gameObject);
		}
	}

    namespace FSM
	{
		public abstract class State<T>
		{
			protected T user;

			protected State(T user)
			{
				this.user = user;
			}

			public virtual void Enter() { }

			public abstract void Process();

			public virtual void Exit() { }
		}
	}
}
