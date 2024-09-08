using DBDef;
using System;

public interface IUnit
{
    int FigureCount();
    string GetDBName();
    DescriptionInfo GetDescriptionInfo();
}

