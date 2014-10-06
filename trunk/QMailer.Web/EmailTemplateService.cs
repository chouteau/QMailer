using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QMailer.Web
{
	public class EmailTemplateService 
	{
		public EmailTemplateService(IEmailViewRenderer renderer
			, QMailer.ILogger logger)
		{
			this.EmailParser = new EmailParser();
			this.Renderer = renderer;
			this.Logger = logger;
		}

		protected IEmailViewRenderer Renderer { get; private set; }
		internal EmailParser EmailParser { get; private set; }
		protected QMailer.ILogger Logger { get; private set; }

		public EmailView CreateEmailView(string viewName, object model = null)
		{
			var email = new EmailView(viewName, model);
			return email;
		}

		public EmailMessage	CreateEmailMessage(EmailView emailView)
		{
			var rawEmailString = Renderer.Render(emailView);
			var result = EmailParser.CreateMailMessage(rawEmailString);
			return result;
		}

		public EmailMessage GetEmailMessage(EmailConfig emailConfig)
		{
			string messageId = Guid.NewGuid().ToString();

			// Deserialize object
			object model = emailConfig.Model;
			if (emailConfig.Model != null
				&& emailConfig.Model.GetType().AssemblyQualifiedName != emailConfig.AssemblyQualifiedTypeNameModel)
			{
				var modelType = Type.GetType(emailConfig.AssemblyQualifiedTypeNameModel);
				model = Newtonsoft.Json.JsonConvert.DeserializeObject(emailConfig.Model.ToString(), modelType);
				Logger.Debug("Email model {0},{1},{2}", emailConfig.EmailName, model, modelType);
			}

			// Create emailView
			dynamic emailView = CreateEmailView(emailConfig.EmailName, model);
			if (emailConfig.Parameters != null)
			{
				foreach (var prm in emailConfig.Parameters)
				{
					emailView.ViewData.Add(prm.Name, prm.Value);
				}
			}

			// Create emailMessage
			EmailMessage emailMessage = null;
			try
			{
				emailMessage = EmailTemplateService.CreateEmailMessage(emailView);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}

			if (emailMessage == null)
			{
				return null;
			}

			if (emailConfig.Sender != null)
			{
				emailMessage.From = new EmailAddress()
				{
					Address = emailConfig.Sender.Email,
					DisplayName = emailConfig.Sender.DisplayName,
				};
			}
			else if (emailMessage.From == null)
			{
				emailMessage.From = new EmailAddress()
				{
					Address = GlobalConfiguration.Configuration.FromEmail,
					DisplayName = GlobalConfiguration.Configuration.FromName
				};
			}

			emailMessage.Recipients.AddRange(emailConfig.Recipients);
			emailMessage.Headers.Add(new EmailMessageHeader() { Name = "X-Mailer", Value = "QMailer" });
			emailMessage.Headers.Add(new EmailMessageHeader() { Name = "X-Mailer-MID", Value = messageId });
			if (emailConfig.Headers != null)
			{
				foreach (var h in emailConfig.Headers)
				{
					emailMessage.Headers.Add(new EmailMessageHeader() { Name = h.Name, Value = h.Value });
				}
			}

			emailMessage.MessageId = messageId;

			return emailMessage;
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
			template.ShortName = "nouveau";

			return template;
		}

		public void DeleteTemplate(EmailTemplate template)
		{
			string path = GetDirectoryPath();
			if (!template.IsCustom)
			{
				throw new Exception("Impossible de supprimer un modèle de base");
			}
			if (template.Id == null || template.ShortName == null)
			{
				return;
			}
			string fullName = GetDirectoryPath() + @"\" + template.ShortName + "." + template.Id + ".custom.cshtml";
			if (File.Exists(fullName))
			{
				try
				{
					File.Delete(fullName);
				}
				catch (Exception e)
				{
					Logger.Error(e);
					throw;
				}
			}
		}

		internal bool IsValid(EmailTemplate template)
		{
			bool valid = false;
			var name = template.ShortName.Split('.');
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
				templateChanged = templateList.SingleOrDefault(i => i.ShortName.Equals(template.ShortName, StringComparison.InvariantCultureIgnoreCase));
				if (templateChanged == null)
				{
					throw new Exception("Impossible de modifier le nom d'un modèle de base");
				}
			}
			else
			{
				templateChanged = templateList.SingleOrDefault(i => !i.ShortName.Equals(template.ShortName, StringComparison.InvariantCultureIgnoreCase) && i.Id.Equals(template.Id, StringComparison.InvariantCultureIgnoreCase));
			}
			string path = GetDirectoryPath();
			EmailTemplateFileSerializer.Serialize(template, path, templateChanged);
		}

		internal string GetDirectoryPath()
		{
			var path = GlobalConfiguration.Configuration.PhysicalPath;
			path = System.IO.Path.Combine(path, "Emailer");
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

		public IDictionary<string, string> GetTemplateNameListByModel(string modelName)
		{
			var list = GetTemplateList();
			var result = new Dictionary<string, string>();
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
					result.Add(model.ShortName, d.Trim());
				}
			}
			return result;
		}

	}
}
