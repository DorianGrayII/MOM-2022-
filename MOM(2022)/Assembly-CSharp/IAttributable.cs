using MOM;

public interface IAttributable
{
    Attributes GetAttributes();

    void AfterDeserialize();

    void AttributesChanged();
}
