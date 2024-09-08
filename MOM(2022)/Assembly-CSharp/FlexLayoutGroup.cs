using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class FlexLayoutGroup : LayoutGroup
{
    [SerializeField]
    protected int m_PreferredRows = 1;

    [SerializeField]
    protected bool m_PrioritiseChildPrefferedWidth = true;

    [SerializeField]
    protected bool m_forceAllWidthsEqual;

    [SerializeField]
    protected Vector2 m_PreferredSpacing = Vector2.zero;

    [SerializeField]
    protected bool m_MinSpacingEnabled;

    [SerializeField]
    protected Vector2 m_MinSpacing = Vector2.zero;

    [SerializeField]
    protected bool m_ChildForceExpandWidth = true;

    [SerializeField]
    protected bool m_ChildForceExpandHeight = true;

    [SerializeField]
    protected bool m_ChildControlWidth = true;

    [SerializeField]
    protected bool m_ChildControlHeight = true;

    [SerializeField]
    protected bool m_ChildScaleWidth;

    [SerializeField]
    protected bool m_ChildScaleHeight;

    [SerializeField]
    protected bool m_RowForceExpandHeight = true;

    private List<int> childRowsIndexes = new List<int>();

    private List<(float min, float preferred, float flexible)> rowHeights = new List<(float, float, float)>();

    private float allPreferred;

    private float allMin;

    private float maxMin;

    private float maxPreferred;

    public int preferredRows
    {
        get
        {
            return this.m_PreferredRows;
        }
        set
        {
            base.SetProperty(ref this.m_PreferredRows, value);
        }
    }

    public bool prioritiseChildPrefferedWidth
    {
        get
        {
            return this.m_PrioritiseChildPrefferedWidth;
        }
        set
        {
            base.SetProperty(ref this.m_PrioritiseChildPrefferedWidth, value);
        }
    }

    public bool forceAllWidthsEqual
    {
        get
        {
            return this.m_forceAllWidthsEqual;
        }
        set
        {
            base.SetProperty(ref this.m_PrioritiseChildPrefferedWidth, value);
        }
    }

    public Vector2 preferredSpacing
    {
        get
        {
            return this.m_PreferredSpacing;
        }
        set
        {
            base.SetProperty(ref this.m_PreferredSpacing, value);
        }
    }

    public bool minSpacingEnabled
    {
        get
        {
            return this.m_MinSpacingEnabled;
        }
        set
        {
            base.SetProperty(ref this.m_MinSpacingEnabled, value);
        }
    }

    public Vector2 minSpacing
    {
        get
        {
            return this.m_MinSpacing;
        }
        set
        {
            base.SetProperty(ref this.m_MinSpacing, value);
        }
    }

    public bool childForceExpandWidth
    {
        get
        {
            return this.m_ChildForceExpandWidth;
        }
        set
        {
            base.SetProperty(ref this.m_ChildForceExpandWidth, value);
        }
    }

    public bool childForceExpandHeight
    {
        get
        {
            return this.m_ChildForceExpandHeight;
        }
        set
        {
            base.SetProperty(ref this.m_ChildForceExpandHeight, value);
        }
    }

    public bool childControlWidth
    {
        get
        {
            return this.m_ChildControlWidth;
        }
        set
        {
            base.SetProperty(ref this.m_ChildControlWidth, value);
        }
    }

    public bool childControlHeight
    {
        get
        {
            return this.m_ChildControlHeight;
        }
        set
        {
            base.SetProperty(ref this.m_ChildControlHeight, value);
        }
    }

    public bool childScaleWidth
    {
        get
        {
            return this.m_ChildScaleWidth;
        }
        set
        {
            base.SetProperty(ref this.m_ChildScaleWidth, value);
        }
    }

    public bool childScaleHeight
    {
        get
        {
            return this.m_ChildScaleHeight;
        }
        set
        {
            base.SetProperty(ref this.m_ChildScaleHeight, value);
        }
    }

    public bool rowForceExpandHeight
    {
        get
        {
            return this.m_RowForceExpandHeight;
        }
        set
        {
            base.SetProperty(ref this.m_RowForceExpandHeight, value);
        }
    }

    private void GetChildSizes(RectTransform child, int axis, bool controlSize, bool childForceExpand, out float min, out float preferred, out float flexible)
    {
        if (!controlSize)
        {
            min = child.sizeDelta[axis];
            preferred = min;
            flexible = 0f;
        }
        else
        {
            min = LayoutUtility.GetMinSize(child, axis);
            preferred = LayoutUtility.GetPreferredSize(child, axis);
            flexible = LayoutUtility.GetFlexibleSize(child, axis);
        }
        if (childForceExpand)
        {
            flexible = Mathf.Max(flexible, 1f);
        }
    }

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        float num = base.padding.horizontal;
        bool childForceExpand = this.m_ChildForceExpandWidth;
        float num2 = 0f;
        float num3 = 0f;
        float num4 = 0f;
        int count = base.rectChildren.Count;
        float[] array = ((this.m_PreferredRows > 1) ? new float[count * 2] : null);
        this.maxMin = 0f;
        this.maxPreferred = 0f;
        for (int i = 0; i < count; i++)
        {
            RectTransform rectTransform = base.rectChildren[i];
            this.GetChildSizes(rectTransform, 0, this.m_ChildControlWidth, childForceExpand, out var min, out var preferred, out var flexible);
            if (this.m_ChildScaleWidth)
            {
                float num5 = rectTransform.localScale[0];
                min *= num5;
                preferred *= num5;
                flexible *= num5;
            }
            this.maxMin = Mathf.Max(this.maxMin, min);
            this.maxPreferred = Mathf.Max(this.maxPreferred, preferred);
            num2 += min;
            num3 += preferred;
            num4 += flexible;
            if (array != null)
            {
                array[i * 2] = min;
                array[i * 2 + 1] = preferred;
            }
        }
        float x = this.preferredSpacing.x;
        if (this.minSpacingEnabled)
        {
            x = this.minSpacing.x;
        }
        if (base.rectChildren.Count > 1)
        {
            float num6 = this.preferredSpacing.x * (float)(base.rectChildren.Count - 1);
            num2 += x * (float)(base.rectChildren.Count - 1);
            num3 += num6;
        }
        num3 = Mathf.Max(num2, num3);
        this.allMin = num2 + x;
        this.allPreferred = num3 + this.preferredSpacing.x;
        if (this.m_PreferredRows > 1 || this.m_forceAllWidthsEqual)
        {
            num2 = this.CalcWidthForPrefferedRows(array, 0, x, this.maxMin, num2);
            num3 = this.CalcWidthForPrefferedRows(array, 1, this.preferredSpacing.x, this.maxPreferred, num3);
        }
        num2 += num;
        num3 += num;
        base.SetLayoutInputForAxis(num2, num3, num4, 0);
    }

    private float CalcWidthForPrefferedRows(float[] widths, int offset, float spacing, float minWidth, float remainingWidth)
    {
        int num = Mathf.Max(1, this.m_PreferredRows);
        if (this.m_forceAllWidthsEqual)
        {
            float num2 = (remainingWidth + spacing) / (float)base.rectChildren.Count;
            int num3 = (base.rectChildren.Count + num - 1) / num;
            return Mathf.Max(minWidth, (float)num3 * num2 - spacing);
        }
        float num4 = Mathf.Max(minWidth, remainingWidth / (float)num);
        float num5 = num - 1;
        int i = offset;
        while (i < widths.Length)
        {
            float num6 = widths[i] + spacing;
            float num7 = num6;
            remainingWidth -= num7;
            for (i += 2; i < widths.Length; i += 2)
            {
                num6 = widths[i] + spacing;
                if (!(num7 + num6 <= num4))
                {
                    break;
                }
                num7 += num6;
                remainingWidth -= num6;
            }
            if (i < widths.Length)
            {
                float num8 = num5 * num4;
                float num9 = remainingWidth - num8;
                float num10 = num6 - (num4 - num7);
                if (num9 > num10)
                {
                    num4 = num7 + num6;
                    remainingWidth -= num6;
                    i += 2;
                }
                else if (num9 > 0f)
                {
                    num4 += num9;
                }
            }
            num5 -= 1f;
        }
        return num4 - spacing;
    }

    public override void SetLayoutHorizontal()
    {
        int num = 0;
        float num2 = base.rectTransform.rect.size[0];
        bool flag = this.m_ChildControlWidth;
        bool flag2 = this.m_ChildScaleWidth;
        bool childForceExpand = this.m_ChildForceExpandWidth;
        float alignmentOnAxis = base.GetAlignmentOnAxis(0);
        float num3 = num2 - (float)base.padding.horizontal;
        float t = 0f;
        if (this.m_PreferredRows > 0)
        {
            float totalMinSize = base.GetTotalMinSize(num);
            float totalPreferredSize = base.GetTotalPreferredSize(num);
            if (totalMinSize != totalPreferredSize)
            {
                t = Mathf.Clamp01((num2 - totalMinSize) / (totalPreferredSize - totalMinSize));
            }
            else if (totalPreferredSize <= num2)
            {
                t = 1f;
            }
        }
        else if (this.maxMin != this.maxPreferred)
        {
            t = Mathf.Clamp01((num2 - this.maxMin) / (this.maxPreferred - this.maxMin));
        }
        else if (this.maxPreferred <= num2)
        {
            t = 1f;
        }
        int num4 = Mathf.Max(1, this.m_PreferredRows);
        int num5 = 0;
        float num6 = this.preferredSpacing.x;
        if (this.minSpacingEnabled)
        {
            num6 = Mathf.Lerp(this.minSpacing.x, this.preferredSpacing.x, t);
        }
        _ = this.preferredSpacing;
        this.childRowsIndexes.Clear();
        if (this.m_forceAllWidthsEqual)
        {
            int num7 = (base.rectChildren.Count + num4 - 1) / num4;
            float num8 = Mathf.Lerp(base.GetTotalMinSize(num), base.GetTotalPreferredSize(num), t);
            float num9 = (base.GetTotalPreferredSize(num) - (float)base.padding.horizontal - this.preferredSpacing.x * (float)(num7 - 1)) / (float)num7;
            float num10 = (num8 - (float)base.padding.horizontal - num6 * (float)(num7 - 1)) / (float)num7;
            float num11 = base.padding.left;
            num7 = Mathf.FloorToInt((num3 + num6 + 0.001f) / (num10 + num6));
            if (num7 > base.rectChildren.Count)
            {
                num7 = base.rectChildren.Count;
            }
            if (num7 < 1)
            {
                num7 = 1;
            }
            float num12 = num3 - (num10 + num6) * (float)num7 + num6;
            if (num12 > 0f)
            {
                float num13 = (num9 + this.preferredSpacing.x) * (float)num7 - this.preferredSpacing.x;
                num13 += num12 - num3;
                float num14 = Mathf.Min(1f, num12 / num13);
                num10 += num14 * (num9 - num10);
                num6 += num14 * (this.preferredSpacing.x - num6);
                num11 = base.GetStartOffset(num, (num10 + num6) * (float)num7 - num6);
            }
            float num15 = num11;
            int num16 = 0;
            for (int i = 0; i < base.rectChildren.Count; i++)
            {
                num16++;
                if (num16 > num7)
                {
                    num5++;
                    num16 = 1;
                    int num17 = base.rectChildren.Count - i;
                    num11 = ((num17 >= num7) ? num15 : base.GetStartOffset(num, (num10 + num6) * (float)num17 - num6));
                }
                RectTransform rectTransform = base.rectChildren[i];
                float num18 = 1f;
                if (flag)
                {
                    base.SetChildAlongAxisWithScale(rectTransform, 0, num11, num10, num18);
                }
                else
                {
                    float num19 = (num10 - rectTransform.sizeDelta[0]) * alignmentOnAxis;
                    base.SetChildAlongAxisWithScale(rectTransform, 0, num11 + num19, num18);
                }
                this.childRowsIndexes.Add(num5);
                num11 += num10 * num18 + num6;
            }
            num5++;
        }
        else
        {
            (float, float, float, float)[] array = new(float, float, float, float)[base.rectChildren.Count];
            int num20 = 0;
            float num21 = 0f;
            float num22 = 0f - num6;
            float num23 = 0f;
            float num24 = Mathf.Lerp(this.allMin, this.allPreferred, t);
            for (int j = 0; j <= base.rectChildren.Count; j++)
            {
                float min = 0f;
                float preferred = 0f;
                float flexible = 0f;
                float num25 = num3 - num22 - num6 + 0.001f;
                float num27;
                if (j < base.rectChildren.Count)
                {
                    RectTransform rectTransform2 = base.rectChildren[j];
                    this.GetChildSizes(rectTransform2, num, flag, childForceExpand, out min, out preferred, out flexible);
                    float item = (flag2 ? rectTransform2.localScale[num] : 1f);
                    float num26 = Mathf.Lerp(min, preferred, t);
                    num27 = num26;
                    if (this.prioritiseChildPrefferedWidth && num27 < num3)
                    {
                        num27 = Mathf.Min(preferred, num3);
                    }
                    num24 -= num26;
                    array[j].Item4 = preferred;
                    array[j].Item1 = num27;
                    array[j].Item2 = flexible;
                    array[j].Item3 = item;
                }
                else
                {
                    num27 = num25 + 1f;
                }
                if (num27 > num25 && j > num20)
                {
                    float num28 = base.padding.left;
                    float num29 = 0f;
                    float num30 = num3 - num22;
                    if (num30 > 0f)
                    {
                        if (num23 > 0f)
                        {
                            float num31 = Mathf.Min(1f, num30 / num23);
                            for (int k = num20; k < j; k++)
                            {
                                (float, float, float, float) tuple = array[k];
                                float num32 = tuple.Item4 - tuple.Item1;
                                if (num32 > 0f)
                                {
                                    num32 *= num31;
                                    array[k].Item1 += num32;
                                    num30 -= num32;
                                }
                            }
                        }
                        if (num30 > 0f)
                        {
                            if (num21 == 0f)
                            {
                                num28 = base.GetStartOffset(num, num22);
                            }
                            else if (num21 > 0f)
                            {
                                num29 = num30 / num21;
                            }
                        }
                    }
                    for (int l = num20; l < j; l++)
                    {
                        (float, float, float, float) tuple2 = array[l];
                        float num33 = tuple2.Item1 + tuple2.Item2 * num29;
                        RectTransform rectTransform3 = base.rectChildren[l];
                        if (flag)
                        {
                            base.SetChildAlongAxisWithScale(rectTransform3, 0, num28, num33, tuple2.Item3);
                        }
                        else
                        {
                            float num34 = (num33 - rectTransform3.sizeDelta[0]) * alignmentOnAxis;
                            base.SetChildAlongAxisWithScale(rectTransform3, 0, num28 + num34, tuple2.Item3);
                        }
                        this.childRowsIndexes.Add(num5);
                        num28 += num33 * tuple2.Item3 + num6;
                    }
                    num21 = 0f;
                    num22 = 0f - num6;
                    num23 = 0f;
                    num20 = j;
                    num5++;
                }
                num21 += flexible;
                num22 += num6 + num27;
                num23 += preferred - num27;
            }
        }
        this.rowHeights.Clear();
    }

    public override void CalculateLayoutInputVertical()
    {
        float num = base.padding.vertical;
        bool controlSize = this.m_ChildControlHeight;
        bool flag = this.m_ChildScaleHeight;
        bool childForceExpand = this.m_ChildForceExpandHeight;
        int count = base.rectChildren.Count;
        int num2 = 0;
        float num3 = 0f;
        float num4 = 0f;
        float num5 = 0f;
        float num6 = (this.minSpacingEnabled ? this.minSpacing.y : this.preferredSpacing.y);
        float num7 = num - num6;
        float num8 = num - this.preferredSpacing.y;
        float num9 = 0f - this.preferredSpacing.y;
        for (int i = 0; i < count; i++)
        {
            if (i >= base.rectChildren.Count)
            {
                continue;
            }
            RectTransform rectTransform = base.rectChildren[i];
            this.GetChildSizes(rectTransform, 1, controlSize, childForceExpand, out var min, out var preferred, out var flexible);
            if (flag)
            {
                float num10 = rectTransform.localScale[1];
                min *= num10;
                preferred *= num10;
                flexible *= num10;
            }
            num3 = Mathf.Max(num3, min);
            num4 = Mathf.Max(num4, preferred);
            num5 = Mathf.Max(num5, flexible);
            if (i == count - 1 || this.childRowsIndexes.Count <= i + 1 || num2 != this.childRowsIndexes[i + 1])
            {
                if (this.rowForceExpandHeight && num5 < 1f)
                {
                    num5 = 1f;
                }
                num7 += num3 + num6;
                this.rowHeights.Add((num3, num4, num5));
                num8 += num4 + this.preferredSpacing.y;
                num9 += num5 + this.preferredSpacing.y;
                num3 = 0f;
                num4 = 0f;
                num5 = 0f;
                num2++;
            }
        }
        num8 = Mathf.Max(num7, num8);
        base.SetLayoutInputForAxis(num7, num8, num9, 1);
    }

    public override void SetLayoutVertical()
    {
        float num = base.rectTransform.rect.size[1];
        bool flag = this.m_ChildControlHeight;
        bool flag2 = this.m_ChildScaleHeight;
        bool childForceExpand = this.m_ChildForceExpandHeight;
        float alignmentOnAxis = base.GetAlignmentOnAxis(1);
        float num2 = base.padding.top;
        float num3 = 0f;
        float num4 = num - base.GetTotalPreferredSize(1);
        if (num4 > 0f)
        {
            if (base.GetTotalFlexibleSize(1) == 0f)
            {
                num2 = base.GetStartOffset(1, base.GetTotalPreferredSize(1) - (float)base.padding.vertical);
            }
            else if (base.GetTotalFlexibleSize(1) > 0f)
            {
                num3 = num4 / base.GetTotalFlexibleSize(1);
            }
        }
        float t = 0f;
        if (base.GetTotalMinSize(1) != base.GetTotalPreferredSize(1))
        {
            t = Mathf.Clamp01((num - base.GetTotalMinSize(1)) / (base.GetTotalPreferredSize(1) - base.GetTotalMinSize(1)));
        }
        float num5 = (this.minSpacingEnabled ? Mathf.Lerp(this.minSpacing.y, this.preferredSpacing.y, t) : this.preferredSpacing.y);
        int num6 = -1;
        float num7 = 0f;
        float num8 = num2;
        for (int i = 0; i < base.rectChildren.Count; i++)
        {
            if (this.childRowsIndexes[i] != num6)
            {
                num2 = num8;
                num6 = this.childRowsIndexes[i];
                (float, float, float) tuple = this.rowHeights[num6];
                num7 = Mathf.Lerp(tuple.Item1, tuple.Item2, t);
                num7 += tuple.Item3 * num3;
                num8 = num2 + num7 + num5;
            }
            RectTransform rectTransform = base.rectChildren[i];
            this.GetChildSizes(rectTransform, 1, flag, childForceExpand, out var min, out var preferred, out var flexible);
            float num9 = (flag2 ? rectTransform.localScale[1] : 1f);
            float num10 = Mathf.Clamp(num7, min, (flexible > 0f) ? num7 : preferred);
            num4 = num7 - num10 * num9;
            float num11 = num2 + num4 * alignmentOnAxis;
            if (flag)
            {
                base.SetChildAlongAxisWithScale(rectTransform, 1, num11, num10, num9);
                continue;
            }
            float num12 = (num10 - rectTransform.sizeDelta[1]) * alignmentOnAxis;
            base.SetChildAlongAxisWithScale(rectTransform, 1, num11 + num12, num9);
        }
    }
}
