using System.Text.RegularExpressions;

namespace Hubtel.Wallets.Api.Utils
{
    public static class Utilities
    {
        public static bool IsValidCreditCardNumber(string creditCardNumber)
        {
            // Remove any non-digit characters from the credit card number
            string cleanedNumber = Regex.Replace(creditCardNumber, @"\D", "");

            // Check if the cleaned number matches the expected pattern
            return Regex.IsMatch(cleanedNumber, @"^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35[0-9]{3})[0-9]{11})$");
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            // Remove any non-digit characters from the phone number
            string cleanedNumber = Regex.Replace(phoneNumber, @"\D", "");

            // Check if the cleaned number matches the expected pattern
            return Regex.IsMatch(cleanedNumber, @"^(?:\+?233|0)[23456789]\d{8}$");
        }
    }
}
