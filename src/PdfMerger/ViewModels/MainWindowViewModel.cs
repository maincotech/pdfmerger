using iTextSharp.text;
using iTextSharp.text.pdf;
using Maincotech.OfficeTools.Pdf.Controls;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Maincotech.OfficeTools.Pdf.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        private ObservableCollection<PdfFileInfoViewModel> _Items;

        public ObservableCollection<PdfFileInfoViewModel> Items
        {
            get
            {
                if (_Items == null)
                {
                    _Items = new ObservableCollection<PdfFileInfoViewModel>();
                }

                return _Items;
            }
        }

        private PdfFileInfoViewModel _SelectedItem;

        public PdfFileInfoViewModel SelectedItem
        {
            get => _SelectedItem;
            set
            {
                this.RaiseAndSetIfChanged(ref _SelectedItem, value);
                this.RaisePropertyChanged(nameof(HasSelected));
            }
        }

        private string _TargetFilePath;

        public string TargetFilePath
        {
            get => _TargetFilePath;
            set => this.RaiseAndSetIfChanged(ref _TargetFilePath, value);
        }

        public bool HasSelected => SelectedItem != null;

        public MainWindowViewModel()
        {
            MoveUp = ReactiveCommand.Create(() =>
            {
                if (SelectedItem == null)
                {
                    return;
                }

                int currentIndex = Items.IndexOf(SelectedItem);
                if (currentIndex > 0)
                {
                    Items.Move(currentIndex, currentIndex - 1);
                }
            });

            MoveDown = ReactiveCommand.Create(() =>
            {
                if (SelectedItem == null)
                {
                    return;
                }
                int currentIndex = Items.IndexOf(SelectedItem);
                if (currentIndex != -1 && currentIndex < Items.Count - 1)
                {
                    Items.Move(currentIndex, currentIndex + 1);
                }
            });

            Add = ReactiveCommand.Create(() =>
            {
                using (OpenFileDialog dialog = new OpenFileDialog
                {
                    Title = "Select PDF File",
                    Filter = "PDF(*.pdf)|*.pdf"
                })
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            //load file and add item
                            using (PdfReader reader = new PdfReader(dialog.FileName))
                            {
                                PdfFileInfoViewModel item = new PdfFileInfoViewModel
                                {
                                    FilePath = dialog.FileName,
                                    PrintPages = "All",
                                    IsEncrypted = reader.IsEncrypted(),
                                    TotalPages = reader.NumberOfPages
                                };
                                Items.Add(item);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
            });

            Remove = ReactiveCommand.Create(() =>
            {
                if (SelectedItem == null)
                {
                    return;
                }
                Items.Remove(SelectedItem);
            });

            Browse = ReactiveCommand.Create(() =>
            {
                using (SaveFileDialog dialog = new SaveFileDialog
                {
                    Title = "Select PDF File",
                    Filter = "PDF(*.pdf)|*.pdf"
                })
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        TargetFilePath = dialog.FileName;
                    }
                }
            });

            Merge = ReactiveCommand.Create(() =>
            {
                IsActive = true;
                DialogHost.Show(new LoadingIndicator(), "RootDialog");
                Task.Run(() => MergeFiles());
            });
        }

        private bool _IsActive;

        public bool IsActive
        {
            get => _IsActive;
            set => this.RaiseAndSetIfChanged(ref _IsActive, value);
        }

        public ReactiveCommand<Unit, Unit> MoveUp { get; }
        public ReactiveCommand<Unit, Unit> MoveDown { get; }

        public ReactiveCommand<Unit, Unit> Add { get; }
        public ReactiveCommand<Unit, Unit> Remove { get; }

        public ReactiveCommand<Unit, Unit> Browse { get; }
        public ReactiveCommand<Unit, Unit> Merge { get; }

        private void MergeFiles()
        {
            try
            {
                //Check source files
                if (Items.Count == 0)
                {
                    MessageBox.Show("Please add some pdf files.");
                    return;
                }

                //Check target file
                if (string.IsNullOrEmpty(TargetFilePath))
                {
                    MessageBox.Show("Please specify the target file.");
                    return;
                }

                //Merge files
                using (var doc = new Document())
                {
                    var pdfCopyProvider = new PdfCopy(doc, new System.IO.FileStream(TargetFilePath, FileMode.Create));
                    doc.Open();

                    foreach (var item in Items)
                    {
                        var reader = new PdfReader(item.FilePath);

                        if (item.PrintPages.Equals("all", StringComparison.OrdinalIgnoreCase))
                        {
                            for (int i = 1; i <= reader.NumberOfPages; i++)
                            {
                                var importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                                pdfCopyProvider.AddPage(importedPage);
                            }
                        }
                        else
                        {
                            var ranges = item.PrintPages.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var range in ranges)
                            {
                                if (range.Contains("-"))
                                {
                                    var startAndEnd = range.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                                    for (int i = Convert.ToInt32(startAndEnd[0]); i <= Convert.ToInt32(startAndEnd[1]); i++)
                                    {
                                        var importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                                        pdfCopyProvider.AddPage(importedPage);
                                    }
                                }
                                else
                                {
                                    var importedPage = pdfCopyProvider.GetImportedPage(reader, Convert.ToInt32(range));
                                    pdfCopyProvider.AddPage(importedPage);
                                }
                            }

                            reader.Close();
                        }
                    }
                    doc.Close();
                }

                Process.Start(TargetFilePath);
            }
            finally
            {
                IsActive = false;
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    DialogHost.Close("RootDialog");
                    //  DialogHost.CloseDialogCommand.Execute(false, null);
                }));
            }
        }
    }
}