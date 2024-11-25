using VideoChatApp.Contracts.Request;

public class ForgotPasswordRequestBuilder
{
    private string _email = "default@test.com";

    public ForgotPasswordRequestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public ForgetPasswordRequestDTO Build()
    {
        return new ForgetPasswordRequestDTO(_email);
    }
}
