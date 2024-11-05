using LanguageExt.Common;

namespace Functional.TryLanguageExt;

public class TryMonadTests
{
    private static Try<int> Divide(int x, int y) => () => x / y;
    
    [Fact]
    public void IfFail_Try_ShouldCaptureException()
    {
        Try<int> divide = Divide(10, 0);
        Result<int> result = divide.Try();

        Assert.True(result.IsFaulted);
        _ = result.IfFail(ex => Assert.IsType<DivideByZeroException>(ex));
    }
    
    [Fact]
    public void MonadicBind_MethodChaining()
    {
        var result = Divide(10, 2)
            .Bind(a => Divide(a, 2))
            .Bind(b => Divide(b, 2));

        result.Match(i => i, _ => -1).Should().Be(1);
    }
    
    [Fact]
    public void MonadicBind_Linq()
    {
        var result =
            from a in Divide(10, 2)
            from b in Divide(a, 2)
            from c in Divide(b, 2)
            select c;

        result.Match(i => i, _ => -1).Should().Be(1);
    }
}