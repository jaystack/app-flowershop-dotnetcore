using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Flowershop.ViewModels
{
    public class IndexViewModel
    {
        public string ItemsContent { get; set; }

        public string SummaryContent { get; set; }

        public string CheckoutContent { get; set; }
    }
}
