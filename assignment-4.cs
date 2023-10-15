using System;
using System.Collections.Generic;
using System.Windows.Forms;

class EquationSolver
{
    public static string Solve(string equation)
    {
        equation = equation.Replace(" ", "");

        if (!IsValid(equation))
        {
            return "Invalid equation";
        }

        Stack<char> operators = new Stack<char>();
        Stack<double> operands = new Stack<double>();
        int i = 0;

        while (i < equation.Length)
        {
            if (Char.IsDigit(equation[i]) || (i == 0 && equation[i] == '-'))
            {
                int start = i;
                while (i < equation.Length && (Char.IsDigit(equation[i]) || equation[i] == '.'))
                {
                    i++;
                }
                double operand = double.Parse(equation.Substring(start, i - start));
                operands.Push(operand);
            }
            else if (IsOperator(equation[i]))
            {
                while (operators.Count > 0 && HasPrecedence(equation[i], operators.Peek()))
                {
                    double operand2 = operands.Pop();
                    double operand1 = operands.Pop();
                    char op = operators.Pop();
                    operands.Push(PerformOperation(op, operand1, operand2));
                }
                operators.Push(equation[i]);
                i++;
            }
            else if (equation[i] == '(')
            {
                operators.Push(equation[i]);
                i++;
            }
            else if (equation[i] == ')')
            {
                while (operators.Count > 0 && operators.Peek() != '(')
                {
                    double operand2 = operands.Pop();
                    double operand1 = operands.Pop();
                    char op = operators.Pop();
                    operands.Push(PerformOperation(op, operand1, operand2));
                }
                if (operators.Count == 0)
                {
                    return "Invalid equation"; // Mismatched closing bracket
                }
                operators.Pop();
                i++;
            }
        }

        while (operators.Count > 0)
        {
            double operand2 = operands.Pop();
            double operand1 = operands.Pop();
            char op = operators.Pop();
            operands.Push(PerformOperation(op, operand1, operand2));
        }

        return operands.Pop().ToString();
    }

    private static bool IsOperator(char c)
    {
        return c == '+' || c == '-' || c == 'x' || c == '/';
    }

    private static int GetPrecedence(char op)
    {
        switch (op)
        {
            case '+':
            case '-':
                return 1;
            case 'x':
            case '/':
                return 2;
            default:
                return 0;
        }
    }

    private static bool HasPrecedence(char op1, char op2)
    {
        int precedence1 = GetPrecedence(op1);
        int precedence2 = GetPrecedence(op2);
        return precedence1 > precedence2 || (precedence1 == precedence2 && (op1 == '+' || op1 == '-'));
    }

    private static double PerformOperation(char op, double operand1, double operand2)
    {
        switch (op)
        {
            case '+':
                return operand1 + operand2;
            case '-':
                return operand1 - operand2;
            case 'x':
                return operand1 * operand2;
            case '/':
                if (operand2 == 0)
                {
                    throw new DivideByZeroException();
                }
                return operand1 / operand2;
            default:
                throw new ArgumentException("Invalid operator");
        }
    }

    private static bool IsValid(string equation)
    {
        int openBracketCount = 0;
        int closeBracketCount = 0;
        bool expectOperand = true;

        for (int i = 0; i < equation.Length; i++)
        {
            char c = equation[i];

            if (Char.IsDigit(c) || c == '.')
            {
                expectOperand = false;
            }
            else if (IsOperator(c))
            {
                if (expectOperand)
                {
                    return false;
                }
                expectOperand = true;
            }
            else if (c == '(')
            {
                openBracketCount++;
            }
            else if (c == ')')
            {
                closeBracketCount++;
                if (closeBracketCount > openBracketCount)
                {
                    return false;
                }
                expectOperand = false;
            }
            else
            {
                return false;
            }
        }

        if (openBracketCount != closeBracketCount)
        {
            return false;
        }

        if (expectOperand)
        {
            return false;
        }

        return true;
    }
}

class CalculatorForm : Form
{
    private TextBox textBoxInput;
    private Button buttonEquals;
    private Button buttonClear;
    private ListBox listBoxHistory;
    private List<string> history = new List<string>();

    public CalculatorForm()
    {
        this.Text = "Calculator";
        this.Size = new System.Drawing.Size(300, 400);

        textBoxInput = new TextBox();
        textBoxInput.Size = new System.Drawing.Size(260, 40);
        textBoxInput.Location = new System.Drawing.Point(10, 10);

        buttonEquals = new Button();
        buttonEquals.Size = new System.Drawing.Size(60, 60);
        buttonEquals.Location = new System.Drawing.Point(10, 80);
        buttonEquals.Text = "=";
        buttonEquals.Click += new EventHandler(ButtonEquals_Click);

        buttonClear = new Button();
        buttonClear.Size = new System.Drawing.Size(60, 60);
        buttonClear.Location = new System.Drawing.Point(80, 80);
        buttonClear.Text = "C";
        buttonClear.Click += new EventHandler(ButtonClear_Click);

        listBoxHistory = new ListBox();
        listBoxHistory.Size = new System.Drawing.Size(260, 200);
        listBoxHistory.Location = new System.Drawing.Point(10, 150);

        this.Controls.Add(textBoxInput);
        this.Controls.Add(buttonEquals);
        this.Controls.Add(buttonClear);
        this.Controls.Add(listBoxHistory);
    }

    private void ButtonEquals_Click(object sender, EventArgs e)
    {
        try
        {
            string input = textBoxInput.Text;
            string result = SolveEquation(input);
            textBoxInput.Text = result;
            AddToHistory(input, result);
        }
        catch (Exception ex)
        {
            textBoxInput.Text = "Error: " + ex.Message;
        }
    }

    private void ButtonClear_Click(object sender, EventArgs e)
    {
        textBoxInput.Clear();
    }

    private string SolveEquation(string equation)
    {
        try
        {
            string result = EquationSolver.Solve(equation);
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception("Error: " + ex.Message);
        }
    }

    private void AddToHistory(string equation, string result)
    {
        history.Add(equation + " = " + result);
        listBoxHistory.Items.Add(equation + " = " + result);
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new CalculatorForm());
    }
}

