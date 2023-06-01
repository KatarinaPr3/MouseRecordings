using Newtonsoft.Json;
using System.Diagnostics;

namespace MouseMovementsDLL
{
    public enum EnumMouseLenght
    {
        VeryShort,
        Short,
        Medium,
        Long
    }
    public enum ValuesForButtons
    {
        Simulate,
        Record,
        Stop
    }
    public class SimulateMoving
    {
        #region members
        private string folderPath;
        private string subfolderPath;

        private List<Point> translated;
        private List<Point> rotated;
        private List<Point> scaled;
        private List<Point> originalPath;
        private MousePath currentMousePath;
        #endregion
        #region Constructor
        public SimulateMoving()
        {      
        }
        #endregion
        #region Properties
       public string FolderPath
        {
            get
            {
                return folderPath;
            }
            set
            {
                folderPath = value;
            }
        }
        public string SubfolderPath
        {
            get
            {
                return subfolderPath;
            }
            set
            {
                subfolderPath = value;
            }
        }
        public List<Point> Translated
        {
            get
            {
                return translated;
            }
        }
        public List<Point> Rotated
        {
            get
            {
                return rotated;
            }
        }
        public List<Point> Scaled
        {
            get
            {
                return scaled;
            }
        }
        public List<Point> OriginalPath
        {
            get
            {
                return originalPath;
            }
        }
        public MousePath CurrentMousePath
        {
            get
            {
                return currentMousePath;
            }
        }

        #endregion
        #region Methods

