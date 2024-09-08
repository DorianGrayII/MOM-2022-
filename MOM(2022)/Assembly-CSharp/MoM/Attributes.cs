// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MOM.Attributes
using System.Collections.Generic;
using DBDef;
using DBEnum;
using MHUtils;
using MOM;
using ProtoBuf;
using UnityEngine;

[ProtoContract]
public class Attributes
{
    [ProtoIgnore]
    public IAttributable owner;

    [ProtoIgnore]
    private bool dirty = true;

    [ProtoMember(1)]
    public NetDictionary<DBReference<Tag>, FInt> baseAttributes = new NetDictionary<DBReference<Tag>, FInt>();

    [ProtoMember(2)]
    public bool baseOnlyMode;

    [ProtoMember(3)]
    public int iteration;

    public NetDictionary<DBReference<Tag>, FInt> finalAttributes = new NetDictionary<DBReference<Tag>, FInt>();

    public List<Multitype<long, Tag, FInt, bool>> changesLog;

    public Attributes()
    {
    }

    public Attributes(IAttributable owner)
    {
        this.owner = owner;
    }

    public Attributes(IAttributable owner, CountedTag[] sourceTag)
    {
        this.owner = owner;
        if (sourceTag != null)
        {
            foreach (CountedTag countedTag in sourceTag)
            {
                this.AddToBase(countedTag.tag, countedTag.amount);
            }
        }
    }

    public Attributes(IAttributable owner, List<CountedTag> sourceTag)
    {
        this.owner = owner;
        if (sourceTag == null)
        {
            return;
        }
        foreach (CountedTag item in sourceTag)
        {
            this.AddToBase(item.tag, item.amount);
        }
    }

    public Attributes(IAttributable owner, Tag[] sourceTag, FInt count = default(FInt))
    {
        this.owner = owner;
        if (sourceTag != null)
        {
            if (count.storage == 0)
            {
                count = FInt.ONE;
            }
            foreach (Tag t in sourceTag)
            {
                this.AddToBase(t, count);
            }
        }
    }

    public void InitializeFrom(Tag[] sourceTag)
    {
        if (sourceTag != null)
        {
            foreach (Tag t in sourceTag)
            {
                this.AddToBase(t, FInt.ONE);
            }
        }
    }

    public Attributes Clone(IAttributable owner)
    {
        Attributes attributes = Serializer.DeepClone(this);
        attributes.owner = owner;
        return attributes;
    }

    public object AddToBase(CountedTag ct, bool setDirty = true, bool returnChangeData = false)
    {
        return this.AddToBase(ct.tag, ct.amount, setDirty, returnChangeData);
    }

    public object AddToBase(TAG t, FInt value, bool setDirty = true, bool returnChangeData = false)
    {
        Tag t2 = (Tag)t;
        return this.AddToBase(t2, value, setDirty, returnChangeData);
    }

    public object AddToBase(Tag t, float value, bool setDirty = true, bool returnChangeData = false)
    {
        return this.AddToBase(t, new FInt(value), setDirty, returnChangeData);
    }

    public object AddToBase(Tag t, FInt value, bool setDirty = true, bool returnChangeData = false)
    {
        if (t == null)
        {
            return null;
        }
        if (value == 0f)
        {
            return null;
        }
        if (this.baseAttributes.ContainsKey(t))
        {
            this.baseAttributes[t] += value;
        }
        else
        {
            this.baseAttributes[t] = value;
        }
        if (setDirty)
        {
            this.SetDirty();
        }
        return null;
    }

    public object SetBaseTo(CountedTag ct, bool setDirty = true, bool returnChangeData = false)
    {
        return this.SetBaseTo(ct.tag, ct.amount, setDirty, returnChangeData);
    }

    public object SetBaseTo(TAG t, float value, bool setDirty = true, bool returnChangeData = false)
    {
        return this.SetBaseTo(t, (FInt)value, setDirty, returnChangeData);
    }

    public object SetBaseTo(TAG t, FInt value, bool setDirty = true, bool returnChangeData = false)
    {
        Tag t2 = (Tag)t;
        return this.SetBaseTo(t2, value, setDirty, returnChangeData);
    }

    public object SetBaseTo(Tag t, float value, bool setDirty = true, bool returnChangeData = false)
    {
        return this.SetBaseTo(t, (FInt)value, setDirty, returnChangeData);
    }

    public object SetBaseTo(Tag t, FInt value, bool setDirty = true, bool returnChangeData = false)
    {
        if (t == null)
        {
            return null;
        }
        if (this.GetBase(t) == value)
        {
            return null;
        }
        if (value == 0f && this.baseAttributes.ContainsKey(t))
        {
            this.baseAttributes.Remove(t);
        }
        else if (value != 0f)
        {
            this.baseAttributes[t] = value;
        }
        if (setDirty)
        {
            this.SetDirty();
        }
        return null;
    }

    public FInt GetBase(TAG t)
    {
        Tag t2 = (Tag)t;
        return this.GetBase(t2);
    }

    public FInt GetBase(Tag t)
    {
        if (!this.baseAttributes.ContainsKey(t))
        {
            return FInt.ZERO;
        }
        return this.baseAttributes[t];
    }

    private FInt GetFinalIgnoreDirty(TAG t)
    {
        if (this.finalAttributes.ContainsKey((Tag)t))
        {
            return this.finalAttributes[(Tag)t];
        }
        return FInt.ZERO;
    }

    public FInt GetFinal(TAG t)
    {
        Tag t2 = (Tag)t;
        return this.GetFinal(t2);
    }

    public FInt GetFinal(Tag t)
    {
        if (!this.GetFinalDictionary().ContainsKey(t))
        {
            return FInt.ZERO;
        }
        return this.GetFinalDictionary()[t];
    }

