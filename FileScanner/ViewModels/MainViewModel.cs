using FileScanner.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;


namespace FileScanner.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string selectedFolder;
        private ObservableCollection<string> folderItems = new ObservableCollection<string>();

        private ObservableCollection<FolderDataModel> foldermodel = new ObservableCollection<FolderDataModel>();

        public DelegateCommand<string> OpenFolderCommand { get; private set; }
        public DelegateCommand<string> ScanFolderCommand { get; private set; }

        public ObservableCollection<string> FolderItems {
            get => folderItems;
            set
            {
                folderItems = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<FolderDataModel> FolderModel
        {
            get => foldermodel;
            set
            {
                foldermodel = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFolder
        {
            get => selectedFolder;
            set
            {
                selectedFolder = value;
                OnPropertyChanged();
                ScanFolderCommand.RaiseCanExecuteChanged();
            }
        }

        public MainViewModel()
        {
            OpenFolderCommand = new DelegateCommand<string>(OpenFolder);
            ScanFolderCommand = new DelegateCommand<string>(ScanFolderAsync, CanExecuteScanFolder);
        }

        private bool CanExecuteScanFolder(string obj)
        {
            return !string.IsNullOrEmpty(SelectedFolder);
        }

        private void OpenFolder(string obj)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SelectedFolder = fbd.SelectedPath;
                }
            }
        }

        private async void ScanFolderAsync(string dir)
        {

            FolderModel = new ObservableCollection<FolderDataModel>();
            

            try
            {
                
                var Folder = await Task.Run(() => GetFolderAsync(dir));
                var File = await Task.Run(() => GetFileAsync(dir));

                //Folder 

                foreach (var item in Folder)
                {
                    FolderModel.Add(new FolderDataModel() { Image = "/images/Folder.png", Folder = item });
                }

                // File

                foreach (var item in File)
                {
                    FolderModel.Add(new FolderDataModel() { Image = "/images/Folder.png", Folder = item });
                }

            }
            catch (SystemException)
            {

                System.Windows.Forms.MessageBox.Show("Erreur le fichier est inaccessible ");
            }
        }
        public List<string> GetFolderAsync(string dir)
        {
            var Folders = new List<string>();

            foreach (var items in Directory.EnumerateDirectories(dir, "*"))
            {
                Folders.Add(items);
            }
            return Folders;
        }
        public List<string> GetFileAsync(string dir)
        {
            var Files = new List<string>();

            foreach (var items in Directory.EnumerateDirectories(dir, "*"))
            {
                Files.Add(items);
            }
            return Files;
        }

    }
}
