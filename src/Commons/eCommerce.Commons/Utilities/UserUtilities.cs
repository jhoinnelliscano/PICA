
namespace eCommerce.Commons.Utilities
{
    public static class UserUtilities
    {
        public static string GetUserId(string claim) 
        {
            var userIdIdems = claim.Split("|");
            var userId = userIdIdems[1];
            return userId;
        }

        public static string GetUserAuthenticationType(string claim)
        {
            var userIdIdems = claim.Split("|");
            var userId = userIdIdems[0];
            return userId;
        }
    }
}
