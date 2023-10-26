// Impletação da classe Forma de Pagamento usando os conceitos da aula anterior 
// (interfaces e classes abstratas) 

public interface IPagar
{
    public void Pagar(double valor);
}

public interface IDesconto
{
    public void Pagar(double valor, double desconto);
}

public interface IParcelado
{
    public void Pagar(double valor, int parcelas);
}


public class FormaDePagamento : IPagar
{
    public void Pagar(double valor)
    {
        Console.WriteLine($"Conta Paga! Valor: {valor}");
    }
}

public class PagamentoComDesconto : FormaDePagamento, IDesconto
{
    public double Desconto { get; set; }

    public void Pagar(double valor, double desconto)
    {
        Console.WriteLine($"Conta Paga! Valor: {valor - desconto}");
    }
}

public class PagamentoParcelado : FormaDePagamento, IParcelado
{
    public void Pagar(double valor, int parcelas)
    {
        Console.WriteLine($"Conta Paga! Valor: {valor} em ");
    }
}

public class ProcessaPagamentos
{
    public void AplicarPagamento(IPagar formaDePagamento, double valor)
    {
        formaDePagamento.Pagar(valor);
    }
}

public class ProcessaPagamentosComDesconto : ProcessaPagamentos
{
    public void AplicarPagamento(IDesconto formaDePagamento, double valor, double desconto)
    {
        formaDePagamento.Pagar(valor, desconto);
    }
}

class Program
{
    static void Main(string[] args)
    {
        FormaDePagamento formaDePagamento = new FormaDePagamento();
        formaDePagamento.Pagar(100);

        PagamentoComDesconto pagamentoComDesconto = new PagamentoComDesconto();
        pagamentoComDesconto.Pagar(100, 10);

        FormaDePagamento formaDePagamentoComDesconto = new PagamentoComDesconto();
        formaDePagamentoComDesconto.Pagar(100);
        // Não funciona -> (PagamentoComDesconto)formaDePagamentoComDesconto.Pagar(100,10);
        PagamentoComDesconto formaDePagamentoComDescontoCasting = (PagamentoComDesconto)formaDePagamentoComDesconto;
        formaDePagamentoComDescontoCasting.Pagar(100, 10);
        formaDePagamentoComDescontoCasting.Pagar(100);

        //var pagamentoComDesconto = (PagamentoComDesconto) formaDePagamento;
        //pagamentoComDesconto.Pagar(100,10);

        //PagamentoComDesconto pagamentoComDesconto = new PagamentoComDesconto();
        //pagamentoComDesconto.Pagar(100, 10);

        ProcessaPagamentos processaPagamentos = new ProcessaPagamentos();
        processaPagamentos.AplicarPagamento(formaDePagamento, 300);
        processaPagamentos.AplicarPagamento(pagamentoComDesconto, 400);
        processaPagamentos.AplicarPagamento(formaDePagamentoComDescontoCasting, 500);
    }
}