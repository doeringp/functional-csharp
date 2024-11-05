using LanguageExt.Common;

namespace Functional.TryLanguageExt;

public class TryMonadTests
{
    private static int Divide(int x, int y) => x / y;
    
    [Fact]
    public void Try_ShouldCaptureException()
    {
        var division = Try(() => Divide(10, 0));
        
        var result = division.Try();

        result.IsFaulted.Should().BeTrue();
        result.IfFail(ex => ex.Should().BeOfType<DivideByZeroException>());
    }
    
    [Fact]
    public void MonadicBind_MethodChaining()
    {
        var division = Try(() => Divide(10, 2))
            .Bind(a => Try(() => Divide(a, 2)))
            .Bind(b => Try(() => Divide(b, 2)));

        division.Try().Match(i => i, _ => -1).Should().Be(1);
    }
    
    [Fact]
    public void MonadicBind_Linq()
    {
        var division =
            from a in Try(() => Divide(10, 2))
            from b in Try(() => Divide(a, 2))
            from c in Try(() => Divide(b, 2))
            select c;

        division.Try().Match(i => i, _ => -1).Should().Be(1);
    }
    
    [Fact]
    public void MonadicBind_Linq_ShouldFail()
    {
        var division =
            from a in Try(() => Divide(10, 2))
            from b in Try(() => Divide(a, 2))
            from c in Try(() => Divide(b, 0))
            select c;

        division.Try().IsFaulted.Should().BeTrue();
    }

    [Fact]
    public async Task TryAsync_ShouldCaptureException()
    {
        var readFile = TryAsync(() => File.ReadAllTextAsync("nonexistingpath"));
        
        var result = await readFile.Try();
        
        result.IsFaulted.Should().BeTrue();
        result.IfFail(ex => ex.Should().BeOfType<FileNotFoundException>());
    }
}