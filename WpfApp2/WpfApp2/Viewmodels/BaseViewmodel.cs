﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp2.Models;
using WpfApp2.Views;

namespace WpfApp2.Viewmodels
{
    public class BaseViewmodel
    {
        private Window Window;
        public BaseViewmodel() { }
        public BaseViewmodel(Window window)
        {
            Window = window;
        }
        public void CloseWindow()
        {
            if (Window != null)
            {
                Window.Close();
            }
        }
        public void EnterClick()
        {
            LoginView newWindow = new LoginView();
            newWindow.Show();
        }
    }
}
