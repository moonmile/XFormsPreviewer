using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamarin.Forms;
using System.Xml.Linq;
using System.IO;



namespace Moonmile.XFormsProvider.Test
{
    [TestClass]
    public class TestSample
    {
        [TestMethod]
        public void LoadSample()
        {
            string xml = @"<?xml version='1.0' encoding='utf-8' ?>
<ContentPage xmlns='http://xamarin.com/schemas/2014/forms'
					   xmlns:x='http://schemas.microsoft.com/winfx/2009/xaml'
					   x:Class='XFormsPreview.NewPage'>
	<StackLayout>
	<Label Text='New Page'  />
	<Button Text='Click me'  />
	</StackLayout>
</ContentPage>
";
            var page = PageXaml.LoadXaml<ContentPage>(xml);
            Assert.IsNotNull(page);
            var layout = page.Content as StackLayout;
            Assert.IsNotNull(layout);
            Assert.AreEqual(2, layout.Children.Count);

            var label = layout.Children[0] as Label;
            var button = layout.Children[1] as Button;
            Assert.IsNotNull(label);
            Assert.IsNotNull(button);
            Assert.AreEqual("New Page", label.Text);
            Assert.AreEqual("Click me", button.Text);

        }
    }
}
