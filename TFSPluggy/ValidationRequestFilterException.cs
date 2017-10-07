using Microsoft.TeamFoundation.Framework.Server;

namespace TFSPluggy
{
    class ValidationRequestFilterException : RequestFilterException
    {
        public ValidationRequestFilterException(string x, bool isWeb)
            : base(x, System.Net.HttpStatusCode.OK)
        {
        }

        public ValidationRequestFilterException(string x)
            : base(x)
        {
        }
    }
}
