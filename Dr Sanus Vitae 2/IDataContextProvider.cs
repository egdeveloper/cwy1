using System;

namespace SanusVitae
{
    internal interface IDataContextProvider<out T>
    {
        T ProvideDataContext();
    }
}
