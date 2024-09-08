using MHUtils;

public class TestTask : ITask
{
    public object Execute()
    {
        int num = 0;
        for (int i = 0; i < 1000000; i++)
        {
            num = i;
        }
        return num;
    }
}
