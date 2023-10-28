using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void axMapControl1_OnMouseDown(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        {
            //IGeometry pGe = axMapControl1.TrackPolygon();
            //IElement pElement = new PolygonElement() as IElement;

            //pElement.Geometry = pGe;
            //IGraphicsContainer GraphicsContainer = axMapControl1.Map as IGraphicsContainer;

            ////IPoint pPl = pGe as IPoint;
            //IPointCollection pTcol = pGe as IPointCollection;
            ////获取面元素的点集
            //List<double> lstX = new List<double>();
            //List<double> lstY = new List<double>();
            //string strPts = "";
            //for (int i = 0; i < pTcol.PointCount - 1; i++)
            //{
            //    IPoint Ptmp = pTcol.Point[i] as IPoint;
            //    double X = Ptmp.X;
            //    double Y = Ptmp.Y;


            //    strPts = PRJtoGCS(X, Y);

            //    string[] sArray = strPts.Split(',');


            //    lstX.Add(double.Parse(sArray[0]));
            //    lstY.Add(double.Parse(sArray[1]));

            //    textBox1.Text = lstX.Min().ToString();
            //    textBox2.Text = lstY.Min().ToString();
            //    textBox3.Text = lstX.Max().ToString();
            //    textBox4.Text = lstY.Max().ToString();



            //}
            ////MessageBox.Show(strPts);



            //GraphicsContainer.AddElement(pElement, 0);

          

            //axMapControl1.ActiveView.Refresh();

        }
        private string PRJtoGCS(double x, double y)
        {
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(x, y);
            ISpatialReferenceFactory pSRF = new SpatialReferenceEnvironmentClass();

            pPoint.SpatialReference = axMapControl1.Map.SpatialReference;//获取axmapControl中地图的坐标系
            pPoint.Project(pSRF.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984));
            string pPts = pPoint.X.ToString() + "," + pPoint.Y.ToString() + " ";

            //double pPtsX = pPoint.X;
            //double pPtsY = pPoint.Y;
            return pPts;

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string MxdPath =Application.StartupPath+ @"\xymap.mxd";
            axMapControl1.LoadMxFile(MxdPath);
            axMapControl1.Refresh();

            tool.tlDrawRange tlDrawRange = new tool.tlDrawRange();
            tlDrawRange.pMc = axMapControl1.Object as IMapControl2;
            tlDrawRange.dlg = this;
            axToolbarControl1.AddItem(tlDrawRange);

            tool.cmdClearElement cmdClearEle = new tool.cmdClearElement();
            cmdClearEle.pMc = axMapControl1.Object as IMapControl2;
            axToolbarControl1.AddItem(cmdClearEle);
             
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IGraphicsContainer GraphicsContainer =axMapControl1.ActiveView as IGraphicsContainer;
            GraphicsContainer.Reset();
            IElement pEle= GraphicsContainer.Next();
            IPolygon pg=null;
            if (pEle != null)
                pg = pEle.Geometry as IPolygon;
            IPointCollection ptc = pg as IPointCollection;

            Form1 frmVideo = new Form1();
            frmVideo.pg = pg;
            frmVideo.pMc = axMapControl1.Object as IMapControl2;
            frmVideo.Show();//点击按钮时显示Form1页面

            frmVideo.textBox1.Text = this.textBox1.Text;
            frmVideo.textBox4.Text = this.textBox2.Text;
            frmVideo.textBox2.Text = this.textBox3.Text;
            frmVideo.textBox5.Text = this.textBox4.Text;
            //获取form2中经纬度的最值
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ////逐帧加载播放
            ////E:\研发原型\视频投射地图图像\0908视频地图
            //DirectoryInfo di = new DirectoryInfo(frmVideo.strVideoDir);
            // FileInfo[] strFiles = di.GetFiles("*.png");


            // IWorkspaceFactory xjRasterWsF = new RasterWorkspaceFactory();
            // for (int k = axMapControl1.LayerCount-2; k >0; k--)
            // {
                  
            //         axMapControl1.DeleteLayer(k);
            // } 

            // for (int ii = 0; ii < strFiles.Length; ii++)
            // {

            //     //if (axMapControl1.LayerCount > 1)
            //     //    button4_Click(sender, e);
            //     for (int k = 0; k < axMapControl1.LayerCount; k++)
            //     {
            //         if (k != 0 && k != axMapControl1.LayerCount - 1)
            //         {
            //             axMapControl1.get_Layer(k).Visible = false;
            //             axMapControl1.DeleteLayer(k);
            //         }
            //     } 
                  
            //     string xjRasterPath = strFiles[ii].FullName;
            //     string xjRasterFolder = System.IO.Path.GetDirectoryName(xjRasterPath);
            //     string xjRasterFileName = System.IO.Path.GetFileName(xjRasterPath);
            //     //工作空间（实例化）
                
            //     IWorkspace xjWs = xjRasterWsF.OpenFromFile(xjRasterFolder, 0); 

            //     IRasterWorkspace xjRasterWs = xjWs as IRasterWorkspace;//强制转换
            //     IRasterDataset xjRasterDS = xjRasterWs.OpenRasterDataset(xjRasterFileName);

            //     //新建栅格图层
            //     IRasterLayer xjRasterLayer = new RasterLayer();//引用Carto

            //     xjRasterLayer.CreateFromRaster(xjRasterDS.CreateDefaultRaster());

            //     //加载显示

            //     IRasterRenderer RR = xjRasterLayer.Renderer; 
            //    IRasterRGBRenderer rasterRGBRenderer = null; 
            //    if (RR is IRasterRGBRenderer) 
            //    rasterRGBRenderer = RR as IRasterRGBRenderer; 
            //    RR.Update(); 
            //    rasterRGBRenderer.RedBandIndex = 0; 
            //    rasterRGBRenderer.GreenBandIndex = 1; 
            //    rasterRGBRenderer.BlueBandIndex = 2; 
            //    IRgbColor rgb = new RgbColorClass(); 
            //    rgb.NullColor = true; 
            //    IRasterStretch2 RS = rasterRGBRenderer as IRasterStretch2; // IRasterStretch无法设置数组，需用2 
            //    double[] background = 
            //    { 
            //    0, 0, 0 
            //    }; 
            //    RS.Background = true; 
            //    RS.BackgroundValue = background as object; 
            //    RS.BackgroundColor = rgb as IColor; 
 


            //     this.axMapControl1.AddLayer(xjRasterLayer);
            //     //this.axMapControl1.ActiveView.Refresh();
            //     IViewRefresh viewRefresh = axMapControl1.Map as IViewRefresh;
            //     viewRefresh.ProgressiveDrawing = true;
            //     viewRefresh.RefreshItem(xjRasterLayer);

            //     System.Runtime.InteropServices.Marshal.ReleaseComObject(xjRasterWs);
            //     System.Runtime.InteropServices.Marshal.ReleaseComObject(xjRasterDS);
            //     System.Runtime.InteropServices.Marshal.ReleaseComObject(xjRasterLayer);
                
            //     xjRasterWs = null;
            //     xjRasterDS = null;
            //     xjRasterLayer = null;

                 


            //    Application.DoEvents(); 
                 
            //     System.Threading.Thread.Sleep(10);
            // }
             
            // System.Runtime.InteropServices.Marshal.ReleaseComObject(xjRasterWsF);
            // xjRasterWsF = null;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (axMapControl1.LayerCount == 2)
            {
                IDataLayer2 pdtLyr = axMapControl1.get_Layer(0) as IDataLayer2;
                pdtLyr.Disconnect();

                axMapControl1.DeleteLayer(0);

                pdtLyr = null;
                GC.Collect();
            }
            for (int i = axMapControl1.LayerCount-2; i >0 ; i--)
            {
                IDataLayer2 pdtLyr = axMapControl1.get_Layer(i) as IDataLayer2;
                pdtLyr.Disconnect();

                axMapControl1.DeleteLayer(i);

                pdtLyr = null;
                GC.Collect();
            }
        }
    }
}