    public void SetDirty()
    {
        if (!this.baseOnlyMode)
        {
            this.iteration++;
            this.dirty = true;
            this.owner?.AttributesChanged();
        }
    }

    public bool GetDirty()
    {
        return this.dirty;
    }

    public void ClearDirty()
    {
        this.dirty = false;
    }

    public NetDictionary<DBReference<Tag>, FInt> GetFinalDictionary(bool produceLog = false)
    {
        if (this.baseOnlyMode)
        {
            return this.baseAttributes;
        }
        if (this.dirty)
        {
            int num = 0;
            Tag tag = (Tag)TAG.HIT_POINTS;
            if (this.owner is BaseUnit && this.finalAttributes != null && this.finalAttributes.ContainsKey(tag))
            {
                num = this.finalAttributes[(Tag)TAG.HIT_POINTS].ToInt();
            }
            int num2 = 0;
            Tag tag2 = (Tag)TAG.MOVEMENT_POINTS;
            if (this.owner is BaseUnit && this.finalAttributes != null && this.finalAttributes.ContainsKey(tag2))
            {
                num2 = this.finalAttributes[(Tag)TAG.MOVEMENT_POINTS].ToInt();
            }
            if (this.baseAttributes != null)
            {
                if (this.owner != null)
                {
                    this.finalAttributes = ScriptLibrary.Call("UpdateAttributes", this.owner) as NetDictionary<DBReference<Tag>, FInt>;
                }
                else
                {
                    this.finalAttributes = this.baseAttributes;
                }
            }
            else
            {
                this.finalAttributes = new NetDictionary<DBReference<Tag>, FInt>();
            }
            this.dirty = false;
            if (this.owner is BaseUnit)
            {
                BaseUnit baseUnit = this.owner as BaseUnit;
                if (this.finalAttributes.ContainsKey((Tag)TAG.NORMAL_RANGE) || this.finalAttributes.ContainsKey((Tag)TAG.MAGIC_RANGE) || this.finalAttributes.ContainsKey((Tag)TAG.BOULDER_RANGE))
                {
                    baseUnit.rangeAttack = true;
                }
                else
                {
                    baseUnit.rangeAttack = false;
                }
            }
            if (num > 0 && this.finalAttributes.ContainsKey(tag))
            {
                int num3 = this.finalAttributes[tag].ToInt();
                if (this.owner is BaseUnit)
                {
                    BaseUnit baseUnit2 = this.owner as BaseUnit;
                    if (num3 > 0 && num3 != num)
                    {
                        int num4 = num3 - num;
                        int currentFigureHP = Mathf.Clamp(baseUnit2.currentFigureHP + num4, 1, num3);
                        baseUnit2.currentFigureHP = currentFigureHP;
                    }
                }
            }
            if (num2 > 0 && this.finalAttributes.ContainsKey(tag2))
            {
                int num5 = this.finalAttributes[tag2].ToInt();
                if (this.owner is BaseUnit)
                {
                    BaseUnit baseUnit3 = this.owner as BaseUnit;
                    if (num5 > 0 && num5 != num2 && baseUnit3.Mp > 0)
                    {
                        int num6 = num5 - num2;
                        int num7 = Mathf.Clamp(baseUnit3.Mp.ToInt() + num6, 1, num5);
                        baseUnit3.Mp = (FInt)num7;
                    }
                }
            }
        }
        return this.finalAttributes;
    }

    public bool Contains(TAG t)
    {
        Tag tag = (Tag)t;
        return this.Contains(tag, FInt.ONE);
    }

    public bool Contains(Tag tag, FInt value)
    {
        if (!this.GetFinalDictionary().ContainsKey(tag))
        {
            if (value == 0)
            {
                return true;
            }
            return false;
        }
        if (value <= 0)
        {
            return this.GetFinalDictionary()[tag] <= value;
        }
        return this.GetFinalDictionary()[tag] >= value;
    }

    public bool DoesNotContains(Tag tag)
    {
        if (!this.GetFinalDictionary().ContainsKey(tag) || this.GetFinalDictionary()[tag] == FInt.ZERO)
        {
            return true;
        }
        return false;
    }

    public bool ContainsAll(Tag[] tags)
    {
        foreach (Tag tag in tags)
        {
            if (!this.Contains(tag, FInt.ONE))
            {
                return false;
            }
        }
        return true;
    }

    public bool ContainsAll(CountedTag[] cTags)
    {
        foreach (CountedTag countedTag in cTags)
        {
            if (!this.Contains(countedTag.tag, countedTag.amount))
            {
                return false;
            }
        }
        return true;
    }

    public bool ContainsNone(Tag[] tags)
    {
        foreach (Tag tag in tags)
        {
            if (this.Contains(tag, FInt.ONE))
            {
                return false;
            }
        }
        return true;
    }

    public bool ContainsNone(CountedTag[] cTags)
    {
        foreach (CountedTag countedTag in cTags)
        {
            if (this.Contains(countedTag.tag, countedTag.amount))
            {
                return false;
            }
        }
        return true;
    }

    public bool ContainsAny(Tag[] tags)
    {
        foreach (Tag tag in tags)
        {
            if (this.Contains(tag, FInt.ONE))
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsAny(TAG[] tags)
    {
        for (int i = 0; i < tags.Length; i++)
        {
            Tag tag = (Tag)tags[i];
            if (this.Contains(tag, FInt.ONE))
            {
                return true;
            }
        }
        return false;
    }

    public bool ContainsAny(CountedTag[] cTags)
    {
        foreach (CountedTag countedTag in cTags)
        {
            if (this.Contains(countedTag.tag, countedTag.amount))
            {
                return true;
            }
        }
        return false;
    }
}
