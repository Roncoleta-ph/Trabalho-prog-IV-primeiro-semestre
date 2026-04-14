using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using umfg.venda.app.Abstracts;
using umfg.venda.app.ViewModels;

namespace umfg.venda.app.Commands
{
    internal sealed class FinalizarRecebimentoCommand : AbstractCommand
    {
        public override void Execute(object? parameter)
        {
            var viewModel = parameter as ReceberPedidoViewModel;

            if (viewModel == null)
            {
                MessageBox.Show("Erro interno ao processar pagamento.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<string> erros = new List<string>();

            if (viewModel.TipoCartaoSelecionado <= 0)
                erros.Add("- Selecione o tipo de cartão (Crédito ou Débito).");

            viewModel.NomeCartao = viewModel.NomeCartao?.ToUpper().Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(viewModel.NomeCartao))
            {
                erros.Add("- Nome no cartão é obrigatório.");
            }
            else if (!Regex.IsMatch(viewModel.NomeCartao, @"^[A-ZÀ-Ÿ]{2,}(?:\s[A-ZÀ-Ÿ]{2,})+$"))
            {
                erros.Add("- Nome inválido. Informe o nome completo como impresso no cartão (mínimo nome e sobrenome).");
            }

            string numeroLimpo = Regex.Replace(viewModel.NumeroCartao ?? string.Empty, @"\s+", "");
            if (string.IsNullOrWhiteSpace(numeroLimpo))
            {
                erros.Add("- Número do cartão é obrigatório.");
            }
            else if (!Regex.IsMatch(numeroLimpo, @"^\d{13,19}$"))
            {
                erros.Add("- O número do cartão deve conter entre 13 e 19 dígitos numéricos.");
            }
            else if (!ValidarCartaoLuhn(numeroLimpo))
            {
                erros.Add("- O número do cartão informado não é válido (falhou na verificação Luhn).");
            }

            if (string.IsNullOrWhiteSpace(viewModel.CVV))
            {
                erros.Add("- Código CVV é obrigatório.");
            }
            else if (!Regex.IsMatch(viewModel.CVV, @"^\d{3}$"))
            {
                erros.Add("- O CVV deve conter exatamente 3 dígitos numéricos.");
            }

            if (viewModel.DataValidade == null)
            {
                erros.Add("- Data de validade é obrigatória.");
            }
            else
            {
                var dataEscolhida = viewModel.DataValidade.Value;

                var ultimoDiaCartao = new DateTime(
                    dataEscolhida.Year,
                    dataEscolhida.Month,
                    DateTime.DaysInMonth(dataEscolhida.Year, dataEscolhida.Month)
                );

                if (ultimoDiaCartao < DateTime.Today)
                {
                    erros.Add($"- O cartão está vencido. Validade informada: {dataEscolhida:MM/yyyy}.");
                }
            }

            if (erros.Any())
            {
                MessageBox.Show(
                    "Não foi possível processar o pagamento:\n\n" + string.Join("\n", erros),
                    "Validação de Pagamento",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show(
                "Pagamento processado e finalizado com sucesso!",
                "Sucesso",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.DataContext = new MainWindowViewModel();
            }
        }

        /// <summary>
        /// Valida o número do cartão usando o algoritmo de Luhn.
        /// Cartões válidos (Visa, Mastercard, Elo, etc.) passam por essa verificação.
        /// Ponto extra do trabalho: validação real do número do cartão.
        /// </summary>
        private bool ValidarCartaoLuhn(string numero)
        {
            string digits = Regex.Replace(numero, @"\D", "");

            if (digits.Length < 13 || digits.Length > 19)
                return false;

            int sum = 0;
            bool alternate = false;

            for (int i = digits.Length - 1; i >= 0; i--)
            {
                int n = int.Parse(digits[i].ToString());

                if (alternate)
                {
                    n *= 2;
                    if (n > 9) n -= 9;
                }

                sum += n;
                alternate = !alternate;
            }

            return sum % 10 == 0;
        }
    }
}