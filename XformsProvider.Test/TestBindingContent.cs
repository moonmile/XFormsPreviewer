using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xamarin.Forms;
using System.Xml.Linq;
using System.IO;
using Moonmile.XFormsProvider;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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

    public class ButtonCommand : ICommand
    {
        private Func<object, bool> canExecute;
        private Action<object> execute;
        public ButtonCommand(Func<object, bool> can, Action<object> exec)
        {
            this.canExecute = can;
            this.execute = exec;
        }
        public bool CanExecute(object parameter)
        {
            if (canExecute != null)
                return canExecute(parameter);
            else
                return false;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (execute != null)
                execute(parameter);
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
        ButtonCommand clickCommand;

        public ButtonCommand ClickCommand
        {
            get
            {
                if (this.clickCommand == null)
                {
                    this.clickCommand =
                        new ButtonCommand(
                            (p) => true,
                            (p) => this.DateTime = new DateTime(2000, 2, 3));
                }
                return this.clickCommand;
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
    <Button x:Name='button1' Text='Click now' Command='OnClickButton1' />
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

        /// <summary>
        /// 名前空間のテスト
        /// </summary>
        [TestClass]
        public class TestBindingContextConentPage
        {
            [TestMethod]
            public void TestBinding()
            {
                string xml = @"<?xml version='1.0' encoding='utf-8' ?>
<ContentPage xmlns='http://xamarin.com/schemas/2014/forms'
             xmlns:x='http://schemas.microsoft.com/winfx/2009/xaml'
             xmlns:local='clr-namespace:XformsProvider.Test;assembly=XformsProvider.Test'
             x:Class='XformsProvider.Test.TestBindingContext'
             Title='Clock Page'>
<ContentPage.BindingContext>
      <local:ClockViewModel />
</ContentPage.BindingContext>

"
     + "<Label x:Name='label1' Text=\"{Binding DateTime, StringFormat='{0:T}'}\""
     + @"    Font='Large'
         HorizontalOptions='Center'
         VerticalOptions='Center'>
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
                var data = page.BindingContext as ClockViewModel;
                Assert.IsNotNull(data);
                // Xamarin.Forms.Forms.Init() が必要
                // プラットフォーム毎の Xamarin.Forms.Core に入っている
                // data.DateTime = new DateTime(2000, 1, 2);
                // Assert.AreEqual("", label.Text);

            }

            [TestMethod]
            public void TestCommand()
            {
                string xml = @"<?xml version='1.0' encoding='utf-8' ?>
<ContentPage xmlns='http://xamarin.com/schemas/2014/forms'
             xmlns:x='http://schemas.microsoft.com/winfx/2009/xaml'
             xmlns:local='clr-namespace:XformsProvider.Test;assembly=XformsProvider.Test'
             x:Class='XformsProvider.Test.TestBindingContext'
             Title='Clock Page'>
<ContentPage.BindingContext>
      <local:ClockViewModel />
</ContentPage.BindingContext>
"
     + "<Label x:Name='label1' Text=\"{Binding DateTime, StringFormat='{0:T}'}\""
     + @"    Font='Large'
         HorizontalOptions='Center'
         VerticalOptions='Center'>
  </Label>
    <Button x:Name='button1' Text='Click now' Command='{Binding ClickCommand}' />
</ContentPage>";

                var page = PageXaml.LoadXaml<ContentPage>(xml);
                Assert.IsNotNull(page);
                var label = page.FindByName<Label>("label1");
                var button = page.FindByName<Button>("button1");
                Assert.IsNotNull(page.BindingContext);
                Assert.IsNotNull(button);
                Assert.IsNotNull(button.Command);

            }
        }
    }
}
