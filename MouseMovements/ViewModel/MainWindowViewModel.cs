
using MathNet.Numerics.Distributions;
using MicroMvvm;
using MouseMovements.View;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Forms;
using Point = System.Windows.Point;
using MouseMovementsDLL;
using static System.Windows.Forms.AxHost;
using System.Windows.Threading;

namespace MouseMovements.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region members
        private SimulateMoving simulateMove;
        private bool recording;
        private bool simulating;
        private int deviation;
        private string recordBtn;
        private string simulateBtnTxt;
        private bool isBackgroundVisible;
        private Visibility showAllLines;
        private bool showLines;
        private double screenWidth;
        private double screenHeight;
        private double sliderValue;
        private Settings settingsView;
        private double timeMS;
        private double translationX;
        private double translationY;
        private ObservableCollection<System.Drawing.Point> collectionMousePath;
        private ObservableCollection<System.Drawing.Point> rotatedCollection;
        private ObservableCollection<System.Drawing.Point> scaledCollection;
        private ObservableCollection<System.Drawing.Point> translatedCollection;
        private int btnWidth;
        private int btnHeight;
        private int currentPointerX;
        private int currentPointerY;
        private int currentPositionX;
        private int currentPositionY;
        List<MousePoint> allMousePoints;
        Stopwatch stopwatch;
        MousePoint mousePoint;
        MousePath mousePath;
        private Visibility showMainUserControl;
        private Visibility enterVisibility;
        private string nameOfUser;
        private string subfolderPath;
        private string folderPath;
        private bool nameInserted;
        private int countingFiles;
        private bool canExecutePlayingLast;
        private bool started;
        private MousePath currentMousePath;
        private MousePath rotatedMousePath;
        private MousePath scaledMousePath;
        private MousePath translatedMousePath;
        private bool isSliderSet;
        private bool canExecute;
        private System.Drawing.Point testNew;
        #endregion
        #region constructor
        public MainWindowViewModel()
        {
            simulateMove = new SimulateMoving();
            deviation = 300;
            showAllLines = Visibility.Collapsed;
            ScreenWidth = SystemParameters.PrimaryScreenWidth;
            ScreenHeight = SystemParameters.PrimaryScreenHeight;
            sliderValue = 1;
            rotatedCollection = new();
            scaledCollection = new();
            translatedCollection = new();
            rotatedMousePath = new();
            scaledCollection = new();
            translatedCollection = new();
            currentMousePath = new();
            started = false;
            enterVisibility = Visibility.Visible;
            isBackgroundVisible = true;
            showMainUserControl = Visibility.Collapsed;
            allMousePoints = new();
            stopwatch = new Stopwatch();
            recordBtn = ValuesForButtons.Record.ToString();
            simulateBtnTxt = ValuesForButtons.Simulate.ToString();
        }
        #endregion
        #region Properties
        public System.Drawing.Point TestNew
        {
            get
            {
                return testNew;
            }
            set
            {
                testNew = value;
                OnPropertyChanged(nameof(testNew));
            }
        }
        public string SimulateTxtBtn
        {
            get
            {
                return simulateBtnTxt;
            }
            set
            {
                simulateBtnTxt = value;
                OnPropertyChanged(nameof(SimulateTxtBtn));
            }
        }
        public int Deviation
        {
            get { return deviation; }
            set
            {
                deviation = value;
                OnPropertyChanged(nameof(Deviation));
            }
        }
        public string RecordBtn
        {
            get
            {
                return recordBtn;
            }
            set
            {
                recordBtn = value;
                OnPropertyChanged(nameof(RecordBtn));
            }
        }
        public bool IsBackgroundVisible
        {
            get
            {
                return isBackgroundVisible;
            }
            set
            {
                isBackgroundVisible = value;
                OnPropertyChanged(nameof(IsBackgroundVisible));
            }
        }
        public Visibility ShowAllLines
        {
            get
            {
                return showAllLines;
            }
            set
            {
                showAllLines = value;
                OnPropertyChanged(nameof(ShowAllLines));
            }
        }

        public bool ShowLines
        {
            get
            {
                return showLines;
            }
            set
            {
                showLines = value;
                if (showLines)
                {
                    ShowAllLines = Visibility.Visible;
                }
                else
                {
                    ShowAllLines = Visibility.Collapsed;
                }
                OnPropertyChanged(nameof(ShowLines));
            }
        }
        public double ScreenWidth
        {
            get
            {
                return screenWidth;
            }
            set
            {
                screenWidth = value;
                OnPropertyChanged(nameof(screenWidth));
            }
        }
        public double ScreenHeight
        {
            get
            {
                return screenHeight;
            }
            set
            {
                screenHeight = value;
                OnPropertyChanged(nameof(screenHeight));
            }
        }
        public double SliderValue
        {
            get
            {
                return sliderValue;
            }
            set
            {
                sliderValue = value;
                isSliderSet = true;
                OnPropertyChanged(nameof(SliderValue));
            }
        }
        public double TimeMS
        {
            get
            {
                return timeMS;
            }
            set
            {
                timeMS = value;
                OnPropertyChanged(nameof(TimeMS));
            }
        }
        public double TranslationX
        {
            get
            {
                return translationX;
            }
            set
            {
                translationX = value;
                OnPropertyChanged(nameof(TranslationX));
            }
        }
        public double TranslationY
        {
            get
            {
                return translationY;
            }
            set
            {
                translationY = value;
                OnPropertyChanged(nameof(TranslationY));
            }
        }
        public MousePath RotatedMousePath
        {
            get
            {
                return rotatedMousePath;
            }
            set
            {
                rotatedMousePath = value;
                OnPropertyChanged(nameof(RotatedMousePath));
            }
        }
        public MousePath ScaledMousePath
        {
            get
            {
                return scaledMousePath;
            }
            set
            {
                scaledMousePath = value;
                OnPropertyChanged(nameof(ScaledMousePath));
            }
        }
        public MousePath TranslatedMousePath
        {
            get
            {
                return translatedMousePath;
            }
            set
            {
                translatedMousePath = value;
                OnPropertyChanged(nameof(TranslatedMousePath));
            }
        }
        public MousePath CurrentMousePath
        {
            get
            {
                return currentMousePath;
            }
            set
            {
                currentMousePath = value;
                OnPropertyChanged(nameof(CurrentMousePath));
            }
        }
        public ObservableCollection<System.Drawing.Point> CollectionMousePath
        {
            get
            {
                return collectionMousePath;
            }
            set
            {
                collectionMousePath = value;
                OnPropertyChanged(nameof(CollectionMousePath));
            }
        }
        public ObservableCollection<System.Drawing.Point> RotatedCollection
        {
            get
            {
                return rotatedCollection;
            }
            set
            {
                rotatedCollection = value;
                OnPropertyChanged(nameof(RotatedCollection));
            }
        }
        public ObservableCollection<System.Drawing.Point> ScaledCollection
        {
            get
            {
                return scaledCollection;
            }
            set
            {
                scaledCollection = value;
                OnPropertyChanged(nameof(ScaledCollection));
            }
        }
        public ObservableCollection<System.Drawing.Point> TranslatedCollection
        {
            get
            {
                return translatedCollection;
            }
            set
            {
                translatedCollection = value;
                OnPropertyChanged(nameof(TranslatedCollection));
            }
        }
        public string NameOfUser
        {
            get
            {
                return nameOfUser;
            }
            set
            {
                nameOfUser = value;
                OnPropertyChanged(nameof(NameOfUser));
            }
        }
        public Visibility EnterVisibility
        {
            get
            {
                return enterVisibility;
            }
            set
            {
                enterVisibility = value;
                OnPropertyChanged(nameof(EnterVisibility));
            }
        }
        public Visibility ShowMainUserControl
        {
            get
            {
                return showMainUserControl;
            }
            set
            {
                showMainUserControl = value;
                if (showMainUserControl == Visibility.Visible)
                {
                    IsBackgroundVisible = false;
                }
                else
                {
                    isBackgroundVisible = true;
                }
                OnPropertyChanged(nameof(ShowMainUserControl));
            }
        }
        public int BtnWidth
        {
            get
            {
                return btnWidth;
            }
            set
            {
                btnWidth = value;
                OnPropertyChanged(nameof(BtnWidth));
            }
        }
        public int BtnHeight
        {
            get
            {
                return btnHeight;
            }
            set
            {
                btnHeight = value;
                OnPropertyChanged(nameof(BtnHeight));
            }
        }
        public int CurrentPointerX
        {
            get
            {
                return currentPointerX;
            }
            set
            {
                currentPointerX = value;
                OnPropertyChanged(nameof(CurrentPointerX));
            }
        }
        public int CurrentPointerY
        {
            get
            {
                return currentPointerY;
            }
            set
            {
                currentPointerY = value;
                OnPropertyChanged(nameof(CurrentPointerY));
            }
        }
        public int BtnCurrentPositionX
        {
            get
            {
                return currentPositionX;
            }
            set
            {
                currentPositionX = value;
                OnPropertyChanged(nameof(BtnCurrentPositionX));
            }
        }
        public int BtnCurrentPositionY
        {
            get
            {
                return currentPositionY;
            }
            set
            {
                currentPositionY = value;
                OnPropertyChanged(nameof(BtnCurrentPositionY));
            }
        }
        #endregion
        public static void CheckCanExecutePlayingLast(int countingFiles, bool canExecutePlayingLast)
        {
            try
            {
                if (countingFiles != 0)
                {
                    canExecutePlayingLast = true;
                }
                else
                {
                    canExecutePlayingLast = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in Checking Can Execute Playing Last: " + ex.Message);
            }
        }
        public Point GetCoordinates()
        {
            try
            {
                Random r = new Random();
                Normal normalDistX = new Normal(screenWidth / 2, Deviation);
                Normal normalDistY = new Normal(screenHeight / 2, Deviation);
                double buttonX = normalDistX.Sample();
                double buttonY = normalDistY.Sample();
                do
                {
                    buttonX = (int)Math.Floor(normalDistX.Sample());
                } while (buttonX < 0 || buttonX > ScreenWidth - BtnWidth);
                do
                {
                    buttonY = (int)Math.Floor(normalDistY.Sample());
                } while (buttonY < 0 || buttonY > ScreenHeight - BtnHeight - 250);
                return new Point((int)buttonX, (int)buttonY);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in Getting Coordinates: " + ex.Message);
                return new Point(0, 0);
            }
        }
        private void MovingMouseSimulate()
        {
            try
            {
                SimulateMoving.StaticMovingMouseSimulate(nameInserted, ref currentPointerX, ref currentPointerY, started, ref mousePoint, ref stopwatch, ref allMousePoints, recording);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in MovingMouseSimulate: " + ex.Message);
            }
        }
        private void UpdatePointsOfButton(System.Drawing.Point WidthHeight)
        {
            try
            {
                BtnWidth = (int)WidthHeight.X;
                BtnHeight = (int)WidthHeight.Y;
                Point currentPositionBtn = GetCoordinates();
                BtnCurrentPositionX = (int)currentPositionBtn.X;
                BtnCurrentPositionY = (int)currentPositionBtn.Y;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in UpdatePointsOfButton: " + ex.Message);
            }
        }
        private void SetButton()
        {
            try
            {
                System.Drawing.Point WidthHeight = SimulateMoving.SetButton();
                UpdatePointsOfButton(WidthHeight);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in SetButton: " + ex.Message);
            }
        }
        private string NewCalculateValuesForTargetingFolder(MousePath mousePath)
        {
            try
            {
                string targetLocation = "";
                double diffWidth = Math.Abs(mousePath.allMousePoints.Last().coordinate.X - mousePath.allMousePoints.First().coordinate.X);
                double diffHeight = Math.Abs(mousePath.allMousePoints.Last().coordinate.Y - mousePath.allMousePoints.First().coordinate.Y);
                targetLocation = SimulateMoving.ArrangePathToFolderSize(diffWidth, diffHeight, targetLocation);
                return targetLocation;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in NewCalculateValuesForTargetingFolder: " + ex.Message);
                return "";
            }
        }
        private void CheckFolderForSave(MousePath mousePath)
        {
            try
            {
                string targetLocation = "";
                targetLocation = NewCalculateValuesForTargetingFolder(mousePath);
                var currentSubfolderPath = subfolderPath.Split('\\');
                string newLocation = "";
                for (int i = 0; i < currentSubfolderPath.Length; i++)
                {
                    if (i != 3)
                    {
                        if (i != currentSubfolderPath.Length - 1)
                        {
                            newLocation += currentSubfolderPath[i] + "\\";
                        }
                        else
                        {
                            newLocation += currentSubfolderPath[i];
                        }
                    }
                    else
                    {
                        newLocation += targetLocation + "\\";
                    }
                }
                subfolderPath = newLocation;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in CheckFolderForSave: " + ex.Message);
            }
        }
        public void ClickingButtonOnScreen()
        {
            try
            {
                BtnCurrentPositionX = BtnCurrentPositionX + BtnWidth / 2;
                BtnCurrentPositionY = BtnCurrentPositionY + BtnHeight / 2;
                mousePath = new MousePath();
                mousePath.allMousePoints = allMousePoints;
                CheckFolderForSave(mousePath);
                if (allMousePoints[allMousePoints.Count - 1].elapsedMS > 5000)
                {
                    mousePath.moveType = MovementType.RandomMovement;
                }
                else
                {
                    mousePath.moveType = MovementType.ButtonClick;
                }
                stopwatch.Restart();
                File.WriteAllText(subfolderPath + "\\" + countingFiles + ".json", JsonConvert.SerializeObject(mousePath));
                countingFiles++;
                CheckCanExecutePlayingLast(countingFiles, canExecutePlayingLast);
                allMousePoints.Clear();
                SetButton();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in ClickingButtonOnScreen: " + ex.Message);
            }
        }
        /// <summary>
        /// When app starts,this method creates important folders for users 
        /// </summary>
        public void InsertingName()
        {
            try
            {
                if (nameOfUser != null)
                {
                    if (nameOfUser.Length < 2)
                    {
                        System.Windows.MessageBox.Show("Enter Valid Name");
                    }
                    else
                    {
                        ShowMainUserControl = Visibility.Visible;
                        EnterVisibility = Visibility.Collapsed;
                        simulateMove.CreateFolders(out subfolderPath, out folderPath, nameOfUser, countingFiles, canExecutePlayingLast);
                        CheckCanExecutePlayingLast(countingFiles, canExecutePlayingLast);
                        nameInserted = true;
                        canExecute = true;
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Enter Valid Name");
                }               
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in InsertingName: " + ex.Message);
            }
        }
        public void StartRecord()
        {
            try
            {
                if (!recording)
                {
                    recording = true;
                    started = true;
                    SetButton();
                    RotatedCollection.Clear();
                    ScaledCollection.Clear();
                    TranslatedCollection.Clear();
                    TranslatedCollection.Clear();
                    RecordBtn = ValuesForButtons.Stop.ToString();
                }
                else
                {
                    recording = false;
                    StopRecord();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in StartRecord: " + ex.Message);
            }
        }
        public void StopRecord()
        {
            try
            {
                RecordBtn = ValuesForButtons.Record.ToString();
                recording = false;
                started = false;
                allMousePoints.Clear();
                BtnCurrentPositionX = 0;
                BtnCurrentPositionY = 0;
                BtnWidth = 0;
                BtnHeight = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in StopRecord: " + ex.Message);
            }
        }
        /// <summary>
        /// This method gets last saved file from all folders in c:/nenad/humanMousePath...
        /// </summary>
        private void MovingMouse()
        {
            try
            {
                Point currentPointStart = new Point(CurrentPointerX, CurrentPointerY);
                Point currentPointEnd = new Point(BtnCurrentPositionX + BtnWidth / 2, BtnCurrentPositionY + BtnHeight / 2);
                if (currentPointEnd.X == 0 && currentPointEnd.Y == 0)
                {
                    SetButton();
                }
                MovingHumanMouse();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in MovingMouse: " + ex.Message);
            }
        }
        private void ClearPaths()
        {
            TranslatedCollection = new();
            RotatedCollection = new();
            ScaledCollection = new();
            CollectionMousePath = new();
            currentMousePath = new();
        }
        private void SetPathPointsFromDll()
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                foreach (var item in simulateMove.OriginalPath)
                {
                    CollectionMousePath.Add(item);
                }
                currentMousePath = simulateMove.CurrentMousePath;
                foreach (var item in simulateMove.Translated)
                {
                    TranslatedCollection.Add(item);
                }
                foreach (var item in simulateMove.Rotated)
                {
                    RotatedCollection.Add(item);
                }
                foreach (var item in simulateMove.Scaled)
                {
                    ScaledCollection.Add(item);
                }
            });
        }
        private async void MovingHumanMouse()
        {
            try
            {
                if (simulateBtnTxt == ValuesForButtons.Simulate.ToString() && showMainUserControl == Visibility.Visible)
                {
                    simulating = true;
                    do
                    {
                        simulateMove = new();
                        if (simulateMove.SubfolderPath == "" || simulateMove.SubfolderPath == null)
                        {
                            simulateMove.SubfolderPath = subfolderPath;
                        }
                        if (simulateMove.FolderPath == "" || simulateMove.FolderPath == null)
                        {
                            simulateMove.FolderPath = folderPath;
                        }
                        ClearPaths();
                        SimulateTxtBtn = ValuesForButtons.Stop.ToString();
                        // make a button                      
                        SetButton();
                        System.Drawing.Point pStart, pEnd;
                        pStart = SimulateMoving.GetMouseLocation();
                        pEnd = SimulateMoving.GetCenterOfButton(BtnWidth, BtnHeight, BtnCurrentPositionX, BtnCurrentPositionY);
                        if (pStart.X == pEnd.X && pStart.Y == pEnd.Y)
                        {
                            string m = "m";
                        }
                        Task task = new Task(async () => await simulateMove.SimulateMove(pStart, pEnd, sliderValue));
                        task.Start();
                        await task.ContinueWith((t) =>
                        {
                           SetPathPointsFromDll();
                           Thread.Sleep(2000);
                        });
                    }
                    while (simulating);
                }
                else
                {
                    simulating = false;
                    SimulateTxtBtn = ValuesForButtons.Simulate.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in MovingHumanMouse: " + ex.Message);
            }
        }
        private void SettingsOpening()
        {
            try
            {
                settingsView = new();
                settingsView.DataContext = this;
                settingsView.Show();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in SettingOpening method: " + ex.Message);
            }
        }
        private void SavingSettings()
        {
            settingsView.Close();
        }
        private void StopExe()
        {
            App.Current.Shutdown();
        }
        private bool CanExecuteInsertName()
        {
            return true;
        }
        public bool CanExecutePlayingLast()
        {
            return canExecutePlayingLast;
        }
        public bool CanExecute()
        {
            return canExecute;
        }
        #region Commands
        public ICommand MouseMoving { get { return new RelayCommand(MovingMouseSimulate, CanExecute); } }
        public ICommand ClickButton { get { return new RelayCommand(ClickingButtonOnScreen, CanExecute); } }
        public ICommand InsertName { get { return new RelayCommand(InsertingName, CanExecuteInsertName); } }
        public ICommand StartRecording { get { return new RelayCommand(StartRecord, CanExecute); } }
        public ICommand MoveMouse { get { return new RelayCommand(MovingMouse, CanExecute); } }
        public ICommand SettingsOpen { get { return new RelayCommand(SettingsOpening, CanExecute); } }
        public ICommand SaveSettings { get { return new RelayCommand(SavingSettings, CanExecute); } }
        public ICommand StopExecuting { get { return new RelayCommand(StopExe, CanExecute); } }
        #endregion
    }
}

