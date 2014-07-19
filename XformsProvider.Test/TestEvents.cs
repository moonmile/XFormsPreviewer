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
    public class TestEvents
    {
        public static void OnClick(object sender, EventArgs e)
        {

        }
        [TestMethod]
        public void TestSetEvent()
        {
            string xml = @"<?xml version='1.0' encoding='utf-8' ?>
<ContentPage xmlns='http://xamarin.com/schemas/2014/forms'
					   xmlns:x='http://schemas.microsoft.com/winfx/2009/xaml'
					   x:Class='XformsProvider.Test.TestEvents'>
	<StackLayout>
	<Label x:Name='label1' Text='New Page'  />
	<Button  x:Name='button1' Clicked='OnClick' />
	</StackLayout>
</ContentPage>
";
            
            var page = PageXaml.LoadXaml<ContentPage>(xml);
            Assert.IsNotNull(page);
            var label = page.FindByName<Label>("label1");
            var button = page.FindByName<Button>("button1");
            Assert.IsNotNull(button);
        }
    }
}
