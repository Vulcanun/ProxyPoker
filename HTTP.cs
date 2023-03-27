using System.Net;
using System;
using System.Text.RegularExpressions;

namespace ProxyPoke
{
    internal class HTTP
    {
        public static int RequestUrl(string url, string proxyAddress = null, bool exceptionArg = false)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 5000;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36";
            if (!string.IsNullOrEmpty(proxyAddress))
            {
                request.Proxy = new WebProxy(proxyAddress);
            }
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return (int)response.StatusCode;
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    string pattern = @"\((\d+)\)";
                    MatchCollection matches = Regex.Matches(ex.Message, pattern);
                    foreach (Match match in matches)
                    {
                        return Convert.ToInt32(match.Value.Replace("(", String.Empty).Replace(")", String.Empty));
                    }
                }

                if (exceptionArg)
                {
                    Console.WriteLine($"[Exception] Failed to request {url} with the following error: {ex.Message}");
                }
                return 999;
            }
        }

    }
}
