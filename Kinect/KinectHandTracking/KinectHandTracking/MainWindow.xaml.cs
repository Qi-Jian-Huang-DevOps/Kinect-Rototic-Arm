using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
//using System.Threading;

namespace KinectHandTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Members
        private SerialPort _serialPort = new SerialPort();
        private int _baudRate = 9600;
        private string _portName = SerialPort.GetPortNames()[1];

        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        IList<Body> _bodies;

        #endregion

        #region Constructor

        public MainWindow()
        {

            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _serialPort.BaudRate = _baudRate;
                _serialPort.PortName = _portName;
                _serialPort.Open();
            }
            catch { /*error*/ }

            Process.Start(@"C:\Windows\System32\Kinect\KinectService.exe");

            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_reader != null)
            {
                _reader.Dispose();
            }

            if (_bodies != null)
            {
                if (_bodies.Count() > 0)
                {
                    foreach (var body in _bodies)
                    {
                        //body.Dispose();
                    }
                }
            }

            if (_sensor != null)
            {
                _sensor.Close();
                _serialPort.Close();
            }
        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();
            //byte[] dataByteON = new byte[] { 1 };
            //byte[] dataByteOFF = new byte[] { 0 };
            //byte[] dataByteERR = new byte[] { 2 };

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    camera.Source = frame.ToBitmap();
                }
            }

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    canvas.Children.Clear();

                    _bodies = new Body[frame.BodyFrameSource.BodyCount];

                    frame.GetAndRefreshBodyData(_bodies);

                    foreach (var body in _bodies)
                    {
                        if (body != null)
                        {
                            if (body.IsTracked)
                            {                              

                                // Find the joints
                                Joint handRight = body.Joints[JointType.HandRight];
                                Joint thumbRight = body.Joints[JointType.ThumbRight];

                                Joint handLeft = body.Joints[JointType.HandLeft];
                                Joint thumbLeft = body.Joints[JointType.ThumbLeft];

                                Joint jointA = body.Joints[JointType.ThumbRight];
                                Joint jointB = body.Joints[JointType.HandTipRight];

                                // Draw hands and thumbs
                                canvas.DrawHand(handRight);
                                canvas.DrawHand(handLeft);
                                canvas.DrawThumb(thumbRight);
                                canvas.DrawThumb(thumbLeft);

                                // Find the hand states
                                string rightHandState = "-";
                                string leftHandState = "-";

                                switch (body.HandRightState)
                                {
                                    case HandState.Open:
                                        rightHandState = getStringPosX(jointA) + "\n" + 
                                            getStringPosX(jointB) + "\n" + 
                                            getClamDis(jointA, jointB) + "\nServo: " + 
                                            getSerialByte();

                                        sendSerialByte(getByteClamDis(jointA, jointB));
                                        break;
                                    case HandState.Closed:
                                        rightHandState = "Closed (OFF)";
                                        sendSerialByte(0);
                                        break;
                                    case HandState.Lasso:
                                        rightHandState = "Lasso";
                                        break;
                                    case HandState.Unknown:
                                        rightHandState = "Unknown...";
                                        break;
                                    case HandState.NotTracked:
                                        rightHandState = "Not tracked";
                                        break;
                                    default:
                                        break;
                                }

                                switch (body.HandLeftState)
                                {
                                    case HandState.Open:
                                        leftHandState = "Open";
                                        //_serialPort.Write(dataByte, 0, dataByte.Length);
                                        break;
                                    case HandState.Closed:
                                        leftHandState = "Closed";
                                        break;
                                    case HandState.Lasso:
                                        leftHandState = "Lasso";
                                        break;
                                    case HandState.Unknown:
                                        leftHandState = "Unknown...";
                                        break;
                                    case HandState.NotTracked:
                                        leftHandState = "Not tracked";
                                        break;
                                    default:
                                        break;
                                }

                                tblRightHandState.Text = rightHandState;
                                tblLeftHandState.Text = leftHandState;
                            }
                        }
                    }
                }
            }
        }

        #endregion
        // get position coordinate in string format and in mm unit
        public string getStringPosX(Joint joint)
        {
            return Convert.ToString(joint.Position.X * 1000.0);
        }

        public string getStringPosY(Joint joint)
        {
            return Convert.ToString(joint.Position.Y * 1000.0);
        }

        public string getStringPosZ(Joint joint)
        {
            return Convert.ToString(joint.Position.Z * 1000.0);
        }

        public string getClamDis(Joint jointA, Joint jointB) 
        {
            float a, b;
            a = jointA.Position.X;
            b = jointB.Position.X;
            return Convert.ToString(Math.Round(Math.Abs(a - b) * 1000.0));
        }

        public byte getByteClamDis(Joint jointA, Joint jointB) 
        {
            float a, b;
            a = jointA.Position.X;
            b = jointB.Position.X;
            return (byte)(Math.Round(Math.Abs(a - b) * 1000.0));
        }

        public void sendSerialByte(byte dataByte)
        {
            byte[] byteArray = new byte[] { dataByte };
            _serialPort.Write(byteArray, 0, byteArray.Length);
        }

        public byte getSerialByte()
        {
            return (byte)_serialPort.ReadByte();
        }

        // Display Port values and prompt user to enter a port. 
        public static string SetPortName(string defaultPortName)
        {
            string portName;

            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
            portName = Console.ReadLine();

            if (portName == "" || !(portName.ToLower()).StartsWith("com"))
            {
                portName = defaultPortName;
            }
            return portName;
        }
        // Display BaudRate values and prompt user to enter a value. 
        public static int SetPortBaudRate(int defaultPortBaudRate)
        {
            string baudRate;

            Console.Write("Baud Rate(default:{0}): ", defaultPortBaudRate);
            baudRate = Console.ReadLine();

            if (baudRate == "")
            {
                baudRate = defaultPortBaudRate.ToString();
            }

            return int.Parse(baudRate);
        }

    }
}
