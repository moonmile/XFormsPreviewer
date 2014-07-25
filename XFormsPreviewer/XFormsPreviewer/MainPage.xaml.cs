using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonmile.XFormsProvider;
using System.IO;
using Xamarin.Forms;
using System.Reflection;
using System.Net.Http;


namespace XFormsPreviewer
{
    public partial class MainPage
    {
        public int Port { get; set; }
        public string Hostname { get; set; }
        public Uri Uri
        {
            get
            {
                return new Uri(string.Format("http://{0}:{1}/get/", this.Hostname, this.Port));
            }
        }

        public MainPage()
        {
            InitializeComponent();

            this.Port = 10150;
            // TODO: Change IP address of XFormsPreviewHost.exe on machine.
            // XAML ファイルをホスティングしている PC の IP アドレスを書く。
            // エミュレータから名前解決ができれば、マシン名でもOK.
            this.Hostname = "172.16.0.9";
        }

        void OnClickPreview(object sender, EventArgs e)
        {
            // XAML 文字列を直接書き込むパターン
            string xml = @"<?xml version='1.0' encoding='utf-8' ?>
<ContentPage xmlns='http://xamarin.com/schemas/2014/forms'
					   xmlns:x='http://schemas.microsoft.com/winfx/2009/xaml'
					   x:Class='XFormsPreview.NewPage'>
	<StackLayout>
	<Label Text='New Page'  />
	<Button Text='click me'  />
	</StackLayout>
</ContentPage>
";
            var page = PageXaml.LoadXaml(xml);
            this.Navigation.PushAsync(page);
        }

        void OnClickGridPreview(object sender, EventArgs e)
        {
            /// リソースから表示するテスト
            var st = ResourceLoader.GetObject("XFormsPreviewer.Xaml.GridDemoPage.xaml");
            var xaml = new StreamReader(st).ReadToEnd();
            var page = PageXaml.LoadXaml<ContentPage>(xaml);
            this.Navigation.PushAsync(page);
        }

        void OnClickAbsoluteLayoutPreview(object sender, EventArgs e)
        {
            var st = ResourceLoader.GetObject("XFormsPreviewer.Xaml.AbsoluteDemoPage.xaml");
            var xaml = new StreamReader(st).ReadToEnd();
            var page = PageXaml.LoadXaml<ContentPage>(xaml);
            this.Navigation.PushAsync(page);
        }

        void OnClickSliderPreview(object sender, EventArgs e)
        {
            var st = ResourceLoader.GetObject("XFormsPreviewer.Xaml.SliderBindingsPage.xaml");
            var xaml = new StreamReader(st).ReadToEnd();
            var page = PageXaml.LoadXaml(xaml);
            this.Navigation.PushAsync(page);
        }
        void OnClickSliderTransPreview(object sender, EventArgs e)
        {
            var st = ResourceLoader.GetObject("XFormsPreviewer.Xaml.SliderTransformsPage.xaml");
            var xaml = new StreamReader(st).ReadToEnd();
            var page = PageXaml.LoadXaml(xaml);
            this.Navigation.PushAsync(page);
        }

        void OnClickKeyboardPreview(object sender, EventArgs e)
        {
            var st = ResourceLoader.GetObject("XFormsPreviewer.Xaml.KeypadPage.xaml");
            var xaml = new StreamReader(st).ReadToEnd();
            var page = PageXaml.LoadXaml(xaml);
            this.Navigation.PushAsync(page);
        }


        /// <summary>
        /// TODO: ホスト接続して *.xaml 一覧を取得する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClickGetList(object sender, EventArgs e)
        {
            /// TODO: ボタンを自動生成して表示する
            /// 再表示はできないので、固定ボタン 1-10 に割り当てる
        }


        /// <summary>
        /// HTTP クライアント経由で XAML ファイルをロードする
        /// http://hostname:10150/get/[num] 形式で取得する
        /// [num] 部分は XFormsPreviewHost と合わせる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async void OnClickSample0(object sender, EventArgs e)
        {
            var hc = new HttpClient();
            try
            {
                var res = await hc.GetStringAsync(this.Uri + "0");
                var page = PageXaml.LoadXaml(res);
                await this.Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

    }
    public static class ResourceLoader
    {
        static internal string[] Names { get; set; }
        static internal Assembly Assembly { get; set; }

        public static System.IO.Stream GetObject(string resourceName)
        {
            if (ResourceLoader.Assembly == null)
            {
                ResourceLoader.Assembly = typeof(ResourceLoader).GetTypeInfo().Assembly;
                ResourceLoader.Names = ResourceLoader.Assembly.GetManifestResourceNames();
            }
            try
            {
                string path = ResourceLoader.Names.First(x => x.EndsWith(resourceName, StringComparison.CurrentCultureIgnoreCase));
                return ResourceLoader.Assembly.GetManifestResourceStream(path);
            }
            catch
            {
                return null;
            }
        }
    }
}
