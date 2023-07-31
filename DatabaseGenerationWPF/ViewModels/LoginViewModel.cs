using DatabaseGenerationWPF.Views;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseGenerationWPF.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        private User loginUser;

        public LoginViewModel()
        {

        }

        public User LoginUser { get => loginUser; set { SetProperty(ref loginUser, value); } }
    }
}
