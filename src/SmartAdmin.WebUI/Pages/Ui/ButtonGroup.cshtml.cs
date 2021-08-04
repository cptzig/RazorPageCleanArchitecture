using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SmartAdmin.WebUI.Pages.Ui
{
    public class ButtonGroupModel : PageModel
    {
        private readonly ILogger<ButtonGroupModel> _logger;

        public ButtonGroupModel(ILogger<ButtonGroupModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
