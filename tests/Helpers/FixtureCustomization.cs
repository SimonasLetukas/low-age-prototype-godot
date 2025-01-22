using System;
using System.Linq.Expressions;
using AutoFixture;

namespace TestProject1.Helpers
{
    public partial class FixtureCustomization<T>
    {
        public IFixture Fixture { get; }

        public FixtureCustomization(IFixture fixture)
        {
            Fixture = fixture;
        }

        public FixtureCustomization<T> With<TProp>(Expression<Func<T, TProp>> expr, TProp value)
        {
            Fixture.Customizations.Add(new OverridePropertyBuilder<T, TProp>(expr, value));
            return this;
        }

        public T Create() => Fixture.Create<T>();
    }

    public static class CompositionExt
    {
        public static FixtureCustomization<T> For<T>(this IFixture fixture)
            => new FixtureCustomization<T>(fixture);
    }
}