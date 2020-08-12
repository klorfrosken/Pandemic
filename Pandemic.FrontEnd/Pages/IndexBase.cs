using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pandemic.FrontEnd.Components;

namespace Pandemic.FrontEnd.Pages
{
    public class IndexBase : ComponentBase
    {
        protected SetDifficultyDialogBase SetDifficultyDialog { get; set; }

        protected void LetsGetStarted()
        {
            SetDifficultyDialog.Show();
        }
    }
}
