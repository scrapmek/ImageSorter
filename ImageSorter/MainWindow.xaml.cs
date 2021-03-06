﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace ImageSorter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>

	public partial class MainWindow : Window
	{

		DataBundle bundle;
		BackgroundWorker worker;
		int filesMoved;
		int duplicatesFound;
		int errors;
		bool transferCancelled;

		public MainWindow()
		{
			InitializeComponent();
		}


		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			bundle = new DataBundle();
			worker = new BackgroundWorker();
			worker.WorkerReportsProgress = true;
			worker.WorkerSupportsCancellation = true;
			worker.DoWork += Worker_DoWork;
			worker.ProgressChanged += Worker_ProgressChanged;
			worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
		}

		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			cleanupUI();
		}

		private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			progressBar.Value = e.ProgressPercentage;
			feedbackLabel.Content = String.Format("Transferring images... {0}%", e.ProgressPercentage);
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{

			var list = Helpers.GetAllInputImageFiles(bundle.sourceFolderBrowser.SelectedPath);
			ImageHandler imgHandler = new ImageHandler(bundle.destinationFolderBrowser.SelectedPath);
			int i = 0;

			foreach (var item in list)
			{
				try
				{
					if (worker.CancellationPending == true)
					{
						e.Cancel = true;
						break;
					}
					else
					{

						if (imgHandler.Transfer(item))
							filesMoved++;
						else
							duplicatesFound++;

						if (list.Count() > 0)
						{
							worker.ReportProgress((int)(((decimal)(i + 1) / ((decimal)list.Count())) * 100));
						}
						else
							worker.ReportProgress(1 * 100);

					}


				}
				catch (Exception ex)
				{
					errors++;
					System.Windows.MessageBox.Show("Error transferring file: " + item.FullName + Environment.NewLine + "Message: " + ex.Message, "Error", MessageBoxButton.OK);
				}

				i++;
			}
		}

		private void organiseImagesButton_Click(object sender, RoutedEventArgs e)
		{
			feedbackLabel.Content = "Please wait...";
			progressBar.Value = 0;
			if (bundle.sourceFolderBrowser.SelectedPath != "" && bundle.destinationFolderBrowser.SelectedPath != "")
			{
				organiseImagesInitialise();
			}

			else
				this.feedbackLabel.Content = "Please select a source and destination folder!";
		}

		private void organiseImagesInitialise()
		{
			if (!worker.IsBusy)
			{
				organiseImagesButton.Content = "Cancel";
				filesMoved = 0;
				duplicatesFound = 0;
				errors = 0;
				transferCancelled = false;
				organiseFiles();
			}
			else if (worker.IsBusy)
			{
				worker.CancelAsync();
				transferCancelled = true;
				organiseImagesButton.Content = "Organise Images";
				enableButtons();
			}

		}

		private void destinationOpenButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult result = bundle.destinationFolderBrowser.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				destinationTextBox.Text = bundle.destinationFolderBrowser.SelectedPath;
			}
		}

		private void sourceOpenButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult result = bundle.sourceFolderBrowser.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				sourceTextBox.Text = bundle.sourceFolderBrowser.SelectedPath;
			}
		}

		private void cleanupUI()
		{
			enableButtons();
			organiseImagesButton.Content = "Organise Images";

			if (transferCancelled)
			{
				feedbackLabel.Content = String.Format("Image transfer cancelled, {0} copied, {1} found.{2}", findFilesTransferedString(), findDuplicatesFoundString(), findErrorsString());
			}
			else
				feedbackLabel.Content = String.Format("Image transfer complete, {0} copied, {1} found.{2}", findFilesTransferedString(), findDuplicatesFoundString(), findErrorsString());
		}

		private string findDuplicatesFoundString()
		{
			if (duplicatesFound == 1)
			{
				return String.Format("{0} duplicate", duplicatesFound);
			}
			else
			{
				return String.Format("{0} duplicates", duplicatesFound);
			}
		}

		private string findErrorsString()
		{
			if (errors < 1)
			{
				return "";
			}
			if (errors == 1)
			{
				return String.Format(" {0} error.", errors);
			}
			else
			{
				return String.Format(" {0} errors.", errors);
			}
		}

		private string findFilesTransferedString()
		{
			if (filesMoved == 1)
			{
				return String.Format("{0} file", filesMoved);
			}
			else
			{
				return String.Format("{0} files", filesMoved);
			}
		}

		private void organiseFiles()
		{

			if (!worker.IsBusy)
			{
				disableButtons();
				worker.RunWorkerAsync(bundle);
			}
		}

		private void disableButtons()
		{
			sourceOpenButton.IsEnabled = false;
			destinationOpenButton.IsEnabled = false;
		}

		private void enableButtons()
		{
			sourceOpenButton.IsEnabled = true;
			destinationOpenButton.IsEnabled = true;
		}

	}

	public class DataBundle
	{
		public FolderBrowserDialog sourceFolderBrowser;
		public FolderBrowserDialog destinationFolderBrowser;

		public DataBundle()
		{
			this.sourceFolderBrowser = new FolderBrowserDialog();
			sourceFolderBrowser.Description = "Select a source folder";
			this.destinationFolderBrowser = new FolderBrowserDialog();
			destinationFolderBrowser.Description = "Select a destination folder";
		}


	}

}
