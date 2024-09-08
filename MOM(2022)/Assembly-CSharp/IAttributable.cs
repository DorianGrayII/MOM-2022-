using MOM;
using System;

public interface IAttributable
{
    void AfterDeserialize();
    void AttributesChanged();
    Attributes GetAttributes();
}

