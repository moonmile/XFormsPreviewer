using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamarin.Forms;
using System.Xml.Linq;
using System.IO;
using Moonmile.XFormsProvider;

namespace XformsProvider.Test
{
    [TestClass]
    public class TestKeypadPage
    {
        [TestMethod]
        public void KeypadPage()
        {
            var fs = File.OpenText(@"Xaml\KeypadPage.xaml");
            var xaml = fs.ReadToEnd();
            var page = PageXaml.LoadXaml<ContentPage>(xaml);
            Assert.IsNotNull(page);
            var grid = page.Content as Grid;
            Assert.AreEqual(5, grid.RowDefinitions.Count);
            Assert.AreEqual(3, grid.ColumnDefinitions.Count);

            var grid2 = grid.Children[0] as Grid;
            Assert.IsNotNull(grid2);
            Assert.AreEqual(2, grid2.ColumnDefinitions.Count);
            Assert.AreEqual(true, grid2.ColumnDefinitions[0].Width.IsStar);
            Assert.AreEqual(true, grid2.ColumnDefinitions[1].Width.IsAuto);
            var frame = grid2.Children[0] as Frame;
            Assert.IsNotNull(frame);
            Assert.AreEqual(Color.Accent, frame.OutlineColor);
            Assert.IsNotNull(frame.Content as Label);
            var button = grid2.Children[1] as Button;
            Assert.IsNotNull(button);

            var b1 = grid.Children[1] as Button;
            Assert.IsNotNull(b1);
            Assert.AreEqual("1", b1.Text);
            var b2 = grid.Children[2] as Button;
            Assert.IsNotNull(b2);
            Assert.AreEqual("2", b2.Text);
            var b3 = grid.Children[3] as Button;
            Assert.IsNotNull(b3);
            Assert.AreEqual("3", b3.Text);
        }
    }
}
