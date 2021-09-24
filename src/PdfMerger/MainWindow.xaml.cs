using MahApps.Metro.Controls;
using Maincotech.OfficeTools.Pdf.ViewModels;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Maincotech.OfficeTools.Pdf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IViewFor<MainWindowViewModel>
    {
        public MainWindowViewModel ViewModel { get; set; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = (MainWindowViewModel)value; }
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainWindowViewModel();

            this.WhenActivated(disposableRegistration =>
            {
                //// Notice we don't have to provide a converter, on WPF a global converter is
                //// registered which knows how to convert a boolean into visibility.
                ///
                //Bind Commands
                this.BindCommand(ViewModel, vm => vm.Add, v => v.AddButton).DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel, vm => vm.Remove, v => v.RemoveButton).DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel, vm => vm.MoveUp, v => v.MoveUpButton).DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel, vm => vm.MoveDown, v => v.MoveDownButton).DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel, vm => vm.Browse, v => v.BrowseButton).DisposeWith(disposableRegistration);
                this.BindCommand(ViewModel, vm => vm.Merge, v => v.MergeButton).DisposeWith(disposableRegistration);


                this.OneWayBind(ViewModel, vm => vm.HasSelected, v => v.RemoveButton.IsEnabled).DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel, vm => vm.HasSelected, v => v.MoveUpButton.IsEnabled).DisposeWith(disposableRegistration);
                this.OneWayBind(ViewModel, vm => vm.HasSelected, v => v.MoveDownButton.IsEnabled).DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel, vm => vm.Items, v => v.ItemsDataGrid.ItemsSource).DisposeWith(disposableRegistration);
                this.Bind(ViewModel, vm => vm.SelectedItem, v => v.ItemsDataGrid.SelectedItem).DisposeWith(disposableRegistration);

                this.Bind(ViewModel, vm => vm.TargetFilePath, v => v.TargetFileTextBox.Text).DisposeWith(disposableRegistration);

                //this.OneWayBind(ViewModel, vm => vm.IsActive, v => v.LoadingIndicator.IsActive).DisposeWith(disposableRegistration);
                //this.OneWayBind(ViewModel, vm => vm.CurrentView, v => v.View.Content).DisposeWith(disposableRegistration);
                //this.OneWayBind(ViewModel, vm => vm.CurrentSlip, v => v.payslipEditor.ViewModel).DisposeWith(disposableRegistration);

                //this.BindCommand(ViewModel, vm => vm.Print, v => v.PrintButton).DisposeWith(disposableRegistration);
                //this.OneWayBind(ViewModel, vm => vm.CanBatchPrint, v => v.BatchPrintButton.IsEnabled).DisposeWith(disposableRegistration);
                //this.BindCommand(ViewModel, vm => vm.BatchPrint, v => v.BatchPrintButton).DisposeWith(disposableRegistration);
                //this.BindCommand(ViewModel, vm => vm.ApplyKey, v => v.ApplyKeyButton).DisposeWith(disposableRegistration);

                //this.BindCommand(ViewModel, vm => vm.Save, v => v.SaveButton).DisposeWith(disposableRegistration);
                //this.BindCommand(ViewModel, vm => vm.Import, v => v.ImportButton).DisposeWith(disposableRegistration);
                //this.BindCommand(ViewModel, vm => vm.Export, v => v.ExportButton).DisposeWith(disposableRegistration);

                //this.OneWayBind(ViewModel, vm => vm.Histories, v => v.Histories.ItemsSource).DisposeWith(disposableRegistration);
                //this.OneWayBind(ViewModel, vm => vm.Title, v => v.Title).DisposeWith(disposableRegistration);

                //this.OneWayBind(ViewModel, vm => vm.Clients, v => v.Clients.ItemsSource).DisposeWith(disposableRegistration);
                //this.Bind(ViewModel, vm => vm.SearchTerm, v => v.SearchTextBox.Text).DisposeWith(disposableRegistration);

                //this.Bind(ViewModel, vm => vm.SelectedViewIndex, v => v.Views.SelectedIndex).DisposeWith(disposableRegistration);
            });
        }
    }
}
