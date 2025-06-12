﻿using BusinessObjects;
using Services;
using System.Windows;

namespace WPFApp
{
    /// <summary> Interaction logic for LoginWindow.xaml
    public partial class LoginWindow : Window
    {
        private readonly IAccountService iAccountService;

        public LoginWindow()
        {
            InitializeComponent();
            iAccountService = new AccountService();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            AccountMember account = iAccountService.GetAccountById(txtUser.Text);
            try
            {
                if (account != null && account.MemberPassword.Equals(txtPass.Password) &&
                account.MemberRole == 1)
                {
                    this.Hide();
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                }
                else
                {
                    MessageBox.Show("You are not permission !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login Error: Invalid user infomation");
            }
            
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}