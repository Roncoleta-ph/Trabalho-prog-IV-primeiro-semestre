using System.Windows.Controls;
using umfg.venda.app.Interfaces;
using umfg.venda.app.Models;
using umfg.venda.app.ViewModels;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace umfg.venda.app.UserControls
{
    /// <summary>
    /// Interação lógica para ucReceberPedido.xaml
    /// </summary>
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

        private void ValidarSomenteLetras(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
