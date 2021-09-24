using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maincotech.OfficeTools.Pdf.ViewModels
{
    public class PdfFileInfoViewModel : ReactiveObject
    {
        public string FilePath { get; set; }

        public int TotalPages { get; set; }

        public bool IsEncrypted { get; set; }

        private string _PrintPages;
        public string PrintPages
        {
            get => _PrintPages;
            set => this.RaiseAndSetIfChanged(ref _PrintPages, value);
        }

    }
}
