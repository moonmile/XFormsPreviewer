using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamarin.Forms;
using System.Xml.Linq;
using System.IO;
using Moonmile.XFormsProvider;

namespace XformsProvider.Test
{

    public class ClockViewModel
    {
        public ClockViewModel()
        {
            this.DateTime = new DateTime(2014, 5, 2);
        }
        public DateTime DateTime { get; set; }
    }

    /// <summary>
    /// 名前空間のテスト
    /// </summary>
    [TestClass]
    public class TestBindingContext
    {
        [TestMethod]
        public void TestBinding()
        {
            string xml = @"<?xml version='1.0' encoding='utf-8' ?>
<ContentPage xmlns='http://xamarin.com/schemas/2014/forms'
             xmlns:x='http://schemas.microsoft.com/winfx/2009/xaml'
             xmlns:local='clr-namespace:XformsProvider.Test;assembly=XformsProvider.Test'
             x:Class='XformsProvider.Test.TestBindingContext'
             Title='Clock Page'>"
 + "<Label x:Name='label1' Text=\"{Binding DateTime, StringFormat='{0:T}'}\"" 
 + @"    Font='Large'
         HorizontalOptions='Center'
         VerticalOptions='Center'>
    <Label.BindingContext>
      <local:ClockViewModel />
    </Label.BindingContext>
  </Label>
</ContentPage>";

            
            var page = PageXaml.LoadXaml<ContentPage>(xml);
            Assert.IsNotNull(page);
            var label = page.FindByName<Label>("label1");
            Assert.IsNotNull(label);
            var bind = label.BindingContext as ClockViewModel;
            Assert.IsNotNull(bind);
            Assert.AreEqual(2014, bind.DateTime.Year);
            Assert.AreEqual(5, bind.DateTime.Month);
            Assert.AreEqual(2, bind.DateTime.Day);
        }
    }
}
