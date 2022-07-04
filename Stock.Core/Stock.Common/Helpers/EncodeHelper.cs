using System.Text;

namespace Stock.Common.Helpers
{
    public class EncodeHelper //MS950
    {
        public static string Big5ToString(byte[] bytes)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding big51 = Encoding.GetEncoding("big5");
            return big51.GetString(bytes);
        }

    }
}
