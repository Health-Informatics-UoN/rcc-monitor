
namespace Monitor.Constants;

public static class ClientRoutes
{
  public const string ResetPassword = "/account/password";

  public const string ResendResetPassword = "/account/password/resend";

  public const string ConfirmAccount = "/account/confirm";

  public const string ResendConfirm = "/account/confirm/resend";

  public const string ConfirmEmailChange = "/account/ConfirmEmailChange";
  
  public const string ConfirmAccountActivation = "/account/activate"; // path to account activation page
}