        private void WriteLog(string debugFile, string debugTextStart, string debugTextEnd)
        {
            string logDirectory = Path.Combine(Environment.CurrentDirectory, "logs");
            Directory.CreateDirectory(logDirectory);
            string logFilePath = Path.Combine(logDirectory, "log.log");
            TextWriterTraceListener writer = new TextWriterTraceListener(logFilePath);
            writer.TraceOutputOptions = TraceOptions.DateTime | TraceOptions.ProcessId;
            writer.WriteLine("====================");
            writer.WriteLine("Log started at: " + DateTime.Now.ToString());
            writer.Flush();
            Trace.Listeners.Add(writer);
            Trace.WriteLine(debugFile);
            Trace.WriteLine(debugTextStart);
            Trace.WriteLine(debugTextEnd);
            Trace.Listeners.Remove(writer);
            writer.Flush();
        }
        public async Task<MousePath> SimulateMove(Point pStart, Point pEnd, double speed)
        {
            try
            {
                currentMousePath = new();
                var filePaths = GettingPathFile(pStart, pEnd);
                if (filePaths != null)
                {
                    Random r = new Random();
                    if (filePaths.Length > 0)
                    {
                        var randomFile = filePaths[r.Next(0, filePaths.Length - 1)];
                        //randomFile = "C://nenad//humanMousePath//VeryShort//nenad//58.json";
                        string debugFile = "File: " + randomFile;
                        Debug.WriteLine(debugFile);
                        var debugTextEnd = "Start: X: " + pStart.X + " Y: " + pStart.Y;
                        var debugTextStart = "END X: " + pEnd.X + " Y: " + pEnd.Y;
                        Debug.WriteLine(debugTextStart);
                        Debug.WriteLine(debugTextEnd);
                        WriteLog(debugFile, debugTextStart, debugTextEnd);
                        var fileFromFolder = File.ReadAllText(randomFile);
                        currentMousePath = TransformPath(fileFromFolder, pStart, pEnd, speed);
                    }
                }
                return currentMousePath;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in SimulateMove: " + ex.Message);
            }
        }
        private string[] GettingPathFile(Point startPoint, Point endPoint)
        {
            try
            {
                string targetLocation = "";
                targetLocation = CalculateForTargetFolder((int)startPoint.X, (int)endPoint.X, (int)startPoint.Y, (int)endPoint.Y);
                var currentfolderPath = folderPath.Split('\\');
                var textChange = currentfolderPath[3];
                string newLocation = "";
                newLocation = SetNewLocation(ref currentfolderPath, ref newLocation, ref targetLocation);
                folderPath = newLocation;
                var filePaths = Directory.GetFiles(folderPath, "*.json", SearchOption.AllDirectories);
                if (filePaths.Length == 0)
                {
                    var newFolderPath = "";
                    for (int i = 0; i < currentfolderPath.Length; i++)
                    {
                        if (i == currentfolderPath.Length - 2)
                        {
                            newFolderPath += currentfolderPath[i];

                        }
                        else if (i != currentfolderPath.Length - 1)
                        {
                            newFolderPath += currentfolderPath[i] + "\\";
                        }
                    }
                    GetRandomFile(newFolderPath);
                    filePaths = Directory.GetFiles(newFolderPath, "*.json", SearchOption.AllDirectories);
                }
                return filePaths;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in GettingPathFile: " + ex.Message);
            }
        }
        private string GetRandomFile(string folderPath)
        {
            try
            {
                var filePaths = Directory.GetFiles(folderPath, "*json", SearchOption.AllDirectories);
                Random r = new Random();
                var randomFile = filePaths[r.Next(0, filePaths.Length - 1)];
                var fileFromFolder = File.ReadAllText(randomFile);
                return randomFile;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in Getiing Random File: " + ex.Message);
            }
        }
        private MousePath TransformPath(string fileFromFolder, Point startPoint, Point endPoint, double speed)
        {
            try
            {
                MousePath mPath = JsonConvert.DeserializeObject<MousePath>(fileFromFolder);
                List<Point> CollectionMousePath = new();
                foreach (var item in mPath.allMousePoints)
                {
                    Point p = new Point(item.coordinate.X, item.coordinate.Y);
                    CollectionMousePath.Add(p);
                }
                int copyOfStartX = (int)startPoint.X;
                int copyOfStartY = (int)startPoint.Y;
                int copyOfEndX = (int)endPoint.X;
                int copyOfEndY = (int)endPoint.Y;
                originalPath = OriginalPathCollection(ref mPath);
                List<Point> tempTranslatedCollection = new();
                tempTranslatedCollection = Translating(ref copyOfEndX, ref copyOfEndY,
                    copyOfStartX, copyOfStartY, ref mPath);
                List<Point> tempRotatedCollection = new();
                tempRotatedCollection = Rotating(ref copyOfEndX, ref copyOfEndY, ref CollectionMousePath, ref tempTranslatedCollection, copyOfStartX, copyOfStartY);
                SimulateCollection(mPath, tempRotatedCollection, false);
                List<Point> tempScaledCollection = new();
                tempScaledCollection = Scaling(ref copyOfEndX, ref copyOfEndY, ref mPath, ref tempRotatedCollection);
                SimulateScaledCollectionAndMouseMovement(tempScaledCollection, mPath, true, speed);
                return mPath;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in TransformPath: " + ex.Message);
            }
        }
        public void CreateFolders(out string subfolderPath,out string folderPath, string nameOfUser, int countingFiles, bool canExecutePlayingLast)
        {
            try
            {
                subfolderPath = "";
                folderPath = "";
                foreach (var item in Enum.GetValues(typeof(EnumMouseLenght)))
                {
                    folderPath = @"C:\nenad\humanMousePath\" + item.ToString();
                    subfolderPath = folderPath + "\\" + nameOfUser;
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                        Directory.CreateDirectory(subfolderPath);
                    }
                    else
                    {
                        if (!Directory.Exists(subfolderPath))
                        {
                            Directory.CreateDirectory(subfolderPath);
                        }
                        else
                        {
                            countingFiles = Directory.GetFiles(subfolderPath).Length;
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in CreateFolders: " + ex.Message);
            }
        }
        private string SetNewLocation(ref string[] currentfolderPath, ref string newLocation, ref string targetLocation)
        {
            try
            {
                for (int i = 0; i < currentfolderPath.Length; i++)
                {
                    if (i != 3)
                    {
                        newLocation += currentfolderPath[i] + "\\";
                    }
                    else
                    {
                        newLocation += targetLocation;
                    }
                }
                return newLocation;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in Setting New Location for Getting file path: " + ex.Message);
            }
        }
        private static Point MakeButton()
        {
            try
            {
                Random r = new Random();
                int width = r.Next(50, 100);
                int heigh = r.Next(15, 55);
                return new Point(width, heigh);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in MakeButton: " + ex.Message);
            }
        }
        public static Point SetButton()
        {
            try
            {
                Point WidthHeight = new(0, 0);
                WidthHeight = MakeButton();               
                return WidthHeight;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in SetButton: " + ex.Message);
            }
        }
        private string CalculateForTargetFolder(int pointStartX, int pointEndX, int pointStartY, int pointEndY)
        {
            try
            {
                string targetLocation = "";
                var diffWidth = Math.Abs(pointStartX - pointEndX);
                var diffHeight = Math.Abs(pointStartY - pointEndY);
                targetLocation = ArrangePathToFolderSize(diffWidth, diffHeight, targetLocation);
                return targetLocation;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in CalculateForTargetFolder: " + ex.Message);
            }
        }
        public static string ArrangePathToFolderSize(double diffWidth, double diffHeight, string targetLocation)
        {
            try
            {
                var maxDim = Math.Max(diffWidth, diffHeight);
                if (maxDim >= 0 && maxDim < 50)
                {
                    targetLocation = EnumMouseLenght.VeryShort.ToString();
                }
                if (maxDim >= 50 && maxDim < 300)
                {
                    targetLocation = EnumMouseLenght.Short.ToString();
                }
                if (maxDim >= 300 && maxDim < 800)
                {
                    targetLocation = EnumMouseLenght.Medium.ToString();
                }
                if (maxDim >= 800 || maxDim >= 800)
                {
                    targetLocation = EnumMouseLenght.Long.ToString();
                }
                return targetLocation;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in ArrangePathToFolderSize: " + ex.Message);
            }
        }
        public static Point GetMouseLocation()
        {
            try
            {
                var point = Control.MousePosition;
                return point;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in GetMouseLocation: " + ex.Message);
            }
        }
        public static void StaticMovingMouseSimulate(bool nameInserted, ref int currentPointerX, ref int currentPointerY, bool started, ref MousePoint mousePoint, ref Stopwatch stopwatch, ref List<MousePoint> allMousePoints, bool recording)
        {
            try
            {
                if (nameInserted)
                {
                    Point mouseXY = GetMouseLocation();

                    currentPointerX = (int)mouseXY.X;
                    currentPointerY = (int)mouseXY.Y;
                   
                    if (started)
                    {
                        mousePoint.coordinate = mouseXY;
                        if (stopwatch.ElapsedMilliseconds > 0 && allMousePoints.Count == 0)
                        {
                            stopwatch.Reset();
                            stopwatch.Stop();
                        }
                        mousePoint.elapsedMS = (long)stopwatch.Elapsed.TotalMilliseconds;

                        allMousePoints.Add(mousePoint);
                        if (allMousePoints.Count == 0 && !recording)
                        {
                            stopwatch.Start();
                        }
                        if (allMousePoints.Count == 1)
                        {
                            stopwatch.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in StaticMovingMouseSimulate: " + ex.Message);
            }
        }
        public static Point GetCenterOfButton(int width, int height, int currentPositionX, int currentPositionY)
        {
            try
            {
                Point center = new Point((int)width / 2, (int)height / 2);
                Point positions = new Point((int)currentPositionX, (int)currentPositionY);
                return new Point(positions.X + center.X, positions.Y + center.Y);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in GetCenterOfButton: " + ex.Message);
            }
        }
        private List<Point> OriginalPathCollection(ref MousePath currentMousePath)
        {
            originalPath = new();
            foreach (var item in currentMousePath.allMousePoints)
            {
                originalPath.Add(item.coordinate);
            }
            return originalPath;
        }
        private List<Point> Translating(ref int currentPositionX, ref int currentPositionY, double currentPointerX, double currentPointerY, ref MousePath currentMousePath)
        {
            try
            {
                translated = new();
                Point newStartPoint = new Point((int)currentPointerX, (int)currentPointerY);
                Point newEndPoint = new(currentPositionX, currentPositionY);
                // Calculate translation vector
                Point translation = new Point(newStartPoint.X - currentMousePath.allMousePoints[0].coordinate.X,
                                              newStartPoint.Y - currentMousePath.allMousePoints[0].coordinate.Y);
                // Create new mouse path with translated, scaled, and rotated points         
                foreach (MousePoint point in currentMousePath.allMousePoints)
                {
                    Point translatedPoint = new Point(point.coordinate.X + translation.X, point.coordinate.Y + translation.Y);

                    translated.Add(translatedPoint);
                }
                return translated;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in Translating: " + ex.Message);
            }
        }
        private List<Point> Rotating(ref int currentPositionX, ref int currentPositionY, ref List<Point> collectionMousePath, ref List<Point> translatedCollection, double currentPointerX, double currentPointerY)
        {
            try
            {
                rotated = new();
                int countOfEements = collectionMousePath.Count;
                // Calculate rotation angle
                double originalAngle = Math.Atan2((double)translatedCollection[translatedCollection.Count - 1].Y - (double)translatedCollection[0].Y,
                                                    (double)translatedCollection[translatedCollection.Count - 1].X - (double)translatedCollection[0].X);
                Point newStartPoint = new Point((int)currentPointerX, (int)currentPointerY);
                Point newEndPoint = new Point(currentPositionX, currentPositionY);
                double newAngle = Math.Atan2(newEndPoint.Y - newStartPoint.Y, newEndPoint.X - newStartPoint.X);
                double rotationAngle = newAngle - originalAngle;
                //double rotationAngle = Math.Abs(newAngle - originalAngle);
                double degreesOld = originalAngle * (180.0 / Math.PI);
                double degreesNew = newAngle * (180.0 / Math.PI);

                var angle = rotationAngle;
                if (rotated.Count > 0)
                {
                    rotated.Clear();
                }
                foreach (var item in translatedCollection)
                {
                    Point p = item;
                    var rotatedPoint = MousePathTransformer.RotatePoint(p, translatedCollection.First(), angle);
                    //Point abs = new(Math.Abs(rotatedPoint.X), Math.Abs(rotatedPoint.Y));
                    Point abs = new(Math.Abs(rotatedPoint.X), rotatedPoint.Y);

                    rotated.Add(abs);
                }
                return rotated;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in Rotating: " + ex.Message);
            }
        }
        private List<Point> Scaling(ref int currentPositionX, ref int currentPositionY, ref MousePath currentMousePath, ref List<Point> rotatedCollection)
        {
            try
            {
                scaled = new();
                MousePath mPath = new();
                mPath = currentMousePath;
                Point newStartPoint = new Point(rotatedCollection[0].X, rotatedCollection[0].Y);
                Point newEndPoint = new(currentPositionX, currentPositionY);
                double newPointX = newEndPoint.X - newStartPoint.X;
                double oldPointX = rotatedCollection[0].X - rotatedCollection[rotatedCollection.Count - 1].X;
                //double newPointY = newEndPoint.Y - newStartPoint.Y;
                double newPointY = Math.Abs(newEndPoint.Y - newStartPoint.Y);

                double oldPointY = rotatedCollection[0].Y - rotatedCollection[rotatedCollection.Count - 1].Y;
                double scaleX = Math.Abs((double)newPointX / oldPointX);
                double scaleY = Math.Abs((double)newPointY / oldPointY);
                if (double.IsNaN(scaleY))
                {
                    scaleY = 0;
                }
                if (double.IsNaN(scaleX))
                {
                    scaleX = 0;
                }

                foreach (Point point in rotatedCollection)
                {
                    Point scaledPoint = new(0, 0);
                    double differenceStartEndOldX = point.X - rotatedCollection[0].X;
                    double differenceStartEndOldY = point.Y - rotatedCollection[0].Y;
                    if ((newStartPoint.X + differenceStartEndOldX * scaleX < 0) || (newStartPoint.Y + differenceStartEndOldY * scaleY < 0))
                    {
                        if (newStartPoint.X + differenceStartEndOldX * scaleX < 0 && newStartPoint.Y + differenceStartEndOldY * scaleY < 0)
                        {
                            scaledPoint = new Point(0, 0);
                        }
                        else if (newStartPoint.X + differenceStartEndOldX * scaleX < 0)
                        {
                            scaledPoint = new Point(0, (int)(newStartPoint.Y + differenceStartEndOldY * scaleY));
                        }
                        else if (newStartPoint.Y + differenceStartEndOldY * scaleY < 0)
                        {
                            scaledPoint = new Point((int)(newStartPoint.X + differenceStartEndOldX * scaleX), 0);

                        }

                    }
                    else if ((newStartPoint.X + differenceStartEndOldX * scaleX > Screen.PrimaryScreen.Bounds.Width) || (newStartPoint.Y + differenceStartEndOldY * scaleY > Screen.PrimaryScreen.Bounds.Height))
                    {
                        if (newStartPoint.X + differenceStartEndOldX * scaleX > Screen.PrimaryScreen.Bounds.Width && newStartPoint.Y + differenceStartEndOldY * scaleY > Screen.PrimaryScreen.Bounds.Height)
                        {
                            scaledPoint = new Point(Screen.PrimaryScreen.Bounds.Width - 10, Screen.PrimaryScreen.Bounds.Height - 10);
                        }
                        else if (newStartPoint.X + differenceStartEndOldX * scaleX > Screen.PrimaryScreen.Bounds.Width)
                        {
                            scaledPoint = new Point(Screen.PrimaryScreen.Bounds.Width - 10, (int)(newStartPoint.Y + differenceStartEndOldY * scaleY));
                            Point pT = new Point(Screen.PrimaryScreen.Bounds.Width - 10, (int)(newStartPoint.Y + differenceStartEndOldY * scaleY));

                        }
                        else if (newStartPoint.Y + differenceStartEndOldY * scaleY > Screen.PrimaryScreen.Bounds.Height)
                        {
                            scaledPoint = new Point((int)(newStartPoint.X + differenceStartEndOldX * scaleX), Screen.PrimaryScreen.Bounds.Height - 10);
                            Point pT = new Point((int)(newStartPoint.X + differenceStartEndOldX * scaleX), Screen.PrimaryScreen.Bounds.Height);
                        }

                    }
                    else if (differenceStartEndOldX == 0 && differenceStartEndOldY == 0)
                    {
                        if (scaleY == 0)
                        {
                            scaledPoint = new Point(point.X, newEndPoint.Y);
                        }
                        if (scaleX == 0)
                        {
                            scaledPoint = new Point(newEndPoint.X, point.Y);
                        }
                        if (scaleY != 0 && scaleX != 0)
                        {
                            scaledPoint = new(newStartPoint.X, newStartPoint.Y);
                        }
                    }
                    else
                    {
                        scaledPoint = new Point((int)(newStartPoint.X + differenceStartEndOldX * scaleX),
                          (int)(newStartPoint.Y + differenceStartEndOldY * scaleY));
                    }
                    //Point scaledPoint = new Point((int)(newStartPoint.X + differenceStartEndOldX * scaleX),
                    //  (int)(newStartPoint.Y + differenceStartEndOldY * scaleY));
                    scaled.Add(scaledPoint);
                }
                return scaled;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in Scaling: " + ex.Message);
            }
        }
        private List<Point> SimulateCollection(MousePath currentMousePath, List<Point> tempCollection, bool translatedNeed)
        {
            try
            {
                List<Point> collection = new();
                for (int i = 0; i < currentMousePath.allMousePoints.Count; i++)
                {
                    if (translatedNeed)
                    {
                        collection.Add(tempCollection[i]);
                    }
                    else
                    {
                        collection.Add(tempCollection[i]);
                    }
                }
                return collection;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in SimulateCollection: " + ex.Message);
            }
        }
        private void SimulateScaledCollectionAndMouseMovement(List<Point> tempScaledCollection, MousePath currentMousePath, bool isSliderSet, double sliderValue)
        {
            try
            {
                for (int i = 0; i < currentMousePath.allMousePoints.Count; i++)
                {
                   // ScaledCollection.Add(tempScaledCollection[i]);

                    Point p = new((int)tempScaledCollection[i].X, (int)tempScaledCollection[i].Y);
                    Point position = p;
                    Cursor.Position = new(position.X, position.Y);
                    if (i != 0)
                    {
                        if (isSliderSet)
                        {
                            Thread.Sleep((int)((currentMousePath.allMousePoints[i].elapsedMS - currentMousePath.allMousePoints[i - 1].elapsedMS) / sliderValue));
                        }
                        else
                        {
                            Thread.Sleep((int)currentMousePath.allMousePoints[i].elapsedMS - (int)currentMousePath.allMousePoints[i - 1].elapsedMS);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred in SimulateScaledCollectionAndMouseMovement: " + ex.Message);
            }
        }
        #endregion
    }
}