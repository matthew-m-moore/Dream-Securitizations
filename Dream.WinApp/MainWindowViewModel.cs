using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using Dream.Common;
using Dream.ConsoleApp.Interfaces;
using Dream.ConsoleApp;
using Dream.WinApp.Core;
using System.Windows.Threading;
using Dream.IO.Database;

namespace Dream.WinApp
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private const string _companyImageFilePath = "Dream.WinApp.Resources.RA-logo-high-res.png";
        private Image _companyImage { get; set; }

        public MainWindowViewModel()
        {
            const bool useEmbeddedColorManagement = true;
            var companyImageFileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(_companyImageFilePath);
            _companyImage = Image.FromStream(companyImageFileStream, useEmbeddedColorManagement);

            var companyImageBitmap = new Bitmap(_companyImage);
            CompanyImageBitmapSource = BitmapToBitmapSource(companyImageBitmap);

            DatabaseConnectionSettings.SetDatabaseContextConnectionsDictionary(DatabaseContextSettings.DatabaseContextConnectionsDictionary);
        }

        // This property is mapped to the image that appears in the application
        private BitmapSource _companyImageBitmapSource;
        public BitmapSource CompanyImageBitmapSource
        {
            get
            {
                return _companyImageBitmapSource;
            }
            set
            {
                _companyImageBitmapSource = value;
                OnPropertyChanged();
            }
        }

        // This propery displays the abbreviated build identifier in the application
        public string BuildIdentiferText
        {
            get
            {
                var buildIdentifer = BuildIdentifier.AbbreviatedBuildId;
                return "Build ID: " + buildIdentifer;
            }
        }

        // This property maps to the combo box of script choices
        private Dictionary<string, IScript> _listOfVisibleScriptsDictionary;
        public List<string> VisibleScriptsList
        {
            get
            {
                var listOfScriptTypes = ScriptsManager.GetEnumerableOfAllScripts().ToList();
                var listOfScripts = listOfScriptTypes.Select(s => (IScript) Activator.CreateInstance(s)).ToList();

                _listOfVisibleScriptsDictionary = listOfScripts
                    .Where(s => s.GetVisibilityStatus())
                    .ToDictionary(l => l.GetFriendlyName(), l => l);

                var listOfVisibleScriptNames = _listOfVisibleScriptsDictionary.Keys.ToList();
                return listOfVisibleScriptNames;
            }
        }

        // This property represents the active script selected
        private IScript _selectedScriptToRun;
        public string SelectedScriptToRun
        {
            get
            {
                if (_selectedScriptToRun == null) return string.Empty;
                return _selectedScriptToRun.GetFriendlyName();
            }
            set
            {
                _selectedScriptToRun = _listOfVisibleScriptsDictionary[value];
                OnPropertyChanged();
            }
        }

        private string _inputsFilePath;
        public string InputsFilePath
        {
            get { return _inputsFilePath; }
            set
            {
                _inputsFilePath = value;
                OnPropertyChanged();
            }
        }

        private ICommand _openFileDialog;
        public ICommand OpenFileDialogCommand
        {
            get
            {
                return _openFileDialog ??
                    (
                        _openFileDialog = new ExecuteCommand
                        (
                            parameter => OpenFileDialogWindow(),
                            parameter => !_isBusyRunning
                        )
                    );
            }
        }

        private void OpenFileDialogWindow()
        {
            var openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.Title = "Select Input File";

            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;

            openFileDialog.DefaultExt = "xls*";
            openFileDialog.Filter = "Excel Files (*.xls/*.xlsx/*.xlsm/*.xlsb)|*.xls;*.xlsx;*.xlsm;*.xlsb|" +
                                    "Comma Separated (*.csv)|*.csv|" +
                                    "All files (*.*)|*.*";

            // This conditional creates a memory for the last file location used
            if (InputsFilePath != null && InputsFilePath != string.Empty)
            {
                openFileDialog.InitialDirectory = Path.GetDirectoryName(InputsFilePath);
            }

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                InputsFilePath = openFileDialog.FileName;
            }
        }

        private bool _isBusyRunning;

        // This is the command bound to the "Solve" button
        private ICommand _run;
        public ICommand Run
        {
            get
            {
                return _run ??
                    (
                        _run = new ExecuteCommand
                        (
                            parameter => TryRunScriptOnThread(),
                            parameter => !string.IsNullOrEmpty(InputsFilePath)
                                      && !string.IsNullOrEmpty(SelectedScriptToRun)
                                      && !_isBusyRunning
                        )
                    );
            }
        }

        private string _runTimerText;
        public string RunTimerText
        {
            get { return _runTimerText ?? string.Empty; }
            set
            {
                _runTimerText = value;
                OnPropertyChanged();
            }
        }

        private DispatcherTimer _dispatcherTimer;
        private BackgroundWorker _runScriptWorker;
        private void TryRunScriptOnThread()
        {
            _startTime = DateTime.Now;
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += new EventHandler(TickTimerEvent);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _dispatcherTimer.Start();

            // While running, the "Is Busy" indicator should prevent too much other interference
            _isBusyRunning = true;
            _runScriptWorker = new BackgroundWorker();
            _runScriptWorker.DoWork += (sender, e) => RunSelectedScript();
            _runScriptWorker.RunWorkerCompleted += RunWorkerCompletedEvent;
            _runScriptWorker.RunWorkerAsync();               
        }

        private void RunSelectedScript()
        {
            var arguments = new string[] { "", InputsFilePath };
            _selectedScriptToRun.RunScript(arguments);
        }

        private void RunWorkerCompletedEvent(object sender, RunWorkerCompletedEventArgs e)
        {
            _dispatcherTimer.Stop();

            if (e.Error != null)
            {
                var exceptionText = e.Error.Message;
                if (e.Error.InnerException != null) exceptionText += (", Inner Exception: " + e.Error.InnerException);

                exceptionText += (", Stack Trace: " + e.Error.StackTrace);

                MessageBox.Show(exceptionText, "Sorry, A Problem Occurred, Please Take a Screenshot:",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            _isBusyRunning = false;          
            RunTimerText = null;
        }

        private DateTime _startTime;
        private void TickTimerEvent(object sender, EventArgs e)
        {
            var timeElapsed = DateTime.Now - _startTime;
            var timeElapsedDateTime = new DateTime(timeElapsed.Ticks);
            RunTimerText = "(Running... " + timeElapsedDateTime.ToString("mm:ss") + ")";
        }

        // To bind to the bitmap image in XAML, it has to be of an object type 
        // like BitmapSource or ImageSource.
        public static BitmapSource BitmapToBitmapSource(Bitmap bitmap)
        {
            // Using a memory stream will allow for updating the image using
            // events, rather than with a static file path.
            using (var memoryStreamForImage = new MemoryStream())
            {
                bitmap.Save(memoryStreamForImage, ImageFormat.Bmp);
                memoryStreamForImage.Position = 0;

                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStreamForImage;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        // This property changed event handler will allow detection of changes to the selected
        // script to run and inputs file path from the user
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler == null) return;

            var e = new PropertyChangedEventArgs(propertyName);
            eventHandler(this, e);
        }
    }
}
