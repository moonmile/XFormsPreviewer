using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamarin.Forms;
using System.Xml.Linq;
using System.IO;
using Moonmile.XFormsProvider;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XformsProvider.Test
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class ClockViewModel : BindableBase
    {
        DateTime dateTime = new DateTime(2014, 5, 2);
        public DateTime DateTime
        {
            get { return dateTime; }
            set
            {
                SetProperty(ref this.dateTime, value);
            }
        }
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

            Assert.AreEqual("2014/05/02 0:00:00", label.Text);
            var data = label.BindingContext as ClockViewModel;
            Assert.IsNotNull(data);
            // Xamarin.Forms.Forms.Init() が必要
            // プラットフォーム毎の Xamarin.Forms.Core に入っている
            // data.DateTime = new DateTime(2000, 1, 2);
            // Assert.AreEqual("", label.Text);


        }
    }
}
