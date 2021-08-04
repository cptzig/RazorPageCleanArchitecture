using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SmartAdmin.WebUI.Pages.Ui
{
    public class AccordionModel : PageModel
    {
        private readonly ILogger<AccordionModel> _logger;

        public AccordionModel(ILogger<AccordionModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
