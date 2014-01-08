namespace IntelliTwitterClient
{
    partial class IntelliTwitterService
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

        private System.Diagnostics.EventLog serviceLog;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceLog = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.serviceLog)).BeginInit();
             
            // IntelliTwitterService
             
            components = new System.ComponentModel.Container();
            this.ServiceName = "Intelli Twitter Service";
            ((System.ComponentModel.ISupportInitialize)(this.serviceLog)).EndInit();
        }

        #endregion
    }
}
