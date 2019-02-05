using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.sldworks;
using System.Linq;

namespace solid_macro
{
    [ProgId(BikeMacro.PROGID)]
    public partial class TaskpaneHostUI : UserControl
    {
        #region Public members

        public TaskpaneHostUI() => InitializeComponent();

        public SldWorks SWApp
        {
            set => _swApp = value;
            get => _swApp;
        }

        /// <summary>
        /// Handler for UI update events
        /// _switcher - must be set to true exactly before step, which involve UI update
        /// handler might be called after json-file will be written, so, don't use it on the last step
        /// </summary>
        /// <returns></returns>
        public int RepaintPostNotifyEventHandler()
        {
            if (_switcher)
            {
                AddJsonString(_steps[_counter++]);
                _switcher = false;
            }
            return 0;
        }

        #endregion

        #region Private members

        private void OpenAssembly()
        {
            int errors = 0;
            int warnings = 0;

            _model = (ModelDoc2)_swApp.OpenDoc6(
                "C:\\VAYU\\AMD - RADEON\\SKLOPNI_PINKY\\AMD_Bike_by_paX.SLDASM",
                (int)swDocumentTypes_e.swDocASSEMBLY,
                (int)swOpenDocOptions_e.swOpenDocOptions_Silent,
                "",
                errors,
                warnings);
        }

        private void CloseAssembly()
        {
            _swApp.CloseDoc("C:\\VAYU\\AMD - RADEON\\SKLOPNI_PINKY\\AMD_Bike_by_paX.SLDASM");
            _model = null;
            _modelView = null;
        }

        private void UpdateLabel(string s)
        {
            this.current_step_label.Text = s;
            this.current_step_label.Update();
        }

        //private void OpenAssembly_Click(object sender, System.EventArgs e)
        //{
        //    OpenAssembly();
        //    //this.execute_test_button.Enabled = true;
        //    //this.open_assembly_button.Enabled = false;
        //}

        private void ExecuteBikeTest()
        {
            _counter = 0;
            _switcher = false;
            _jsonRows = new List<string>();

            this.BikeProgressBar.Value = 0;
            UpdateLabel("Opening an assembly");

            OpenAssembly();

            UpdateLabel("Step 1 (rectangle)");

            AddString("{");

            _startTime = DateTime.Now;
            _previousTime = _startTime;
            // start of rectangle and cut
            // step - time between the previous and the next logged time starting from this point
            AddJsonStartEndString("Start");

            _model = (ModelDoc2)_swApp.ActiveDoc;

            _modelView = _model.ActiveView;
            _modelView.RepaintPostNotify += this.RepaintPostNotifyEventHandler;

            // Macros' start

            _model.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
            _model.SketchManager.InsertSketch(true);
            _model.ClearSelection2(true);

            _model.Extension.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchAddConstToRectEntity,
                (int)swUserPreferenceOption_e.swDetailingNoOptionSpecified, false);
            _model.Extension.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchAddConstLineDiagonalType,
                (int)swUserPreferenceOption_e.swDetailingNoOptionSpecified, true);
            _model.SketchManager.CreateCornerRectangle(-9.49321665096613E-02, 0.322788632046317, 0, 5.92322920318928E-02, 0.18259570352514, 0);

            _model.ClearSelection2(true);
            _model.SketchManager.InsertSketch(true);

            this.BikeProgressBar.Value = 2;
            AddJsonString("Step 1-2 (Rectangle)");
            UpdateLabel("Step 3 (Extruded cut)");

