using System;

namespace UnityEngine.PostProcessing
{
	// Token: 0x02000467 RID: 1127
	public abstract class PostProcessingComponent<T> : global::UnityEngine.PostProcessing.PostProcessingComponentBase where T : global::UnityEngine.PostProcessing.PostProcessingModel
	{
		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06001696 RID: 5782 RVA: 0x0006E3F5 File Offset: 0x0006C5F5
		// (set) Token: 0x06001697 RID: 5783 RVA: 0x0006E3FD File Offset: 0x0006C5FD
		public T model { get; internal set; }

		// Token: 0x06001698 RID: 5784 RVA: 0x0006E406 File Offset: 0x0006C606
		public virtual void Init(global::UnityEngine.PostProcessing.PostProcessingContext pcontext, T pmodel)
		{
			this.context = pcontext;
			this.model = pmodel;
		}

		// Token: 0x06001699 RID: 5785 RVA: 0x0006E416 File Offset: 0x0006C616
		public override global::UnityEngine.PostProcessing.PostProcessingModel GetModel()
		{
			return this.model;
		}
	}
}
