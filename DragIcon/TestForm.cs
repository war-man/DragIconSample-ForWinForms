using System;
using System.Windows.Forms;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WinForms;

namespace DraggingIcon
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            winformsMap1.MapUnit = GeographyUnit.DecimalDegree;
            winformsMap1.CurrentExtent = new RectangleShape(-97.7591, 30.3126, -97.7317, 30.2964);
            winformsMap1.BackgroundOverlay.BackgroundBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 198, 255, 255));

            //Displays the World Map Kit as a background.
            ThinkGeo.MapSuite.WinForms.WorldMapKitWmsDesktopOverlay worldMapKitDesktopOverlay = new ThinkGeo.MapSuite.WinForms.WorldMapKitWmsDesktopOverlay();
            winformsMap1.Overlays.Add(worldMapKitDesktopOverlay);

            //EditInteractiveOverlay used because it already have the logic for dragging.
            EditInteractiveOverlay editInteractiveOverlay = new EditInteractiveOverlay();

            //Sets the property IsActive for DragControlPointsLayer to false so that the control point (as four arrows) is not visible.
            editInteractiveOverlay.DragControlPointsLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.IsActive = false;
            editInteractiveOverlay.DragControlPointsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            //Sets the property IsActive for all the Styles of EditShapesLayer because we are using a ValueStyle instead.
            editInteractiveOverlay.EditShapesLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.IsActive = false; 
            editInteractiveOverlay.EditShapesLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.IsActive = false; 
            editInteractiveOverlay.EditShapesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.IsActive = false; 

            //ValueStyle used for displaying the feature according to the value of the column "Type" for displaying with a bus or car icon.
            ValueStyle valueStyle = new ValueStyle();
            valueStyle.ColumnName = "Type";

            PointStyle carPointStyle = new PointStyle(new GeoImage("../../data/car_normal.png"));
            carPointStyle.PointType = PointType.Bitmap;
            PointStyle busPointStyle = new PointStyle(new GeoImage("../../data/bus_normal.png"));
            busPointStyle.PointType = PointType.Bitmap;

            valueStyle.ValueItems.Add(new ValueItem("Car", carPointStyle));
            valueStyle.ValueItems.Add(new ValueItem("Bus", busPointStyle));

            editInteractiveOverlay.EditShapesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(valueStyle);
            editInteractiveOverlay.EditShapesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            editInteractiveOverlay.EditShapesLayer.Open();
            editInteractiveOverlay.EditShapesLayer.Columns.Add(new FeatureSourceColumn("Type"));
            editInteractiveOverlay.EditShapesLayer.Close();

            Feature carFeature = new Feature(new PointShape(-97.7507, 30.3092));
            carFeature.ColumnValues["Type"] = "Car";

            Feature busFeature = new Feature(new PointShape(-97.7428, 30.3053));
            busFeature.ColumnValues["Type"] = "Bus";

            editInteractiveOverlay.EditShapesLayer.InternalFeatures.Add("Car", carFeature);
            editInteractiveOverlay.EditShapesLayer.InternalFeatures.Add("Bus", busFeature);

            //Sets the properties of EditInteractiveOverlay to have the appropriate behavior.
            //Make sure CanDrag is set to true.
            editInteractiveOverlay.CanAddVertex = false;
            editInteractiveOverlay.CanDrag = true;
            editInteractiveOverlay.CanRemoveVertex = false;
            editInteractiveOverlay.CanResize = false;
            editInteractiveOverlay.CanRotate = false;
            editInteractiveOverlay.CalculateAllControlPoints();

            winformsMap1.EditOverlay = editInteractiveOverlay;

            winformsMap1.Refresh();
        }
      
        private void winformsMap1_MouseMove(object sender, MouseEventArgs e)
        {
            //Displays the X and Y in screen coordinates.
            statusStrip1.Items["toolStripStatusLabelScreen"].Text = "X:" + e.X + " Y:" + e.Y;

            //Gets the PointShape in world coordinates from screen coordinates.
            PointShape pointShape = ExtentHelper.ToWorldCoordinate(winformsMap1.CurrentExtent, new ScreenPointF(e.X, e.Y), winformsMap1.Width, winformsMap1.Height);

            //Displays world coordinates.
            statusStrip1.Items["toolStripStatusLabelWorld"].Text = "(world) X:" + Math.Round(pointShape.X, 4) + " Y:" + Math.Round(pointShape.Y, 4);
        }
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
