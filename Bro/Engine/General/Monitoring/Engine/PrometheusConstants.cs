#if BRO_SERVER || BRO_SERVICE || BRO_TEST

using System.Text;

namespace Bro.Monitoring
{
    public static class PrometheusConstants
    {
        public const string ExporterContentType = "text/plain; version=0.0.4; charset=utf-8";

        // ASP.NET does not want to accept the parameters in PushStreamContent for whatever reason...
        public const string ExporterContentTypeMinimal = "text/plain";

        // Use UTF-8 encoding, but provide the flag to ensure the Unicode Byte Order Mark is never
        // pre-pended to the output stream.
        public static readonly Encoding ExportEncoding = new UTF8Encoding(false);
    }
}
#endif