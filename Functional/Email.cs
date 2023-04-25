using Xunit.Abstractions;

namespace Functional;

public class Email
{
    public string Value { get; }

    public static Option<Email> Create(string emailAddress)
        => IsValid(emailAddress) ? Some(new Email(emailAddress)) : None;

    private Email(string emailAddress) 
        => Value = emailAddress;

    private static bool IsValid(Option<string> emailAddress)
        => emailAddress.Match
        (
            (value) => value.Contains('@'),
            () => false
        );

    public static implicit operator string(Email email) => email.Value;

    public override string ToString() => Value;
}

public class EmailTests
{
    private readonly ITestOutputHelper _output;

    public EmailTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ValidEmailFormat_ShouldBeSome()
    {
        var email = Email.Create("john.doe@example.com");

        Assert.True(email.IsSome);
        
        email.IfSome(m => SendEmail(m));
    }

    [Fact]
    public void BadEmailFormat_ShouldBeNone()
    {
        var email = Email.Create("john.de");
        
        Assert.True(email.IsNone);
        
        email.IfSome(m => SendEmail(m));
    }

    private void SendEmail(string emailAddress)
        => _output.WriteLine($"Sent email to {emailAddress}");
}