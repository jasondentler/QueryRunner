namespace DB2Query
{
    public static class StringExtensions
    {

// ReSharper disable InconsistentNaming
        public static string GetMD5Hash(this string input)
// ReSharper restore InconsistentNaming
        {
            input = input ?? string.Empty;
            var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = md5.ComputeHash(bs);
            var s = new System.Text.StringBuilder();
            foreach (var b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            return s.ToString();
        }

    }
}