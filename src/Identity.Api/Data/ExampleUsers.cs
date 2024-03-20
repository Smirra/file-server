namespace Identity.Api.Data;
public static class ExampleUsers
{
    public static ExampleUser Bob { get; } = new()
    {
        UserName = "bob",
        Email = "bob@bob.com",
        Password = "Pass123$",
    };

    public static ExampleUser Alice { get; } = new()
    {
        UserName = "alice",
        Email = "alice@alice.com",
        Password = "Pass123$",
    };

    public static ExampleUser Carlos { get; } = new()
    {
        UserName = "carlos",
        Email = "carlos@carlos.com",
        Password = "Pass123$",
    };
}

public class ExampleUser
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

