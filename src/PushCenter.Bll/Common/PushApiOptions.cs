using PushCenter.Common.Attributes;

namespace PushCenter.Bll.Common
{
    [OptionModel]
    public class PushApiOptions
    {
        public string Subject { get; set; }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }
    }
}
