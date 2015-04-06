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
        private string _portName = SerialPort.GetPortNames()[2];
        // unit of the joint positions 
        private double UNIT = 100.0;

        // flag to decise if need to send serial data
        private Boolean signalSend = false;

        private static byte BASE = 0;
        private static byte SHOULDER = 1;
        private static byte ELBOW = 2;
        private static byte WRIST = 3;
        private static byte CLAM = 4;

        private byte baseData, shoulderData, elbowData, wristData, clamData;

        private int ClipBoundsThickness = 10;

        KinectSensor _sensor = null;

        MultiSourceFrameReader _reader;
        IList<Body> _bodies;

        #endregion

        #region Constructor

        public MainWindow()
        {

            InitializeComponent();

            // get size of joint space
            _sensor = KinectSensor.GetDefault();
            FrameDescription frameDes = this._sensor.DepthFrameSource.FrameDescription;
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

                    //using (DrawingContext dc = this.drawingGroup.Open())
                    // {

                    // Draw a transparent background to set the render size
                    //dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, this.displayWidth, this.displayHeight));

                    foreach (var body in _bodies)
                    {
                        if (body != null)
                        {
                            if (body.IsTracked)
                            {
                                // Find the joints
                                Joint handLeft = body.Joints[JointType.HandLeft];
                                Joint thumbLeft = body.Joints[JointType.ThumbLeft];

                                Joint thumbRight = body.Joints[JointType.ThumbRight];
                                Joint handTipRight = body.Joints[JointType.HandTipRight];
                                Joint handRight = body.Joints[JointType.HandRight];
                                Joint elbowRight = body.Joints[JointType.ElbowRight];
                                Joint shoulderRight = body.Joints[JointType.ShoulderRight];

                                // for arm wrist
                                Joint wristRight = body.Joints[JointType.WristRight];

                                // Draw hands and thumbs
                                //canvas.DrawHand(elbowRight);
                                //canvas.DrawHand(shoulderRight);
                                //canvas.DrawThumb(wristRight);
                                canvas.DrawThumb(handRight);

                                // Find the hand states
                                string rightHandState = "-";
                                string leftHandState = "-";

                                // functions calls for all servos
                                baseData = getByteBaseAng(shoulderRight, handRight);
                                shoulderData = getByteShoulderDis(shoulderRight, elbowRight);
                                elbowData = getByteElbowDis(elbowRight, wristRight);
                                wristData = getByteWristDis(wristRight, handTipRight);
                                clamData = getByteClamDis(handTipRight, thumbRight);

                                // display all the servos' informations
                                // clam, wrist, elbow, shoulder and shoulder-base
                                rightHandState = _portName + "\nBase: " + Convert.ToString(baseData) + "\n" +
                                    "Shoulder: " + Convert.ToString(shoulderData) + "\n" +
                                    "Elbow: " + Convert.ToString(elbowData) + "\n" +
                                    "Wrist: " + Convert.ToString(wristData) + "\n" +
                                    "Clam: " + Convert.ToString(clamData) + "\n";

                                //Convert.ToString(getByteShoulderDis(handRight));
                                sendSerialByteArray(baseData, shoulderData, elbowData, wristData, clamData);

                                switch (body.HandLeftState)
                                {
                                    case HandState.Open:
                                        leftHandState = "ACTIVATE";
                                        signalSend = true;
                                        break;
                                  
                                    default:
                                        leftHandState = "INACTIVATE";
                                        signalSend = false;
                                        break;
                                }
  
                                // display text on the window
                                tblRightHandState.Text = rightHandState;
                                tblLeftHandState.Text = leftHandState;
                            }
                        }
                    }
                }
            }
        }

        #endregion
        // get position coordinate in string format
        public string getStringPosX(Joint joint)
        {
            return Convert.ToString(joint.Position.X * UNIT);
        }

        public string getStringPosY(Joint joint)
        {
            return Convert.ToString(joint.Position.Y * UNIT);
        }

        public string getStringPosZ(Joint joint)
        {
            return Convert.ToString(joint.Position.Z * UNIT);
        }

        // need hand and thumb joints
        public byte getByteClamDis(Joint jointA, Joint jointB)
        {
            float a_x, a_y, a_z, b_x, b_y, b_z;
            byte dis = 0;
            a_x = jointA.Position.X;
            a_y = jointA.Position.Y;
            a_z = jointA.Position.Z;
            b_x = jointB.Position.X;
            b_y = jointB.Position.Y;
            b_z = jointB.Position.Z;
            //return (byte)(Math.Round(Math.Abs(a - b) * 1000.0));
            dis = (byte)(Math.Sqrt(Math.Pow(a_x - b_x, 2) +
                Math.Pow(a_y - b_y, 2) +
                Math.Pow(a_z - b_z, 2)) * UNIT);
            return dis;
        }

        // need shoulder and elbow
        public byte getByteShoulderDis(Joint jointShoulder, Joint jointElbow)
        {
            double shoulderDis = jointShoulder.Position.Y;
            double elbowDis = jointElbow.Position.Y;
            double distance = 0;

            if (shoulderDis > elbowDis)
            {// down
                distance = 25 - Math.Abs(shoulderDis - elbowDis) * UNIT;
            }
            else
            {// up
                distance = 25 + Math.Abs(shoulderDis - elbowDis) * UNIT;
            }

            return (byte)(distance);
        }

        // need shoulder and elbow joints
        public byte getByteBaseAng(Joint jointA, Joint jointB)
        {
            float x_val, z_val;
            double angle = 0; // in degree

            x_val = (jointA.Position.X - jointB.Position.X);
            z_val = (jointA.Position.Z - jointB.Position.Z);
            angle = Math.Atan((z_val / x_val)) * (float)(180 / Math.PI);
            // angle only goes from 0 - 90 degree
            angle = Math.Abs(angle);

            if (jointA.Position.X < jointB.Position.X)
            {
                // if shoulder x position is smaller than elbow x position
                // convert it to 90 - 180 degree
                angle = angle + (90 - angle) * 2.0;
            }
            else
            {
                // 0 - 90 degree
            }

            return (byte)angle;
        }

        // need elbow and wrist joints
        public byte getByteElbowDis(Joint jointA, Joint jointB)
        {
            float a_val, b_val;
            double dis = 0;
            a_val = jointA.Position.Y;
            b_val = jointB.Position.Y;

            // (a_val - b_val) * UNIT = distance is about -30 -> 30 cm
            // hence, this will gives 
            dis = Math.Abs((b_val - a_val) * UNIT + 30);

            return (byte)dis;
        }

        // need wrist and hand joints
        public byte getByteWristDis(Joint jointA, Joint jointB)
        {
            double a_val, b_val;
            double dis = 0;
            a_val = jointA.Position.Y;
            b_val = jointB.Position.Y;

            if (a_val < b_val)
            { // up
                dis = Math.Abs((b_val - a_val) * UNIT) + 15;
            }

            else
            {// down
                dis = (-Math.Abs((b_val - a_val) * UNIT)) + 10;
            }

            return (byte)dis;
        }

        public void sendSerialByte(byte dataByte)
        {
            byte[] byteArray = new byte[] { dataByte };
            _serialPort.Write(byteArray, 0, byteArray.Length);
        }

        public void sendSerialByteArray(byte baseData, byte shoulderData, byte elbowData, byte wristData, byte clamData)
        {
            byte[] byteArray = new byte[] { baseData, shoulderData, elbowData, wristData, clamData };

            if (signalSend)
            {
                _serialPort.Write(byteArray, 0, byteArray.Length);
            }
        }

        public byte getSerialByte()
        {
            return (byte)_serialPort.ReadByte();
        }

        /*private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, this.displayHeight - ClipBoundsThickness, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, this.displayWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, this.displayHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(this.displayWidth - ClipBoundsThickness, 0, ClipBoundsThickness, this.displayHeight));
            }
        }*/

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
