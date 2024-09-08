using MHUtils;
using System;

public class TestTask : ITask
{
    public object Execute()
    {
        int num = 0;
        for (int i = 0; i < 0xf4240; i++)
        {
            num = i;
        }
        return num;
    }
}

