using System.Text.RegularExpressions;

namespace DoingTasks.SharedKernel.Util
{
    public static class EmailRules
    {
        public const int EmailMaxLength = 254;
        public const int EmailMinLength = 5;
        public static bool Verify(string email)
        {
            var regexEmail = new Regex(@"^(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");

            return regexEmail.IsMatch(email) && email.Length <= EmailMaxLength && email.Length >= EmailMinLength;

        }
    }
}
