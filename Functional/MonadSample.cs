// This is an example how to implement support for Map and Bind
// in a C# class using the Select and SelectMany methods from LINQ.

namespace Functional;

public class Val<A>(A value)
{
    public A Value => value;
    
    /// <summary>Projection from one value to another (Functor map)</summary>
    public Val<B> Select<B>(Func<A, B> f) => new Val<B>(f(value));

    /// <summary>Monad bind operation</summary>
    /// <remarks>The bind function is shape-preserving</remarks>
    public Val<C> SelectMany<B, C>(Func<A, Val<B>> bind, Func<A, B, C> project)
    {
        var b = bind(value);
        var c = project(value, b.Value);
        return new Val<C>(c);
    }
}

public class MonadSampleTests
{
    [Fact]
    public void FunctorMap_Linq_Select()
    {
        var value = new Val<string>("Hello");

        Val<string> output = from s in value select s;

        Assert.Equal("Hello", output.Value);
    }
    
    [Fact]
    public void MonadicBind_Linq_SelectMany()
    {
        var value1 = new Val<string>("Hello");
        var value2 = new Val<string>("World");

        Val<string> output =
            from s1 in value1
            from s2 in value2
            select s1 + ", " + s2;

        Assert.Equal("Hello, World", output.Value);
    }
}