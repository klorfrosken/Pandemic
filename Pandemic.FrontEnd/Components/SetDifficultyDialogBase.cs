using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pandemic.FrontEnd.Components
{
    public class SetDifficultyDialogBase : ComponentBase
    {
        public bool ShowDialog { get; set; }

        protected void IntroductoryGame()
        {

        }

        protected void StandardGame()
        {

        }

        protected void HeroicGame()
        {

        }

        public void Show()
        {
            ShowDialog = true;
            StateHasChanged();
        }
    }
}
