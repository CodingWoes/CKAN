﻿using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace CKAN
{
    public partial class Main : Form
    {
        private BackgroundWorker m_UpdateRepoWorker;

        public void UpdateRepo()
        {
            m_TabController.RenameTab("WaitTabPage", "Updating repository");

            m_UpdateRepoWorker.RunWorkerAsync();

            Util.Invoke(this, () => Enabled = false);

            SetDescription("Contacting repository..");
            ClearLog();
            ShowWaitDialog();
        }

        private void UpdateRepo(object sender, DoWorkEventArgs e)
        {
            try
            {
                Repo.Update(m_Configuration.Repository);
            }
            catch (Exception)
            {
                m_ErrorDialog.ShowErrorDialog("Failed to connect to repository");
            }
        }

        private void PostUpdateRepo(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateModsList();
            UpdateModFilterList();

            SetDescription("Scanning for manually installed mods");
            KSPManager.CurrentInstance.ScanGameData();

            HideWaitDialog(true);
            AddStatusMessage("Repository successfully updated");

            Util.Invoke(this, () => Enabled = true);
            Util.Invoke(this, () => RecreateDialogs());
        }
    }
}