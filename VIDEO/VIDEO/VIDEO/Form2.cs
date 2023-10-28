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
            IGeometry pGe = axMapControl1.TrackPolygon();
            IElement pElement = new PolygonElement() as IElement;

            pElement.Geometry = pGe;
            IGraphicsContainer GraphicsContainer = axMapControl1.Map as IGraphicsContainer;

            //IPoint pPl = pGe as IPoint;
            IPointCollection pTcol = pGe as IPointCollection;
            //获取面元素的点集
            List<double> lstX = new List<double>();
            List<double> lstY = new List<double>();
            string strPts = "";
            for (int i = 0; i < pTcol.PointCount - 1; i++)
            {
                IPoint Ptmp = pTcol.Point[i] as IPoint;
                double X = Ptmp.X;
                double Y = Ptmp.Y;


                strPts = PRJtoGCS(X, Y);

                string[] sArray = strPts.Split(',');


                lstX.Add(double.Parse(sArray[0]));
                lstY.Add(double.Parse(sArray[1]));

                textBox1.Text = lstX.Min().ToString();
                textBox2.Text = lstY.Min().ToString();
                textBox3.Text = lstX.Max().ToString();
                textBox4.Text = lstY.Max().ToString();



            }
            //MessageBox.Show(strPts);



            GraphicsContainer.AddElement(pElement, 0);

          

            axMapControl1.ActiveView.Refresh();

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
            string MxdPath = @"d:\\xymap.mxd";
            axMapControl1.LoadMxFile(MxdPath);
            axMapControl1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 frmVideo = new Form1();
            frmVideo.Show();//点击按钮时显示Form1页面

            frmVideo.textBox1.Text = this.textBox1.Text;
            frmVideo.textBox4.Text = this.textBox2.Text;
            frmVideo.textBox2.Text = this.textBox3.Text;
            frmVideo.textBox5.Text = this.textBox4.Text;
            //获取form2中经纬度的最值
        }
    }
}
