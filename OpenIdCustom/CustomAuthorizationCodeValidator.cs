/*
public class CustomAuthorizationCodeValidator : IAuthorizationCodeValidator
{
    private readonly IMongoCollection<AuthorizationCode> _codes;

    public CustomAuthorizationCodeValidator(IMongoDatabase database)
    {
        _codes = database.GetCollection<AuthorizationCode>(nameof(AuthorizationCode));
    }

    public async Task ValidateAuthorizationCodeAsync(ValidateAuthorizationCodeContext context)
    {
        // Your custom validation logic here
        if (context.Code == null)
        {
            context.Fail("Invalid authorization code.");
            return;
        }

        var code = await _codes.Find(x => x.Code == context.Code).FirstOrDefaultAsync();
        if (code == null)
        {
            context.Fail("Invalid authorization code.");
            return;
        }

        // Call the base validation logic
        await base.ValidateAuthorizationCodeAsync(context);
    }
}
*/