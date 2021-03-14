using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace PushCenter.Server.Common
{
    public class TextPlainInputFormatter : TextInputFormatter
    {
        public TextPlainInputFormatter()
        {
            SupportedMediaTypes.Add("text/plain");
            SupportedEncodings.Add(UTF8EncodingWithoutBOM);
            SupportedEncodings.Add(UTF16EncodingLittleEndian);
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            using (var reader = new StreamReader(context.HttpContext.Request.Body, encoding))
            {
                return await InputFormatterResult.SuccessAsync(await reader.ReadToEndAsync());
            }
        }
    }
}