namespace Behavior.Examples;

public class Calculator
{
    public decimal Operand1 { get; set; }

    public decimal Operand2 { get; set; }

    public decimal Result { get; set; }

    public void Add() => Result = Operand1 + Operand2;

    public void Subtract() => Result = Operand1 - Operand2;
}
