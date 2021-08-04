using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace SmartAdmin.WebUI.Pages.Icons
{
    public class FontawesomeLightModel : PageModel
    {
        private readonly ILogger<FontawesomeLightModel> _logger;

        public FontawesomeLightModel(ILogger<FontawesomeLightModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
