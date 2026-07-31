using System;
using System.Globalization;
using System.Net;

namespace SearchBar.Common
{
    internal static class WebAddressParser
    {
        public static bool TryNormalize(string value, out string normalizedUrl)
        {
            normalizedUrl = null;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string candidate = value.Trim();
            if (candidate.IndexOfAny(new[] { '\r', '\n', '\t' }) >= 0 || candidate.Contains(" "))
            {
                return false;
            }

            Uri absoluteUri;
            if (Uri.TryCreate(candidate, UriKind.Absolute, out absoluteUri) && IsSupportedScheme(absoluteUri.Scheme))
            {
                normalizedUrl = absoluteUri.AbsoluteUri;
                return true;
            }

            if (candidate.StartsWith("//", StringComparison.Ordinal))
            {
                candidate = candidate.Substring(2);
            }

            IPAddress unbracketedAddress;
            if (IPAddress.TryParse(candidate, out unbracketedAddress)
                && unbracketedAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                normalizedUrl = "http://[" + unbracketedAddress + "]/";
                return true;
            }

            string host = GetHostPart(candidate);
            if (!LooksLikeWebHost(host))
            {
                return false;
            }

            string defaultScheme = IsLocalOrIp(host) ? "http://" : "https://";
            if (!Uri.TryCreate(defaultScheme + candidate, UriKind.Absolute, out absoluteUri))
            {
                return false;
            }

            normalizedUrl = absoluteUri.AbsoluteUri;
            return true;
        }

        private static bool IsSupportedScheme(string scheme)
        {
            return string.Equals(scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                || string.Equals(scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
                || string.Equals(scheme, Uri.UriSchemeFtp, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetHostPart(string value)
        {
            int end = value.IndexOfAny(new[] { '/', '?', '#' });
            string authority = end >= 0 ? value.Substring(0, end) : value;

            if (authority.StartsWith("[", StringComparison.Ordinal))
            {
                int closingBracket = authority.IndexOf(']');
                return closingBracket > 0 ? authority.Substring(1, closingBracket - 1) : authority;
            }

            int colon = authority.LastIndexOf(':');
            return colon > 0 ? authority.Substring(0, colon) : authority;
        }

        private static bool LooksLikeWebHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                return false;
            }

            if (string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            IPAddress address;
            if (IPAddress.TryParse(host, out address))
            {
                return true;
            }

            UriHostNameType hostType = Uri.CheckHostName(host);
            if (hostType != UriHostNameType.Dns || host.IndexOf('.') <= 0)
            {
                return false;
            }

            try
            {
                string asciiHost = new IdnMapping().GetAscii(host);
                int lastDot = asciiHost.LastIndexOf('.');
                return lastDot > 0 && lastDot < asciiHost.Length - 1;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private static bool IsLocalOrIp(string host)
        {
            IPAddress address;
            return string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase)
                || IPAddress.TryParse(host, out address);
        }
    }
}
