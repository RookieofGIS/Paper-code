using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using ESRI.ArcGIS.Controls;
using System.IO;
 



using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;

using ESRI.ArcGIS.DataSourcesFile;
using System.IO;
namespace VIDEO
{
    public partial class Form1 : Form
    {
        public IPolygon pg;
        public IMapControl2 pMc;
        public Form1()
        {
            InitializeComponent();
           // this.WindowState = FormWindowState.Maximized;
        }
       public string strVideoDir;
        private void button1_Click(object sender, EventArgs e)
        {
            //删除目录下所有文件
            //视频地图目录
           
            strVideoDir = textBox7.Text+"VMap\\";

            if (System.IO.Directory.Exists(strVideoDir))
              System.IO.Directory.Delete(strVideoDir,true);
            System.IO.Directory.CreateDirectory(strVideoDir);
            //CalOutputpng( ); 
            CalOutputjpg();
        }
        private bool inPolygon(double x, double y,IPolygon pg)
        {
            IPoint pt = new PointClass();
            pt.PutCoords(x, y);
            IRelationalOperator pRel = pg as IRelationalOperator;
            if (pRel.Contains(pt) == true)
                return true;
            else
              return false;
        }

        public static bool IsInPolygon2(IPoint checkPoint, List<IPoint> polygonPoints)
        {
            int counter = 0;
            int i;
            double xinters;
            IPoint p1, p2;
            int pointCount = polygonPoints.Count;
            p1 = polygonPoints[0];
            for (i = 1; i <= pointCount; i++)
            {
                p2 = polygonPoints[i % pointCount];
                if (checkPoint.X > Math.Min(p1.X, p2.X)//校验点的Y大于线段端点的最小Y
                    && checkPoint.X <= Math.Max(p1.X, p2.X))//校验点的Y小于线段端点的最大Y
                {
                    if (checkPoint.Y <= Math.Max(p1.Y, p2.Y))//校验点的X小于等线段端点的最大X(使用校验点的左射线判断).
                    {
                        if (p1.X != p2.X)//线段不平行于X轴
                        {
                            xinters = (checkPoint.X - p1.X) * (p2.Y - p1.Y) / (p2.X - p1.X) + p1.Y;
                            if (p1.Y == p2.Y || checkPoint.Y <= xinters)
                            {
                                counter++;
                            }
                        }
                    }

                }
                p1 = p2;
            }

            if (counter % 2 == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void CalOutputpng( )
        {
            string LBottomlon = textBox1.Text;
            double Lbtmlon = double.Parse(LBottomlon);//左下角经度

            string LBottomlat = textBox4.Text;
            double Lbtmlat = double.Parse(LBottomlat);//左下角纬度

            string RUplon = textBox2.Text;
            double RdbUplon = double.Parse(RUplon);//右上角经度

            string RUplat = textBox5.Text;
            double RdbUplat = double.Parse(RUplat);//右上角纬度

            string strResolution = textBox9.Text;
            double dbResolution = double.Parse(strResolution);//分辨率


            double r = (RdbUplat - Lbtmlat) / dbResolution;
            double c = (RdbUplon - Lbtmlon) / dbResolution;
            int row = (int)r;
            int col = (int)c;//行、列数

            double[,] geoxy = new double[row * col, 2];
            bool[] geoxybool = new bool[row * col];
            int n = 0;

             
            IPointCollection pts=pg as IPointCollection;
            List<IPoint> polygonPoints = new List<IPoint>();
            for (int i = 0; i < pts.PointCount; i++)
            {
                polygonPoints.Add(pts.get_Point(i));
            }

            IPoint pt = new PointClass();
            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {
                    double ll=Lbtmlon + (0.5 + j) * dbResolution;
                    double la=Lbtmlat + (0.5 + i) * dbResolution;

                     
                   
                    pt.PutCoords(ll,la);

                    if (IsInPolygon2(pt, polygonPoints))
                    {
                        geoxy[n, 0] = Lbtmlon + (0.5 + j) * dbResolution;
                        geoxy[n, 1] = Lbtmlat + (0.5 + i) * dbResolution;
                       
                        geoxybool[n] = true;
                        n++;
                    }
                    else
                    {
                        geoxy[n, 0] = Lbtmlon + (0.5 + j) * dbResolution;
                        geoxy[n, 1] = Lbtmlat + (0.5 + i) * dbResolution;

                        geoxybool[n] = false;
                        n++;
                    }
                }

        //    double[,] m = {
        //    {
        //        0.0101883693380459
        //        ,0.575463139519265
        //        ,114.031117174023 },
        //      {
        //        0.00287061530468817
        //        ,0.162166996887854
        //        ,32.1370285759963 }
        //        , {
        //        8.93261740125187E-05
        //        ,0.00504649193189877
        //        ,1} 
        //};

 




            double[,] mH = new double[3, 3];
            string[] strTmph = textBox8.Text.Split(';');
            strTmph[0] = strTmph[0].Substring(0);
            strTmph[1] = strTmph[1].Substring(0);
            strTmph[2] = strTmph[2].Substring(0);
            //逐个赋值
            string[] str0 = strTmph[0].Split(',');
            mH[0, 0] = double.Parse(str0[0]); mH[0, 1] = double.Parse(str0[1]); ; mH[0, 2] = double.Parse(str0[2]);
            string[] str1 = strTmph[1].Split(',');
            mH[1, 0] = double.Parse(str1[0]); mH[1, 1] = double.Parse(str1[1]); mH[1, 2] = double.Parse(str1[2]);
            string[] str2 = strTmph[2].Split(',');
            mH[2, 0] = double.Parse(str2[0]); mH[2, 1] = double.Parse(str2[1]); mH[2, 2] = double.Parse(str2[2]); 

            double[,] mInverst = new double[row * col, 2];
            mInverst = GeoFenceSys.VideoMap.CMatirxCal.MatInver(mH);


            double[,] geopix = GeoFenceSys.VideoMap.CMatirxCal.GetPixCoors(geoxy, mInverst);

            //针对每一幅图像进行处理
            DirectoryInfo di = new DirectoryInfo(textBox7.Text);
            FileInfo[] strFiles = di.GetFiles("*.jpg");
            List<Bgra> lstPix = new List<Bgra>();
           for (int ii = 0; ii < strFiles.Length; ii++)
           {
               progressBar1.Value =(int)( ii*1.0 / strFiles.Length * 100);
               Mat imgage = CvInvoke.Imread(strFiles[ii].FullName, ImreadModes.Unchanged);
               double[,] pix = new double[row * col, 2];
               Image<Bgra, Byte> img = new Image<Bgra, Byte>(col, row);
               Image<Bgra, byte> img1 = new Image<Bgra, byte>(imgage.Bitmap);
               
               lstPix.Clear();
             
               for (int i = 0; i < row * col; i++)
               {
                   int pixi = (int)geopix[i, 0];
                   int pixj = (int)geopix[i, 1];

                   Bgra tmp1;
                   if (pixi < 0 || pixj < 0 || pixi >= img1.Width || pixj >= img1.Height||geoxybool[i]==false)
                   {
                       tmp1 = new Bgra(0, 0, 0, 0);
                   }
                   else
                       tmp1 = img1[pixj, pixi];

                   lstPix.Add(tmp1);

               }

               for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                  {
                    Bgra temp = lstPix[row * col - (i + 1) * col + j];
                    img[i, j] = temp;
                    //geoxy[] = temp;
                   }

               imageBox2.Image = img;//在ImageBox2控件中显示图像
               string strtmpfilename = System.IO.Path.GetFileNameWithoutExtension(strFiles[ii].FullName);
               img.Save(  strVideoDir  + strtmpfilename + ".png");
               ///-----------------------------------------------------
              // imageBox2.Image.Save(@"E:\研发原型\视频投射地图图像\0908视频地图\" + System.IO.Path.GetFileNameWithoutExtension(strFiles[ii].FullName) + ".png");
               //同时保存pwg
               StreamWriter swWriteFile = File.CreateText(strVideoDir + System.IO.Path.GetFileNameWithoutExtension(strFiles[ii].FullName) + ".pgw");
               double lon1 = double.Parse(textBox1.Text);
               double lon2 = double.Parse(textBox2.Text);
               double londd = lon2 - lon1;
               int w = imageBox2.Image.Size.Width;
               double xx = londd / w * 1.0;

               swWriteFile.WriteLine(xx.ToString());  //x方向的分辨率
               swWriteFile.WriteLine("0.0000000000");
               swWriteFile.WriteLine("0.0000000000");

               double lat1 = double.Parse(textBox4.Text);
               double lat2 = double.Parse(textBox5.Text);
               double latdd = lat1 - lat2;
               int h = imageBox2.Image.Size.Height;
               double yy = latdd / h * 1.0;
               swWriteFile.WriteLine(yy.ToString());//y方向的分辨率

               double leftlon, leftlat;
               leftlon = double.Parse(textBox1.Text);
               leftlat = double.Parse(textBox5.Text);
               swWriteFile.WriteLine(leftlon.ToString());//左上角
               swWriteFile.WriteLine(leftlat.ToString());

               swWriteFile.Close();

               Application.DoEvents();
               System.Threading.Thread.Sleep(100);
               
               img.Dispose();
               img = null;
               img1.Dispose();
               img1 = null;
               swWriteFile.Dispose();
               swWriteFile = null;
               imageBox2.Image.Dispose();
               imageBox2.Image = null;
           }
           progressBar1.Value = 100;


        }

        private void CalOutputjpg()
        {
            string LBottomlon = textBox1.Text;
            double Lbtmlon = double.Parse(LBottomlon);//左下角经度

            string LBottomlat = textBox4.Text;
            double Lbtmlat = double.Parse(LBottomlat);//左下角纬度

            string RUplon = textBox2.Text;
            double RdbUplon = double.Parse(RUplon);//右上角经度

            string RUplat = textBox5.Text;
            double RdbUplat = double.Parse(RUplat);//右上角纬度

            string strResolution = textBox9.Text;
            double dbResolution = double.Parse(strResolution);//分辨率


            double r = (RdbUplat - Lbtmlat) / dbResolution;
            double c = (RdbUplon - Lbtmlon) / dbResolution;
            int row = (int)r;
            int col = (int)c;//行、列数

            double[,] geoxy = new double[row * col, 2];
            bool[] geoxybool = new bool[row * col];
            int n = 0;


            IPointCollection pts = pg as IPointCollection;
            List<IPoint> polygonPoints = new List<IPoint>();
            for (int i = 0; i < pts.PointCount; i++)
            {
                polygonPoints.Add(pts.get_Point(i));
            }

            IPoint pt = new PointClass();
            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {
                    double ll = Lbtmlon + (0.5 + j) * dbResolution;
                    double la = Lbtmlat + (0.5 + i) * dbResolution;



                    pt.PutCoords(ll, la);

                    if (IsInPolygon2(pt, polygonPoints))
                    {
                        geoxy[n, 0] = Lbtmlon + (0.5 + j) * dbResolution;
                        geoxy[n, 1] = Lbtmlat + (0.5 + i) * dbResolution;

                        geoxybool[n] = true;
                        n++;
                    }
                    else
                    {
                        geoxy[n, 0] = Lbtmlon + (0.5 + j) * dbResolution;
                        geoxy[n, 1] = Lbtmlat + (0.5 + i) * dbResolution;

                        geoxybool[n] = false;
                        n++;
                    }
                }

            //    double[,] m = {
            //    {
            //        0.0101883693380459
            //        ,0.575463139519265
            //        ,114.031117174023 },
            //      {
            //        0.00287061530468817
            //        ,0.162166996887854
            //        ,32.1370285759963 }
            //        , {
            //        8.93261740125187E-05
            //        ,0.00504649193189877
            //        ,1} 
            //};






            double[,] mH = new double[3, 3];
            string[] strTmph = textBox8.Text.Split(';');
            strTmph[0] = strTmph[0].Substring(0);
            strTmph[1] = strTmph[1].Substring(0);
            strTmph[2] = strTmph[2].Substring(0);
            //逐个赋值
            string[] str0 = strTmph[0].Split(',');
            mH[0, 0] = double.Parse(str0[0]); mH[0, 1] = double.Parse(str0[1]); ; mH[0, 2] = double.Parse(str0[2]);
            string[] str1 = strTmph[1].Split(',');
            mH[1, 0] = double.Parse(str1[0]); mH[1, 1] = double.Parse(str1[1]); mH[1, 2] = double.Parse(str1[2]);
            string[] str2 = strTmph[2].Split(',');
            mH[2, 0] = double.Parse(str2[0]); mH[2, 1] = double.Parse(str2[1]); mH[2, 2] = double.Parse(str2[2]);

            double[,] mInverst = new double[row * col, 2];
            mInverst = GeoFenceSys.VideoMap.CMatirxCal.MatInver(mH);


            double[,] geopix = GeoFenceSys.VideoMap.CMatirxCal.GetPixCoors(geoxy, mInverst);

            //针对每一幅图像进行处理
            DirectoryInfo di = new DirectoryInfo(textBox7.Text);
            FileInfo[] strFiles = di.GetFiles("*.jpg");
            List<Bgr> lstPix = new List<Bgr>();
            for (int ii = 0; ii < strFiles.Length; ii++)
            {
                progressBar1.Value = (int)(ii * 1.0 / strFiles.Length * 100);
                Mat imgage = CvInvoke.Imread(strFiles[ii].FullName, ImreadModes.Unchanged);
                double[,] pix = new double[row * col, 2];
                Image<Bgr, Byte> img = new Image<Bgr, Byte>(col, row);
                Image<Bgr, Byte> img1 = new Image<Bgr, Byte>(imgage.Bitmap);

                lstPix.Clear();

                for (int i = 0; i < row * col; i++)
                {
                    int pixi = (int)geopix[i, 0];
                    int pixj = (int)geopix[i, 1];

                    Bgr tmp1;
                    if (pixi < 0 || pixj < 0 || pixi >= img1.Width || pixj >= img1.Height || geoxybool[i] == false)
                    {
                        tmp1 = new Bgr(0, 0, 0);
                    }
                    else
                        tmp1 = img1[pixj, pixi];

                    lstPix.Add(tmp1);

                }

                for (int i = 0; i < row; i++)
                    for (int j = 0; j < col; j++)
                    {
                        Bgr temp = lstPix[row * col - (i + 1) * col + j];
                        img[i, j] = temp;
                        //geoxy[] = temp;
                    }

                imageBox2.Image = img;//在ImageBox2控件中显示图像
                string strtmpfilename = System.IO.Path.GetFileNameWithoutExtension(strFiles[ii].FullName);
                img.Save(strVideoDir + strtmpfilename + ".jpg");
                ///-----------------------------------------------------
                // imageBox2.Image.Save(@"E:\研发原型\视频投射地图图像\0908视频地图\" + System.IO.Path.GetFileNameWithoutExtension(strFiles[ii].FullName) + ".png");
                //同时保存pwg
                StreamWriter swWriteFile = File.CreateText(strVideoDir + System.IO.Path.GetFileNameWithoutExtension(strFiles[ii].FullName) + ".jgw");
                double lon1 = double.Parse(textBox1.Text);
                double lon2 = double.Parse(textBox2.Text);
                double londd = lon2 - lon1;
                int w = imageBox2.Image.Size.Width;
                double xx = londd / w * 1.0;

                swWriteFile.WriteLine(xx.ToString());  //x方向的分辨率
                swWriteFile.WriteLine("0.0000000000");
                swWriteFile.WriteLine("0.0000000000");

                double lat1 = double.Parse(textBox4.Text);
                double lat2 = double.Parse(textBox5.Text);
                double latdd = lat1 - lat2;
                int h = imageBox2.Image.Size.Height;
                double yy = latdd / h * 1.0;
                swWriteFile.WriteLine(yy.ToString());//y方向的分辨率

                double leftlon, leftlat;
                leftlon = double.Parse(textBox1.Text);
                leftlat = double.Parse(textBox5.Text);
                swWriteFile.WriteLine(leftlon.ToString());//左上角
                swWriteFile.WriteLine(leftlat.ToString());

                swWriteFile.Close();

                Application.DoEvents();
                System.Threading.Thread.Sleep(100);

                img.Dispose();
                img = null;
                img1.Dispose();
                img1 = null;
                swWriteFile.Dispose();
                swWriteFile = null;
                imageBox2.Image.Dispose();
                imageBox2.Image = null;
            }
            progressBar1.Value = 100;


        }
        private void SavePng()
        {
            imageBox2.Image.Save("C:\\testvideo\\zxg1.png");
            //同时保存pwg
            StreamWriter swWriteFile = File.CreateText("C:\\testvideo\\zxg1.pgw");
            double lon1 = double.Parse(textBox1.Text);
            double lon2 = double.Parse(textBox2.Text);
            double londd = lon2 - lon1;
            int w = imageBox2.Image.Size.Width;
            double xx = londd / w * 1.0;

            swWriteFile.WriteLine(xx.ToString());  //x方向的分辨率
            swWriteFile.WriteLine("0.0000000000");
            swWriteFile.WriteLine("0.0000000000");

            double lat1 = double.Parse(textBox4.Text);
            double lat2 = double.Parse(textBox5.Text);
            double latdd = lat1 - lat2;
            int h = imageBox2.Image.Size.Height;
            double yy = latdd / h * 1.0;
            swWriteFile.WriteLine(yy.ToString());//y方向的分辨率

            double leftlon, leftlat;
            leftlon = double.Parse(textBox1.Text);
            leftlat = double.Parse(textBox5.Text);
            swWriteFile.WriteLine(leftlon.ToString());//左上角
            swWriteFile.WriteLine(leftlat.ToString());

            swWriteFile.Close();
        }



private void button2_Click(object sender, EventArgs e)
        {
            double[,] m = {
       {  0.0426698374472059
         , 0.728937470968091
         ,114.030405597979 },
      { 0.0120240776355513
        ,0.205416148353671
        ,32.136940543624 }
        , { 0.000374163903906221
        ,0.0063923748654382
        ,1.0 }
    };
            double[,] mInverst = new double[3, 3];
            mInverst = GeoFenceSys.VideoMap.CMatirxCal.MatInver(m);

            double[,] geoxy ={{114.0316,32.1364},{114.0318,32.1364 },{114.032,32.1364 }
                          ,{114.0316,32.1362},{114.0318,32.1362},{114.032,32.1362}
                          ,{114.0316,32.1361},{114.0318,32.1361 },{114.032,32.1361 }};
            double[,] geopix = GeoFenceSys.VideoMap.CMatirxCal.GetPixCoors(geoxy, mInverst);
            //Mat img0 = CvInvoke.Imread("d:\\test.jpg", LoadImageType.Unchanged);
            Mat img0 = CvInvoke.Imread("d:\\test.jpg", ImreadModes.Unchanged);
            double[,] pix = new double[3, 3];
            //for(int q=0; q<img0.Height; q++)
            //    for (int p = 0; p < img0.Width; p++)
            //    {
            //}
            Image<Bgra, Byte> img = new Image<Bgra, Byte>(3, 3);
            Image<Bgra, byte> img1 = new Image<Bgra, byte>(img0.Bitmap);


            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    int pixi = (int)geopix[i * 3 + j, 0];
                    int pixj = (int)geopix[i * 3 + j, 1];

                    Bgra tmp1 = img1[pixi, pixj];
                    //Bgra tmp = new Bgra(255, 0, 0, 255);
                    img[i, j] = tmp1;

                }
            //imageBox1.Image = img;//在ImageBox1控件中显示图像

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void imageBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

            string LBottomlon = textBox1.Text;
            double Lbtmlon = double.Parse(LBottomlon);

            string LBottomlat = textBox4.Text;
            double Lbtmlat = double.Parse(LBottomlat);

            string RUplon = textBox2.Text;
            double RdbUplon = double.Parse(RUplon);

            string RUplat = textBox5.Text;
            double RdbUplat = double.Parse(RUplat);

            string strResolution = textBox9.Text;
            double dbResolution = double.Parse(strResolution);


            double r = (RdbUplat - Lbtmlat) / dbResolution;
           double c = (RdbUplon - Lbtmlon) / dbResolution;
            int row = (int)r;
            int col = (int)c;
            textBox3.Text = row.ToString();
            textBox6.Text = col.ToString();

            double R = ((RdbUplon - Lbtmlon) / col);
            double C = ((RdbUplat - Lbtmlat) / row);

            double[,] lonMat = new double[row, col];
            double[,] latMat = new double[row, col];



            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                {
                    lonMat[i, j] = Lbtmlon + (0.5 + j) * dbResolution;
                    latMat[i, j] = Lbtmlat + (0.5 + i) * dbResolution;
                  //  textBox7.Text = textBox7.Text +/*"{"+*/ lonMat[i, j].ToString() + "," + latMat[i, j].ToString()/*+*//*"},"*/;
                   
                }
             
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
             //splitContainer1.SplitterDistance = 150;
            button3_Click(sender,e);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            imageBox2.Image.Save("C:\\testvideo\\zxg1.png");
            //同时保存pwg
            StreamWriter swWriteFile = File.CreateText("C:\\testvideo\\zxg1.pgw");
            double lon1 = double.Parse(textBox1.Text);
            double lon2 = double.Parse(textBox2.Text);
            double londd = lon2 - lon1;
            int w=imageBox2.Image.Size.Width;
            double xx = londd / w * 1.0;

            swWriteFile.WriteLine(xx.ToString());  //x方向的分辨率
            swWriteFile.WriteLine("0.0000000000");
            swWriteFile.WriteLine("0.0000000000");

            double lat1 = double.Parse(textBox4.Text);
            double lat2 = double.Parse(textBox5.Text);
            double latdd = lat1 - lat2;
            int h = imageBox2.Image.Size.Height;
            double yy = latdd / h * 1.0;
            swWriteFile.WriteLine(yy.ToString());//y方向的分辨率

            double leftlon, leftlat;
            leftlon =double.Parse( textBox1.Text);
            leftlat = double.Parse(textBox5.Text);
            swWriteFile.WriteLine(leftlon.ToString());//左上角
            swWriteFile.WriteLine(leftlat.ToString());

            swWriteFile.Close();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            try
            {
                splitContainer1.SplitterDistance = 200;
            }
            catch (Exception ex)
            {
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = @"E:\研发原型\视频投射地图图像\0908操场视频帧";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBox7.Text = dlg.SelectedPath;

                //同时读取单应矩阵文本文件
                string[] files = Directory.GetFiles(dlg.SelectedPath, "*.txt");
                StreamReader sr = new System.IO.StreamReader(files[0]);
                string str=sr.ReadToEnd();
                textBox8.Text = str;
                sr.Close();
            }
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            //逐帧加载播放
            //E:\研发原型\视频投射地图图像\0908视频地图
            DirectoryInfo di = new DirectoryInfo(textBox7.Text + "VMap\\");
            FileInfo[] strFiles = di.GetFiles("*.jpg");


            IWorkspaceFactory xjRasterWsF = new RasterWorkspaceFactory();
            for (int k = pMc.LayerCount - 2; k > 0; k--)
            {

                pMc.DeleteLayer(k);
            }

            for (int ii = 0; ii < strFiles.Length; ii++)
            {

                //if (axMapControl1.LayerCount > 1)
                //    button4_Click(sender, e);
                for (int k = 0; k < pMc.LayerCount; k++)
                {
                    if (k != 0 && k != pMc.LayerCount - 1)
                    {
                        pMc.get_Layer(k).Visible = false;
                        pMc.DeleteLayer(k);
                    }
                }

                string xjRasterPath = strFiles[ii].FullName;
                string xjRasterFolder = System.IO.Path.GetDirectoryName(xjRasterPath);
                string xjRasterFileName = System.IO.Path.GetFileName(xjRasterPath);
                //工作空间（实例化）

                IWorkspace xjWs = xjRasterWsF.OpenFromFile(xjRasterFolder, 0);

                IRasterWorkspace xjRasterWs = xjWs as IRasterWorkspace;//强制转换
                IRasterDataset xjRasterDS = xjRasterWs.OpenRasterDataset(xjRasterFileName);

                //新建栅格图层
                IRasterLayer xjRasterLayer = new RasterLayer();//引用Carto

                xjRasterLayer.CreateFromRaster(xjRasterDS.CreateDefaultRaster());

                //加载显示

                IRasterRenderer RR = xjRasterLayer.Renderer;
                IRasterRGBRenderer rasterRGBRenderer = null;
                if (RR is IRasterRGBRenderer)
                    rasterRGBRenderer = RR as IRasterRGBRenderer;
                RR.Update();
                rasterRGBRenderer.RedBandIndex = 0;
                rasterRGBRenderer.GreenBandIndex = 1;
                rasterRGBRenderer.BlueBandIndex = 2;
                IRgbColor rgb = new RgbColorClass();
                rgb.NullColor = true;
                IRasterStretch2 RS = rasterRGBRenderer as IRasterStretch2; // IRasterStretch无法设置数组，需用2 
                double[] background = 
                { 
                0, 0, 0 
                };
                RS.Background = true;
                RS.BackgroundValue = background as object;
                RS.BackgroundColor = rgb as ESRI.ArcGIS.Display.IColor;



                pMc.AddLayer(xjRasterLayer);
                //this.axMapControl1.ActiveView.Refresh();
                IViewRefresh viewRefresh = pMc.Map as IViewRefresh;
                viewRefresh.ProgressiveDrawing = true;
                viewRefresh.RefreshItem(xjRasterLayer);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(xjRasterWs);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xjRasterDS);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xjRasterLayer);

                xjRasterWs = null;
                xjRasterDS = null;
                xjRasterLayer = null;




                Application.DoEvents();

                System.Threading.Thread.Sleep(10);
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(xjRasterWsF);
            xjRasterWsF = null;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(textBox7.Text + "VMap\\");
            FileInfo[] strFiles = di.GetFiles("*.jpg");
            List<Bgra> lstPix = new List<Bgra>();
            for (int ii = 0; ii < strFiles.Length; ii++)
            {
                Mat imgage = CvInvoke.Imread(strFiles[ii].FullName, ImreadModes.Unchanged);
                imageBox2.Image = imgage;

                System.Threading.Thread.Sleep(300);
                Application.DoEvents();
                imageBox2.Image.Dispose();
                imageBox2.Image = null;
            }
        }

        
    }
}

