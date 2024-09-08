using System;
using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
	// Token: 0x02000468 RID: 1128
	public abstract class PostProcessingComponentCommandBuffer<T> : global::UnityEngine.PostProcessing.PostProcessingComponent<T> where T : global::UnityEngine.PostProcessing.PostProcessingModel
	{
		// Token: 0x0600169B RID: 5787
		public abstract global::UnityEngine.Rendering.CameraEvent GetCameraEvent();

		// Token: 0x0600169C RID: 5788
		public abstract string GetName();

		// Token: 0x0600169D RID: 5789
		public abstract void PopulateCommandBuffer(global::UnityEngine.Rendering.CommandBuffer cb);
	}
}
