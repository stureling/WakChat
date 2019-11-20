using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp2.Viewmodels.Commands;
using WpfApp2.Views;

namespace WpfApp2.Viewmodels
{
    public class LoginViewmodel : BaseViewmodel
    {

        private int _port = 0;
        private string _username = "";
        private string _ip = "";
        private bool _enableConnect;
        private bool _enableListen;

        public int Port
        {
            get 
            {
                return _port;
            }
            set
            {
                if (value < 1024 || value > 65535)
                {
                    MessageBoxResult result = MessageBox.Show("INVALID PORT", "ERROR", MessageBoxButton.OK);
                    _port = 0;
                }
                else
                {
                    _port = value;
                    Debug.WriteLine(_port);
                }
            }
        }

        public string User
        {
            get
            {
                return _username;
            }
            set
            {
                if (value.Length == 0)
                {
                    _username = "Anon";
                }

                else if (value.Length > 24)
                {
                    MessageBoxResult result = MessageBox.Show("USERNAME TOO BIG", "ERROR", MessageBoxButton.OK);
                    _username = "";
                }

                else
                {
                    _username = value;
                    Debug.WriteLine(_username);
                }
            }
        }

        public string IP
        {
            get
            {
                return _ip;
            }
            set
            {
                if (!Regex.Match(value, @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$").Success)
                {
                    MessageBoxResult result = MessageBox.Show("INVALID IP", "ERROR", MessageBoxButton.OK);
                    _ip = "";
                }
                else
                {
                    _ip = value;

                    Debug.WriteLine(_ip);
                }
            }
        }

        public bool EnableConnect
        {
            get 
            {
                return _enableConnect;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(_username) || String.IsNullOrWhiteSpace(_ip) || String.IsNullOrWhiteSpace(_port.ToString()))
                {
                    _enableConnect = false;
                }
                else
                {
                    _enableConnect = true;
                }
            }
        }

        public bool EnableListen
        {
            get
            {
                return _enableListen;
            }
            set
            {
                if (String.IsNullOrWhiteSpace(_username) || String.IsNullOrWhiteSpace(_port.ToString()))
                {
                    _enableListen = false;
                }
                else
                {
                    _enableListen = true;
                }
            }
        }

        public ICommand ExitWindowCommand { get; set; }
        public ICommand EnterWindowCommand { get; set; }

        public LoginViewmodel(Window window) : base(window)
        {
            this.ExitWindowCommand = new ExitWindowCommand(this);
            this.EnterWindowCommand = new EnterWindowCommand(this);
        }
    }
}
