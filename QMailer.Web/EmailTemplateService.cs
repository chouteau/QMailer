using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

using Microsoft.Practices.Unity;

using QMailer;

namespace QMailer.Web
{
	public class EmailTemplateService : QMailer.Web.IEmailTemplateService 
	{
		public EmailTemplateService(
			Ariane.IServiceBus bus,
			IModelResolver resolver
			)
		{
			this.EmailParser = new EmailParser();
			this.Bus = bus;
			this.ModelResolver = resolver;
		}

		internal EmailParser EmailParser { get; private set; }
		protected Ariane.IServiceBus Bus { get; private set; }
		protected IModelResolver ModelResolver { get; private set;}

		public EmailView CreateEmailView(string viewName, object model = null)
		{
			var email = new EmailView(viewName, model);
			return email;
		}

		public EmailMessage GetEmailMessage(EmailConfig emailConfig)
		{
			var sender = emailConfig.Sender;
			if (sender == null
				|| sender.Email == null)
			{
				sender = new Sender()
				{
					Email = GlobalConfiguration.Configuration.SenderEmail,
					DisplayName = GlobalConfiguration.Configuration.SenderName,
					Code = GlobalConfiguration.Configuration.SenderCode,
					JobTitle = GlobalConfiguration.Configuration.SenderJobTitle
				};
			}

			// Deserialize object
			EmailModel em = null;
			
			try
			{
				em = ModelResolver.Convert(emailConfig);
			}
			catch(Exception ex)
			{
				GlobalConfiguration.Configuration.Logger.Error(ex);
				var failMessage = new SentFail();
				failMessage.FailDate = DateTime.Now;
				failMessage.Message = ex.Message;
				failMessage.MessageId = emailConfig.MessageId;
				failMessage.Recipients = emailConfig.Recipients;
				failMessage.Stack = string.Format("EmailConfig : {0}\r\n{1}", emailConfig, ex.ToString());
				failMessage.Subject = "Fail to convert emailconfig model";

				var queueName = emailConfig.SentFailQueueName ?? GlobalConfiguration.Configuration.SentFailQueueName;
				Bus.Send(queueName, failMessage);
			}

			if (em == null)
			{
				em = new EmailModel();
			}

			// Create emailView
			dynamic emailView = CreateEmailView(emailConfig.EmailName, em.Model);
			if (emailConfig.Parameters != null)
			{
				foreach (var prm in emailConfig.Parameters)
				{
					emailView.ViewData.Add(prm.Name, prm.Value);
				}
			}
			emailView.Sender = sender;
			emailView.MessageId = emailConfig.MessageId;

			// Create emailMessage
			EmailMessage emailMessage = null;
			try
			{
				emailMessage = CreateEmailMessage(emailView);
			}
			catch (Exception ex)
			{
				GlobalConfiguration.Configuration.Logger.Error(ex);
				var failMessage = new SentFail();
				failMessage.FailDate = DateTime.Now;
				failMessage.Message = ex.Message;
				failMessage.MessageId = emailConfig.MessageId;
				failMessage.Recipients = emailConfig.Recipients;
				failMessage.Stack = ex.ToString();
				failMessage.Subject = "Fail create email message";
				failMessage.ViewName = emailConfig.EmailName;

				var queueName = emailConfig.SentFailQueueName ?? GlobalConfiguration.Configuration.SentFailQueueName;
				Bus.Send(queueName, failMessage);
			}

			if (emailMessage == null)
			{
				return null;
			}

			emailMessage.Sender = sender;
			emailMessage.SenderAlias = emailConfig.SenderAlias;

			var recipientList = emailMessage.Recipients.ToList();
			recipientList.AddRange(emailConfig.Recipients);
			emailMessage.Recipients.Clear();

			while(true)
			{
				var first = recipientList.FirstOrDefault();
				if (first == null)
				{
					break;
				}
				recipientList.Remove(first);
				var existing = emailMessage.Recipients.SingleOrDefault(i => i.Address.Equals(first.Address, StringComparison.InvariantCultureIgnoreCase));
				if (existing != null)
				{
					existing.DisplayName = existing.DisplayName ?? first.DisplayName;
					existing.RecipientId = existing.RecipientId ?? first.RecipientId;
				}
				else
				{
					emailMessage.Recipients.Add(first);
				}
			}

			emailMessage.Headers.Add(new EmailMessageHeader() { Name = "X-Mailer-GEN", Value = "QMailer" });
			emailMessage.Headers.Add(new EmailMessageHeader() { Name = "X-Mailer-MID", Value = emailConfig.MessageId });
			if (emailConfig.Headers != null)
			{
				foreach (var h in emailConfig.Headers)
				{
					emailMessage.Headers.Add(new EmailMessageHeader() { Name = h.Name, Value = h.Value });
				}
			}

			emailMessage.MessageId = emailConfig.MessageId;
			emailMessage.DoNotTrack = emailConfig.DoNotTrack;
			emailMessage.EntityId = em.EntityId ?? emailConfig.EntityId;
			emailMessage.EntityName = em.EntityName ?? emailConfig.EntityName;
			if (emailMessage.Attachments == null || emailMessage.Attachments.Count == 0)
			{
				emailMessage.Attachments = emailConfig.Attachments;
			}
			else if(emailConfig.Attachments != null)
			{
				emailMessage.Attachments.AddRange(emailConfig.Attachments);
			}
			emailMessage.EmailBodyRequestedQueueName = emailConfig.EmailBodyRequestedQueueName;
			emailMessage.SendEmailQueueName = emailConfig.SendEmailQueueName;
			emailMessage.SentFailQueueName = emailConfig.SentFailQueueName;
			emailMessage.SentMessageQueueName = emailConfig.SentMessageQueueName;

			return emailMessage;
		}

