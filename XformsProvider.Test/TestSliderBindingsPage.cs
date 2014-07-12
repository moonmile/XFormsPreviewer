using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamarin.Forms;
using System.Xml.Linq;
using System.IO;
using Moonmile.XFormsProvider;

namespace XformsProvider.Test
{
    [TestClass]
    public class TestSliderBindingsPage
    {
        [TestMethod]
        public void SliderBindingsPage()
        {
            var fs = File.OpenText(@"Xaml\SliderBindingsPage.xaml");
            var xaml = fs.ReadToEnd();
            var page = PageXaml.LoadXaml<ContentPage>(xaml);

            Assert.IsNotNull(page);
            var layout = page.Content as StackLayout;
            Assert.AreEqual(3, layout.Children.Count);
            
            var sl = layout.Children[0] as Slider;
            Assert.AreEqual(360.0, sl.Maximum);
            Assert.AreEqual(LayoutOptions.CenterAndExpand, sl.VerticalOptions);
            var label = layout.Children[1] as Label;
            // Assert.AreEqual(Font, label.Font);
            Assert.AreEqual( "ROTATION", label.Text );
            Assert.AreEqual(LayoutOptions.Center, label.HorizontalOptions);
            Assert.AreEqual(LayoutOptions.CenterAndExpand, label.VerticalOptions);
        }

        [TestMethod]
        public void SliderTransformsPage()
        {
            var fs = File.OpenText(@"Xaml\SliderTransformsPage.xaml");
            var xaml = fs.ReadToEnd();
            var page = PageXaml.LoadXaml<ContentPage>(xaml);

            Assert.IsNotNull(page);
            var grid = page.Content as Grid;
            Assert.AreEqual(5, grid.RowDefinitions.Count);
            Assert.AreEqual(true, grid.RowDefinitions[0].Height.IsStar);
            Assert.AreEqual(true, grid.RowDefinitions[1].Height.IsAuto);
            Assert.AreEqual(true, grid.RowDefinitions[2].Height.IsAuto);
            Assert.AreEqual(2, grid.ColumnDefinitions.Count);
            Assert.AreEqual(true, grid.ColumnDefinitions[0].Width.IsAuto);
            Assert.AreEqual(true, grid.ColumnDefinitions[1].Width.IsStar);

            Assert.AreEqual( 9, grid.Children.Count );

            var layout = grid.Children[0] as StackLayout;
            Assert.IsNotNull(layout);

            var slider = grid.Children[1] as Slider;
            Assert.IsNotNull(slider);
            Assert.AreEqual(10.0, slider.Maximum);
            var label = grid.Children[2] as Label;
            Assert.AreEqual(TextAlignment.Center, label.YAlign);
        }
    }
}
