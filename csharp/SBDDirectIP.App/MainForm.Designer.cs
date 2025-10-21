using System.Windows.Forms;

namespace SBDDirectIP.App;

partial class MainForm
{
    /// <summary> 
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer? components = null;

    /// <summary>
    ///  Clean up any resources being used.
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

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.imeiLabel = new Label();
        this.imeiTextBox = new TextBox();
        this.keyLabel = new Label();
        this.keyTextBox = new TextBox();
        this.serverLabel = new Label();
        this.serverTextBox = new TextBox();
        this.payloadLabel = new Label();
        this.payloadTextBox = new TextBox();
        this.sendButton = new Button();
        this.outputTextBox = new TextBox();
        this.encryptCheckBox = new CheckBox();
        this.decryptCheckBox = new CheckBox();
        this.SuspendLayout();

        // imeiLabel
        this.imeiLabel.AutoSize = true;
        this.imeiLabel.Location = new System.Drawing.Point(10, 15);
        this.imeiLabel.Name = "imeiLabel";
        this.imeiLabel.Size = new System.Drawing.Size(130, 15);
        this.imeiLabel.Text = "IMEI";

        // imeiTextBox
        this.imeiTextBox.Location = new System.Drawing.Point(150, 10);
        this.imeiTextBox.Name = "imeiTextBox";
        this.imeiTextBox.Size = new System.Drawing.Size(440, 23);

        // keyLabel
        this.keyLabel.AutoSize = true;
        this.keyLabel.Location = new System.Drawing.Point(10, 45);
        this.keyLabel.Name = "keyLabel";
        this.keyLabel.Size = new System.Drawing.Size(130, 15);
        this.keyLabel.Text = "AES Key (hex)";

        // keyTextBox
        this.keyTextBox.Location = new System.Drawing.Point(150, 40);
        this.keyTextBox.Name = "keyTextBox";
        this.keyTextBox.Size = new System.Drawing.Size(440, 23);

        // serverLabel
        this.serverLabel.AutoSize = true;
        this.serverLabel.Location = new System.Drawing.Point(10, 75);
        this.serverLabel.Name = "serverLabel";
        this.serverLabel.Size = new System.Drawing.Size(130, 15);
        this.serverLabel.Text = "Gateway host:port";

        // serverTextBox
        this.serverTextBox.Location = new System.Drawing.Point(150, 70);
        this.serverTextBox.Name = "serverTextBox";
        this.serverTextBox.Size = new System.Drawing.Size(240, 23);
        this.serverTextBox.Text = "directip.sbd.iridium.com:10800";

        // payloadLabel
        this.payloadLabel.AutoSize = true;
        this.payloadLabel.Location = new System.Drawing.Point(10, 105);
        this.payloadLabel.Name = "payloadLabel";
        this.payloadLabel.Size = new System.Drawing.Size(130, 15);
        this.payloadLabel.Text = "Payload (hex)";

        // payloadTextBox
        this.payloadTextBox.Location = new System.Drawing.Point(150, 100);
        this.payloadTextBox.Name = "payloadTextBox";
        this.payloadTextBox.Size = new System.Drawing.Size(240, 23);

        // sendButton
        this.sendButton.Location = new System.Drawing.Point(150, 130);
        this.sendButton.Name = "sendButton";
        this.sendButton.Size = new System.Drawing.Size(75, 23);
        this.sendButton.Text = "Send";
        this.sendButton.UseVisualStyleBackColor = true;
        this.sendButton.Click += new System.EventHandler(this.OnSend);

        // encryptCheckBox
        this.encryptCheckBox.Location = new System.Drawing.Point(240, 130);
        this.encryptCheckBox.Name = "encryptCheckBox";
        this.encryptCheckBox.Size = new System.Drawing.Size(80, 24);
        this.encryptCheckBox.Text = "Encrypt";
        this.encryptCheckBox.Checked = true;
        this.encryptCheckBox.AutoSize = true;

        // decryptCheckBox
        this.decryptCheckBox.Location = new System.Drawing.Point(330, 130);
        this.decryptCheckBox.Name = "decryptCheckBox";
        this.decryptCheckBox.Size = new System.Drawing.Size(80, 24);
        this.decryptCheckBox.Text = "Decrypt";
        this.decryptCheckBox.Checked = true;
        this.decryptCheckBox.AutoSize = true;

        // outputTextBox
        this.outputTextBox.Location = new System.Drawing.Point(10, 160);
        this.outputTextBox.Multiline = true;
        this.outputTextBox.Name = "outputTextBox";
        this.outputTextBox.ReadOnly = true;
        this.outputTextBox.ScrollBars = ScrollBars.Vertical;
        this.outputTextBox.Size = new System.Drawing.Size(580, 390);

        // MainForm
        this.ClientSize = new System.Drawing.Size(620, 600);
        this.Controls.AddRange(new Control[] { this.imeiLabel, this.imeiTextBox, this.keyLabel, this.keyTextBox, this.serverLabel, this.serverTextBox, this.payloadLabel, this.payloadTextBox, this.sendButton, this.encryptCheckBox, this.decryptCheckBox, this.outputTextBox });
        this.Name = "MainForm";
        this.Text = "SBD DirectIP Client";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private Label imeiLabel;
    private Label keyLabel;
    private Label serverLabel;
    private Label payloadLabel;
    private CheckBox encryptCheckBox;
    private CheckBox decryptCheckBox;
}
