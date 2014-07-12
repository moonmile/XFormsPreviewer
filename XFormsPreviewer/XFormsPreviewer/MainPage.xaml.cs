using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonmile.XFormsProvider;
using System.IO;
using Xamarin.Forms;
using System.Reflection;


namespace XFormsPreviewer
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void OnClickPreview(object sender, EventArgs e)
        {
            // var page = new NewPage();
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
