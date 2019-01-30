using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace solid_macro
{
    public class BikeMacro : ISwAddin
    {
        #region Private Members

        /// <summary>
        /// The cookie to the current instance of SW we are running inside of
        /// </summary>
        private int _swCookie;

        /// <summary>
        /// The current instance of SW
        /// </summary>
        private SldWorks _swApp;

        /// <summary>
        /// The taskplane view for our add-in
        /// </summary>
        private TaskpaneView _taskpaneView;

        /// <summary>
        /// The UI control that is going to be inside the SW taskpane view
        /// </summary>
        private TaskpaneHostUI _taskpaneHost;

        #endregion

        #region Public Members

        /// <summary>
        /// The ID to the taskpane used for registration in COM
        /// </summary>
        public const string PROGID = "SolidWorksBikeMacro";

        #endregion

        #region SW Add-in Callbacks

        /// <summary>
        /// Called when SW has loaded our add-in and wants us to do our
        /// connection logic
        /// </summary>
        /// <param name="ThisSW">The current SW instance</param>
        /// <param name="Cookie">The current SW cookie ID</param>
        /// <returns></returns>
        public bool ConnectToSW(object ThisSW, int Cookie)
        {
            // Store a reference to the current SW instance
            _swApp = (SldWorks)ThisSW;

            // Store cookie ID
            _swCookie = Cookie;

            // Setup callback info
            var _var = _swApp.SetAddinCallbackInfo2(0, this, _swCookie);

            // Create our UI
            LoadUI();

            return true;
        }

        /// <summary>
        /// Called when SW has is about to unload our add-in and wants us to do
        /// our disconnection logic
        /// </summary>
        /// <returns></returns>
        public bool DisconnectFromSW()
        {
            // Clean up our UI
            UnloadUI();
            return true;
        }

        #endregion

        #region Create UI

        /// <summary>
        /// Create our Taskpane and inject our host UI
        /// </summary>
        private void LoadUI()
        {
            var imgPath = Path.Combine(Path.GetDirectoryName(typeof(BikeMacro).Assembly.CodeBase).Replace(@"file:\", "")
                                       , "bm_rd.png");

            // create our taskpane
            _taskpaneView = _swApp.CreateTaskpaneView2(imgPath, "BM");

            // load oru UI into the taskpane
            _taskpaneHost = (TaskpaneHostUI)_taskpaneView.AddControl(BikeMacro.PROGID, string.Empty);
            _taskpaneHost.SWApp = _swApp;
        }

        /// <summary>
        /// Cleanup the Taskpane when we disconnect
        /// </summary>
        private void UnloadUI()
        {
            _taskpaneHost = null;
            _taskpaneView.DeleteView();

            // Release COM reference and cleanup memory
            Marshal.ReleaseComObject(_taskpaneView);
            _taskpaneView = null;
        }

        #endregion

        #region COM Registration

        /// <summary>
        /// The COM registration call to add our registry entries to the SW add-in registry
        /// </summary>
        /// <param name="t"></param>
        [ComRegisterFunction()]
        private static void ComRegister(Type t)
        {
            var keyPath = string.Format(@"SOFTWARE\SolidWorks\AddIns\{0:b}", t.GUID);

            // Create our registry folder for the add-in
            using (var rk = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(keyPath))
            {
                // Load add-in when SW opens
                rk.SetValue(null, 1);

                // Setup SW add-in title and description
                rk.SetValue("Title", "BikeMacro");
                rk.SetValue("Description", "Bike test app");
            }
        }

        /// <summary>
        /// The COM unregister call to remove our custom entries we added in the COM register function
        /// </summary>
        /// <param name="t"></param>
        [ComUnregisterFunction()]
        private static void ComUnregister(Type t)
        {
            var keyPath = string.Format(@"SOFTWARE\SolidWorks\AddIns\{0:b}", t.GUID);

            //Remove our registry entry
            Microsoft.Win32.Registry.LocalMachine.DeleteSubKeyTree(keyPath);
        }
        #endregion
    }
}
