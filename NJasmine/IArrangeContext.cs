using System;

namespace NJasmine
{
    public interface IArrangeContext
    {
        void beforeEach(Action action);
        void afterEach(Action action);
        TFixture importNUnit<TFixture>() where TFixture : class, new();
        TArranged arrange<TArranged>() where TArranged : class, new();
        TArranged arrange<TArranged>(Func<TArranged> factory);
        void arrange(Action action);
        void arrange(string description, Action action);
    }
}