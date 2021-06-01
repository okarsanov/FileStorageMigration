using System.Text;

namespace Helpers
{
    public static class Hex
    {

        static readonly string _hex = "0123456789abcdef";
        public static string ToHexString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder(bytes.Length * 2);

            foreach (byte b in bytes)
            {
                builder.Append(_hex[b >> 4]);
                builder.Append(_hex[b & 0xF]);
            }

            return builder.ToString();
        }
    }
}