           //rename sketch
           _model.Extension.SelectByID2("Sketch", "SKETCH", 0, 0, 0, false, 0, null, 0);
            _model.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, "rectangle");

            // Named View
            _model.ShowNamedView2("*Trimetric", 8);
            _model.ViewZoomtofit2();
            _switcher = true;
            _model.FeatureManager.FeatureCut4(false, false, false, 9, 1, 0.01, 0.01, false, false, false, false,
                1.74532925199433E-02, 1.74532925199433E-02, false, false, false, false, false, true, true, true, true, false, 0, 0, false, false);
            _model.SelectionManager.EnableContourSelection = false;

            _model.Extension.SelectByID2("Cut-Extrude", "BODYFEATURE", 0, 0, 0, false, 0, null, 0);
            _model.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, "extrude1");
            _model.ClearSelection2(true);
            // end of rectangle's extruded cut - logged by ui handler

            // four circles
            _model.Extension.SelectByID2("Top Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
            _model.Extension.SelectByRay(-0.195096867060234, 0.373588736559441, 2.42894725591936E-02, 0, -1, 0, 7.97352354467751E-03, 2, false, 0, 0);
            _model.SketchManager.InsertSketch(true);
            _model.ClearSelection2(true);
            this.BikeProgressBar.Value = 6;
            UpdateLabel("Step 4-5 (four circles)");


            _model.SketchManager.CreateCircle(-0.182878, -0.022516, 0.0, -0.154241, -0.023788, 0.0);
            _model.ClearSelection2(true);
            _model.SketchManager.CreateCircle(-0.077304, 0.086443, 0.0, -0.056874, 0.06976, 0.0);
            _model.ClearSelection2(true);
            _model.SketchManager.CreateCircle(0.036674, -0.022516, 0.0, 0.065311, -0.025698, 0.0);
            _model.ClearSelection2(true);
            _model.SketchManager.CreateCircle(-0.076589, -0.108653, 0.0, -0.049238, -0.124973, 0.0);
            _model.ClearSelection2(true);
            _model.SketchManager.InsertSketch(true);
            this.BikeProgressBar.Value = 4;

            //rename sketch
            _model.Extension.SelectByID2("Sketch", "SKETCH", 0, 0, 0, false, 0, null, 0);
            _model.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, "circles");

            AddJsonString("Step 4-5 (four circles)");
            UpdateLabel("Step 6 (extrude cut)");
            // four circles end

            _switcher = true;
            _model.FeatureManager.FeatureCut4(false, false, false, 9, 1, 0.01, 0.01, false, false, false, false,
                1.74532925199433E-02, 1.74532925199433E-02, false, false, false, false, false, true, true, true, true, false, 0, 0, false, false);
            _model.SelectionManager.EnableContourSelection = false;

            _model.Extension.SelectByID2("Cut-Extrude", "BODYFEATURE", 0, 0, 0, false, 0, null, 0);
            _model.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, "extrude2");
            _model.ClearSelection2(true);
            this.BikeProgressBar.Value = 16;
            UpdateLabel("Step 7 (Freeform)");
            // end of four circles' extruding - logged by ui handler


            // step 7 - freeform
            _model.Extension.SelectByID2("REZERVOAR_pA_asembly-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, false, 0, null, 0);

            // Open
            // change of active doc to the fuel tank
            int errors = 0, warnings = 0;
            _model = _swApp.OpenDoc6("C:\\VAYU\\AMD - RADEON\\SKLOPNI_PINKY\\Frame\\Rezervoar\\REZERVOAR_pA_asembly.SLDASM", 2, 0, "", errors, warnings);
            _model = _swApp.ActiveDoc;
            _modelView = _model.ActiveView;
            _modelView.FrameLeft = 0;
            _modelView.FrameTop = 22;
            _modelView = _model.ActiveView;
            _modelView.FrameState = (int)swWindowState_e.swWindowMaximized;
            _swApp.ActivateDoc2("REZERVOAR_pA_asembly", false, errors);
            _model = _swApp.ActiveDoc;

            //' Take Snapshot
            _model.ModelViewManager.AddSnapShot("Home");
            _model.ClearSelection2(true);

            _model = _swApp.ActiveDoc;
            _model.Extension.SelectByRay(0.176996131337546, 9.60811344441481E-02, -1.03786650661277E-02,
                -0.533045219416617, -0.404114440817868, 0.743339971197267, 3.00924318184973E-03, 2, false, 0, 0);

            //' Open
            _model = _swApp.OpenDoc6("C:\\VAYU\\AMD - RADEON\\SKLOPNI_PINKY\\Frame\\Rezervoar\\2Rezervoar_1_pA_rezevoarXXXXXXXXXXXXXXX.SLDPRT", 1, 0, "", errors, warnings);
            _model = _swApp.ActiveDoc;
            _modelView = _model.ActiveView;
            _modelView.FrameLeft = 0;
            _modelView.FrameTop = 22;
            _modelView = _model.ActiveView;
            _modelView.FrameState = (int)swWindowState_e.swWindowMaximized;
            _swApp.ActivateDoc2("2Rezervoar_1_pA_rezevoarXXXXXXXXXXXXXXX", false, errors);
            _model = _swApp.ActiveDoc;
            _model.ClearSelection2(true);
            //_model.Extension.SelectByRay(-0.11243124114651, 0.41242278871249, -2.74096530663428E-02,
            //    -0.751191127381133, -0.463399926235297, -0.470077013380769, 2.80786861049484E-03, 2, false, 0, 0);
            //_model.ClearSelection2(true);
            _model.Extension.SelectByRay(-0.11243124114651, 0.41242278871249, -2.74096530663428E-02,
                -0.751191127381133, -0.463399926235297, -0.470077013380769, 2.80786861049484E-03, 2, false, 1, 0);
            _model.FeatureManager.SetFreeformBoundaryContinuity(0, 0);
            _model.FeatureManager.SetFreeformBoundaryContinuity(1, 0);
            _model.FeatureManager.SetFreeformBoundaryContinuity(2, 0);
            _model.FeatureManager.SetFreeformBoundaryContinuity(3, 0);
            _model.FeatureManager.SetFreeformBoundaryContinuity(4, 0);
            _model.FeatureManager.SetFreeformBoundaryContinuity(5, 0);
            _model.FeatureManager.SetFreeformBoundaryContinuity(6, 0);
            _model.FeatureManager.SetFreeformBoundaryContinuity(7, 0);
            _model.FeatureManager.SetFreeformBoundaryContinuity(8, 0);
            _model.FeatureManager.SetFreeformBoundaryContinuity(9, 0);
            _model.FeatureManager.SetFreeformBoundaryContinuity(10, 0);
            _model.FeatureManager.SetFreeformBoundaryContinuity(11, 0);
            _model.FeatureManager.SetFreeformCurveData(0, 0, 0, 0, 0, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(0, 0, 0, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(0, 0, 1, 0, 0, 0);
            _model.FeatureManager.SetFreeformCurveData(0, 0.56876816435434, 0, 0, 0, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(0, 0.56876816435434, 0, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(0, 0.56876816435434, 0.223074463729122, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(0, 0.56876816435434, 0.763971505757796, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(0, 0.56876816435434, 1, 0, 0, 0);
            _model.FeatureManager.SetFreeformCurveData(0, 0.769693613018321, 0, 0, 0, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(0, 0.769693613018321, 0, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(0, 0.769693613018321, 0.232144785263388, 9.52785002235345E-02, 2.11925319290072E-02, 2.77743299567113E-02);
            _model.FeatureManager.SetFreeformPointData(0, 0.769693613018321, 0.72747771444167, 0.12769487744291, 2.84028165958798E-02, 1.42262327022097E-02);
            _model.FeatureManager.SetFreeformPointData(0, 0.769693613018321, 1, 0, 0, 0);
            _model.FeatureManager.SetFreeformCurveData(0, 1, 0, 0, 0, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(0, 1, 0, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(0, 1, 1, 0, 0, 0);
            _model.FeatureManager.SetFreeformCurveData(1, 0, 0, 0, 0, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(1, 0, 0, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(1, 0, 0.56876816435434, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(1, 0, 0.769693613018321, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(1, 0, 1, 0, 0, 0);
            _model.FeatureManager.SetFreeformCurveData(1, 1, 0, 0, 0, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(1, 1, 0, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(1, 1, 0.56876816435434, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(1, 1, 0.769693613018321, 0, 0, 0);
            _model.FeatureManager.SetFreeformPointData(1, 1, 1, 0, 0, 0);
            _model.FeatureManager.InsertFreeform2(true, false, false, 0).GetDefinition();

            _model.Extension.SelectByID2("Freeform", "BODYFEATURE", 0, 0, 0, false, 0, null, 0);
            _model.SelectedFeatureProperties(0, 0, 0, 0, 0, 0, 0, true, false, "Freeform");

            AddJsonString("Step 7 (Freeform)");

            //Rotate
            Rotate(_model);
            _model.ClearSelection2(true);
            AddJsonString("Step 7 (Rotate Freeform)");

            //Roll
            Roll(_model);
            _model.ClearSelection2(true);
            AddJsonString("Step 7 (Roll View Freeform)");

            //Pan
            Pan(_model);
            AddJsonString("Step 7 (Pan Freeform)");

            this.BikeProgressBar.Value = 22;

            _model.Extension.SelectByID2("Freeform", "BODYFEATURE", 0, 0, 0, false, 0, null, 0);
            _model.EditDelete();

            //' Close Document
            _swApp.CloseDoc("2Rezervoar_1_pA_rezevoarXXXXXXXXXXXXXXX");
            _model = _swApp.ActiveDoc;
            _modelView = _model.ActiveView;
            _modelView.FrameLeft = 0;
            _modelView.FrameTop = 22;
            _modelView = _model.ActiveView;
            _modelView.FrameState = (int)swWindowState_e.swWindowMaximized;
            _swApp.ActivateDoc2("REZERVOAR_pA_asembly", false, errors);
            _model = _swApp.ActiveDoc;

            //' Close Document
            _swApp.CloseDoc("REZERVOAR_pA_asembly");
            _model = _swApp.ActiveDoc;
            _modelView = _model.ActiveView;
            _modelView.FrameLeft = 0;
            _modelView.FrameTop = 0;
            _modelView = _model.ActiveView;
            _modelView.FrameState = (int)swWindowState_e.swWindowMaximized;
            _swApp.ActivateDoc2("AMD_Bike_by_paX", false, errors);
            _model = _swApp.ActiveDoc;

            AddJsonString("Step 7 (Delete Freeform, close docs)");

            // step 7 - freeform - end

            //' Named View
            _model.ShowNamedView2("*Isometric", 7);
            _model.ViewZoomtofit2();
            UpdateLabel("Step 8 (Mirroring 1)");

            this.BikeProgressBar.Value = 23;

            // step 8 - - - - -
            // 1 - - - - - -
            // Selecting for mirror
            _model.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, true, 1, null, 0);
            _model.Extension.SelectByID2("PIPAK_ZA_ENGINE-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("10_pA_LAST_ONE-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("lanac_sa_lancanicima_MP_6-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("LOCK_MALI_Lancanik-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("WHEELIE_STAND_ASEMBLY-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cage.Lijevi-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cage.Desni-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_5_Intake_LIJEVI-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_4_malo_mirror-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_6_Intake_DESNI-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_1-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_2-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("CHIP_pA_CABLE_to-EcU-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Vijak.Predkomore-4@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Vijak.Predkomore-3@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("StartStop_5_pA_KABLOVI-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Akomulator_to_ECU-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NAVRTKE_ZA_RAM_PAX-3@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-4@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NAVRTKE_ZA_RAM_PAX-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("CHIP_pA_body_1-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-9@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Vijak.Predkomore-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-16@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Predkomora.PAX.design-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Predkomora.PAX.design-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-3@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-8@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-7@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-6@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Vijak.Predkomore-6@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-8@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_3_malo-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-5@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-3@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("accumulator_MP_sklop_7-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-7@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-15@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("ECU_Sklopni_saNOSACIMA-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_1_pA_Vijak.Donji.Hladnjak-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Sjediste_pA_asembly-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Zadnji_Blatobran_assembly-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("REZERVOAR_pA_asembly-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Plastike_1_pA_Sjediste-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("DUPE_XXXXXX-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-4@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-4@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_1_pA_Vijak.Donji.Hladnjak-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-3@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-3@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("HEADSET-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_16_pA_VIJAK_ENGINE_RAM2-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_15_pA_VIJAK_ENGINE_RAM-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_15_pA_VIJAK_ENGINE_RAM-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_6_pA_KOCNICA_sa_PaknamA-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_3_pA_nosac_sjedista-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_1_pA_kostur-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_5_pA_Vijak_R_Shock-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Zadnji_Tocak-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Prednji_Tocak-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_4_pA_navrtka_R_Shock-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Veza_Swing_Kostur_pA_Full-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_4_pA_navrtka_R_Shock-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_2_pA_swingarm-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_16_pA_VIJAK_ENGINE_RAM2-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_pA_Distributer_rear_brake-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_5_pA_Vijak_R_Shock-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_6_pA_KOCNICA_sa_PaknamA-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("REAR_Shock_pA-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-14@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_10_pA_USIS_manji_SPOJEVI-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-13@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-5@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_10_pA_USIS_manji_SPOJEVI-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_Prednja_Glava-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_11_pA_INTAKE_ZADNJA_Glava-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("DESNI_USIS_pA_sklop-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("LIJEVI_USIS_pA_sklop-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Turbina_pA_Sklopni-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-6@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_4_pA_UXHAUST-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("FULL_ENGINE-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_1_pA_Ulaz.Desna.turbina-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("VIJAK_ZA_ENGINE-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("VIJAK_ZA_ENGINE-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-20@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-16@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-15@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("SKLOP_BRIZGAC_AS_15-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-12@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-10@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-9@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_SA_NOSACEM-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Headset_34_pA_Handlebar.Crijevo3_Prednja_Kocnica_ULJE-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-18@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("SKLOP_BRIZGAC_AS_15-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-14@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-13@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("TurbinaLIJEVA_pA_Sklopni-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-19@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("CRIJEVO_Kvacilo__1_pA_na_.16-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("8_pA_Crijevo_glava_hepek_2-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("6_pA_spojnica_HEPEK-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("3_pA_spojnica_gorivo_hladnjak-4@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("3_pA_spojnica_gorivo_hladnjak-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("3_pA_spojnica_gorivo_hladnjak-3@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("2_pA_spojnica_rezervoar-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("3_pA_spojnica_gorivo_hladnjak-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Headset_Crijevo_6-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Headset_Crijevo_4-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Donji.Vijak-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("2_pA_spojnica_rezervoar-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Sklop hladnjak_AS_5-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("7_pA_Crijevo_glava_hepek_1-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Headset_Crijevo_3-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Donji.Vijak-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("9_pA_spojnica_HEAD_LLL-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("1_pA_rezervoar_crijevo_D-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("5_pA_spojnica_HEAD_1-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("4_pA_G_hladnjak_D_injection_D-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("1_pA_rezervoar_crijevo_L-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Gornji.Vijak-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("inercooler_MP_sklop_-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Headset_34_pA_Handlebar.Crijevo3_Prednja_Kocnica-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("4_pA_G_hladnjak_D_injection_L-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("6_pA_spojnica_HEPEK-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Gornji.Vijak-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);

            // Mirror
            Feature swSelMirrorPlane = _model.SelectionManager.GetSelectedObject6(1, 1);
            Feature swMirrorPlane = swSelMirrorPlane.GetSpecificFeature2();

            //List<Component2> swCompsInst = new List<Component2>(136);
            Component2[] swCompsInst = new Component2[136];
            int[] swCompsOrient = Enumerable.Range(0, 136).Select(i => 0).ToArray();

            AssemblyDoc swAssy = default(AssemblyDoc);

            swAssy = (AssemblyDoc)_model;
            swCompsInst[0] = swAssy.GetComponentByName("PIPAK_ZA_ENGINE-1");
            swCompsInst[1] = swAssy.GetComponentByName("10_pA_LAST_ONE-2");
            swCompsInst[2] = swAssy.GetComponentByName("lanac_sa_lancanicima_MP_6-1");
            swCompsInst[3] = swAssy.GetComponentByName("LOCK_MALI_Lancanik-1");
            swCompsInst[4] = swAssy.GetComponentByName("WHEELIE_STAND_ASEMBLY-1");
            swCompsInst[5] = swAssy.GetComponentByName("Cage.Lijevi-1");
            swCompsInst[6] = swAssy.GetComponentByName("Cage.Desni-1");
            swCompsInst[7] = swAssy.GetComponentByName("NOS_Crijevo_5_Intake_LIJEVI-1");
            swCompsInst[8] = swAssy.GetComponentByName("NOS_Crijevo_4_malo_mirror-1");
            swCompsInst[9] = swAssy.GetComponentByName("NOS_Crijevo_6_Intake_DESNI-1");
            swCompsInst[10] = swAssy.GetComponentByName("NOS_Crijevo_1-1");
            swCompsInst[11] = swAssy.GetComponentByName("NOS_Crijevo_2-1");
            swCompsInst[12] = swAssy.GetComponentByName("CHIP_pA_CABLE_to-EcU-1");
            swCompsInst[13] = swAssy.GetComponentByName("Vijak.Predkomore-4");
            swCompsInst[14] = swAssy.GetComponentByName("Vijak.Predkomore-3");
            swCompsInst[15] = swAssy.GetComponentByName("StartStop_5_pA_KABLOVI-1");
            swCompsInst[16] = swAssy.GetComponentByName("Akomulator_to_ECU-2");
            swCompsInst[17] = swAssy.GetComponentByName("NAVRTKE_ZA_RAM_PAX-3");
            swCompsInst[18] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-4");
            swCompsInst[19] = swAssy.GetComponentByName("NAVRTKE_ZA_RAM_PAX-1");
            swCompsInst[20] = swAssy.GetComponentByName("CHIP_pA_body_1-1");
            swCompsInst[21] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-2");
            swCompsInst[22] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-9");
            swCompsInst[23] = swAssy.GetComponentByName("Vijak.Predkomore-1");
            swCompsInst[24] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-16");
            swCompsInst[25] = swAssy.GetComponentByName("Predkomora.PAX.design-2");
            swCompsInst[26] = swAssy.GetComponentByName("Predkomora.PAX.design-1");
            swCompsInst[27] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-3");
            swCompsInst[28] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-8");
            swCompsInst[29] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-7");
            swCompsInst[30] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-6");
            swCompsInst[31] = swAssy.GetComponentByName("Vijak.Predkomore-6");
            swCompsInst[32] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-8");
            swCompsInst[33] = swAssy.GetComponentByName("NOS_Crijevo_3_malo-1");
            swCompsInst[34] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-5");
            swCompsInst[35] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-3");
            swCompsInst[36] = swAssy.GetComponentByName("accumulator_MP_sklop_7-1");
            swCompsInst[37] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-7");
            swCompsInst[38] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-15");
            swCompsInst[39] = swAssy.GetComponentByName("ECU_Sklopni_saNOSACIMA-1");
            swCompsInst[40] = swAssy.GetComponentByName("Dzidze_1_pA_Vijak.Donji.Hladnjak-2");
            swCompsInst[41] = swAssy.GetComponentByName("Sjediste_pA_asembly-2");
            swCompsInst[42] = swAssy.GetComponentByName("Zadnji_Blatobran_assembly-1");
            swCompsInst[43] = swAssy.GetComponentByName("REZERVOAR_pA_asembly-1");
            swCompsInst[44] = swAssy.GetComponentByName("Plastike_1_pA_Sjediste-1");
            swCompsInst[45] = swAssy.GetComponentByName("DUPE_XXXXXX-1");
            swCompsInst[46] = swAssy.GetComponentByName("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-4");
            swCompsInst[47] = swAssy.GetComponentByName("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-4");
            swCompsInst[48] = swAssy.GetComponentByName("Dzidze_1_pA_Vijak.Donji.Hladnjak-1");
            swCompsInst[49] = swAssy.GetComponentByName("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-3");
            swCompsInst[50] = swAssy.GetComponentByName("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-3");
            swCompsInst[51] = swAssy.GetComponentByName("HEADSET-1");
            swCompsInst[52] = swAssy.GetComponentByName("Frame_16_pA_VIJAK_ENGINE_RAM2-2");
            swCompsInst[53] = swAssy.GetComponentByName("Frame_15_pA_VIJAK_ENGINE_RAM-2");
            swCompsInst[54] = swAssy.GetComponentByName("Frame_15_pA_VIJAK_ENGINE_RAM-1");
            swCompsInst[55] = swAssy.GetComponentByName("Frame_6_pA_KOCNICA_sa_PaknamA-2");
            swCompsInst[56] = swAssy.GetComponentByName("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-2");
            swCompsInst[57] = swAssy.GetComponentByName("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-2");
            swCompsInst[58] = swAssy.GetComponentByName("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-1");
            swCompsInst[59] = swAssy.GetComponentByName("Frame_3_pA_nosac_sjedista-1");
            swCompsInst[60] = swAssy.GetComponentByName("Frame_1_pA_kostur-1");
            swCompsInst[61] = swAssy.GetComponentByName("Frame_5_pA_Vijak_R_Shock-2");
            swCompsInst[62] = swAssy.GetComponentByName("pA_Zadnji_Tocak-1");
            swCompsInst[63] = swAssy.GetComponentByName("pA_Prednji_Tocak-1");
            swCompsInst[64] = swAssy.GetComponentByName("Frame_4_pA_navrtka_R_Shock-1");
            swCompsInst[65] = swAssy.GetComponentByName("Veza_Swing_Kostur_pA_Full-1");
            swCompsInst[66] = swAssy.GetComponentByName("Frame_4_pA_navrtka_R_Shock-2");
            swCompsInst[67] = swAssy.GetComponentByName("Frame_2_pA_swingarm-1");
            swCompsInst[68] = swAssy.GetComponentByName("Frame_16_pA_VIJAK_ENGINE_RAM2-1");
            swCompsInst[69] = swAssy.GetComponentByName("Frame_pA_Distributer_rear_brake-1");
            swCompsInst[70] = swAssy.GetComponentByName("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-1");
            swCompsInst[71] = swAssy.GetComponentByName("Frame_5_pA_Vijak_R_Shock-1");
            swCompsInst[72] = swAssy.GetComponentByName("Frame_6_pA_KOCNICA_sa_PaknamA-1");
            swCompsInst[73] = swAssy.GetComponentByName("REAR_Shock_pA-1");
            swCompsInst[74] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-14");
            swCompsInst[75] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-11");
            swCompsInst[76] = swAssy.GetComponentByName("Cjevcuga_10_pA_USIS_manji_SPOJEVI-1");
            swCompsInst[77] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-13");
            swCompsInst[78] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-5");
            swCompsInst[79] = swAssy.GetComponentByName("Cjevcuga_10_pA_USIS_manji_SPOJEVI-2");
            swCompsInst[80] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_Prednja_Glava-1");
            swCompsInst[81] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-21");
            swCompsInst[82] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-1");
            swCompsInst[83] = swAssy.GetComponentByName("Cjevcuga_11_pA_INTAKE_ZADNJA_Glava-1");
            swCompsInst[84] = swAssy.GetComponentByName("DESNI_USIS_pA_sklop-1");
            swCompsInst[85] = swAssy.GetComponentByName("LIJEVI_USIS_pA_sklop-1");
            swCompsInst[86] = swAssy.GetComponentByName("Turbina_pA_Sklopni-1");
            swCompsInst[87] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-6");
            swCompsInst[88] = swAssy.GetComponentByName("Cjevcuga_4_pA_UXHAUST-1");
            swCompsInst[89] = swAssy.GetComponentByName("FULL_ENGINE-2");
            swCompsInst[90] = swAssy.GetComponentByName("Cjevcuga_1_pA_Ulaz.Desna.turbina-2");
            swCompsInst[91] = swAssy.GetComponentByName("VIJAK_ZA_ENGINE-2");
            swCompsInst[92] = swAssy.GetComponentByName("VIJAK_ZA_ENGINE-1");
            swCompsInst[93] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-2");
            swCompsInst[94] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-20");
            swCompsInst[95] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-16");
            swCompsInst[96] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-15");
            swCompsInst[97] = swAssy.GetComponentByName("SKLOP_BRIZGAC_AS_15-2");
            swCompsInst[98] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-12");
            swCompsInst[99] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-10");
            swCompsInst[100] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-9");
            swCompsInst[101] = swAssy.GetComponentByName("NOS_SA_NOSACEM-1");
            swCompsInst[102] = swAssy.GetComponentByName("Headset_34_pA_Handlebar.Crijevo3_Prednja_Kocnica_ULJE-1");
            swCompsInst[103] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-18");
            swCompsInst[104] = swAssy.GetComponentByName("SKLOP_BRIZGAC_AS_15-1");
            swCompsInst[105] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-14");
            swCompsInst[106] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-13");
            swCompsInst[107] = swAssy.GetComponentByName("TurbinaLIJEVA_pA_Sklopni-2");
            swCompsInst[108] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-19");
            swCompsInst[109] = swAssy.GetComponentByName("CRIJEVO_Kvacilo__1_pA_na_.16-1");
            swCompsInst[110] = swAssy.GetComponentByName("8_pA_Crijevo_glava_hepek_2-1");
            swCompsInst[111] = swAssy.GetComponentByName("6_pA_spojnica_HEPEK-2");
            swCompsInst[112] = swAssy.GetComponentByName("3_pA_spojnica_gorivo_hladnjak-4");
            swCompsInst[113] = swAssy.GetComponentByName("3_pA_spojnica_gorivo_hladnjak-2");
            swCompsInst[114] = swAssy.GetComponentByName("3_pA_spojnica_gorivo_hladnjak-3");
            swCompsInst[115] = swAssy.GetComponentByName("2_pA_spojnica_rezervoar-2");
            swCompsInst[116] = swAssy.GetComponentByName("3_pA_spojnica_gorivo_hladnjak-1");
            swCompsInst[117] = swAssy.GetComponentByName("pA_Headset_Crijevo_6-1");
            swCompsInst[118] = swAssy.GetComponentByName("pA_Headset_Crijevo_4-1");
            swCompsInst[119] = swAssy.GetComponentByName("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Donji.Vijak-1");
            swCompsInst[120] = swAssy.GetComponentByName("2_pA_spojnica_rezervoar-1");
            swCompsInst[121] = swAssy.GetComponentByName("Sklop hladnjak_AS_5-1");
            swCompsInst[122] = swAssy.GetComponentByName("7_pA_Crijevo_glava_hepek_1-1");
            swCompsInst[123] = swAssy.GetComponentByName("pA_Headset_Crijevo_3-1");
            swCompsInst[124] = swAssy.GetComponentByName("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Donji.Vijak-2");
            swCompsInst[125] = swAssy.GetComponentByName("9_pA_spojnica_HEAD_LLL-1");
            swCompsInst[126] = swAssy.GetComponentByName("1_pA_rezervoar_crijevo_D-1");
            swCompsInst[127] = swAssy.GetComponentByName("5_pA_spojnica_HEAD_1-2");
            swCompsInst[128] = swAssy.GetComponentByName("4_pA_G_hladnjak_D_injection_D-1");
            swCompsInst[129] = swAssy.GetComponentByName("1_pA_rezervoar_crijevo_L-1");
            swCompsInst[130] = swAssy.GetComponentByName("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Gornji.Vijak-2");
            swCompsInst[131] = swAssy.GetComponentByName("inercooler_MP_sklop_-1");
            swCompsInst[132] = swAssy.GetComponentByName("Headset_34_pA_Handlebar.Crijevo3_Prednja_Kocnica-1");
            swCompsInst[133] = swAssy.GetComponentByName("4_pA_G_hladnjak_D_injection_L-1");
            swCompsInst[134] = swAssy.GetComponentByName("6_pA_spojnica_HEPEK-1");
            swCompsInst[135] = swAssy.GetComponentByName("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Gornji.Vijak-1");

            object swMirrorStatus = null;
            _switcher = true;
            swMirrorStatus = swAssy.MirrorComponents3(swMirrorPlane, (swCompsInst),
                (swCompsOrient), false, null, true, null, 0, "Mirror", "", 2098193, false, false, false);
            _model.ClearSelection2(true);

            this.BikeProgressBar.Value = 80;

            //swMirrorStatus = swAssy.MirrorComponents3(swMirrorPlane, (swCompsInst),
            //    (swCompsOrient), false, null, false, null, 0, "Mirror", "", 2098193, false, false, false);

            //swMirrorStatus = swAssy.MirrorComponents3(swMirrorPlane, null,
            //    null, false, null, true, null, 0, "Mirror", "", 2098193, false, false, false);

            //if (swMirrorStatus == null)
            //    DebugLog("first mirroring: swMirrorStatus is null, swAssy.MirrorComponents3 didn't return anything");

            _model.ClearSelection();
            _model.ClearSelection2(true);

            // 2 - - - - - -
            // Selecting for mirror
            _model.Extension.SelectByID2("Top Plane", "PLANE", 0, 0, 0, true, 1, null, 0);
            _model.Extension.SelectByID2("9_pA_spojnica_HEAD_LLL-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Gornji.Vijak-22@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("6_pA_spojnica_HEPEK-22@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("5_pA_spojnica_HEAD_1-12@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Gornji.Vijak-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("1_pA_rezervoar_crijevo_L-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Headset_34_pA_Handlebar.Crijevo3_Prednja_Kocnica-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Donji.Vijak-22@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("4_pA_G_hladnjak_D_injection_L-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("1_pA_rezervoar_crijevo_D-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("inercooler_MP_sklop_-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("4_pA_G_hladnjak_D_injection_D-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("VIJAK_ZA_ENGINE-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_1_pA_Ulaz.Desna.turbina-12@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_4_pA_UXHAUST-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("FULL_ENGINE-12@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Turbina_pA_Sklopni-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-86@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("6_pA_spojnica_HEPEK-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("8_pA_Crijevo_glava_hepek_2-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("CRIJEVO_Kvacilo__1_pA_na_.16-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-101@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("VIJAK_ZA_ENGINE-22@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("SKLOP_BRIZGAC_AS_15-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-97@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-96@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-95@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-87@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("2_pA_spojnica_rezervoar-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("3_pA_spojnica_gorivo_hladnjak-43@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Headset_34_pA_Handlebar.Crijevo3_Prednja_Kocnica_ULJE-7@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-98@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_SA_NOSACEM-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-95@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-94@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-96@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("WHEELIE_STAND_ASEMBLY-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_1-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_6_Intake_DESNI-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_4_malo_mirror-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_5_Intake_LIJEVI-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cage.Desni-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cage.Lijevi-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("LOCK_MALI_Lancanik-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("lanac_sa_lancanicima_MP_6-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("10_pA_LAST_ONE-12@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("PIPAK_ZA_ENGINE-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_5_pA_Vijak_R_Shock-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-43@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_1_pA_kostur-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_3_pA_nosac_sjedista-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-44@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_16_pA_VIJAK_ENGINE_RAM2-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("HEADSET-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-43@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_6_pA_KOCNICA_sa_PaknamA-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_15_pA_VIJAK_ENGINE_RAM-22@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_15_pA_VIJAK_ENGINE_RAM-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-42@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_1_pA_Vijak.Donji.Hladnjak-22@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-41@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-41@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("DUPE_XXXXXX-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-42@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("3_pA_spojnica_gorivo_hladnjak-42@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("3_pA_spojnica_gorivo_hladnjak-41@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Headset_Crijevo_6-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("3_pA_spojnica_gorivo_hladnjak-44@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("DESNI_USIS_pA_sklop-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("LIJEVI_USIS_pA_sklop-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_11_pA_INTAKE_ZADNJA_Glava-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-88@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_10_pA_USIS_manji_SPOJEVI-18@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-94@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_Prednja_Glava-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("7_pA_Crijevo_glava_hepek_1-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("PIPAK_ZA_ENGINE-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("10_pA_LAST_ONE-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("lanac_sa_lancanicima_MP_6-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("LOCK_MALI_Lancanik-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("WHEELIE_STAND_ASEMBLY-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cage.Lijevi-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cage.Desni-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_5_Intake_LIJEVI-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_4_malo_mirror-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_6_Intake_DESNI-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_1-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_2-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("CHIP_pA_CABLE_to-EcU-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Vijak.Predkomore-4@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Vijak.Predkomore-3@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("StartStop_5_pA_KABLOVI-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Akomulator_to_ECU-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NAVRTKE_ZA_RAM_PAX-3@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-4@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NAVRTKE_ZA_RAM_PAX-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("CHIP_pA_body_1-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-9@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Vijak.Predkomore-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-16@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Predkomora.PAX.design-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Predkomora.PAX.design-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-3@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-8@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-7@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-6@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Vijak.Predkomore-6@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-8@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_3_malo-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-5@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-3@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("accumulator_MP_sklop_7-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-7@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-15@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("ECU_Sklopni_saNOSACIMA-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_1_pA_Vijak.Donji.Hladnjak-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Sjediste_pA_asembly-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Zadnji_Blatobran_assembly-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("REZERVOAR_pA_asembly-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Plastike_1_pA_Sjediste-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("DUPE_XXXXXX-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-4@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-4@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_1_pA_Vijak.Donji.Hladnjak-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-3@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-3@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("HEADSET-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_16_pA_VIJAK_ENGINE_RAM2-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_15_pA_VIJAK_ENGINE_RAM-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_15_pA_VIJAK_ENGINE_RAM-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_6_pA_KOCNICA_sa_PaknamA-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_3_pA_nosac_sjedista-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_1_pA_kostur-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_5_pA_Vijak_R_Shock-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Zadnji_Tocak-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Prednji_Tocak-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_4_pA_navrtka_R_Shock-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Veza_Swing_Kostur_pA_Full-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_4_pA_navrtka_R_Shock-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_2_pA_swingarm-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_16_pA_VIJAK_ENGINE_RAM2-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_pA_Distributer_rear_brake-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_5_pA_Vijak_R_Shock-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_6_pA_KOCNICA_sa_PaknamA-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("REAR_Shock_pA-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-14@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_10_pA_USIS_manji_SPOJEVI-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-13@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-5@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_10_pA_USIS_manji_SPOJEVI-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_Prednja_Glava-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_11_pA_INTAKE_ZADNJA_Glava-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("DESNI_USIS_pA_sklop-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("LIJEVI_USIS_pA_sklop-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Turbina_pA_Sklopni-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-6@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_4_pA_UXHAUST-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("FULL_ENGINE-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_1_pA_Ulaz.Desna.turbina-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("VIJAK_ZA_ENGINE-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("VIJAK_ZA_ENGINE-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-20@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-16@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-15@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("SKLOP_BRIZGAC_AS_15-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-12@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-10@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-9@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_SA_NOSACEM-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Headset_34_pA_Handlebar.Crijevo3_Prednja_Kocnica_ULJE-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-18@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("SKLOP_BRIZGAC_AS_15-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-14@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-13@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("TurbinaLIJEVA_pA_Sklopni-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-19@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("CRIJEVO_Kvacilo__1_pA_na_.16-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("8_pA_Crijevo_glava_hepek_2-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("6_pA_spojnica_HEPEK-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("3_pA_spojnica_gorivo_hladnjak-4@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("3_pA_spojnica_gorivo_hladnjak-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("3_pA_spojnica_gorivo_hladnjak-3@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("2_pA_spojnica_rezervoar-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("3_pA_spojnica_gorivo_hladnjak-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Headset_Crijevo_6-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Headset_Crijevo_4-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Donji.Vijak-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("2_pA_spojnica_rezervoar-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Sklop hladnjak_AS_5-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("7_pA_Crijevo_glava_hepek_1-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Headset_Crijevo_3-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Donji.Vijak-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("9_pA_spojnica_HEAD_LLL-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("1_pA_rezervoar_crijevo_D-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("5_pA_spojnica_HEAD_1-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("4_pA_G_hladnjak_D_injection_D-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("1_pA_rezervoar_crijevo_L-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Gornji.Vijak-2@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("inercooler_MP_sklop_-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Headset_34_pA_Handlebar.Crijevo3_Prednja_Kocnica-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("4_pA_G_hladnjak_D_injection_L-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("6_pA_spojnica_HEPEK-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Gornji.Vijak-1@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Headset_Crijevo_3-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-91@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("REAR_Shock_pA-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_6_pA_KOCNICA_sa_PaknamA-22@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-93@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-87@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-92@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_10_pA_USIS_manji_SPOJEVI-17@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Sklop hladnjak_AS_5-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("2_pA_spojnica_rezervoar-22@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("TurbinaLIJEVA_pA_Sklopni-12@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-100@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("SKLOP_BRIZGAC_AS_15-22@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-99@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Plastike_1_pA_Sjediste-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("REZERVOAR_pA_asembly-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_1_pA_Vijak.Donji.Hladnjak-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("ECU_Sklopni_saNOSACIMA-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Zadnji_Blatobran_assembly-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Sjediste_pA_asembly-12@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-90@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-85@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("accumulator_MP_sklop_7-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-86@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-84@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-83@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-83@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-89@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-81@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Vijak.Predkomore-45@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Predkomora.PAX.design-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("CHIP_pA_body_1-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-81@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NAVRTKE_ZA_RAM_PAX-22@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("StartStop_5_pA_KABLOVI-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-82@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-85@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Predkomora.PAX.design-22@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-84@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Akomulator_to_ECU-12@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Cjevcuga_5_pA_Vijci-82@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_3_malo-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Vijak.Predkomore-46@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NAVRTKE_ZA_RAM_PAX-23@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Vijak.Predkomore-44@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Vijak.Predkomore-43@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("CHIP_pA_CABLE_to-EcU-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("NOS_Crijevo_2-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_4_pA_navrtka_R_Shock-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Veza_Swing_Kostur_pA_Full-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Prednji_Tocak-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Zadnji_Tocak-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_5_pA_Vijak_R_Shock-22@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_2_pA_swingarm-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-44@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_pA_Distributer_rear_brake-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_16_pA_VIJAK_ENGINE_RAM2-22@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Frame_4_pA_navrtka_R_Shock-22@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("pA_Headset_Crijevo_4-11@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);
            _model.Extension.SelectByID2("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Donji.Vijak-21@AMD_Bike_by_paX", "COMPONENT", 0, 0, 0, true, 2, null, 0);

            swSelMirrorPlane = _model.SelectionManager.GetSelectedObject6(1, 1);
            swMirrorPlane = swSelMirrorPlane.GetSpecificFeature2();

            swCompsInst = new Component2[272];
            swCompsOrient = Enumerable.Range(0, 272).Select(i => 0).ToArray();

            swCompsInst[0] = swAssy.GetComponentByName("9_pA_spojnica_HEAD_LLL-11");
            swCompsInst[1] = swAssy.GetComponentByName("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Gornji.Vijak-22");
            swCompsInst[2] = swAssy.GetComponentByName("6_pA_spojnica_HEPEK-22");
            swCompsInst[3] = swAssy.GetComponentByName("5_pA_spojnica_HEAD_1-12");
            swCompsInst[4] = swAssy.GetComponentByName("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Gornji.Vijak-21");
            swCompsInst[5] = swAssy.GetComponentByName("1_pA_rezervoar_crijevo_L-11");
            swCompsInst[6] = swAssy.GetComponentByName("Headset_34_pA_Handlebar.Crijevo3_Prednja_Kocnica-11");
            swCompsInst[7] = swAssy.GetComponentByName("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Donji.Vijak-22");
            swCompsInst[8] = swAssy.GetComponentByName("4_pA_G_hladnjak_D_injection_L-11");
            swCompsInst[9] = swAssy.GetComponentByName("1_pA_rezervoar_crijevo_D-11");
            swCompsInst[10] = swAssy.GetComponentByName("inercooler_MP_sklop_-11");
            swCompsInst[11] = swAssy.GetComponentByName("4_pA_G_hladnjak_D_injection_D-11");
            swCompsInst[12] = swAssy.GetComponentByName("VIJAK_ZA_ENGINE-21");
            swCompsInst[13] = swAssy.GetComponentByName("Cjevcuga_1_pA_Ulaz.Desna.turbina-12");
            swCompsInst[14] = swAssy.GetComponentByName("Cjevcuga_4_pA_UXHAUST-11");
            swCompsInst[15] = swAssy.GetComponentByName("FULL_ENGINE-12");
            swCompsInst[16] = swAssy.GetComponentByName("Turbina_pA_Sklopni-11");
            swCompsInst[17] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-86");
            swCompsInst[18] = swAssy.GetComponentByName("6_pA_spojnica_HEPEK-21");
            swCompsInst[19] = swAssy.GetComponentByName("8_pA_Crijevo_glava_hepek_2-11");
            swCompsInst[20] = swAssy.GetComponentByName("CRIJEVO_Kvacilo__1_pA_na_.16-11");
            swCompsInst[21] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-101");
            swCompsInst[22] = swAssy.GetComponentByName("VIJAK_ZA_ENGINE-22");
            swCompsInst[23] = swAssy.GetComponentByName("SKLOP_BRIZGAC_AS_15-21");
            swCompsInst[24] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-97");
            swCompsInst[25] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-96");
            swCompsInst[26] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-95");
            swCompsInst[27] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-87");
            swCompsInst[28] = swAssy.GetComponentByName("2_pA_spojnica_rezervoar-21");
            swCompsInst[29] = swAssy.GetComponentByName("3_pA_spojnica_gorivo_hladnjak-43");
            swCompsInst[30] = swAssy.GetComponentByName("Headset_34_pA_Handlebar.Crijevo3_Prednja_Kocnica_ULJE-7");
            swCompsInst[31] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-98");
            swCompsInst[32] = swAssy.GetComponentByName("NOS_SA_NOSACEM-11");
            swCompsInst[33] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-95");
            swCompsInst[34] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-94");
            swCompsInst[35] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-96");
            swCompsInst[36] = swAssy.GetComponentByName("WHEELIE_STAND_ASEMBLY-11");
            swCompsInst[37] = swAssy.GetComponentByName("NOS_Crijevo_1-11");
            swCompsInst[38] = swAssy.GetComponentByName("NOS_Crijevo_6_Intake_DESNI-11");
            swCompsInst[39] = swAssy.GetComponentByName("NOS_Crijevo_4_malo_mirror-11");
            swCompsInst[40] = swAssy.GetComponentByName("NOS_Crijevo_5_Intake_LIJEVI-11");
            swCompsInst[41] = swAssy.GetComponentByName("Cage.Desni-11");
            swCompsInst[42] = swAssy.GetComponentByName("Cage.Lijevi-11");
            swCompsInst[43] = swAssy.GetComponentByName("LOCK_MALI_Lancanik-11");
            swCompsInst[44] = swAssy.GetComponentByName("lanac_sa_lancanicima_MP_6-11");
            swCompsInst[45] = swAssy.GetComponentByName("10_pA_LAST_ONE-12");
            swCompsInst[46] = swAssy.GetComponentByName("PIPAK_ZA_ENGINE-11");
            swCompsInst[47] = swAssy.GetComponentByName("Frame_5_pA_Vijak_R_Shock-21");
            swCompsInst[48] = swAssy.GetComponentByName("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-43");
            swCompsInst[49] = swAssy.GetComponentByName("Frame_1_pA_kostur-11");
            swCompsInst[50] = swAssy.GetComponentByName("Frame_3_pA_nosac_sjedista-11");
            swCompsInst[51] = swAssy.GetComponentByName("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-44");
            swCompsInst[52] = swAssy.GetComponentByName("Frame_16_pA_VIJAK_ENGINE_RAM2-21");
            swCompsInst[53] = swAssy.GetComponentByName("HEADSET-11");
            swCompsInst[54] = swAssy.GetComponentByName("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-43");
            swCompsInst[55] = swAssy.GetComponentByName("Frame_6_pA_KOCNICA_sa_PaknamA-21");
            swCompsInst[56] = swAssy.GetComponentByName("Frame_15_pA_VIJAK_ENGINE_RAM-22");
            swCompsInst[57] = swAssy.GetComponentByName("Frame_15_pA_VIJAK_ENGINE_RAM-21");
            swCompsInst[58] = swAssy.GetComponentByName("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-42");
            swCompsInst[59] = swAssy.GetComponentByName("Dzidze_1_pA_Vijak.Donji.Hladnjak-22");
            swCompsInst[60] = swAssy.GetComponentByName("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-41");
            swCompsInst[61] = swAssy.GetComponentByName("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-41");
            swCompsInst[62] = swAssy.GetComponentByName("DUPE_XXXXXX-11");
            swCompsInst[63] = swAssy.GetComponentByName("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-42");
            swCompsInst[64] = swAssy.GetComponentByName("3_pA_spojnica_gorivo_hladnjak-42");
            swCompsInst[65] = swAssy.GetComponentByName("3_pA_spojnica_gorivo_hladnjak-41");
            swCompsInst[66] = swAssy.GetComponentByName("pA_Headset_Crijevo_6-11");
            swCompsInst[67] = swAssy.GetComponentByName("3_pA_spojnica_gorivo_hladnjak-44");
            swCompsInst[68] = swAssy.GetComponentByName("DESNI_USIS_pA_sklop-11");
            swCompsInst[69] = swAssy.GetComponentByName("LIJEVI_USIS_pA_sklop-11");
            swCompsInst[70] = swAssy.GetComponentByName("Cjevcuga_11_pA_INTAKE_ZADNJA_Glava-11");
            swCompsInst[71] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-88");
            swCompsInst[72] = swAssy.GetComponentByName("Cjevcuga_10_pA_USIS_manji_SPOJEVI-18");
            swCompsInst[73] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-94");
            swCompsInst[74] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_Prednja_Glava-11");
            swCompsInst[75] = swAssy.GetComponentByName("7_pA_Crijevo_glava_hepek_1-11");
            swCompsInst[76] = swAssy.GetComponentByName("PIPAK_ZA_ENGINE-1");
            swCompsInst[77] = swAssy.GetComponentByName("10_pA_LAST_ONE-2");
            swCompsInst[78] = swAssy.GetComponentByName("lanac_sa_lancanicima_MP_6-1");
            swCompsInst[79] = swAssy.GetComponentByName("LOCK_MALI_Lancanik-1");
            swCompsInst[80] = swAssy.GetComponentByName("WHEELIE_STAND_ASEMBLY-1");
            swCompsInst[81] = swAssy.GetComponentByName("Cage.Lijevi-1");
            swCompsInst[82] = swAssy.GetComponentByName("Cage.Desni-1");
            swCompsInst[83] = swAssy.GetComponentByName("NOS_Crijevo_5_Intake_LIJEVI-1");
            swCompsInst[84] = swAssy.GetComponentByName("NOS_Crijevo_4_malo_mirror-1");
            swCompsInst[85] = swAssy.GetComponentByName("NOS_Crijevo_6_Intake_DESNI-1");
            swCompsInst[86] = swAssy.GetComponentByName("NOS_Crijevo_1-1");
            swCompsInst[87] = swAssy.GetComponentByName("NOS_Crijevo_2-1");
            swCompsInst[88] = swAssy.GetComponentByName("CHIP_pA_CABLE_to-EcU-1");
            swCompsInst[89] = swAssy.GetComponentByName("Vijak.Predkomore-4");
            swCompsInst[90] = swAssy.GetComponentByName("Vijak.Predkomore-3");
            swCompsInst[91] = swAssy.GetComponentByName("StartStop_5_pA_KABLOVI-1");
            swCompsInst[92] = swAssy.GetComponentByName("Akomulator_to_ECU-2");
            swCompsInst[93] = swAssy.GetComponentByName("NAVRTKE_ZA_RAM_PAX-3");
            swCompsInst[94] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-4");
            swCompsInst[95] = swAssy.GetComponentByName("NAVRTKE_ZA_RAM_PAX-1");
            swCompsInst[96] = swAssy.GetComponentByName("CHIP_pA_body_1-1");
            swCompsInst[97] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-2");
            swCompsInst[98] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-9");
            swCompsInst[99] = swAssy.GetComponentByName("Vijak.Predkomore-1");
            swCompsInst[100] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-16");
            swCompsInst[101] = swAssy.GetComponentByName("Predkomora.PAX.design-2");
            swCompsInst[102] = swAssy.GetComponentByName("Predkomora.PAX.design-1");
            swCompsInst[103] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-3");
            swCompsInst[104] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-8");
            swCompsInst[105] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-7");
            swCompsInst[106] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-6");
            swCompsInst[107] = swAssy.GetComponentByName("Vijak.Predkomore-6");
            swCompsInst[108] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-8");
            swCompsInst[109] = swAssy.GetComponentByName("NOS_Crijevo_3_malo-1");
            swCompsInst[110] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-5");
            swCompsInst[111] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-3");
            swCompsInst[112] = swAssy.GetComponentByName("accumulator_MP_sklop_7-1");
            swCompsInst[113] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-7");
            swCompsInst[114] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-15");
            swCompsInst[115] = swAssy.GetComponentByName("ECU_Sklopni_saNOSACIMA-1");
            swCompsInst[116] = swAssy.GetComponentByName("Dzidze_1_pA_Vijak.Donji.Hladnjak-2");
            swCompsInst[117] = swAssy.GetComponentByName("Sjediste_pA_asembly-2");
            swCompsInst[118] = swAssy.GetComponentByName("Zadnji_Blatobran_assembly-1");
            swCompsInst[119] = swAssy.GetComponentByName("REZERVOAR_pA_asembly-1");
            swCompsInst[120] = swAssy.GetComponentByName("Plastike_1_pA_Sjediste-1");
            swCompsInst[121] = swAssy.GetComponentByName("DUPE_XXXXXX-1");
            swCompsInst[122] = swAssy.GetComponentByName("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-4");
            swCompsInst[123] = swAssy.GetComponentByName("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-4");
            swCompsInst[124] = swAssy.GetComponentByName("Dzidze_1_pA_Vijak.Donji.Hladnjak-1");
            swCompsInst[125] = swAssy.GetComponentByName("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-3");
            swCompsInst[126] = swAssy.GetComponentByName("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-3");
            swCompsInst[127] = swAssy.GetComponentByName("HEADSET-1");
            swCompsInst[128] = swAssy.GetComponentByName("Frame_16_pA_VIJAK_ENGINE_RAM2-2");
            swCompsInst[129] = swAssy.GetComponentByName("Frame_15_pA_VIJAK_ENGINE_RAM-2");
            swCompsInst[130] = swAssy.GetComponentByName("Frame_15_pA_VIJAK_ENGINE_RAM-1");
            swCompsInst[131] = swAssy.GetComponentByName("Frame_6_pA_KOCNICA_sa_PaknamA-2");
            swCompsInst[132] = swAssy.GetComponentByName("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-2");
            swCompsInst[133] = swAssy.GetComponentByName("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-2");
            swCompsInst[134] = swAssy.GetComponentByName("Frame_8_pA_Vijak_1_Kostur_Nosac_sjedista-1");
            swCompsInst[135] = swAssy.GetComponentByName("Frame_3_pA_nosac_sjedista-1");
            swCompsInst[136] = swAssy.GetComponentByName("Frame_1_pA_kostur-1");
            swCompsInst[137] = swAssy.GetComponentByName("Frame_5_pA_Vijak_R_Shock-2");
            swCompsInst[138] = swAssy.GetComponentByName("pA_Zadnji_Tocak-1");
            swCompsInst[139] = swAssy.GetComponentByName("pA_Prednji_Tocak-1");
            swCompsInst[140] = swAssy.GetComponentByName("Frame_4_pA_navrtka_R_Shock-1");
            swCompsInst[141] = swAssy.GetComponentByName("Veza_Swing_Kostur_pA_Full-1");
            swCompsInst[142] = swAssy.GetComponentByName("Frame_4_pA_navrtka_R_Shock-2");
            swCompsInst[143] = swAssy.GetComponentByName("Frame_2_pA_swingarm-1");
            swCompsInst[144] = swAssy.GetComponentByName("Frame_16_pA_VIJAK_ENGINE_RAM2-1");
            swCompsInst[145] = swAssy.GetComponentByName("Frame_pA_Distributer_rear_brake-1");
            swCompsInst[146] = swAssy.GetComponentByName("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-1");
            swCompsInst[147] = swAssy.GetComponentByName("Frame_5_pA_Vijak_R_Shock-1");
            swCompsInst[148] = swAssy.GetComponentByName("Frame_6_pA_KOCNICA_sa_PaknamA-1");
            swCompsInst[149] = swAssy.GetComponentByName("REAR_Shock_pA-1");
            swCompsInst[150] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-14");
            swCompsInst[151] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-11");
            swCompsInst[152] = swAssy.GetComponentByName("Cjevcuga_10_pA_USIS_manji_SPOJEVI-1");
            swCompsInst[153] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-13");
            swCompsInst[154] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-5");
            swCompsInst[155] = swAssy.GetComponentByName("Cjevcuga_10_pA_USIS_manji_SPOJEVI-2");
            swCompsInst[156] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_Prednja_Glava-1");
            swCompsInst[157] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-21");
            swCompsInst[158] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-1");
            swCompsInst[159] = swAssy.GetComponentByName("Cjevcuga_11_pA_INTAKE_ZADNJA_Glava-1");
            swCompsInst[160] = swAssy.GetComponentByName("DESNI_USIS_pA_sklop-1");
            swCompsInst[161] = swAssy.GetComponentByName("LIJEVI_USIS_pA_sklop-1");
            swCompsInst[162] = swAssy.GetComponentByName("Turbina_pA_Sklopni-1");
            swCompsInst[163] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-6");
            swCompsInst[164] = swAssy.GetComponentByName("Cjevcuga_4_pA_UXHAUST-1");
            swCompsInst[165] = swAssy.GetComponentByName("FULL_ENGINE-2");
            swCompsInst[166] = swAssy.GetComponentByName("Cjevcuga_1_pA_Ulaz.Desna.turbina-2");
            swCompsInst[167] = swAssy.GetComponentByName("VIJAK_ZA_ENGINE-2");
            swCompsInst[168] = swAssy.GetComponentByName("VIJAK_ZA_ENGINE-1");
            swCompsInst[169] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-2");
            swCompsInst[170] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-20");
            swCompsInst[171] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-16");
            swCompsInst[172] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-15");
            swCompsInst[173] = swAssy.GetComponentByName("SKLOP_BRIZGAC_AS_15-2");
            swCompsInst[174] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-12");
            swCompsInst[175] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-10");
            swCompsInst[176] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-9");
            swCompsInst[177] = swAssy.GetComponentByName("NOS_SA_NOSACEM-1");
            swCompsInst[178] = swAssy.GetComponentByName("Headset_34_pA_Handlebar.Crijevo3_Prednja_Kocnica_ULJE-1");
            swCompsInst[179] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-18");
            swCompsInst[180] = swAssy.GetComponentByName("SKLOP_BRIZGAC_AS_15-1");
            swCompsInst[181] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-14");
            swCompsInst[182] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-13");
            swCompsInst[183] = swAssy.GetComponentByName("TurbinaLIJEVA_pA_Sklopni-2");
            swCompsInst[184] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-19");
            swCompsInst[185] = swAssy.GetComponentByName("CRIJEVO_Kvacilo__1_pA_na_.16-1");
            swCompsInst[186] = swAssy.GetComponentByName("8_pA_Crijevo_glava_hepek_2-1");
            swCompsInst[187] = swAssy.GetComponentByName("6_pA_spojnica_HEPEK-2");
            swCompsInst[188] = swAssy.GetComponentByName("3_pA_spojnica_gorivo_hladnjak-4");
            swCompsInst[189] = swAssy.GetComponentByName("3_pA_spojnica_gorivo_hladnjak-2");
            swCompsInst[190] = swAssy.GetComponentByName("3_pA_spojnica_gorivo_hladnjak-3");
            swCompsInst[191] = swAssy.GetComponentByName("2_pA_spojnica_rezervoar-2");
            swCompsInst[192] = swAssy.GetComponentByName("3_pA_spojnica_gorivo_hladnjak-1");
            swCompsInst[193] = swAssy.GetComponentByName("pA_Headset_Crijevo_6-1");
            swCompsInst[194] = swAssy.GetComponentByName("pA_Headset_Crijevo_4-1");
            swCompsInst[195] = swAssy.GetComponentByName("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Donji.Vijak-1");
            swCompsInst[196] = swAssy.GetComponentByName("2_pA_spojnica_rezervoar-1");
            swCompsInst[197] = swAssy.GetComponentByName("Sklop hladnjak_AS_5-1");
            swCompsInst[198] = swAssy.GetComponentByName("7_pA_Crijevo_glava_hepek_1-1");
            swCompsInst[199] = swAssy.GetComponentByName("pA_Headset_Crijevo_3-1");
            swCompsInst[200] = swAssy.GetComponentByName("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Donji.Vijak-2");
            swCompsInst[201] = swAssy.GetComponentByName("9_pA_spojnica_HEAD_LLL-1");
            swCompsInst[202] = swAssy.GetComponentByName("1_pA_rezervoar_crijevo_D-1");
            swCompsInst[203] = swAssy.GetComponentByName("5_pA_spojnica_HEAD_1-2");
            swCompsInst[204] = swAssy.GetComponentByName("4_pA_G_hladnjak_D_injection_D-1");
            swCompsInst[205] = swAssy.GetComponentByName("1_pA_rezervoar_crijevo_L-1");
            swCompsInst[206] = swAssy.GetComponentByName("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Gornji.Vijak-2");
            swCompsInst[207] = swAssy.GetComponentByName("inercooler_MP_sklop_-1");
            swCompsInst[208] = swAssy.GetComponentByName("Headset_34_pA_Handlebar.Crijevo3_Prednja_Kocnica-1");
            swCompsInst[209] = swAssy.GetComponentByName("4_pA_G_hladnjak_D_injection_L-1");
            swCompsInst[210] = swAssy.GetComponentByName("6_pA_spojnica_HEPEK-1");
            swCompsInst[211] = swAssy.GetComponentByName("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Gornji.Vijak-1");
            swCompsInst[212] = swAssy.GetComponentByName("pA_Headset_Crijevo_3-11");
            swCompsInst[213] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-91");
            swCompsInst[214] = swAssy.GetComponentByName("REAR_Shock_pA-11");
            swCompsInst[215] = swAssy.GetComponentByName("Frame_6_pA_KOCNICA_sa_PaknamA-22");
            swCompsInst[216] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-93");
            swCompsInst[217] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-87");
            swCompsInst[218] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-92");
            swCompsInst[219] = swAssy.GetComponentByName("Cjevcuga_10_pA_USIS_manji_SPOJEVI-17");
            swCompsInst[220] = swAssy.GetComponentByName("Sklop hladnjak_AS_5-11");
            swCompsInst[221] = swAssy.GetComponentByName("2_pA_spojnica_rezervoar-22");
            swCompsInst[222] = swAssy.GetComponentByName("TurbinaLIJEVA_pA_Sklopni-12");
            swCompsInst[223] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-100");
            swCompsInst[224] = swAssy.GetComponentByName("SKLOP_BRIZGAC_AS_15-22");
            swCompsInst[225] = swAssy.GetComponentByName("Cjevcuga_2_pA_ULAZ_Turbine_Veliki_Vijci-99");
            swCompsInst[226] = swAssy.GetComponentByName("Plastike_1_pA_Sjediste-11");
            swCompsInst[227] = swAssy.GetComponentByName("REZERVOAR_pA_asembly-11");
            swCompsInst[228] = swAssy.GetComponentByName("Dzidze_1_pA_Vijak.Donji.Hladnjak-21");
            swCompsInst[229] = swAssy.GetComponentByName("ECU_Sklopni_saNOSACIMA-11");
            swCompsInst[230] = swAssy.GetComponentByName("Zadnji_Blatobran_assembly-11");
            swCompsInst[231] = swAssy.GetComponentByName("Sjediste_pA_asembly-12");
            swCompsInst[232] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-90");
            swCompsInst[233] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-85");
            swCompsInst[234] = swAssy.GetComponentByName("accumulator_MP_sklop_7-11");
            swCompsInst[235] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-86");
            swCompsInst[236] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-84");
            swCompsInst[237] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-83");
            swCompsInst[238] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-83");
            swCompsInst[239] = swAssy.GetComponentByName("Cjevcuga_3_pA_ULAZ_Turbine_Mali_Vijci-89");
            swCompsInst[240] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-81");
            swCompsInst[241] = swAssy.GetComponentByName("Vijak.Predkomore-45");
            swCompsInst[242] = swAssy.GetComponentByName("Predkomora.PAX.design-21");
            swCompsInst[243] = swAssy.GetComponentByName("CHIP_pA_body_1-11");
            swCompsInst[244] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-81");
            swCompsInst[245] = swAssy.GetComponentByName("NAVRTKE_ZA_RAM_PAX-22");
            swCompsInst[246] = swAssy.GetComponentByName("StartStop_5_pA_KABLOVI-11");
            swCompsInst[247] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-82");
            swCompsInst[248] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-85");
            swCompsInst[249] = swAssy.GetComponentByName("Predkomora.PAX.design-22");
            swCompsInst[250] = swAssy.GetComponentByName("Cjevcuga_12_pA_INTAKE_ZADNJA_Glava_Vijci-84");
            swCompsInst[251] = swAssy.GetComponentByName("Akomulator_to_ECU-12");
            swCompsInst[252] = swAssy.GetComponentByName("Cjevcuga_5_pA_Vijci-82");
            swCompsInst[253] = swAssy.GetComponentByName("NOS_Crijevo_3_malo-11");
            swCompsInst[254] = swAssy.GetComponentByName("Vijak.Predkomore-46");
            swCompsInst[255] = swAssy.GetComponentByName("NAVRTKE_ZA_RAM_PAX-23");
            swCompsInst[256] = swAssy.GetComponentByName("Vijak.Predkomore-44");
            swCompsInst[257] = swAssy.GetComponentByName("Vijak.Predkomore-43");
            swCompsInst[258] = swAssy.GetComponentByName("CHIP_pA_CABLE_to-EcU-11");
            swCompsInst[259] = swAssy.GetComponentByName("NOS_Crijevo_2-11");
            swCompsInst[260] = swAssy.GetComponentByName("Frame_4_pA_navrtka_R_Shock-21");
            swCompsInst[261] = swAssy.GetComponentByName("Veza_Swing_Kostur_pA_Full-11");
            swCompsInst[262] = swAssy.GetComponentByName("pA_Prednji_Tocak-11");
            swCompsInst[263] = swAssy.GetComponentByName("pA_Zadnji_Tocak-11");
            swCompsInst[264] = swAssy.GetComponentByName("Frame_5_pA_Vijak_R_Shock-22");
            swCompsInst[265] = swAssy.GetComponentByName("Frame_2_pA_swingarm-11");
            swCompsInst[266] = swAssy.GetComponentByName("Frame_9_pA_Vijak_2_Kostur_Nosac_sjedista-44");
            swCompsInst[267] = swAssy.GetComponentByName("Frame_pA_Distributer_rear_brake-11");
            swCompsInst[268] = swAssy.GetComponentByName("Frame_16_pA_VIJAK_ENGINE_RAM2-22");
            swCompsInst[269] = swAssy.GetComponentByName("Frame_4_pA_navrtka_R_Shock-22");
            swCompsInst[270] = swAssy.GetComponentByName("pA_Headset_Crijevo_4-11");
            swCompsInst[271] = swAssy.GetComponentByName("Dzidze_2_pA_Vijak.Gornji.Hladnjak.Donji.Vijak-21");

            UpdateLabel("Step 8 (Mirroring 2)");
            swMirrorStatus = null;
            _switcher = true;
            swMirrorStatus = swAssy.MirrorComponents3(swMirrorPlane, (swCompsInst),
                (swCompsOrient), false, null, true, null, 0, "Mirror", "", 2098193, false, false, false);
            _model.ClearSelection2(true);

            this.BikeProgressBar.Value = 90;

            //if (swMirrorStatus == null)
            //    DebugLog("second mirroring: swMirrorStatus is null, swAssy.MirrorComponents3 didn't return anything");

            _model.ClearSelection();

            //Rotate
            Rotate(_model);
            UpdateLabel("Rotate/Roll/Pan/Zoom");
            _model.ClearSelection2(true);
            AddJsonString("Step 9 (Rotate)");
            this.BikeProgressBar.Value = 93;
            //Roll
            Roll(_model);
            _model.ClearSelection2(true);
            AddJsonString("Step 10 (Roll View)");
            this.BikeProgressBar.Value = 97;
            //Pan
            Pan(_model);
            AddJsonString("Step 11 (Pan)");
            this.BikeProgressBar.Value = 98;

            //Zoom
            Zoom(_model);
            AddJsonString("Step 12 (Zoom)");
            AddTotalTimeJsonString();

            _model.ShowNamedView2("*Isometric", 7);
            _model.ViewZoomtofit2();

            AddJsonStartEndString("End", "");
            AddString("}");
            File.WriteAllLines(@"C:\\VAYU\\bike_log" + _resCount++ + ".json", _jsonRows);

            UpdateLabel("Closing an assembly");

            CloseAssembly();
            this.BikeProgressBar.Value = 0;
            UpdateLabel("");
        }

        private void BikeTest_Click(object sender, System.EventArgs e)
        {
            for (var idx = 0; idx < Int32.Parse(this.bike_tests_num.Text); ++idx)
                ExecuteBikeTest();
        }

        private void ExecuteScrewTest()
        {
            string templateName = _swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
            ModelDoc2 model = (ModelDoc2)_swApp.NewDocument(templateName, (int)swDwgPaperSizes_e.swDwgPaperAsize, 0.0, 0.0);

            _switcher = false;
            _jsonRows = new List<string>();

            AddString("{");

            _startTime = DateTime.Now;
            _previousTime = _startTime;
            AddJsonStartEndString("Start");

            //Hexagon and Cylinder
            model.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
            model.SketchManager.InsertSketch(true);
            model.ClearSelection2(true);

            model.SketchManager.CreatePolygon(0, 0, 0, 0.025546367301763, 0, 0, 6, true);

            //Named View
            model.ShowNamedView2("*Trimetric", 8);
            model.ViewZoomtofit2();

            //Step 1-3 (Hexagon)
            model.FeatureManager.FeatureExtrusion2(true, false, false, 0, 0, 0.02, 0.01, false, false, false, false,
                1.74532925199433E-02, 1.74532925199433E-02, false, false, false, false, true, true, true, 0, 0, false);

            model.SelectionManager.EnableContourSelection = false;
            model.Extension.SelectByRay(-1.47485738049795E-02, -0.003612593690832, 1.99999999997544E-02,
                -0.400036026779312, -0.515038074910024, -0.758094294050284, 3.0517988408116E-04, 2, true, 4096, 0);
            model.Extension.SelectByRay(5.28953108641872E-03, 8.56172647530684E-04, 0,
                0.760430765016907, -0.289290941784557, 0.58142566387906, 3.0517988408116E-04, 2, true, 4096, 0);

            //Chamfer
            model.FeatureManager.InsertFeatureChamfer(6, 1, 0.001, 0.78539816339745, 0, 0, 0, 0);
            model.Extension.SelectByID2("Front Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
            model.Extension.SelectByRay(-1.05769655615865E-03, 4.73025404282065E-03,
                1.99999999999818E-02, 0, 0, -1, 1.99787127274411E-04, 2, false, 0, 0);
            model.SketchManager.InsertSketch(true);
            model.ClearSelection2(true);

            AddJsonString("Step 1-3 (Hexagon)");

            //Cylinder
            model.SketchManager.CreateCircle(0, 0, 0, 0.014455, -0.000264, 0);
            model.ClearSelection2(true);
            model.Extension.SelectByID2("Arc1", "SKETCHSEGMENT", 0, 0, 0, false, 0, null, 0);
            model.FeatureManager.FeatureExtrusion2(true, false, false, 0, 0, 0.07, 0.02, false, false, false, false,
                1.74532925199433E-02, 1.74532925199433E-02, false, false, false, false, true, true, true, 0, 0, false);
            model.SelectionManager.EnableContourSelection = false;

            model.Extension.SelectByRay(-1.89668180212266E-04, 3.24672680986282E-03, 8.99999999999181E-02,
                -0.672751486214634, -0.261383051568739, -0.692159185555328, 3.38382529449491E-04, 2, true, 4096, 0);
            model.FeatureManager.InsertFeatureChamfer(6, 1, 0.001, 0.78539816339745, 0, 0, 0, 0);
            model.Extension.SelectByRay(5.0516610114073E-03, 3.21092821388902E-03, 8.99999999999181E-02,
                -0.672751486214634, -0.261383051568739, -0.692159185555328, 3.38382529449491E-04, 2, false, 0, 0);
            model.SketchManager.InsertSketch(true);
            model.ClearSelection2(true);

            AddJsonString("Step 4-5 (Cylinder)");

            //Helix
            model.SketchManager.CreateCircle(0, 0, 0, 0.014338, 0.001733, 0);
            model.ClearSelection2(true);
            model.SketchManager.SketchUseEdge3(false, false);

            model.InsertHelix(true, true, false, false, 0, 0.06984, 0.00194, 36, 0, 1.5707963267949);
            model.Extension.SelectByID2("Right Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
            model.SketchManager.InsertSketch(true);

            AddJsonString("Step 6 (Helix)");

            // Zoom In/Out (MouseWheel)
            ModelView modelView = model.ActiveView;
            modelView.Scale2 = 62.9559929971706;
            //double[] translation = new double[3];
            //translation[0] = 5.69150794265615;
            //translation[1] = -0.859489220519109;
            //translation[2] = 0;


            //MathUtility mathUtils = _swApp.GetMathUtility();
            //MathVector transVector = mathUtils.CreateVector(translation);
            //modelView.Translation3 = transVector;
            model.ClearSelection2(true);

            model.SketchManager.CreateLine(-0.09, 0.013531, 0, -0.089095, 0.014551, 0);
            model.SketchManager.CreateLine(-0.089095, 0.014551, 0, -0.09, 0.014551, 0);
            model.SketchManager.CreateCenterLine(-0.09, 0.014551, 0, -0.09, 0.013531, 0);
            model.SetPickMode();
            model.ClearSelection2(true);
            model.Extension.SelectByID2("Line2", "SKETCHSEGMENT", 0, 1.45645377215483E-02, 8.92704200997787E-02, true, 0, null, 0);
            model.Extension.SelectByID2("Line1", "SKETCHSEGMENT", 0, 1.43523713777065E-02, 8.92627049600027E-02, true, 0, null, 0);
            model.Extension.SelectByID2("Line3", "SKETCHSEGMENT", 0, 1.41402050338646E-02, 9.00072159483932E-02, true, 0, null, 0);
            model.SketchMirror();
            model.ClearSelection2(true);
            model.SketchManager.InsertSketch(true);


            //Thread SweepCut start
            model.Extension.SelectByID2("Helix/Spiral1", "REFERENCECURVES", 0, 0, 0, true, 0, null, 0);
            model.ClearSelection2(true);
            model.Extension.SelectByID2("Sketch4", "SKETCH", 0, 0, 0, false, 1, null, 0);
            model.Extension.SelectByID2("Helix/Spiral1", "REFERENCECURVES", 0, 0, 0, true, 4, null, 0);

            FeatureManager featureManager = model.FeatureManager;
            ISweepFeatureData featureData = featureManager.CreateDefinition((int)swFeatureNameID_e.swFmSweepCut);
            featureData.AdvancedSmoothing = false;
            featureData.AlignWithEndFaces = true;
            featureData.AssemblyFeatureScope = true;
            featureData.AutoSelect = true;
            featureData.AutoSelectComponents = true;
            featureData.D1ReverseTwistDir = false;
            featureData.Direction = -1;
            featureData.EndTangencyType = 0;
            featureData.FeatureScope = true;
            featureData.MaintainTangency = false;
            featureData.MergeSmoothFaces = true;
            featureData.PathAlignmentType = 10;
            featureData.PropagateFeatureToParts = false;
            featureData.StartTangencyType = 0;
            featureData.ThinFeature = false;
            featureData.ThinWallType = 0;
            featureData.TwistControlType = 0;
            featureData.SetTwistAngle(0);
            featureData.SetWallThickness(true, 0);
            featureManager.CreateFeature(featureData);

            // Named View
            model.ShowNamedView2("*Isometric", 7);
            model.ViewZoomtofit2();
            model.Extension.SelectByID2("Helix/Spiral1", "REFERENCECURVES", 0, 0, 0, false, 0, null, 0);
            model.BlankRefGeom();

            AddJsonString("Step 7 (Thread SweepCut)");

            // Extrude surface
            model.Extension.SelectByRay(-1.46820212705734E-04, -1.33191491067919E-03, 0,
                -0.635779820787715, -0.322377397137213, 0.701325055373167, 4.74911194925974E-04, 2, false, 0, 0);
            model.SketchManager.InsertSketch(true);
            SketchSlot sketchSlot = model.SketchManager.CreateSketchSlot((int)swSketchSlotCreationType_e.swSketchSlotCreationType_3pointarc,
                (int)swSketchSlotLengthType_e.swSketchSlotLengthType_CenterCenter, 7.46050070207881E-03, -5.68372098285822E-03,
                -5.31974616330937E-03, 0, 9.76698445197102E-03, -5.31974616330937E-03, 0, 2.04163173455641E-03, 1.45073831546939E-02, 0, 1, false);
            model.FeatureManager.FeatureExtruRefSurface3(true, false, 0, 0, 0, 0, 0.276, 0.01, false, false, false,
                false, 1.74532925199433E-02, 1.74532925199433E-02, false, false, false, false, false, false, false, false);
            model.SelectionManager.EnableContourSelection = false;
            model.ClearSelection2(true);


            AddJsonString("Step 8-9 (Extruded Elliptical Slot)");

            // drill
            model.Extension.SelectByID2("Right Plane", "PLANE", 0, 0, 0, false, 0, null, 0);
            model.SketchManager.InsertSketch(true);
            model.ClearSelection2(true);

            model.SketchManager.CreateLine(-0.12319, 0.021157, 0, -0.12319, -0.021461, 0);
            model.SketchManager.CreateLine(-0.12319, -0.021461, 0, -0.120605, -0.021461, 0);
            model.SketchManager.CreateLine(-0.120605, -0.021461, 0, -0.11929, -0.016995, 0);
            model.SketchManager.CreateLine(-0.11929, -0.016995, 0, -0.11929, -0.015043, 0);
            model.SketchManager.CreateLine(-0.11929, -0.015043, 0, -0.11689, -0.015043, 0);
            model.SketchManager.CreateLine(-0.11689, -0.015043, 0, -0.11689, -0.010443, 0);
            model.SketchManager.CreateLine(-0.11689, -0.010443, 0, -0.11449, -0.010443, 0);
            model.SketchManager.CreateLine(-0.11449, -0.010443, 0, -0.11449, -0.005843, 0);
            model.SketchManager.CreateLine(-0.11449, -0.005843, 0, -0.11209, -0.005843, 0);
            model.SketchManager.CreateLine(-0.11209, -0.005843, 0, -0.11209, -0.001243, 0);
            model.SketchManager.CreateLine(-0.11209, -0.001243, 0, -0.10969, -0.001243, 0);
            model.SketchManager.CreateLine(-0.10969, -0.001243, 0, -0.10969, 0.003357, 0);
            model.SketchManager.CreateLine(-0.10969, 0.003357, 0, -0.10729, 0.003357, 0);
            model.SketchManager.CreateLine(-0.10729, 0.003357, 0, -0.10729, 0.007957, 0);
            model.SketchManager.CreateLine(-0.10729, 0.007957, 0, -0.10489, 0.007957, 0);
            model.SketchManager.CreateLine(-0.10489, 0.007957, 0, -0.10489, 0.012557, 0);
            model.SketchManager.CreateLine(-0.10489, 0.012557, 0, -0.120605, 0.012557, 0);
            model.SketchManager.CreateLine(-0.120605, 0.012557, 0, -0.120605, 0.021157, 0);
            model.SketchManager.CreateLine(-0.120605, 0.021157, 0, -0.12319, 0.021157, 0);
            model.ClearSelection2(true);
            model.SketchManager.InsertSketch(true);

            AddJsonString("Step 10-11 (Drill sketch)");

            // lathe
            model.Extension.SelectByID2("Line1@Sketch6", "EXTSKETCHSEGMENT", -0.12319, 5.8905731879122E-03, 0, true, 0, null, 0);
            model.ClearSelection2(true);
            model.Extension.SelectByID2("Sketch6", "SKETCH", 0, 0, 0, false, 0, null, 0);
            model.Extension.SelectByID2("Line1@Sketch6", "EXTSKETCHSEGMENT", -0.12319, 5.8905731879122E-03, 0, true, 16, null, 0);
            model.FeatureManager.FeatureRevolve2(true, true, false, false, false, false,
                0, 0, 6.2831853071796, 0, false, false, 0.01, 0.01, 0, 0, 0, true, true, true);
            model.SelectionManager.EnableContourSelection = false;

            // chamfer
            model.Extension.SelectByRay(1.30113381220553E-02, 7.82997383123529E-03, 0.132168861045955,
                -0.577452781453554, 0.577145190037229, -0.577452781453552, 2.44860737276636E-04, 1, true, 4096, 0);
            model.Extension.SelectByRay(9.69066745773262E-03, -1.14652629019929E-03, 0.128746341741817,
                -0.577452781453554, 0.577145190037229, -0.577452781453552, 2.44860737276636E-04, 1, true, 4096, 0);
            model.Extension.SelectByRay(5.96543519679926E-03, -1.02513363735284E-02, 0.125632201320059,
                -0.577452781453554, 0.577145190037229, -0.577452781453552, 2.44860737276636E-04, 1, true, 4096, 0);
            model.FeatureManager.InsertFeatureChamfer(6, 1, 0.0002, 0.78539816339745, 0, 0, 0, 0);

            AddJsonString("Step 12-13 (Revolved Boss)");

            // helix for drill
            model.Extension.SelectByRay(1.02971118523298E-03, -0.021460999999988, 0.123860828008796, 0, 1, 0, 2.08868055506518E-04, 2, false, 0, 0);
            model.SketchManager.InsertSketch(true);
            model.ClearSelection2(true);
            model.SketchManager.CreateCircle(0, 0.12319, 0, 0.003978, 0.123799, 0);
            model.ClearSelection2(true);
            model.SketchManager.SketchUseEdge3(false, false);
            model.InsertHelix(true, true, false, true, 0, 0.045, 0.03, 1.5, 0, 0);
            model.Extension.SelectByID2("Sketch6", "SKETCH", 0, 0, 0, false, 0, null, 0);
            model.BlankSketch();

            AddJsonString("Step 14 (Helix - drill)");

            // sweep cut
            // ' triangle
            model.Extension.SelectByRay(7.02926080713429E-04, -0.021460999999988, 0.123943226252511, 0, 1, 0, 1.43313646624686E-04, 2, false, 0, 0);
            model.SketchManager.InsertSketch(true);
            model.ClearSelection2(true);
            model.SketchManager.CreateLine(0, 0.12319, 0, 0.021045, 0.144235, 0);
            model.SketchManager.CreateLine(0.021045, 0.144235, 0, -0.021045, 0.144235, 0);
            model.SketchManager.CreateLine(-0.021045, 0.144235, 0, 0, 0.12319, 0);
            model.ClearSelection2(true);
            model.SketchManager.InsertSketch(true);
            model.Extension.SelectByID2("Helix/Spiral2", "REFERENCECURVES", 0, 0, 0, true, 0, null, 0);
            model.ClearSelection2(true);
            model.Extension.SelectByID2("Sketch8", "SKETCH", 0, 0, 0, false, 1, null, 0);
            model.Extension.SelectByID2("Helix/Spiral2", "REFERENCECURVES", 0, 0, 0, true, 4, null, 0);

            // ' Drill SweepCut start
            model.Extension.SelectByID2("Sketch8", "SKETCH", 0, 0, 0, true, 0, null, 0);
            model.Extension.SelectByID2("Helix/Spiral2", "REFERENCECURVES", 0, 0, 0, true, 0, null, 0);
            model.ClearSelection2(true);
            model.Extension.SelectByID2("Sketch8", "SKETCH", 0, 0, 0, false, 1, null, 0);
            model.Extension.SelectByID2("Helix/Spiral2", "REFERENCECURVES", 0, 0, 0, true, 4, null, 0);

            //Set featureManager = model.FeatureManager
            featureData = featureManager.CreateDefinition((int)swFeatureNameID_e.swFmSweepCut);
            featureData = model.FeatureManager.CreateDefinition((int)swFeatureNameID_e.swFmSweepCut);
            featureData.AdvancedSmoothing = false;
            featureData.AlignWithEndFaces = true;
            featureData.AssemblyFeatureScope = true;
            featureData.AutoSelect = true;
            featureData.AutoSelectComponents = true;
            featureData.D1ReverseTwistDir = false;
            featureData.Direction = -1;
            featureData.EndTangencyType = 0;
            featureData.FeatureScope = true;
            featureData.MaintainTangency = false;
            featureData.MergeSmoothFaces = true;
            featureData.PathAlignmentType = 10;
            featureData.PropagateFeatureToParts = false;
            featureData.StartTangencyType = 0;
            featureData.ThinFeature = false;
            featureData.ThinWallType = 0;
            featureData.TwistControlType = 0;
            featureData.SetTwistAngle(0);
            featureData.SetWallThickness(true, 0);
            featureManager.CreateFeature(featureData);

            AddJsonString("Step 14 (SweepCut - drill)");

            // hide triangle
            model.Extension.SelectByID2("Cut-Sweep2", "BODYFEATURE", 0, 0, 0, false, 0, null, 0);
            model.Extension.SelectByID2("Sketch7", "SKETCH", 0, 0, 0, false, 0, null, 0);
            model.BlankSketch();
            model.Extension.SelectByID2("Helix/Spiral2", "REFERENCECURVES", 0, 0, 0, false, 0, null, 0);
            model.BlankRefGeom();


            // Lofted Boss
            // plain
            model.Extension.SelectByRay(4.72387966260612E-03, 1.25570000000721E-02, 0.114611295359737,
                -0.577381545199981, -0.577287712085548, -0.577381545199979, 4.24697076437645E-04, 2, true, 0, 0);
            model.FeatureManager.InsertRefPlane(8, 0.15, 0, 0, 0, 0);
            model.ClearSelection2(true);
            // polygon
            model.Extension.SelectByID2("Plane1", "PLANE", -0.079169728816069, 8.05256942520496E-02, 4.40670329553178E-02, false, 0, null, 0);
            model.SketchManager.InsertSketch(true);
            model.ClearSelection2(true);
            model.SketchManager.CreatePolygon(0, -0.125199442952862, 0, 1.27137317035704E-02, -0.125199442952862, 0, 8, true);
            model.SketchManager.InsertSketch(true);
            model.ClearSelection2(true);

            // ellipse
            model.Extension.SelectByRay(-1.34787084062395E-02, 1.25570000000721E-02, 0.126732598704564,
                -0.577381545199981, -0.577287712085548, -0.577381545199979, 3.38649741149015E-04, 2, false, 0, 0);
            model.SketchManager.InsertSketch(true);
            model.ClearSelection2(true);
            model.SketchManager.CreateEllipse(-1.25215562737822E-02, -0.127971470282205, 0,
                -1.47629910445062E-02, -0.120940804943189, 0, -1.52506649262156E-02, -0.128841532891031, 0);
            model.ClearSelection2(true);
            model.SketchManager.InsertSketch(true);
            model.ClearSelection2(true);
            // loft
            model.Extension.SelectByID2("Sketch9", "SKETCH", 0, 0, 0, true, 0, null, 0);
            model.Extension.SelectByID2("Sketch10", "SKETCH", 0, 0, 0, true, 0, null, 0);
            model.ClearSelection2(true);
            model.Extension.SelectByID2("Sketch9", "SKETCH", 0, 0.162557, 0.137913174656432, false, 1, null, 0);
            model.Extension.SelectByID2("Sketch10", "SKETCH", -1.47629910445062E-02, 0.012557, 0.120940804943189, true, 1, null, 0);
            model.FeatureManager.InsertProtrusionBlend(false, true, false, 1, 0, 0, 1, 1, true, true, false, 0, 0, 0, true, true, true);

            AddJsonString("Step 15-17 (Lofted Boss)");

            // Named View
            model.ShowNamedView2("*Isometric", 7);
            model.ViewZoomtofit2();
            model.Extension.SelectByID2("Helix/Spiral1", "REFERENCECURVES", 0, 0, 0, false, 0, null, 0);
            model.BlankRefGeom();

            model.Extension.SelectByID2("Plane1", "PLANE", 0, 0, 0, false, 0, null, 0);
            model.BlankRefGeom();
            model.SetPickMode();

            // Lofted surfaces
            model.Extension.SelectByID2("Sketch9", "SKETCH", 0, 0, 0, true, 0, null, 0);
            model.Extension.SelectByID2("Sketch9", "SKETCH", 0, 0, 0, true, 0, null, 0);
            model.Extension.SelectByRay(4.8249696897642E-03, 2.21238030574682E-02, 9.32491215371556E-03,
                -0.238336023812682, -0.885393186981596, -0.39909252586303, 7.25851146194483E-04, 2, true, 0, 0);
            model.ClearSelection2(true);
            model.Extension.SelectByID2("Sketch9", "SKETCH", 0, 0, 0, false, 1, null, 0);
            model.Extension.SelectByRay(1.27731836508814E-02, 2.21238030577349E-02, 0.001,
                -0.238336023812682, -0.885393186981596, -0.39909252586303, 7.25851146194483E-04, 2, true, 1, 0);
            model.InsertLoftRefSurface2(false, true, false, 1, 0, 0);

            model.Extension.SelectByID2("Sketch9", "SKETCH", 0, 0, 0, true, 0, null, 0);
            model.Extension.SelectByID2("Sketch9", "SKETCH", 0, 0, 0, true, 0, null, 0);
            model.Extension.SelectByRay(-1.62796112426804E-02, 1.60504923159124E-02, 1.09616451881891E-02,
                0.854132574744087, -0.468775007192627, -0.225183341729776, 1.54795090413449E-03, 2, true, 0, 0);
            model.ClearSelection2(true);
            model.Extension.SelectByID2("Sketch9", "SKETCH", 0, 0, 0, false, 1, null, 0);
            model.Extension.SelectByRay(-1.27731836508816E-02, 2.21238030577348E-02, 0.019,
                0.854132574744087, -0.468775007192627, -0.225183341729776, 1.54795090413449E-03, 2, true, 1, 0);
            model.InsertLoftRefSurface2(false, true, false, 1, 0, 0);

            model.Extension.SelectByID2("Sketch9", "SKETCH", 0, 0, 0, true, 0, null, 0);
            model.Extension.SelectByID2("Sketch9", "SKETCH", 0, 0, 0, true, 0, null, 0);
            model.Extension.SelectByRay(-3.55197371669647E-03, 2.21238030576956E-02, 1.22293458917966E-02,
                -0.243411722191736, -0.566408164985102, 0.787357939020026, 1.10772203851776E-03, 2, true, 0, 0);
            model.ClearSelection2(true);
            model.Extension.SelectByID2("Sketch9", "SKETCH", 0, 0, 0, false, 1, null, 0);
            model.Extension.SelectByRay(-1.27731836508816E-02, 2.21238030577348E-02, 0.019,
                -0.243411722191736, -0.566408164985102, 0.787357939020026, 1.10772203851776E-03, 2, true, 1, 0);
            model.InsertLoftRefSurface2(false, true, false, 1, 0, 0);

            model.ClearSelection2(true);

            AddJsonString("Step 18 (Lofted Surfaces)");

            // Zoom
            Zoom(model);
            model.ClearSelection2(true);
            AddJsonString("Step 19 (Zoom View)");

            // Roll View
            Roll(model);
            model.ClearSelection2(true);
            AddJsonString("Step 20 (Roll View)");

            // Rotate
            Rotate(model);
            AddJsonString("Step 21 (Rotate)");
            model.ClearSelection2(true);

            // Pan
            Pan(model);
            model.ClearSelection2(true);
            AddJsonString("Step 22 (Pan)");
            AddTotalTimeJsonString();

            model.ShowNamedView2("*Isometric", 7);
            model.ViewZoomtofit2();

            AddJsonStartEndString("End", "");
            AddString("}");
            File.WriteAllLines(@"C:\\VAYU\\screw_log" + _scrResCount++ + ".json", _jsonRows);

            _swApp.CloseDoc("");
            model = null;
            modelView = null;
        }

        private void ScrewMacro_Click(object sender, EventArgs e)
        {
            for (var idx = 0; idx < Int32.Parse(this.screw_tests_num.Text); ++idx)
                ExecuteScrewTest();
        }

        private void Rotate(ModelDoc2 model)
        {
            ModelView mv = model.ActiveView;
            mv.RotateAboutCenter(-3.43611696486384E-03, 7.42108500847984E-03);
            mv.RotateAboutCenter(-3.43611696486384E-03, 2.96843400339194E-02);
            mv.RotateAboutCenter(-6.87223392972769E-03, 0.148421700169597);
            mv.RotateAboutCenter(0, 0.46010727052575);
            mv.RotateAboutCenter(0, 0.430422930491831);
            mv.RotateAboutCenter(0, 0.705003075805585);
            mv.RotateAboutCenter(0, 0.385896420440952);
            mv.RotateAboutCenter(0, 0.363633165415512);
            mv.RotateAboutCenter(0, 0.356212080407032);
            mv.RotateAboutCenter(-3.43611696486384E-03, 0.311685570356153);
            mv.RotateAboutCenter(-3.43611696486384E-03, 5.93686800678387E-02);
            mv.RotateAboutCenter(-3.43611696486384E-03, 0);
            mv.RotateAboutCenter(0, -7.42108500847984E-03);
            mv.RotateAboutCenter(0, -0.252316890288315);
            mv.RotateAboutCenter(0, -0.282001230322234);
            mv.RotateAboutCenter(0, -0.645634395737746);
            mv.RotateAboutCenter(0, -0.400738590457911);
            mv.RotateAboutCenter(0, -0.504633780576629);
            mv.RotateAboutCenter(0, -0.423001845483351);
            mv.RotateAboutCenter(0, -0.734687415839504);
            mv.RotateAboutCenter(0, -0.348790995398553);
            mv.RotateAboutCenter(0, -0.148421700169597);
            mv.RotateAboutCenter(0, -0.103895190118718);
            mv.RotateAboutCenter(0, -3.71054250423992E-02);
            mv.RotateAboutCenter(0, -7.42108500847984E-03);
            mv.RotateAboutCenter(0, -1.48421700169597E-02);
            mv.RotateAboutCenter(0, 7.42108500847984E-03);
            mv.RotateAboutCenter(-2.06167017891831E-02, 3.71054250423992E-02);
            mv.RotateAboutCenter(-0.065286222332413, 8.90530201017581E-02);
            mv.RotateAboutCenter(-8.24668071567323E-02, 6.67897650763186E-02);
            mv.RotateAboutCenter(-0.226783719681014, 0.118737360135677);
            mv.RotateAboutCenter(-0.161497497348601, 5.93686800678387E-02);
            mv.RotateAboutCenter(-0.31268664380261, 7.42108500847984E-03);
            mv.RotateAboutCenter(-0.127136327699962, 0);
            mv.RotateAboutCenter(-0.144316912524281, 0);
            mv.RotateAboutCenter(-8.59029241215961E-02, -4.45265100508791E-02);
            mv.RotateAboutCenter(-5.15417544729577E-02, -2.22632550254395E-02);
            mv.RotateAboutCenter(-1.71805848243192E-02, -2.22632550254395E-02);
            mv.RotateAboutCenter(-3.43611696486384E-02, -2.96843400339194E-02);
            mv.RotateAboutCenter(-4.81056375080938E-02, -2.96843400339194E-02);
            mv.RotateAboutCenter(-3.43611696486384E-02, -1.48421700169597E-02);
            mv.RotateAboutCenter(-1.71805848243192E-02, -1.48421700169597E-02);
            mv.RotateAboutCenter(-1.37444678594554E-02, 0);
            mv.RotateAboutCenter(-1.03083508945915E-02, 0);
            mv.RotateAboutCenter(3.43611696486384E-03, 0);
            mv.RotateAboutCenter(5.15417544729577E-02, 1.48421700169597E-02);
            mv.RotateAboutCenter(7.90306901918684E-02, 2.96843400339194E-02);
            mv.RotateAboutCenter(0.171805848243192, 4.45265100508791E-02);
            mv.RotateAboutCenter(0.151189146454009, 2.22632550254395E-02);
            mv.RotateAboutCenter(0.254272655399924, 4.45265100508791E-02);
            mv.RotateAboutCenter(0.17867808217292, 0);
            mv.RotateAboutCenter(0.140880795559418, 0);
            mv.RotateAboutCenter(0.103083508945915, -2.22632550254395E-02);
            mv.RotateAboutCenter(8.59029241215961E-02, -2.22632550254395E-02);
            mv.RotateAboutCenter(5.84139884026854E-02, -7.42108500847984E-03);
            mv.RotateAboutCenter(9.96473919810515E-02, -1.48421700169597E-02);
            mv.RotateAboutCenter(5.49778714378215E-02, 0);
            mv.RotateAboutCenter(-6.87223392972769E-03, 0);
            mv.RotateAboutCenter(-3.43611696486384E-03, 0);
        }

        private void Roll(ModelDoc2 model)
        {
            ModelView mv = model.ActiveView;
            mv.RollBy(-0.3);
            mv.RollBy(-0.3);
            mv.RollBy(-0.3);
            mv.RollBy(-0.3);
            mv.RollBy(-0.3);
            mv.RollBy(-0.3);
            mv.RollBy(-0.3);
            mv.RollBy(-0.3);
            mv.RollBy(-0.3);
            mv.RollBy(-0.3);
            mv.RollBy(-0.3);
            mv.RollBy(-0.3);
            mv.RollBy(-0.3);
            mv.RollBy(-0.3);
            mv.RollBy(-0.3);
            mv.RollBy(0.3);
            mv.RollBy(0.3);
            mv.RollBy(0.3);
            mv.RollBy(0.3);
            mv.RollBy(0.3);
            mv.RollBy(0.3);
            mv.RollBy(0.3);
            mv.RollBy(0.3);
            mv.RollBy(0.3);
            mv.RollBy(0.3);
            mv.RollBy(0.3);
            mv.RollBy(0.3);
            mv.RollBy(0.3);
            mv.RollBy(0.3);
            mv.RollBy(0.3);
        }

        private void Pan(ModelDoc2 model)
        {
            ModelView mv = model.ActiveView;
            mv.TranslateBy(-0.0005535897435897, 0);
            mv.TranslateBy(-0.0005535897435897, 0);
            mv.TranslateBy(-0.0005535897435897, 0.0005535897435897);
            mv.TranslateBy(-0.003875128205128, 0.001660769230769);
            mv.TranslateBy(-0.004982307692308, 0.0005535897435897);
            mv.TranslateBy(-0.005535897435897, 0.0005535897435897);
            mv.TranslateBy(-0.007196666666667, 0);
            mv.TranslateBy(-0.006089487179487, 0);
            mv.TranslateBy(-0.006643076923077, 0);
            mv.TranslateBy(-0.004982307692308, 0);
            mv.TranslateBy(-0.005535897435897, 0);
            mv.TranslateBy(-0.002214358974359, 0);
            mv.TranslateBy(-0.0005535897435897, 0);
            mv.TranslateBy(0, -0.0005535897435897);
            mv.TranslateBy(0, -0.001107179487179);
            mv.TranslateBy(0.001107179487179, -0.002214358974359);
            mv.TranslateBy(0.001660769230769, -0.004982307692308);
            mv.TranslateBy(0.002214358974359, -0.006643076923077);
            mv.TranslateBy(0.001660769230769, -0.006089487179487);
            mv.TranslateBy(0.001107179487179, -0.003321538461538);
            mv.TranslateBy(0, -0.001107179487179);
            mv.TranslateBy(0, 0.0005535897435897);
            mv.TranslateBy(0, 0.001107179487179);
            mv.TranslateBy(0.0005535897435897, 0.0005535897435897);
            mv.TranslateBy(0.001107179487179, 0.003875128205128);
            mv.TranslateBy(0.002767948717949, 0.004428717948718);
            mv.TranslateBy(0.002214358974359, 0.006643076923077);
            mv.TranslateBy(0.002767948717949, 0.005535897435897);
            mv.TranslateBy(0.003875128205128, 0.006643076923077);
            mv.TranslateBy(0.002214358974359, 0.004428717948718);
            mv.TranslateBy(0, 0.001107179487179);
            mv.TranslateBy(-0.0005535897435897, 0);
            mv.TranslateBy(-0.0005535897435897, 0);
            mv.TranslateBy(-0.001660769230769, 0);
            mv.TranslateBy(-0.003875128205128, 0.0005535897435897);
            mv.TranslateBy(-0.01328615384615, 0.002767948717949);
            mv.TranslateBy(-0.01328615384615, 0.002767948717949);
            mv.TranslateBy(-0.0160541025641, 0.003321538461538);
            mv.TranslateBy(-0.01273256410256, 0.003321538461538);
            mv.TranslateBy(-0.006643076923077, 0.001660769230769);
            mv.TranslateBy(-0.002767948717949, 0.0005535897435897);
            mv.TranslateBy(0.0005535897435897, 0);
            mv.TranslateBy(0.002767948717949, 0);
            mv.TranslateBy(0.008857435897436, -0.0005535897435897);
            mv.TranslateBy(0.02435794871795, -0.001660769230769);
            mv.TranslateBy(0.03044743589744, -0.002214358974359);
            mv.TranslateBy(0.01826846153846, -0.004428717948718);
            mv.TranslateBy(0.01217897435897, -0.004982307692308);
            mv.TranslateBy(0.004428717948718, -0.004982307692308);
            mv.TranslateBy(0.001660769230769, -0.003875128205128);
            mv.TranslateBy(0, -0.001660769230769);
            mv.TranslateBy(0.0005535897435897, -0.0005535897435897);
            mv.TranslateBy(0.001660769230769, 0);
            mv.TranslateBy(0.002214358974359, 0);
            mv.TranslateBy(0.008303846153846, 0.001107179487179);
            mv.TranslateBy(0.009411025641026, 0.004428717948718);
            mv.TranslateBy(0.01328615384615, 0.008303846153846);
            mv.TranslateBy(0.003875128205128, 0.007750256410256);
            mv.TranslateBy(-0.002767948717949, 0.007750256410256);
            mv.TranslateBy(-0.006643076923077, 0.003875128205128);
            mv.TranslateBy(-0.006643076923077, 0);
            mv.TranslateBy(-0.006089487179487, 0);
            mv.TranslateBy(-0.008857435897436, 0);
            mv.TranslateBy(-0.006643076923077, -0.001107179487179);
            mv.TranslateBy(-0.002767948717949, -0.003321538461538);
            mv.TranslateBy(-0.001660769230769, -0.001660769230769);
            mv.TranslateBy(-0.0005535897435897, -0.001107179487179);
            mv.TranslateBy(0, -0.0005535897435897);
            mv.TranslateBy(0, 0.0005535897435897);
            mv.TranslateBy(0, 0.0005535897435897);
        }

        private void Zoom(ModelDoc2 model)
        {
            ModelView mv = model.ActiveView;
            mv.ZoomByFactor(0.99);
            mv.ZoomByFactor(0.98);
            mv.ZoomByFactor(0.97);
            mv.ZoomByFactor(0.96);
            mv.ZoomByFactor(0.95);
            mv.ZoomByFactor(0.94);
            mv.ZoomByFactor(0.93);
            mv.ZoomByFactor(0.92);
            mv.ZoomByFactor(0.91);
            mv.ZoomByFactor(0.9);
            mv.ZoomByFactor(0.89);
            mv.ZoomByFactor(0.88);
            mv.ZoomByFactor(0.87);
            mv.ZoomByFactor(0.86);
            mv.ZoomByFactor(0.85);
            mv.ZoomByFactor(0.84);
            mv.ZoomByFactor(0.83);
            mv.ZoomByFactor(0.82);
            mv.ZoomByFactor(0.81);
            mv.ZoomByFactor(0.8);
            mv.ZoomByFactor(1.01);
            mv.ZoomByFactor(1.02);
            mv.ZoomByFactor(1.03);
            mv.ZoomByFactor(1.04);
            mv.ZoomByFactor(1.05);
            mv.ZoomByFactor(1.06);
            mv.ZoomByFactor(1.07);
            mv.ZoomByFactor(1.08);
            mv.ZoomByFactor(1.09);
            mv.ZoomByFactor(1.1);
            mv.ZoomByFactor(1.11);
            mv.ZoomByFactor(1.12);
            mv.ZoomByFactor(1.13);
            mv.ZoomByFactor(1.14);
            mv.ZoomByFactor(1.15);
            mv.ZoomByFactor(1.16);
            mv.ZoomByFactor(1.17);
            mv.ZoomByFactor(1.18);
            mv.ZoomByFactor(1.19);
            mv.ZoomByFactor(1.2);
            mv.ZoomByFactor(1.21);
            mv.ZoomByFactor(1.22);
            mv.ZoomByFactor(1.23);
        }

        private void AddString(string s) => _jsonRows.Add(String.Format("{0}", s));

        private void AddJsonString(string s)
        {
            _jsonRows.Add(String.Format("\t\"{0}\": {1},", s, (DateTime.Now - _previousTime).TotalSeconds));
            _previousTime = DateTime.Now;
        }

        private void AddJsonStartEndString(string s, string end = ",")
        {
            _jsonRows.Add(String.Format("\t\"{0}\": \"{1}\"{2}", s, DateTime.Now.ToString("dd/MM/y hh:mm:ss tt"), end));
            _previousTime = DateTime.Now;
        }

        private void AddTotalTimeJsonString() =>
            _jsonRows.Add(String.Format("\t\"Total time\": {0},", (DateTime.Now - _startTime).TotalSeconds));

        private void DebugLog(string s)
        {
            // TODO: if "C:\\VAYU\\debug.txt" doesn't exist - create it
            File.AppendAllText(@"C:\\VAYU\\debug.txt", String.Format("{0}:\t{1}\n", DateTime.Now.ToString("HH:mm:ss.fff"), s));
        }

        /// <summary>
        /// The current instance of SW
        /// </summary>
        private SldWorks _swApp;

        /// <summary>
        /// current opened document
        /// </summary>
        private ModelDoc2 _model;

        /// <summary>
        /// current model view holder
        /// </summary>
        private ModelView _modelView;

        private List<string> _jsonRows = new List<string>();

        // list of steps for use as prefixes in repaint event handler
        // if will be needed
        private List<string> _steps = new List<string> { "Step 3 (Extruded cut)", "Step 6 (extruded cut)", "Step 8 (Mirroring 1)", "Step 8 (Mirroring 2)" };
        private int _counter = 0;
        private int _resCount = 1;
        private int _scrResCount = 1;
        private DateTime _startTime;
        private DateTime _previousTime;

        /// <summary>
        /// switcher to avoid redurant logging
        /// </summary>
        private bool _switcher = false;

        #endregion
    }
}
