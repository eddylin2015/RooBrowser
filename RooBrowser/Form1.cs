using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RooBrowser
{
    public partial class Form1 : Form
    {
        public const string ExampleDomain = "cefsharp.example";
        public const string BaseUrl = "https://" + ExampleDomain;
        public const string DefaultUrl = BaseUrl + "/home.html";
        public const string BindingTestUrl = BaseUrl + "/BindingTest.html";
        public const string BindingTestNetCoreUrl = BaseUrl + "/BindingTestNetCore.html";
        public const string BindingTestSingleUrl = BaseUrl + "/BindingTestSingle.html";
        public const string BindingTestsAsyncTaskUrl = BaseUrl + "/BindingTestsAsyncTask.html";
        public const string LegacyBindingTestUrl = BaseUrl + "/LegacyBindingTest.html";
        public const string PostMessageTestUrl = BaseUrl + "/PostMessageTest.html";
        public const string PluginsTestUrl = BaseUrl + "/plugins.html";
        public const string PopupTestUrl = BaseUrl + "/PopupTest.html";
        public const string TooltipTestUrl = BaseUrl + "/TooltipTest.html";
        public const string BasicSchemeTestUrl = BaseUrl + "/SchemeTest.html";
        public const string ResponseFilterTestUrl = BaseUrl + "/ResponseFilterTest.html";
        public const string DraggableRegionTestUrl = BaseUrl + "/DraggableRegionTest.html";
        public const string DragDropCursorsTestUrl = BaseUrl + "/DragDropCursorsTest.html";
        public const string CssAnimationTestUrl = BaseUrl + "/CssAnimationTest.html";
        public const string CdmSupportTestUrl = BaseUrl + "/CdmSupportTest.html";
        public const string BindingApiCustomObjectNameTestUrl = BaseUrl + "/BindingApiCustomObjectNameTest.html";
        public const string TestResourceUrl = "http://test/resource/load";
        public const string RenderProcessCrashedUrl = "http://processcrashed";
        public const string TestUnicodeResourceUrl = "http://test/resource/loadUnicode";
        public const string PopupParentUrl = "http://www.w3schools.com/jsref/tryit.asp?filename=tryjsref_win_close";
        public const string ChromeInternalUrls = "chrome://chrome-urls";
        public const string ChromeNetInternalUrls = "chrome://net-internals";
        public const string ChromeProcessInternalUrls = "chrome://process-internals";
        // Use when debugging the actual SubProcess, to make breakpoints etc. inside that project work.
        private static string PluginInformation = "";

        static string home_url = "http://mail.mbc.edu.mo";
        static string internalstu_url = "http://stu.mbc.edu.mo";
        public Form1()
        {
            InitializeComponent();
            
            CefSettings cefsettings = new CefSettings();
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            cefsettings.IgnoreCertificateErrors = true;
            cefsettings.CefCommandLineArgs["CachePath"] = "cache";
            cefsettings.CefCommandLineArgs["enable-npapi"]= "1";
            cefsettings.CefCommandLineArgs["enable-system-flash"] = "true"; //启用flash
            cefsettings.CefCommandLineArgs["plugin-policy"] = "allow";
            cefsettings.CefCommandLineArgs["plugins.run_all_flash_in_allow_mode"] = "1";
            cefsettings.CefCommandLineArgs["enable-media-stream"] = "1"; //启用媒体流
            cefsettings.CefCommandLineArgs["ppapi-flash-version"] = "32.0.0.465"; //设置flash插件版本
            cefsettings.CefCommandLineArgs["ppapi-flash-path"] = AppDomain.CurrentDomain.BaseDirectory + "PepperFlash\\32.0.0.465\\pepflashplayer.dll";
            cefsettings.CefCommandLineArgs["autoplay-policy"] = "no-user-gesture-required";



            CefSharp.Cef.Initialize(cefsettings);
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            MainMenu mainMenu1 = new MainMenu();
            // Add two MenuItem objects to the MainMenu.
            MenuItem menuItem2 =mainMenu1.MenuItems.Add("校內主頁", MenuItemInternalstu_Click);
            MenuItem menuItem1 = mainMenu1.MenuItems.Add("應用網站", MenuItemHome_Click);
            // Bind the MainMenu to Form1.
            Menu = mainMenu1;
        }
        private void MenuItemHome_Click(Object sender, System.EventArgs e)
        {
            Browser.Load(home_url);
        }
        private void MenuItemInternalstu_Click(Object sender, System.EventArgs e)
        {
            Browser.Load(internalstu_url);
        }

        
        public IWinFormsWebBrowser Browser { get; private set; }
        private void Form1_Load(object sender, EventArgs e)
        {
            
            var browser = new ChromiumWebBrowser(String.Empty) { 
                Dock = DockStyle.Fill ,
                BrowserSettings = new BrowserSettings
                {
                    FileAccessFromFileUrls = CefState.Enabled,
                    UniversalAccessFromFileUrls = CefState.Enabled,
                    Javascript = CefState.Enabled,
                    ImageLoading = CefState.Enabled,
                    JavascriptAccessClipboard = CefState.Enabled,
                    JavascriptCloseWindows = CefState.Enabled,
                    JavascriptDomPaste = CefState.Enabled
                }

            };
            browser.RequestContext = new RequestContext(new RequestContextHandler());
            
            browser.LoadingStateChanged += OnLoadingStateChanged;
            browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;
            panel1.Controls.Add(browser);
            Browser = browser;
            browser.LoadHtml("<html><body>Wait Login...\n<script>\n callbackObj.LoginWebFrame(\"aaa\");\n</script>\n</body></html>");
            var bindScriptOption = new CefSharp.BindingOptions();
            //bindScriptOption.CamelCaseJavascriptNames = false;
            browser.Load(home_url);
            //browser.MenuHandler = new MenuHandler();
            //browser.RequestHandler = new WinFormsRequestHandler(openNewTab);
            //browser.JsDialogHandler = new JsDialogHandler();
            //browser.DownloadHandler = new MyDownLoadFile();
            //browser.RequestHandler = new MyRequestHandler();
            
            Browser.FrameLoadStart += (sender0, e0) =>
            {
                var uri = new Uri(e0.Url);
                this.toolStripStatusLabel1.Text = uri.ToString() +" loading";
            };
            Browser.FrameLoadEnd += (sender0, e0) =>
            {
                var uri = new Uri(e0.Url);
                this.toolStripStatusLabel1.Text = uri.ToString() + " ";
            };
        }

        private void Browser_IsBrowserInitializedChanged(object sender, EventArgs e)
        {
            if (Browser.IsBrowserInitialized)
            {
                Cef.UIThreadTaskFactory.StartNew(() =>
                {
                    string error = "";
                    var requestContext = Browser.GetBrowser().GetHost().RequestContext;
                    requestContext.SetPreference("profile.default_content_setting_values.plugins", 1, out error);
                });
            }
        }

        private void OnLoadingStateChanged(object sender, LoadingStateChangedEventArgs args)
        {
            /*   
            if (!args.IsLoading && start_print)
            {
                MessageBox.Show(Browser.Address);
                if (Browser.Address.Contains("admissionformQuery"))
                {
                        PrintAdmissionPDF(@"D:\admissionformQuery_" + (temp_pdf_int++) + ".pdf");
                        start_print = false;
                }
                else
                {
                    // Page has finished loading, do whatever you want here
                    // browser.MainFrame.ViewSource();
                    PrintPDF(@"D:\" + Sess + "_" + term_int + "_" + classno[curr_index] + ".pdf");
                    start_print = false;
                }
            }
            */
        }
    }
    public class RequestContextHandler : IRequestContextHandler
    {
        public ICookieManager GetCookieManager()
        {
            return null;
        }

        public IResourceRequestHandler GetResourceRequestHandler(IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            //throw new NotImplementedException();
            return null;
        }

        public bool OnBeforePluginLoad(string mimeType, string url, bool isMainFrame, string topOriginUrl, WebPluginInfo pluginInfo, ref PluginPolicy pluginPolicy)
        {
            //MessageBox.Show(pluginInfo.Name);
            bool blockPluginLoad = pluginInfo.Name.ToLower().Contains("flash");
            if (blockPluginLoad)
            {
                pluginPolicy = PluginPolicy.Allow;
            }
            return true;
        }

        public void OnRequestContextInitialized(IRequestContext requestContext)
        {
            //throw new NotImplementedException();
            
        }
    }
}
