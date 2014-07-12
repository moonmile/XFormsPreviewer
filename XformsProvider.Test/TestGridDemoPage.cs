using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamarin.Forms;
using System.Xml.Linq;
using System.IO;
using Moonmile.XFormsProvider;

namespace XformsProvider.Test
{
    [TestClass]
    public class TestGridDemoPage
    {
        [TestMethod]
        public void GridDemoPage()
        {
            var fs = File.OpenText(@"Xaml\GridDemoPage.xaml");
            var xaml = fs.ReadToEnd();
            var page = PageXaml.LoadXaml<ContentPage>(xaml);

            Assert.IsNotNull(page);
            Assert.AreEqual("Grid Demo Page", page.Title );
            Assert.AreEqual(new Thickness(0, 20, 0, 0), page.Padding);
            
            var grid = page.Content as Grid;
            Assert.IsNotNull( grid );
            Assert.AreEqual(3, grid.RowDefinitions.Count);
            Assert.AreEqual(3, grid.ColumnDefinitions.Count);
            Assert.AreEqual(7, grid.Children.Count);

            Assert.AreEqual(true, grid.RowDefinitions[0].Height.IsAuto);
            Assert.AreEqual(true, grid.RowDefinitions[1].Height.IsStar);
            Assert.AreEqual(100.0, grid.RowDefinitions[2].Height.Value);
            Assert.AreEqual(true, grid.ColumnDefinitions[0].Width.IsAuto);
            Assert.AreEqual(true, grid.ColumnDefinitions[1].Width.IsStar);
            Assert.AreEqual(100.0, grid.ColumnDefinitions[2].Width.Value);

            var label = grid.Children[0] as Label;
            Assert.IsNotNull(label);
            Assert.AreEqual(Color.White, label.TextColor);
            Assert.AreEqual(Color.Blue, label.BackgroundColor);
            var boxview = grid.Children[1] as BoxView;
            Assert.IsNotNull(boxview);
            Assert.AreEqual(Color.Silver, boxview.Color);
            boxview = grid.Children[2] as BoxView;
            Assert.IsNotNull(boxview);
            Assert.AreEqual(Color.Teal, boxview.Color);


        }
    }
}
