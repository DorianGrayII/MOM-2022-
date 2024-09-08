using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200014C RID: 332
	public abstract class ComponentAction<T> : global::HutongGames.PlayMaker.FsmStateAction where T : global::UnityEngine.Component
	{
		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060007DF RID: 2015 RVA: 0x0003227B File Offset: 0x0003047B
		protected global::UnityEngine.Rigidbody rigidbody
		{
			get
			{
				return this.cachedComponent as global::UnityEngine.Rigidbody;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060007E0 RID: 2016 RVA: 0x0003228D File Offset: 0x0003048D
		protected global::UnityEngine.Rigidbody2D rigidbody2d
		{
			get
			{
				return this.cachedComponent as global::UnityEngine.Rigidbody2D;
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060007E1 RID: 2017 RVA: 0x0003229F File Offset: 0x0003049F
		protected global::UnityEngine.Renderer renderer
		{
			get
			{
				return this.cachedComponent as global::UnityEngine.Renderer;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060007E2 RID: 2018 RVA: 0x000322B1 File Offset: 0x000304B1
		protected global::UnityEngine.Animation animation
		{
			get
			{
				return this.cachedComponent as global::UnityEngine.Animation;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060007E3 RID: 2019 RVA: 0x000322C3 File Offset: 0x000304C3
		protected global::UnityEngine.AudioSource audio
		{
			get
			{
				return this.cachedComponent as global::UnityEngine.AudioSource;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060007E4 RID: 2020 RVA: 0x000322D5 File Offset: 0x000304D5
		protected global::UnityEngine.Camera camera
		{
			get
			{
				return this.cachedComponent as global::UnityEngine.Camera;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060007E5 RID: 2021 RVA: 0x000322E7 File Offset: 0x000304E7
		protected global::UnityEngine.Light light
		{
			get
			{
				return this.cachedComponent as global::UnityEngine.Light;
			}
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x000322FC File Offset: 0x000304FC
		protected bool UpdateCache(global::UnityEngine.GameObject go)
		{
			if (go == null)
			{
				return false;
			}
			if (this.cachedComponent == null || this.cachedGameObject != go)
			{
				this.cachedComponent = go.GetComponent<T>();
				this.cachedGameObject = go;
				if (this.cachedComponent == null)
				{
					base.LogWarning("Missing component: " + typeof(T).FullName + " on: " + go.name);
				}
			}
			return this.cachedComponent != null;
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x00032398 File Offset: 0x00030598
		protected bool UpdateCacheAddComponent(global::UnityEngine.GameObject go)
		{
			if (go == null)
			{
				return false;
			}
			if (this.cachedComponent == null || this.cachedGameObject != go)
			{
				this.cachedComponent = go.GetComponent<T>();
				this.cachedGameObject = go;
				if (this.cachedComponent == null)
				{
					this.cachedComponent = go.AddComponent<T>();
					this.cachedComponent.hideFlags = global::UnityEngine.HideFlags.DontSaveInEditor;
				}
			}
			return this.cachedComponent != null;
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x00032425 File Offset: 0x00030625
		protected void SendEvent(global::HutongGames.PlayMaker.FsmEventTarget eventTarget, global::HutongGames.PlayMaker.FsmEvent fsmEvent)
		{
			base.Fsm.Event(this.cachedGameObject, eventTarget, fsmEvent);
		}

		// Token: 0x040006EE RID: 1774
		protected global::UnityEngine.GameObject cachedGameObject;

		// Token: 0x040006EF RID: 1775
		protected T cachedComponent;
	}
}
