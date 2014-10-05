namespace QMailer.SvcHost
{
	partial class ProjectInstaller
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.QMailerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			this.QMailerProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			// 
			// QMailerServiceInstaller
			// 
			this.QMailerServiceInstaller.DelayedAutoStart = true;
			this.QMailerServiceInstaller.Description = "QMailer Services Host";
			this.QMailerServiceInstaller.DisplayName = "QMailer Services Host";
			this.QMailerServiceInstaller.ServiceName = "QMailerSvcHost";
			this.QMailerServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			// 
			// QMailerProcessInstaller
			// 
			this.QMailerProcessInstaller.Account = System.ServiceProcess.ServiceAccount.NetworkService;
			this.QMailerProcessInstaller.Password = null;
			this.QMailerProcessInstaller.Username = null;
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.QMailerServiceInstaller,
            this.QMailerProcessInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceInstaller QMailerServiceInstaller;
		private System.ServiceProcess.ServiceProcessInstaller QMailerProcessInstaller;
	}
}