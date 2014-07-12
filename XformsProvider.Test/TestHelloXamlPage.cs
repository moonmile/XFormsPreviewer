using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamarin.Forms;
using System.Xml.Linq;
using System.IO;
using Moonmile.XFormsProvider;

namespace XformsProvider.Test
{
    [TestClass]
    public class TestHelloXamlPage
    {
        [TestMethod]
        public void HelloXamlPage()
        {
            var fs = File.OpenText(@"Xaml\HelloXamlPage.xaml");
            var xaml = fs.ReadToEnd();
            var page = PageXaml.LoadXaml<ContentPage>(xaml);

            Assert.IsNotNull(page);
            var label = page.Content as Label;
            Assert.IsNotNull(label);
            Assert.AreEqual("Hello, XAML!", label.Text);
            Assert.AreEqual(LayoutOptions.Start, label.VerticalOptions);
            Assert.AreEqual(TextAlignment.Center, label.XAlign);
            Assert.AreEqual(-15.0, label.Rotation);
            Assert.AreEqual(true, label.IsVisible);
            // Assert.AreEqual(label.Font)
            Assert.AreEqual(Color.Aqua, label.TextColor);
        }

        [TestMethod]
        public void XamlPlusCodePage()
        {
            var fs = File.OpenText(@"Xaml\XamlPlusCodePage.xaml");
            var xaml = fs.ReadToEnd();
            var page = PageXaml.LoadXaml<ContentPage>(xaml);

            Assert.IsNotNull(page);
            Assert.AreEqual("XAML + Code Page", page.Title);
            var layout = page.Content as StackLayout;
            Assert.IsNotNull(layout);
            Assert.AreEqual(3, layout.Children.Count);

            var slider = layout.Children[0] as Slider;
            var label = layout.Children[1] as Label;
            var button = layout.Children[2] as Button;
            Assert.IsNotNull(slider);
            Assert.IsNotNull(label);
            Assert.IsNotNull(button);

            Assert.AreEqual(LayoutOptions.CenterAndExpand, slider.VerticalOptions);
            Assert.AreEqual(LayoutOptions.Center, label.HorizontalOptions);
            Assert.AreEqual(LayoutOptions.CenterAndExpand, label.VerticalOptions);
            Assert.AreEqual("Click Me!", button.Text);
        }
    }
}
