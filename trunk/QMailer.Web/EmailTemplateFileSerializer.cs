using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QMailer.Web
{
	internal static class EmailTemplateFileSerializer
	{
		public static void Serialize(EmailTemplate template, string path, EmailTemplate oldTemplate)
		{
			string fileName = GetFileName(template, path);
			if (oldTemplate != null)
			{
				var oldFileName = GetFileName(oldTemplate, path);
				System.IO.File.Copy(oldFileName, fileName + "3DD51AE2-37B7-11E1-8726-432B4924019B", true);
				System.IO.File.Delete(oldFileName);
				System.IO.File.Copy(fileName + "3DD51AE2-37B7-11E1-8726-432B4924019B", oldFileName, true);
			}

			using (var emailfs = new System.IO.StreamWriter(fileName, false, System.Text.Encoding.UTF8))
			{
				emailfs.Write(template.Content);
				emailfs.Flush();
				emailfs.Close();
			}

			//using(FileStream streamCreate = File.Create(fileName))
			//{
			//    var buffer = System.Text.Encoding.UTF8.GetBytes(template.Content);
			//    streamCreate.Write(buffer,0,buffer.Length);
			//    streamCreate.Flush();
			//    streamCreate.Close();
			//}			
		}

		private static string GetFileName(EmailTemplate template, string path)
		{
			string fileName = null;
			if (template.IsCustom)
			{
				fileName = string.Format("{0}.{1}.custom.cshtml", template.ViewName, template.Id);
			}
			else
			{
				fileName = string.Format("{0}.cshtml", template.ViewName);
			}
			fileName = System.IO.Path.Combine(path, fileName);
			return fileName;
		}


		public static EmailTemplate Deserialize(string fullName)
		{
			if (fullName == null)
			{
				return null;
			}
			var fileName = System.IO.Path.GetFileNameWithoutExtension(fullName);
			var template = new EmailTemplate();
			var nameParts = fileName.Split('.');
			//var nameParts = template.FullName.Split('.');
			template.ViewName = nameParts[0];
			if (nameParts.Length < 2)
			{
				template.IsCustom = false;
				template.Id = template.ViewName;
			}
			else
			{
				try
				{

					if (nameParts[2].Equals("custom", StringComparison.InvariantCultureIgnoreCase))
					{
						template.IsCustom = true;
						template.Id = nameParts[1];
					}
					else
					{
						template.IsCustom = false;
						template.Id = nameParts[1];
					}
				}
				catch (Exception)
				{
					
					//throw new Exception("Le nom du fichier n'est pas correct. Vérifiez d'avoir bien renseigné l'extension");
				}
			}

			if (!File.Exists(fullName))
			{
				return null;
			}
			string content = null;
			using (var reader = new StreamReader(fullName, System.Text.Encoding.Default))
			{
				content = reader.ReadToEnd();
				template.Content = content;
				reader.Close();
			}

			var info = new FileInfo(fullName);
			template.CreationDate = info.CreationTime;
			template.LastUpdate = info.LastWriteTime;

			return template;
		}
	}
}
