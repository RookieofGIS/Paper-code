using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using System.Collections.Generic;
 
using System.ComponentModel;
using System.Data;
 
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESRI.ArcGIS.Display;

 


namespace VIDEO.tool
{
    /// <summary>
    /// Summary description for tlDrawRange.
    /// </summary>
    [Guid("dfa778ae-1bb6-4272-858d-81ef4e7fd0b1")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("VIDEO.tool.tlDrawRange")]
    public sealed class tlDrawRange : BaseTool
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IHookHelper m_hookHelper;
        public IMapControl2 pMc;
        public Form2 dlg;
        public tlDrawRange()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = ""; //localizable text 
            base.m_caption = "";  //localizable text 
            base.m_message = "";  //localizable text
            base.m_toolTip = "";  //localizable text
            base.m_name = "";   //unique id, non-localizable (e.g. "MyCategory_MyTool")
            try
            {
                //
                // TODO: change resource name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;

            // TODO:  Add tlDrawRange.OnCreate implementation
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add tlDrawRange.OnClick implementation
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add tlDrawRange.OnMouseDown implementation
            IGeometry pGe = pMc.TrackPolygon();
           // IElement pElement = new PolygonElement() as IElement;
            mysymbol.PolygonElement pElement = new mysymbol.PolygonElement();
            pElement.Opacity = 50;
            pElement.Geometry = pGe;

            ISimpleFillSymbol pSimpleFillSymbol;
            pSimpleFillSymbol = new SimpleFillSymbol();
            pSimpleFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
            RgbColor fillColor = new RgbColor();
            fillColor.Red = 255;
            fillColor.Green = 255;
            fillColor.Blue = 0;
            pSimpleFillSymbol.Color = fillColor;

            ISimpleLineSymbol pSimpleLineSymbol;
            pSimpleLineSymbol = new SimpleLineSymbol();
            pSimpleLineSymbol.Width = 1;

            RgbColor lineColor = new RgbColor();
            lineColor.Red = 0;
            lineColor.Green = 0;
            lineColor.Blue = 255;

            pSimpleLineSymbol.Color = lineColor;
            pSimpleLineSymbol.Style = esriSimpleLineStyle.esriSLSDash;

            pSimpleFillSymbol.Outline = (ILineSymbol)pSimpleLineSymbol;

            pElement.Symbol = pSimpleFillSymbol;

            IGraphicsContainer GraphicsContainer = pMc.Map as IGraphicsContainer;
            GraphicsContainer.DeleteAllElements();
            //IPoint pPl = pGe as IPoint;
            IPointCollection pTcol = pGe as IPointCollection;
            //获取面元素的点集
            List<double> lstX = new List<double>();
            List<double> lstY = new List<double>();
            string strPts = "";
            for (int i = 0; i < pTcol.PointCount - 1; i++)
            {
                IPoint Ptmp = pTcol.Point[i] as IPoint;
                double XX = Ptmp.X;
                double YY = Ptmp.Y;


                strPts = PRJtoGCS(XX, YY);

                string[] sArray = strPts.Split(',');


                lstX.Add(double.Parse(sArray[0]));
                lstY.Add(double.Parse(sArray[1]));

                dlg.textBox1.Text = lstX.Min().ToString();
                dlg.textBox2.Text = lstY.Min().ToString();
                dlg.textBox3.Text = lstX.Max().ToString();
                dlg.textBox4.Text = lstY.Max().ToString();



            }
            //MessageBox.Show(strPts);



            GraphicsContainer.AddElement(pElement, 0);



            pMc.ActiveView.Refresh();
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add tlDrawRange.OnMouseMove implementation
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add tlDrawRange.OnMouseUp implementation
        }

        private string PRJtoGCS(double x, double y)
        {
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(x, y);
            ISpatialReferenceFactory pSRF = new SpatialReferenceEnvironmentClass();

            pPoint.SpatialReference = pMc.Map.SpatialReference;//获取axmapControl中地图的坐标系
            pPoint.Project(pSRF.CreateGeographicCoordinateSystem((int)esriSRGeoCSType.esriSRGeoCS_WGS1984));
            string pPts = pPoint.X.ToString() + "," + pPoint.Y.ToString() + " ";

            //double pPtsX = pPoint.X;
            //double pPtsY = pPoint.Y;
            return pPts;

        }
        #endregion
    }
}
