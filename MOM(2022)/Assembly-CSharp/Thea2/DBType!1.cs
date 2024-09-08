namespace Thea2
{
    using System;
    using System.Collections.Generic;

    public class DBType<T>
    {
        public List<T> typeCollection;

        public DBType()
        {
            this.typeCollection = new List<T>();
        }
    }
}

