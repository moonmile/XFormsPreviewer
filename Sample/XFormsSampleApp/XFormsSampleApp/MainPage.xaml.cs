using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moonmile.XFormsProvider;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Windows.Input;
using System.Xml.Linq;
using System.IO;


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

    public class DelegateCommand : ICommand
    {
        private Func<object, bool> canExecute;
        private Action<object> execute;
        public DelegateCommand(Func<object, bool> can, Action<object> exec)
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
        DelegateCommand clickCommand;

        public DelegateCommand ClickCommand
        {
            get
            {
                if (this.clickCommand == null)
                {
                    this.clickCommand =
                        new DelegateCommand(
                            (p) => true,
                            (p) => this.DateTime = new DateTime(2000, 2, 3));
                }
                return this.clickCommand;
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
             Title='Clock Page'>
<ContentPage.BindingContext>
    <local:ClockViewModel />
</ContentPage.BindingContext>
<StackLayout>
"
 + "<Label x:Name='label1' Text=\"{Binding DateTime, StringFormat='{0:T}'}\""
 + @"    Font='Large'
         HorizontalOptions='Center'
         VerticalOptions='Center' />
    <Button Text='Now' x:Name='button1' />
    <Button Text='bind now' Command='{Binding ClickCommand}' />
</StackLayout></ContentPage>";


            var page = PageXaml.LoadXaml<ContentPage>(xml);
            var vm = page.BindingContext as ClockViewModel;
            page.FindByName<Button>("button1").Clicked += (s, ee) =>
            {
                vm.DateTime = DateTime.Now;
            };
            this.Navigation.PushAsync(page);
        }
    }
}
