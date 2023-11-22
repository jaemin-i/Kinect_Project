using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;


namespace kinectsource
{
    /// <summary>
    /// cctv.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class cctv : Page
    {
        double detect = 3;
        string name;
        string phone;
        string email;

        public cctv()
        {


        }
        public cctv(string name, string phone, string email)
        {
            this.name = name;
            this.phone = phone;
            this.email = email;
            InitializeComponent();
            // InitializeKinect();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            txtTime.Text = "시스템시간 : " + DateTime.Now.ToString("yy.MM.dd HH:mm");
        }

        KinectSensor objKS = null;

        private void InitializeKinect()
        {
            try
            {
                objKS = KinectSensor.KinectSensors[0];
                objKS.ColorStream.Enable();
                objKS.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(objKS_ColorFrameReady);

                objKS.DepthStream.Enable();
                objKS.SkeletonStream.Enable();
                objKS.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(objKS_AllFramesReady);
                objKS.Start();
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("카메라가 연결되어있지않습니다.\n프로그램을 종료합니다.");
            }


        }

        BitmapSource src = null;
        private void objKS_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            ColorImageFrame ImageParam = e.OpenColorImageFrame();
            if (ImageParam == null) return;
            byte[] ImageBits = new byte[ImageParam.PixelDataLength];
            ImageParam.CopyPixelDataTo(ImageBits);

            src = BitmapSource.Create(ImageParam.Width,
                                      ImageParam.Height,
                                      96, 96,
                                      PixelFormats.Bgr32,
                                      null,
                                      ImageBits,
                                      ImageParam.Width * ImageParam.BytesPerPixel);
            image1.Source = src;
        }

        /********************************************************************************************************/
        private void objKS_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            SkeletonFrame sf = e.OpenSkeletonFrame();
            if (sf == null) return;
            Skeleton[] skeletonData = new Skeleton[sf.SkeletonArrayLength];
            sf.CopySkeletonDataTo(skeletonData);
            using (DepthImageFrame depthImageFrame = e.OpenDepthImageFrame())
            {
                if (depthImageFrame != null)
                {
                    foreach (Skeleton sd in skeletonData)
                    {
                        if (sd.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            Joint joint = sd.Joints[JointType.Head];

                            DepthImagePoint depthPoint;
                            depthPoint = depthImageFrame.MapFromSkeletonPoint(joint.Position);

                            Point point = new Point((int)(image1.Height * depthPoint.X / depthImageFrame.Height), (int)(image1.Height * depthPoint.X / depthImageFrame.Height));
                            double y = point.Y;
                            textBlock1.Text = String.Format("Y : {1:0.00}", y);
                            Canvas.SetTop(ellipse1, y);

                            //현재 y좌표가 설정한 값보다 낮다면 아래 시작.
                            if (y <= detect)
                            {
                                cctvImageSave();  //카메라 찍고 로그저장
                                if (btnDetect.IsChecked == true)
                                {
                                    //문자보내는 api시작
                                }
                            }
                        }
                    }
                }
            }
        }

        // y좌표 설정버튼
        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            detect = double.Parse(txtY.Text);
            MessageBox.Show("감지좌표가 " + detect.ToString() + "로 설정되었습니다.");
        }
        /********************************************************************************************************/
        /********************************************************************************************************/
        /********************************************************************************************************/
        /********************************************************************************************************/
        /********************************************************************************************************/
        //기능

        private void cctvImageSave()
        {
            DateTime time = DateTime.Now;
            string fileName = time.ToString("yy/MM/dd/HH-mm-ss") + ".png";
            if (src != null)
            {
                BitmapEncoder encoder = null;
                encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(src));

                FileStream fStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                encoder.Save(fStream);
                fStream.Close();
                System.Diagnostics.Process exe = new System.Diagnostics.Process();
                exe.StartInfo.FileName = fileName;
                exe.Start();
            }
        }

        private void btnDetect_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}