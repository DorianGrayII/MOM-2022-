using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
    [TupleElementNames(new string[] { "min", "preferred", "flexible" })]
    private List<ValueTuple<float, float, float>> rowHeights = new List<ValueTuple<float, float, float>>();
    private float allPreferred;
    private float allMin;
    private float maxMin;
    private float maxPreferred;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        float horizontal = base.padding.horizontal;
        bool childForceExpandWidth = this.m_ChildForceExpandWidth;
        float a = 0f;
        float b = 0f;
        float totalFlexible = 0f;
        int count = base.rectChildren.Count;
        float[] widths = (this.m_PreferredRows > 1) ? new float[count * 2] : null;
        this.maxMin = 0f;
        this.maxPreferred = 0f;
        for (int i = 0; i < count; i++)
        {
            float num8;
            float num9;
            float num10;
            RectTransform child = base.rectChildren[i];
            this.GetChildSizes(child, 0, this.m_ChildControlWidth, childForceExpandWidth, out num8, out num9, out num10);
            if (this.m_ChildScaleWidth)
            {
                float num11 = child.localScale[0];
                num8 *= num11;
                num9 *= num11;
                num10 *= num11;
            }
            this.maxMin = Mathf.Max(this.maxMin, num8);
            this.maxPreferred = Mathf.Max(this.maxPreferred, num9);
            a += num8;
            b += num9;
            totalFlexible += num10;
            if (widths != null)
            {
                widths[i * 2] = num8;
                widths[(i * 2) + 1] = num9;
            }
        }
        float x = this.preferredSpacing.x;
        if (this.minSpacingEnabled)
        {
            x = this.minSpacing.x;
        }
        if (base.rectChildren.Count > 1)
        {
            a += x * (base.rectChildren.Count - 1);
            b += this.preferredSpacing.x * (base.rectChildren.Count - 1);
        }
        b = Mathf.Max(a, b);
        this.allMin = a + x;
        this.allPreferred = b + this.preferredSpacing.x;
        if ((this.m_PreferredRows > 1) || this.m_forceAllWidthsEqual)
        {
            a = this.CalcWidthForPrefferedRows(widths, 0, x, this.maxMin, a);
            b = this.CalcWidthForPrefferedRows(widths, 1, this.preferredSpacing.x, this.maxPreferred, b);
        }
        a += horizontal;
        base.SetLayoutInputForAxis(a, b + horizontal, totalFlexible, 0);
    }

    public override void CalculateLayoutInputVertical()
    {
        bool childControlHeight = this.m_ChildControlHeight;
        bool childScaleHeight = this.m_ChildScaleHeight;
        bool childForceExpandHeight = this.m_ChildForceExpandHeight;
        int count = base.rectChildren.Count;
        int num2 = 0;
        float a = 0f;
        float num4 = 0f;
        float num5 = 0f;
        float num6 = this.minSpacingEnabled ? this.minSpacing.y : this.preferredSpacing.y;
        float vertical = base.padding.vertical;
        float totalMin = vertical - num6;
        float b = vertical - this.preferredSpacing.y;
        float totalFlexible = -this.preferredSpacing.y;
        for (int i = 0; i < count; i++)
        {
            if (i < base.rectChildren.Count)
            {
                float num11;
                float num12;
                float num13;
                RectTransform child = base.rectChildren[i];
                this.GetChildSizes(child, 1, childControlHeight, childForceExpandHeight, out num11, out num12, out num13);
                if (childScaleHeight)
                {
                    float num14 = child.localScale[1];
                    num11 *= num14;
                    num12 *= num14;
                    num13 *= num14;
                }
                a = Mathf.Max(a, num11);
                num4 = Mathf.Max(num4, num12);
                num5 = Mathf.Max(num5, num13);
                if ((i == (count - 1)) || ((this.childRowsIndexes.Count <= (i + 1)) || (num2 != this.childRowsIndexes[i + 1])))
                {
                    if (this.rowForceExpandHeight && (num5 < 1f))
                    {
                        num5 = 1f;
                    }
                    totalMin += a + num6;
                    this.rowHeights.Add((a, num4, num5));
                    b += num4 + this.preferredSpacing.y;
                    totalFlexible += num5 + this.preferredSpacing.y;
                    a = 0f;
                    num4 = 0f;
                    num5 = 0f;
                    num2++;
                }
            }
        }
        base.SetLayoutInputForAxis(totalMin, Mathf.Max(totalMin, b), totalFlexible, 1);
    }

    private float CalcWidthForPrefferedRows(float[] widths, int offset, float spacing, float minWidth, float remainingWidth)
    {
        int num = Mathf.Max(1, this.m_PreferredRows);
        if (this.m_forceAllWidthsEqual)
        {
            float num5 = (remainingWidth + spacing) / ((float) base.rectChildren.Count);
            int num6 = ((base.rectChildren.Count + num) - 1) / num;
            return Mathf.Max(minWidth, (num6 * num5) - spacing);
        }
        float num2 = Mathf.Max(minWidth, remainingWidth / ((float) num));
        float num3 = num - 1;
        int index = offset;
        while (index < widths.Length)
        {
            float num7 = widths[index] + spacing;
            float num8 = num7;
            remainingWidth -= num8;
            index += 2;
            while (true)
            {
                if (index < widths.Length)
                {
                    num7 = widths[index] + spacing;
                    if ((num8 + num7) <= num2)
                    {
                        num8 += num7;
                        remainingWidth -= num7;
                        index += 2;
                        continue;
                    }
                }
                if (index < widths.Length)
                {
                    float num9 = num3 * num2;
                    float num10 = remainingWidth - num9;
                    if (num10 > (num7 - (num2 - num8)))
                    {
                        num2 = num8 + num7;
                        remainingWidth -= num7;
                        index += 2;
                    }
                    else if (num10 > 0f)
                    {
                        num2 += num10;
                    }
                }
                num3--;
                break;
            }
        }
        return (num2 - spacing);
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

    public override unsafe void SetLayoutHorizontal()
    {
        int axis = 0;
        float num2 = base.rectTransform.rect.size[0];
        bool childControlWidth = this.m_ChildControlWidth;
        bool childScaleWidth = this.m_ChildScaleWidth;
        bool childForceExpandWidth = this.m_ChildForceExpandWidth;
        float alignmentOnAxis = base.GetAlignmentOnAxis(0);
        float b = num2 - base.padding.horizontal;
        float t = 0f;
        if (this.m_PreferredRows <= 0)
        {
            if (this.maxMin != this.maxPreferred)
            {
                t = Mathf.Clamp01((num2 - this.maxMin) / (this.maxPreferred - this.maxMin));
            }
            else if (this.maxPreferred <= num2)
            {
                t = 1f;
            }
        }
        else
        {
            float totalMinSize = base.GetTotalMinSize(axis);
            float totalPreferredSize = base.GetTotalPreferredSize(axis);
            if (totalMinSize != totalPreferredSize)
            {
                t = Mathf.Clamp01((num2 - totalMinSize) / (totalPreferredSize - totalMinSize));
            }
            else if (totalPreferredSize <= num2)
            {
                t = 1f;
            }
        }
        int num6 = Mathf.Max(1, this.m_PreferredRows);
        int item = 0;
        float x = this.preferredSpacing.x;
        if (this.minSpacingEnabled)
        {
            x = Mathf.Lerp(this.minSpacing.x, this.preferredSpacing.x, t);
        }
        Vector2 preferredSpacing = this.preferredSpacing;
        this.childRowsIndexes.Clear();
        if (this.m_forceAllWidthsEqual)
        {
            int count = ((base.rectChildren.Count + num6) - 1) / num6;
            float num12 = ((base.GetTotalPreferredSize(axis) - base.padding.horizontal) - (this.preferredSpacing.x * (count - 1))) / ((float) count);
            float size = ((Mathf.Lerp(base.GetTotalMinSize(axis), base.GetTotalPreferredSize(axis), t) - base.padding.horizontal) - (x * (count - 1))) / ((float) count);
            float left = base.padding.left;
            count = Mathf.FloorToInt(((b + x) + 0.001f) / (size + x));
            if (count > base.rectChildren.Count)
            {
                count = base.rectChildren.Count;
            }
            if (count < 1)
            {
                count = 1;
            }
            float num15 = (b - ((size + x) * count)) + x;
            if (num15 > 0f)
            {
                float num19 = Mathf.Min((float) 1f, (float) (num15 / ((((num12 + this.preferredSpacing.x) * count) - this.preferredSpacing.x) + (num15 - b))));
                size += num19 * (num12 - size);
                x += num19 * (this.preferredSpacing.x - x);
                left = base.GetStartOffset(axis, ((size + x) * count) - x);
            }
            float num16 = left;
            int num17 = 0;
            int num20 = 0;
            while (true)
            {
                if (num20 >= base.rectChildren.Count)
                {
                    item++;
                    break;
                }
                if ((num17 + 1) > count)
                {
                    item++;
                    num17 = 1;
                    int num22 = base.rectChildren.Count - num20;
                    left = (num22 >= count) ? num16 : base.GetStartOffset(axis, ((size + x) * num22) - x);
                }
                RectTransform rect = base.rectChildren[num20];
                float scaleFactor = 1f;
                if (childControlWidth)
                {
                    base.SetChildAlongAxisWithScale(rect, 0, left, size, scaleFactor);
                }
                else
                {
                    base.SetChildAlongAxisWithScale(rect, 0, left + ((size - rect.sizeDelta[0]) * alignmentOnAxis), scaleFactor);
                }
                this.childRowsIndexes.Add(item);
                left += (size * scaleFactor) + x;
                num20++;
            }
        }
        else
        {
            ValueTuple<float, float, float, float>[] tupleArray = new ValueTuple<float, float, float, float>[base.rectChildren.Count];
            int num24 = 0;
            float num25 = 0f;
            float requiredSpaceWithoutPadding = -x;
            float num27 = 0f;
            float num28 = Mathf.Lerp(this.allMin, this.allPreferred, t);
            for (int i = 0; i <= base.rectChildren.Count; i++)
            {
                float num30;
                float min = 0f;
                float preferred = 0f;
                float flexible = 0f;
                float num34 = ((b - requiredSpaceWithoutPadding) - x) + 0.001f;
                if (i >= base.rectChildren.Count)
                {
                    num30 = num34 + 1f;
                }
                else
                {
                    RectTransform child = base.rectChildren[i];
                    this.GetChildSizes(child, axis, childControlWidth, childForceExpandWidth, out min, out preferred, out flexible);
                    float num35 = childScaleWidth ? child.localScale[axis] : 1f;
                    float num36 = Mathf.Lerp(min, preferred, t);
                    num30 = num36;
                    if (this.prioritiseChildPrefferedWidth && (num30 < b))
                    {
                        num30 = Mathf.Min(preferred, b);
                    }
                    num28 -= num36;
                    tupleArray[i].Item4 = preferred;
                    tupleArray[i].Item1 = num30;
                    tupleArray[i].Item2 = flexible;
                    tupleArray[i].Item3 = num35;
                }
                if ((num30 > num34) && (i > num24))
                {
                    float left = base.padding.left;
                    float num38 = 0f;
                    float num39 = b - requiredSpaceWithoutPadding;
                    if (num39 > 0f)
                    {
                        if (num27 > 0f)
                        {
                            float num40 = Mathf.Min((float) 1f, (float) (num39 / num27));
                            for (int j = num24; j < i; j++)
                            {
                                ValueTuple<float, float, float, float> tuple = tupleArray[j];
                                float num42 = tuple.Item4 - tuple.Item1;
                                if (num42 > 0f)
                                {
                                    num42 *= num40;
                                    float* localPtr1 = &tupleArray[j].Item1;
                                    localPtr1[0] += num42;
                                    num39 -= num42;
                                }
                            }
                        }
                        if (num39 > 0f)
                        {
                            if (num25 == 0f)
                            {
                                left = base.GetStartOffset(axis, requiredSpaceWithoutPadding);
                            }
                            else if (num25 > 0f)
                            {
                                num38 = num39 / num25;
                            }
                        }
                    }
                    int index = num24;
                    while (true)
                    {
                        if (index >= i)
                        {
                            num25 = 0f;
                            requiredSpaceWithoutPadding = -x;
                            num27 = 0f;
                            num24 = i;
                            item++;
                            break;
                        }
                        ValueTuple<float, float, float, float> tuple2 = tupleArray[index];
                        float size = tuple2.Item1 + (tuple2.Item2 * num38);
                        RectTransform rect = base.rectChildren[index];
                        if (childControlWidth)
                        {
                            base.SetChildAlongAxisWithScale(rect, 0, left, size, tuple2.Item3);
                        }
                        else
                        {
                            base.SetChildAlongAxisWithScale(rect, 0, left + ((size - rect.sizeDelta[0]) * alignmentOnAxis), tuple2.Item3);
                        }
                        this.childRowsIndexes.Add(item);
                        left += (size * tuple2.Item3) + x;
                        index++;
                    }
                }
                num25 += flexible;
                requiredSpaceWithoutPadding += x + num30;
                num27 += preferred - num30;
            }
        }
        this.rowHeights.Clear();
    }

    public override void SetLayoutVertical()
    {
        float num = base.rectTransform.rect.size[1];
        bool childControlHeight = this.m_ChildControlHeight;
        bool childScaleHeight = this.m_ChildScaleHeight;
        bool childForceExpandHeight = this.m_ChildForceExpandHeight;
        float alignmentOnAxis = base.GetAlignmentOnAxis(1);
        float top = base.padding.top;
        float num4 = 0f;
        float num5 = num - base.GetTotalPreferredSize(1);
        if (num5 > 0f)
        {
            if (base.GetTotalFlexibleSize(1) == 0f)
            {
                top = base.GetStartOffset(1, base.GetTotalPreferredSize(1) - base.padding.vertical);
            }
            else if (base.GetTotalFlexibleSize(1) > 0f)
            {
                num4 = num5 / base.GetTotalFlexibleSize(1);
            }
        }
        float t = 0f;
        if (base.GetTotalMinSize(1) != base.GetTotalPreferredSize(1))
        {
            t = Mathf.Clamp01((num - base.GetTotalMinSize(1)) / (base.GetTotalPreferredSize(1) - base.GetTotalMinSize(1)));
        }
        float num7 = this.minSpacingEnabled ? Mathf.Lerp(this.minSpacing.y, this.preferredSpacing.y, t) : this.preferredSpacing.y;
        int num8 = -1;
        float num9 = 0f;
        float num10 = top;
        for (int i = 0; i < base.rectChildren.Count; i++)
        {
            float num12;
            float num13;
            float num14;
            if (this.childRowsIndexes[i] != num8)
            {
                top = num10;
                num8 = this.childRowsIndexes[i];
                ValueTuple<float, float, float> tuple = this.rowHeights[num8];
                num9 = Mathf.Lerp(tuple.Item1, tuple.Item2, t) + (tuple.Item3 * num4);
                num10 = (top + num9) + num7;
            }
            RectTransform child = base.rectChildren[i];
            this.GetChildSizes(child, 1, childControlHeight, childForceExpandHeight, out num12, out num13, out num14);
            float scaleFactor = childScaleHeight ? child.localScale[1] : 1f;
            float size = Mathf.Clamp(num9, num12, (num14 > 0f) ? num9 : num13);
            float pos = top + ((num9 - (size * scaleFactor)) * alignmentOnAxis);
            if (childControlHeight)
            {
                base.SetChildAlongAxisWithScale(child, 1, pos, size, scaleFactor);
            }
            else
            {
                base.SetChildAlongAxisWithScale(child, 1, pos + ((size - child.sizeDelta[1]) * alignmentOnAxis), scaleFactor);
            }
        }
    }

    public int preferredRows
    {
        get
        {
            return this.m_PreferredRows;
        }
        set
        {
            base.SetProperty<int>(ref this.m_PreferredRows, value);
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
            base.SetProperty<bool>(ref this.m_PrioritiseChildPrefferedWidth, value);
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
            base.SetProperty<bool>(ref this.m_PrioritiseChildPrefferedWidth, value);
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
            base.SetProperty<Vector2>(ref this.m_PreferredSpacing, value);
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
            base.SetProperty<bool>(ref this.m_MinSpacingEnabled, value);
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
            base.SetProperty<Vector2>(ref this.m_MinSpacing, value);
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
            base.SetProperty<bool>(ref this.m_ChildForceExpandWidth, value);
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
            base.SetProperty<bool>(ref this.m_ChildForceExpandHeight, value);
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
            base.SetProperty<bool>(ref this.m_ChildControlWidth, value);
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
            base.SetProperty<bool>(ref this.m_ChildControlHeight, value);
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
            base.SetProperty<bool>(ref this.m_ChildScaleWidth, value);
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
            base.SetProperty<bool>(ref this.m_ChildScaleHeight, value);
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
            base.SetProperty<bool>(ref this.m_RowForceExpandHeight, value);
        }
    }
}

