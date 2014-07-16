using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamarin.Forms;
using System.Xml.Linq;
using System.IO;
using Moonmile.XFormsProvider;

namespace XformsProvider.Test
{


    /// <summary>
    /// 名前空間のテスト
    /// </summary>
    [TestClass]
    public class TestNames
    {
        [TestMethod]
        public void TestName()
        {
            string xml = @"<?xml version='1.0' encoding='utf-8' ?>
<ContentPage xmlns='http://xamarin.com/schemas/2014/forms'
					   xmlns:x='http://schemas.microsoft.com/winfx/2009/xaml'
					   x:Class='XFormsPreview.NewPage'>
	<StackLayout>
	<Label x:Name='label1' Text='New Page'  />
	<Button  x:Name='button1' Text='Click me'  />
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

            // Assert.AreEqual("XFormsPreview.NewPage", layout.ClassName);
            // test

            // var label1 = PageXaml.FindByName(page, "label1") as Label;
            // var button1 = PageXaml.FindByName(page, "button1") as Button;
            var label1 = page.FindByName<Label>("label1");
            var button1 = page.FindByName<Button>("button1");
            /*
            var label1 = page.FindByName<Label>("label1");
            var button1 = page.FindByName<Button>("button1");
            */
            Assert.IsNotNull( label1 );
            Assert.AreEqual("New Page", label1.Text);
            Assert.AreEqual("Click me", button1.Text);
        }
    }
}
