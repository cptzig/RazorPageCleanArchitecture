using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SmartAdmin.WebUI.Pages.Ui
{
    public class ModalModel : PageModel
    {
        private readonly ILogger<ModalModel> _logger;

        public ModalModel(ILogger<ModalModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
