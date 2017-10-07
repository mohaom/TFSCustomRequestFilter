using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace TFSPluggy
{
    public class CustomRequestFilter : ITeamFoundationRequestFilter
    {
        public void BeginRequest(IVssRequestContext requestContext)
        {
            if (!requestContext.ServiceHost.HostType.HasFlag(TeamFoundationHostType.ProjectCollection)) return;
            if (HttpContext.Current.Request.Url.ToString().Contains("/tfs/DefaultCollection/_api/_wit/updateWorkItems"))
            {
                string content = ReadHttpContextInputStream(HttpContext.Current.Request.InputStream);
                content = HttpUtility.UrlDecode(content);
                var serializer = new JavaScriptSerializer();
                serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

                dynamic Workitem = serializer.Deserialize(content.Split('=')[1].Split(new string[] { "&__RequestVerificationToken" }, StringSplitOptions.None)[0], typeof(object));
                if (Workitem[0].fields["10157"] > 2)
                {
                    throw new ValidationRequestFilterException("Priorities grater than 2 are not allowed.");
                }
            }
        }

        public void EndRequest(IVssRequestContext requestContext)
        {

        }

        public void EnterMethod(IVssRequestContext requestContext)
        {

        }

        public void LeaveMethod(IVssRequestContext requestContext)
        {

        }

        public Task PostLogRequestAsync(IVssRequestContext requestContext)
        {
            return Task.CompletedTask;
        }

        public Task RequestReady(IVssRequestContext requestContext)
        {
            return Task.CompletedTask;
        }

        private string ReadHttpContextInputStream(Stream stream)
        {
            string requestContent = "";
            using (var memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[1024 * 4];
                int count = 0;
                while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, count);
                }
                memoryStream.Seek(0, SeekOrigin.Begin);
                stream.Seek(0, SeekOrigin.Begin);
                requestContent = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            }
            return requestContent;
        }
    }
}
