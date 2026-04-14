using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using umfg.venda.app.Interfaces;
using umfg.venda.app.Models;
using umfg.venda.app.ViewModels;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System;
using System.Windows;

namespace umfg.venda.app.UserControls
{
    /// <summary>
    /// Interação lógica para ucReceberPedido.xaml
    /// </summary>
    /// 
    ///
    public partial class ucReceberPedido : UserControl
    {
        private ucReceberPedido(IObserver observer, PedidoModel pedido)
        {
            InitializeComponent();
            DataContext = new ReceberPedidoViewModel(this, observer, pedido);
        }

        internal static void Exibir(IObserver observer, PedidoModel pedido)
        {
            (new ucReceberPedido(observer, pedido).DataContext as ReceberPedidoViewModel).Notify();
        }

        private void ValidarSomenteNumeros(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ValidarSomenteLetrasEEspaco(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-ZÀ-ÿ ]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DatePicker_DateValidationError(object sender, DatePickerDateValidationErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void DatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            if (sender is DatePicker dp)
            {
                dp.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (dp.Template.FindName("PART_Popup", dp) is Popup popup &&
                        popup.Child is Calendar cal)
                    {
                        cal.DisplayMode = CalendarMode.Year;
                        cal.DisplayModeChanged += Cal_DisplayModeChanged;
                    }
                }));
            }
        }

        private void Cal_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            if (sender is Calendar cal && cal.DisplayMode == CalendarMode.Month)
            {
                cal.DisplayMode = CalendarMode.Year;
            }
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DatePicker dp && dp.SelectedDate.HasValue)
            {
                var d = dp.SelectedDate.Value;
                var normalizado = new DateTime(d.Year, d.Month, 1);
                if (dp.SelectedDate.Value != normalizado)
                    dp.SelectedDate = normalizado;
            }
        }
    }
}
