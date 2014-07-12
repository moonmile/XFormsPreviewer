using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamarin.Forms;
using System.Xml.Linq;
using System.IO;
using Moonmile.XFormsProvider;

namespace XformsProvider.Test
{
    [TestClass]
    public class TestAbsoluteDemoPage
    {
        [TestMethod]
        public void AbsoluteDemoPage()
        {
            var fs = File.OpenText(@"Xaml\AbsoluteDemoPage.xaml");
            var xaml = fs.ReadToEnd();
            var page =  PageXaml.LoadXaml<ContentPage>(xaml);

            Assert.IsNotNull(page);
            var layout = page.Content as AbsoluteLayout;
            Assert.AreEqual(8, layout.Children.Count);
            Assert.AreEqual(Color.FromHex("#FF8080"), layout.BackgroundColor);
            var box = layout.Children[0] as BoxView;
            Assert.AreEqual(Color.FromHex("#8080FF"), box.Color);
            box = layout.Children[1] as BoxView;
            Assert.AreEqual(Color.FromHex("#8080FF"), box.Color);
        }
    }
}
