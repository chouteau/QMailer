using System;

namespace QMailer.Web
{
    public interface IEmailViewRenderer
    {
        string Render(EmailView email, string viewName = null);
    }
}
