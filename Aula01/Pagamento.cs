using System.Numerics;
using System.Reflection;

namespace Pagamento;

public interface IPagamento
{
    public void Pagar(double valor);
}

public abstract class FormaPagamento : IPagamento
{
    public abstract void Pagar(double valor);
}

public class Boleto : FormaPagamento
{
    public Boleto(int codigoBoleto)
    {
        CodigoBoleto = codigoBoleto;
    }
    public int CodigoBoleto { get; }
    public override void Pagar(double valor) => Console.WriteLine($"Boleto {CodigoBoleto} emitido no valor de {valor}");
}

public interface IProcessaPagamento
{
    public void ProcessarPagamento<T>(T formaPagamento, double valor) where T : IPagamento => formaPagamento.Pagar(valor);
}

public class ProcessaPagamento : IProcessaPagamento
{
    public void ProcessarPagamento<T>(T formaPagamento, double valor) where T : IPagamento => formaPagamento.Pagar(valor);
}

public interface IParcelamento
{
    public int Parcelas { get; set; }
    public double ValorParcelas(double valor);
}

// Extendendo abstrações
public abstract class FormaPagamentoParcelada : FormaPagamento, IParcelamento
{
    public int Parcelas { get; set; } = 1;
    public abstract double ValorParcelas(double valor);
}

public class CarneLoja : FormaPagamentoParcelada
{
    public override double ValorParcelas(double valor) => valor / Parcelas;
    public override void Pagar(double valor)
    {
        Console.WriteLine($"Pagamento realizado! Valor total: {valor}. Valor da parcela: {ValorParcelas(valor)}. Número de parcelas: {Parcelas}x");
    }
}

public class ProcessaPagamentoParcelado : ProcessaPagamento
{
    public void ProcessarPagamento<T>(T formaPagamento, double valor, int parcelas) where T : IPagamento, IParcelamento
    {
        formaPagamento.Parcelas = parcelas;
        formaPagamento.Pagar(valor);
    }
}

public interface IJuros
{
    public double TaxaJuros { get; set; }
    public double ValorJuros(double valor);
    public double ValorTotal(double valor);
}

// Extendendo abstrações
public abstract class FormaPagamentoParceladaComJuros : FormaPagamentoParcelada, IJuros
{
    public double TaxaJuros { get; set; } = 0.1;
    public double ValorJuros(double valor) => valor * TaxaJuros;
    public double ValorTotal(double valor) => valor + ValorJuros(valor);
    public override double ValorParcelas(double valor) => ValorTotal(valor) / Parcelas;
}

public class Cartao : FormaPagamentoParceladaComJuros, IJuros
{
    public override void Pagar(double valor)
    {
        Console.WriteLine($"Pagamento realizado! Valor total: {ValorTotal(valor)}. Valor juros: {ValorJuros(valor)}. Valor da parcela: {ValorParcelas(valor)}. Número de parcelas: {Parcelas}x");
    }
}

public class ProcessaPagamentoParceladoComJuros : ProcessaPagamentoParcelado
{
    public void ProcessarPagamento<T>(T formaPagamento, double valor, int parcelas, double juros) where T : IPagamento, IParcelamento, IJuros
    {
        formaPagamento.TaxaJuros = juros;
        formaPagamento.Parcelas = parcelas;
        formaPagamento.Pagar(valor);
    }
}

public static class Exemplo
{
    public static void Run()
    {
        // A linha abaixo não funciona pois não é possível criar uma instância de uma classe abstrada 
        // IPagamento formaPagamento = new FormaPagamento(); 

        // Criando um boleto
        Boleto boletoObj = new Boleto(1); // podemos acessar boletoObj.CodigoBoleto;
        boletoObj.Pagar(100);

        FormaPagamento boletoFormaPagamento = new Boleto(2); // não podemos acessar boletoObj.CodigoBoleto;
        boletoFormaPagamento.Pagar(200);

        Boleto boletoCast = (Boleto)boletoFormaPagamento; // podemos acessar boletoObj.CodigoBoleto;
        boletoCast.Pagar(20);

        IPagamento boletoInteface = new Boleto(3);
        boletoInteface.Pagar(300);

        Boleto boletoCastInterface = (Boleto)boletoInteface; // podemos acessar boletoObj.CodigoBoleto;
        boletoCastInterface.Pagar(20);

        //Criando um Carnê
        FormaPagamento carneLojaFormaPagamento = new CarneLoja();
        FormaPagamentoParcelada carneLojaParcelada = new CarneLoja();

        //Criando Cartao
        FormaPagamento cartaoFormaPagamento = new Cartao();
        FormaPagamentoParcelada cartaoParcelada = new Cartao();
        FormaPagamentoParceladaComJuros cartaoComJuros = new Cartao();

        //Criando processador de compra genérico
        ProcessaPagamento processaPagamentoGenerico = new ProcessaPagamento();

        //Usando processador genérico
        processaPagamentoGenerico.ProcessarPagamento(boletoFormaPagamento, 300); // não funciona com boletoInteface
        processaPagamentoGenerico.ProcessarPagamento(carneLojaFormaPagamento, 1000);
        processaPagamentoGenerico.ProcessarPagamento(cartaoFormaPagamento, 500);

        //Criando processador de compra parcelado
        ProcessaPagamentoParcelado processaPagamentoParcelado = new ProcessaPagamentoParcelado();

        //Usando processador genérico
        processaPagamentoParcelado.ProcessarPagamento(carneLojaParcelada, 300);
        processaPagamentoParcelado.ProcessarPagamento(cartaoParcelada, 500, 10);
        processaPagamentoParcelado.ProcessarPagamento(cartaoComJuros, 500);

        //Criando processador de compra genérico
        ProcessaPagamentoParceladoComJuros processaPagamentoParceladoComJuros = new ProcessaPagamentoParceladoComJuros();

        //Usando processador genérico
        processaPagamentoParceladoComJuros.ProcessarPagamento(cartaoComJuros, 300);
        processaPagamentoParceladoComJuros.ProcessarPagamento(cartaoComJuros, 300, 2);
        processaPagamentoParceladoComJuros.ProcessarPagamento(cartaoComJuros, 300, 10, 0.8);
    }
}