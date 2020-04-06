using System;
using System.Windows;
using Aaron.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace Aaron.Services
{
    /// <summary>
    /// Provides an interface to send alerts to the user.
    /// </summary>
    public class AlertService
    {
        public AlertService()
        {
            Messenger.Default.Register<AlertMessage>(this, HandleAlertMessage);
        }

        private void HandleAlertMessage(AlertMessage message)
        {
            switch (message.Type)
            {
                case AlertType.Information:
                    MessageBox.Show(message.Body, message.Title, MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case AlertType.Warning:
                    MessageBox.Show(message.Body, message.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case AlertType.Error:
                    MessageBox.Show(message.Body, message.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}