		public EmailMessage CreateEmailMessage(EmailView emailView)
		{
			var container = GlobalConfiguration.Configuration.DependencyResolver.GetConfiguredContainer();
			var renderer = container.Resolve<IEmailViewRenderer>(emailView.RendererName);
			var rawEmailString = renderer.Render(emailView);
			var result = EmailParser.CreateMailMessage(rawEmailString);
			result.Body = ToSingleLine(result.Body);
			return result;
		}

		public IList<TemplateInfo> GetTemplateNameListByModel(string modelName)
		{
			modelName = ModelResolver.Resolve(modelName);
			var list = GetTemplateList();
			var result = new List<TemplateInfo>();
			foreach (var model in list)
			{
				string m = null;
				string d = null;
				var matchModel = System.Text.RegularExpressions.Regex.Match(model.Content, @"modelname:(?<model>\S+)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
				if (matchModel.Success)
				{
					m = matchModel.Groups["model"].Value;
				}
				var matchDescription = System.Text.RegularExpressions.Regex.Match(model.Content, @"modeldescription:(?<description>.*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
				if (matchDescription.Success)
				{
					d = matchDescription.Groups["description"].Value;
				}
				if (m != null
					&& modelName.Equals(m, StringComparison.InvariantCultureIgnoreCase)
					&& d != null)
				{
					var ti = new TemplateInfo();
					ti.ModelName = modelName;
					ti.Description = d.Trim();
					ti.ViewName = model.ViewName;
					ti.CreationDate = model.CreationDate;
					ti.LastUpdate = model.CreationDate;
					result.Add(ti);
				}
			}
			return result;
		}

		public List<EmailTemplate> GetTemplateList()
		{
			var templates = new List<EmailTemplate>();
			var DirectoryPath = GetDirectoryPath();
			var files = Directory.GetFiles(DirectoryPath, "*.cshtml");

			foreach (string filePath in files)
			{
				var temp = EmailTemplateFileSerializer.Deserialize(filePath);
				templates.Add(temp);
			}

			return templates;
		}

		public EmailTemplate CreateCustomTemplate()
		{
			var template = new EmailTemplate();
			var directoryPath = GetDirectoryPath();
			var filePath = System.IO.Path.Combine(directoryPath, "_DefaultTemplate.cshtml");
			if (File.Exists(filePath))
			{
				StreamReader reader = new StreamReader(filePath, System.Text.Encoding.UTF8);
				string content = reader.ReadToEnd();
				template.Content = content;
			}
			else
			{
				template.Content = "";
			}

			template.CreationDate = DateTime.Now;
			template.LastUpdate = DateTime.Now;
			Guid guid = Guid.NewGuid();
			template.Id = guid.ToString();
			template.IsCustom = true;
			template.ViewName = "nouveau";

			return template;
		}

		public void DeleteTemplate(EmailTemplate template)
		{
			string path = GetDirectoryPath();
			if (!template.IsCustom)
			{
				throw new Exception("Impossible de supprimer un modèle de base");
			}
			if (template.Id == null || template.ViewName == null)
			{
				return;
			}
			string fullName = GetDirectoryPath() + @"\" + template.ViewName + "." + template.Id + ".custom.cshtml";
			if (File.Exists(fullName))
			{
				try
				{
					File.Delete(fullName);
				}
				catch (Exception e)
				{
					GlobalConfiguration.Configuration.Logger.Error(e);
					throw;
				}
			}
		}

		public void SaveTemplate(EmailTemplate template)
		{
			bool isValid = IsValid(template);
			if (isValid != true)
			{
				throw new Exception("Le nom du modèle n'est pas valide");
			}
			var templateList = GetTemplateList();

			EmailTemplate templateChanged = null;
			if (template.Id == null)
			{
				templateChanged = templateList.SingleOrDefault(i => i.ViewName.Equals(template.ViewName, StringComparison.InvariantCultureIgnoreCase));
				if (templateChanged == null)
				{
					throw new Exception("Impossible de modifier le nom d'un modèle de base");
				}
			}
			else
			{
				templateChanged = templateList.SingleOrDefault(i => !i.ViewName.Equals(template.ViewName, StringComparison.InvariantCultureIgnoreCase) && i.Id.Equals(template.Id, StringComparison.InvariantCultureIgnoreCase));
			}
			string path = GetDirectoryPath();
			EmailTemplateFileSerializer.Serialize(template, path, templateChanged);
		}

		#region Helpers

		internal bool IsValid(EmailTemplate template)
		{
			bool valid = false;
			var name = template.ViewName.Split('.');
			var regex = new Regex("^_|^[a-zA-Z0-9]*$");
			bool test = regex.IsMatch(name.First());
			if (name.First() == null || name.First() == "" || !regex.IsMatch(name.First()))
			{
				throw new Exception("Le modèle doit avoir un nom et ne comporter aucun caractère spécial");
			}
			if (template.Content == null)
			{
				throw new Exception("Le modèle doit avoir un corps");
			}
			valid = true;
			return valid;
		}

		internal string GetDirectoryPath()
		{
			var path = GlobalConfiguration.Configuration.PhysicalPath;
			return path;
		}

		internal DateTime GetCreationDateByPath(string path)
		{
			FileInfo info = new FileInfo(path);
			return info.CreationTime;
		}

		internal DateTime GetLastUpdateByPath(string path)
		{
			FileInfo info = new FileInfo(path);
			return info.LastWriteTime;
		}

		internal string GetTemplateNameByPath(string path)
		{
			string[] sections = path.Split('\\');
			string templateName = sections.Last();
			return templateName;
		}

		internal string GetContentTemplateByPath(string path)
		{
			if (File.Exists(path))
			{
				string content = null;
				using (StreamReader reader = new StreamReader(path, System.Text.Encoding.UTF8))
				{
					try
					{
						content = reader.ReadToEnd();
					}
					catch (Exception)
					{
					}
					finally
					{
						reader.Close();
					}
				}
				return content;
			}
			return "Le fichier spécifié n'existe pas";
		}

		private string ToSingleLine(string input)
		{
			if (input == null)
			{
				return null;
			}
			var result = input.Replace("\t", "").Replace("\r", "").Replace("\n", "");
			result = System.Text.RegularExpressions.Regex.Replace(result, @"\s{2,}", " ");
			result = System.Text.RegularExpressions.Regex.Replace(result, @"<!--(.*?)-->", "");
			return result;
		}

		#endregion
	}
}
