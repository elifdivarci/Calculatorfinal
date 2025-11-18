using System.Globalization;
namespace Calculator__ders;
public partial class MainPage : ContentPage
{
    private double firstNumber = 0;
    private string currentOperator = "";
    private bool isNewEntry = true;

    public MainPage()
    {
        InitializeComponent();
    }
    private bool TryParseInput(string input, out double result)
    {
        string invariantInput = input.Replace(',', '.');
        return double.TryParse(
            invariantInput, 
            NumberStyles.Any, 
            CultureInfo.InvariantCulture, 
            out result
        );
    }
    private void OnNumberPressed(object? sender, EventArgs e)
    {
        Button btn = sender as Button;

        if (isNewEntry)
        {
            Display.Text = "";
            isNewEntry = false;
        }

        if (btn.Text == ".")
        {
            string currentNumber = GetCurrentNumber();
            if (currentNumber.Contains(".") || currentNumber.Contains(","))
                return; 
            
            if (string.IsNullOrEmpty(currentNumber))
                Display.Text += "0"; 
        }

        Display.Text += btn.Text;
    }
    private void OnOperatorPressed(object? sender, EventArgs e)
    {
        Button btn = sender as Button;
        string op = btn.Text;

        // CE - Clear Everything
        if (op == "CE")
        {
            firstNumber = 0;
            currentOperator = "";
            Display.Text = "";
            isNewEntry = true;
            return;
        }

        if (op == "C")
        {
            if (Display.Text.Length > 0)
            {
                string lastChar = Display.Text[^1].ToString();
                Display.Text = Display.Text[..^1];
                
                if (lastChar == "+" || lastChar == "-" || lastChar == "*" || 
                    lastChar == "/" || lastChar == "%" || lastChar == "√" || lastChar == "²")
                {
                    currentOperator = "";
                    isNewEntry = false;
                }
     
                if (string.IsNullOrEmpty(Display.Text))
                {
                    isNewEntry = true;
                }
            }
            return;
        }

        if (op == "√" || op == "x²")
        {
            if (string.IsNullOrEmpty(currentOperator))
            {
                currentOperator = op;
                Display.Text += op;
                isNewEntry = false;
            }
            return;
        }

        if (op == "=")
        {
            if (string.IsNullOrEmpty(currentOperator))
                return;

            double result = CalculateResult();
            Display.Text = result.ToString(CultureInfo.InvariantCulture);
            firstNumber = result;
            currentOperator = "";
            isNewEntry = true;
            return;
        }
        if (string.IsNullOrEmpty(currentOperator))
        {
            string currentNum = GetCurrentNumber();
            if (!string.IsNullOrEmpty(currentNum) && TryParseInput(currentNum, out firstNumber))
            {
                currentOperator = op;
                Display.Text += op;
                isNewEntry = false;
            }
        }
        else
        {
            if (!isNewEntry)
            {
                double result = CalculateResult();
                Display.Text = result.ToString(CultureInfo.InvariantCulture) + op;
                firstNumber = result;
                currentOperator = op;
                isNewEntry = false;
            }
            else
            {
                Display.Text = firstNumber.ToString(CultureInfo.InvariantCulture) + op;
                currentOperator = op;
            }
        }
    }
    private double CalculateResult()
    {
        string secondNumStr = GetCurrentNumber();
        
        if (string.IsNullOrEmpty(secondNumStr))
            return firstNumber;

        if (!TryParseInput(secondNumStr, out double secondNumber))
            return firstNumber;

        return currentOperator switch
        {
            "+" => firstNumber + secondNumber,
            "-" => firstNumber - secondNumber,
            "*" => firstNumber * secondNumber,
            "/" => secondNumber != 0 ? firstNumber / secondNumber : 0,
            "%" => firstNumber % secondNumber,
            "√" => Math.Sqrt(secondNumber),
            "x²" => secondNumber * secondNumber,
            _ => firstNumber
        };
    }

    private string GetCurrentNumber()
    {
        string text = Display.Text;
        if (string.IsNullOrEmpty(text))
            return "";
        
        if (text.Contains("x²"))
        {
            int xSquaredIndex = text.IndexOf("x²");
            return text.Substring(xSquaredIndex + 2);
        }
        
        for (int i = text.Length - 1; i >= 0; i--)
        {
            char c = text[i];
            
            if (c == '+' || c == '*' || c == '/' || c == '%' || c == '√' || c == '²')
            {
                return text.Substring(i + 1);
            }
            
            if (c == 'x' && i + 1 < text.Length && text[i + 1] == '²')
            {
                continue;
            }
            
            if (c == '-')
            {
                if (i == 0)
                    return text;
                char prevChar = text[i - 1];
                if (prevChar == '+' || prevChar == '*' || prevChar == '/' || 
                    prevChar == '%' || prevChar == '√' || prevChar == '²' || prevChar == 'x')
                {
                    return text.Substring(i);
                }
                return text.Substring(i + 1);
            }
        }
        return text;
    }
}