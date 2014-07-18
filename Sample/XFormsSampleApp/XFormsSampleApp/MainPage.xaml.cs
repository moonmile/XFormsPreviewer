using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonmile.XFormsProvider;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;


namespace XFormsSampleApp
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

    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        void OnClickedClock(object sender, EventArgs e)
        {
            string xml = @"<?xml version='1.0' encoding='utf-8' ?>
<ContentPage xmlns='http://xamarin.com/schemas/2014/forms'
             xmlns:x='http://schemas.microsoft.com/winfx/2009/xaml'
             xmlns:local='clr-namespace:XFormsSampleApp;assembly=XFormsSampleApp'
             x:Class='XFormsSampleApp.MainPage'
             Title='Clock Page'><StackLayout>"
 + "<Label x:Name='label1' Text=\"{Binding DateTime, StringFormat='{0:T}'}\""
 + @"    Font='Large'
         HorizontalOptions='Center'
         VerticalOptions='Center'>
    <Label.BindingContext>
      <local:ClockViewModel />
    </Label.BindingContext>
  </Label>
    <Button Text='Now' x:Name='button1' />
</StackLayout></ContentPage>";

            var page = PageXaml.LoadXaml<ContentPage>(xml);
            this.Navigation.PushAsync(page);
            var vm = page.FindByName<Label>("label1").BindingContext as ClockViewModel;


            page.FindByName<Button>("button1").Clicked += (s, ee) =>
            {
                vm.DateTime = DateTime.Now;
            };
        }
    }
}
