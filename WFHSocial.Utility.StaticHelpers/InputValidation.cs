using System.Text.RegularExpressions;

namespace WFHSocial.Utility.StaticHelpers
{
    public static class InputValidation
    {
        public static bool ContainsHtmlOrScript(List<string> input)
        {
            string pattern = @"<.*?>|<script.*?>.*?</script>";
            foreach (string item in input)
            {               
                if (Regex.IsMatch(item, pattern, RegexOptions.IgnoreCase))
                {
                    return true;
                }
            }return false;
        }
        public static bool ContainsHtmlOrScript(string input)
        {
            string pattern = @"<.*?>|<script.*?>.*?</script>";
            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
        }
    }
}
