﻿/*
  KeePassRPC - Uses JSON-RPC to provide RPC facilities to KeePass.
  Example usage includes the KeeFox firefox extension.
  
  Copyright 2010 Chris Tomlinson <keefox@christomlinson.name>

  This program is free software; you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation; either version 2 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Resources;

using KeePass;
using KeePass.UI;
using KeePass.Plugins;
using KeePass.Resources;
using KeePassRPC;
using System.Reflection;

namespace KeePassRPC.Forms
{
    public partial class OptionsForm : Form
    {
        private IPluginHost _host;
        private KeePassRPCExt _plugin;

        public OptionsForm(IPluginHost host, KeePassRPCExt plugin)
        {
            _host = host;
            _plugin = plugin;
            
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));

            InitializeComponent();
            Icon = global::KeePassRPC.Properties.Resources.keefox;
            this.checkBox1.Text = resources.GetString("code.checkBox1.Text");
            if (host.CustomConfig.GetBool("KeePassRPC.KeeFox.autoCommit", true))
                this.checkBox1.Checked = true;
            else
                this.checkBox1.Checked = false;

            this.checkBox2.Text = resources.GetString("code.checkBox2.Text");
            if (host.CustomConfig.GetBool("KeePassRPC.KeeFox.editNewEntries", false))
                this.checkBox2.Checked = true;
            else
                this.checkBox2.Checked = false;

            this.textBoxAuthExpiry.Text = (_host.CustomConfig.GetLong("KeePassRPC.AuthorisationExpiryTime", 8760 * 3600) / 3600).ToString();

            long secLevel = _host.CustomConfig.GetLong("KeePassRPC.SecurityLevel", 2);
            long secLevelClientMin = _host.CustomConfig.GetLong("KeePassRPC.SecurityLevelClientMinimum", 2);
            switch (secLevel)
            {
                case 1: comboBoxSecLevelKeePass.SelectedItem = "Low"; break;
                case 2: comboBoxSecLevelKeePass.SelectedItem = "Medium"; break;
                default: comboBoxSecLevelKeePass.SelectedItem = "High"; break;
            }
            switch (secLevelClientMin)
            {
                case 1: comboBoxSecLevelMinClient.SelectedItem = "Low"; break;
                case 2: comboBoxSecLevelMinClient.SelectedItem = "Medium"; break;
                default: comboBoxSecLevelMinClient.SelectedItem = "High"; break;
            }

            this.label6.Text = resources.GetString("code.label6.Text");
            this.textBoxPort.Text = _host.CustomConfig.GetLong("KeePassRPC.webSocket.port", 12546).ToString();

            UpdateAuthorisedConnections();
        }

        private void UpdateAuthorisedConnections()
        {
            KeyContainerClass[] kcs = FindAuthorisedConnections();

            if (kcs == null)
            {
                // Tell the user it's not worked.
                dataGridView1.Visible = false;
                labelAuthorisedClientsFail.Visible = true;
                return;
            }

            List<string> connectedClientUsernames = new List<string>();

            foreach (KeePassRPCClientConnection client in _plugin.GetConnectedRPCClients())
                if (!string.IsNullOrEmpty(client.UserName))
                    connectedClientUsernames.Add(client.UserName);

            // Update the screen
            foreach (KeyContainerClass kc in kcs)
            {
                bool connected = false;
                if (connectedClientUsernames.Contains(kc.Username))
                    connected = true;

                string[] row = new string[] { kc.ClientName, kc.Username, kc.AuthExpires.ToString() };
                int rowid = dataGridView1.Rows.Add(row);
                dataGridView1.Rows[rowid].Cells[3].Value = connected;
            }
            return;

        }

        private KeyContainerClass[] FindAuthorisedConnections()
        {
            //This might not work, especially in .NET 2.0 RTM, a shame but more
            //up to date users might as well use the feature if possible.
            Dictionary<string, string> configValues;
            try
            {
                FieldInfo fi = typeof(KeePass.App.Configuration.AceCustomConfig)
                               .GetField("m_vItems", BindingFlags.NonPublic | BindingFlags.Instance);
                configValues = (Dictionary<string, string>)fi.GetValue(_host.CustomConfig);

                
            }
            catch
            {
                return null;
            }

            List<KeyContainerClass> keyContainers = new List<KeyContainerClass>();

            foreach (KeyValuePair<string, string> kvp in configValues)
            {
                if (kvp.Key.StartsWith("KeePassRPC.Key."))
                {
                    string username = kvp.Key.Substring(15);
                    byte[] serialisedKeyContainer = null;

                    // Assume config entry is encrypted but fall back to attempting direct deserialisation if something goes wrong
                    
                    if (string.IsNullOrEmpty(kvp.Value))
                        return null;
                    try
                    {
                        byte[] keyBytes = System.Security.Cryptography.ProtectedData.Unprotect(
                        Convert.FromBase64String(kvp.Value),
                        new byte[] { 172, 218, 37, 36, 15 },
                        System.Security.Cryptography.DataProtectionScope.CurrentUser);
                        serialisedKeyContainer = keyBytes;
                        System.Xml.Serialization.XmlSerializer mySerializer = new System.Xml.Serialization.XmlSerializer(typeof(KeyContainerClass));
                        keyContainers.Add((KeyContainerClass)mySerializer.Deserialize(new System.IO.MemoryStream(serialisedKeyContainer)));
                    }
                    catch (Exception)
                    {
                        try
                        {
                            serialisedKeyContainer = Convert.FromBase64String(kvp.Value);
                            System.Xml.Serialization.XmlSerializer mySerializer = new System.Xml.Serialization.XmlSerializer(typeof(KeyContainerClass));
                            keyContainers.Add((KeyContainerClass)mySerializer.Deserialize(new System.IO.MemoryStream(serialisedKeyContainer)));
                        }
                        catch (Exception)
                        {
                            // It's not a valid entry so ignore it and move on
                            continue;
                        }
                    }
                    
                }
            }
            return keyContainers.ToArray();
        }

        private void m_btnOK_Click(object sender, EventArgs e)
        {
            ulong port = 0;
            try
            {
                if (this.textBoxPort.Text.Length > 0)
                {
                    port = ulong.Parse(this.textBoxPort.Text);
                    if (port <= 0 || port > 65535)
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid listening port. Type a number between 1 and 65535 or leave empty to use the default port.");
                DialogResult = DialogResult.None;
                return;
            }

            long expTime = 8760;
            try
            {
                expTime = long.Parse(this.textBoxAuthExpiry.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Invalid expiry time.");
                DialogResult = DialogResult.None;
                return;
            }

            long secLevel = 2;
            long secLevelClientMin = 2;
            switch ((string)comboBoxSecLevelKeePass.SelectedItem)
            {
                case "Low": secLevel = 1; break;
                case "Medium": secLevel = 2; break;
                default: secLevel = 3; break;
            }
            switch ((string)comboBoxSecLevelMinClient.SelectedItem)
            {
                case "Low": secLevelClientMin = 1; break;
                case "Medium": secLevelClientMin = 2; break;
                default: secLevelClientMin = 3; break;
            }

            _host.CustomConfig.SetBool("KeePassRPC.KeeFox.autoCommit", this.checkBox1.Checked);
            _host.CustomConfig.SetBool("KeePassRPC.KeeFox.editNewEntries", this.checkBox2.Checked);
            _host.CustomConfig.SetLong("KeePassRPC.AuthorisationExpiryTime", expTime * 3600);
            _host.CustomConfig.SetLong("KeePassRPC.SecurityLevel", secLevel);
            _host.CustomConfig.SetLong("KeePassRPC.SecurityLevelClientMinimum", secLevelClientMin);

            if (port > 0)
                _host.CustomConfig.SetULong("KeePassRPC.webSocket.port", port);

            _host.MainWindow.Invoke((MethodInvoker)delegate { _host.MainWindow.SaveConfig(); });
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            // Prevent one cell being blue by default to reduce distraction
            dataGridView1.ClearSelection();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
        }

        private void comboBoxSecLevelKeePass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSecLevelKeePass.SelectedItem == "Low")
                labelSecLevelWarning.Text = "A low security setting could increase the chance of your passwords being stolen. Please make sure you read the information in the manual (see link above).";
            else if (comboBoxSecLevelKeePass.SelectedItem == "High")
                labelSecLevelWarning.Text = "A high security setting will require you to enter a randomly generated password every time you start KeePass or its client. A medium setting should suffice in most situations, especially if you set a low authorisation timeout below.";
            else
                labelSecLevelWarning.Text = "";
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                string username = (string)dataGridView1.Rows[e.RowIndex].Cells[1].Value;

                // Revoke authorisation by deleting stored key data
                _host.CustomConfig.SetString("KeePassRPC.Key." + username, null);

                // If this connection is active, destroy it now
                foreach (KeePassRPCClientConnection client in _plugin.GetConnectedRPCClients())
                    if (!string.IsNullOrEmpty(client.UserName) && client.UserName == username)
                    {
                        client.WebSocketConnection.Close();
                        break;
                    }

                // Refresh the view
                dataGridView1.Rows.RemoveAt(e.RowIndex);
                dataGridView1.Refresh();
                //UpdateAuthorisedConnections();
            }
        }

    }
}
