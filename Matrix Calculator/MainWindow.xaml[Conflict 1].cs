﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Matrix_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Begins by declaring and assigning arrays
        // 1d array used for positioning and modifying of textboxes
        NumericTextBox[] matrixX = new NumericTextBox[10000];
        // 2d array used for editing the contents of the textboxes (kinda not needed)
        NumericTextBox[,] matrixXY = new NumericTextBox[100, 100];
        // Boolean value used to keep track of the changes made in comboBoxMatrixSize
        bool changed = false;
        double[,] upperTri = new double[100, 100]; // Upper triangular matrix values
        double[,] lowerTri = new double[100, 100]; // Lower triangular matrix values

        public MainWindow()
        {
            InitializeComponent();
            // Assigns vlaues of matrixX
            for (int x = 0; x < 10000; x++)
            {
                matrixX[x] = new NumericTextBox();
            }
            matrixSizeFunc();
        }

        public int matrixSizeFunc()
        {
            return comboBoxMatrixSize.SelectedIndex + 2;
        }

        private void comboBoxMatrixSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Integer used to keep track of posisitioning
            int loop = 0;

            // Removes all the other NumericTextBoxes on the screen for new maniuplation
            if (changed == true)
            {
                grid1.Children.Clear();
            }
            changed = true;

            // Position numeric text boxes
            for (int nY = 0; nY < matrixSizeFunc(); nY++)
            {
                for (int nX = 0; nX < matrixSizeFunc(); nX++)
                {
                    matrixXY[nX, nY] = matrixX[nX + loop];
                    grid1.Children.Add(matrixX[nX + loop]);
                    Grid.SetColumn(matrixX[nX + loop], nX);
                    Grid.SetRow(matrixX[nX + loop], nY);
                    matrixX[nX + loop].Width = 30;
                    matrixX[nX + loop].Height = 20;
                }
                loop = loop + matrixSizeFunc();
            }
        }

        private void buttonSolveDet_Click_1(object sender, RoutedEventArgs e)
        {
            richTextBoxOutput.Document.Blocks.Add(new Paragraph(new Run("det(X) = " + solveDeterminant().ToString())));
        }

        private void buttonSolveInv_Click_1(object sender, RoutedEventArgs e)
        {
            if (solveDeterminant() == 0)
            {
                richTextBoxOutput.Document.Blocks.Add(new Paragraph(new Run("det(X) = " + solveDeterminant().ToString())));
                richTextBoxOutput.Document.Blocks.Add(new Paragraph(new Run("Determinant is 0 so there is no inverse")));                
            }
            else
            {
                richTextBoxOutput.Document.Blocks.Add(new Paragraph(new Run("det(X) = " + solveDeterminant().ToString())));
                richTextBoxOutput.Document.Blocks.Add(new Paragraph(new Run("inv(X) = " + solveInverse().ToString())));
            }
        }

        public double solveDeterminant()
        {
            // Declares numMatrix and assigns it as a new 2d array the size
            // of the matrix dimensions
            double[,] numMatrix = new double[matrixSizeFunc(), matrixSizeFunc()];
            for(int x = 0; x < (matrixSizeFunc()); x++)
            {
                for (int y = 0; y < (matrixSizeFunc()); y++)
                {
                    // Error checking. Looks for invalid characters or blank characters
                    // then defaults them to zero
                    try
                    {
                        if (matrixXY[x, y].Text == null || matrixXY[x, y].Text == "" || matrixXY[x, y].Text == "-" || matrixXY[x, y].Text == "+" || matrixXY[x, y].Text == ".")
                            matrixXY[x,y].Text = "0";
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    // Assigns the values of numMatrix to the new modified matrix 
                    numMatrix[y, x] = Convert.ToDouble(matrixXY[x, y].Text);
                }
            }
            return calculateDet(numMatrix);
        }

        public double calculateDet(double[,] matrix)
        {
            int dimension = matrix.GetUpperBound(0) + 1; // Dimensions of the Matrix
            double finalDet = 1; // Final Determinant
            double tempRow = 0; // Multiplier for the lower matrix

            // Assigns upperTri to the original matrix for later modifying
            for (int y = 0; y < dimension; y++)
            {
                for (int x = 0; x < dimension; x++)
                {
                    upperTri[x, y] = matrix[x, y];
                }
            }
            
            // Begin asigning the 2d arrays (lower & upper triangle matrix)
            for (int y = 0; y < (dimension - 1); y++)
            {
                for (int x = (y + 1); x < dimension; x++)
                {
                    // Checks for a zero in the diagonal (cannot divide a zero)
                    if (upperTri[y, y] == 0)
                    {
                        // Boolean value to keep track of switching rows to allow dividing
                        bool rowSwitched = false;
                        // When switching rows the final answer must be negative or positive
                        // depending on how many times you switch the rows
                        finalDet = finalDet * -1;
                        // Begin row switching
                        for (int z = (y + 1); z < dimension; z++)
                        {
                            // Makes sure the one below it is not zero else it cannot divide zero
                            if (upperTri[y, z] != 0)
                            {
                                // Row has been switched with the one below it
                                rowSwitched = true;
                                for (int w = 0; w < dimension; w++)
                                {
                                    tempRow = upperTri[y, w];
                                    upperTri[y, w] = upperTri[y, z];
                                    upperTri[y, z] = tempRow;
                                }
                            }
                        }
                        // If the entire collumn below the diagonal is zero the determinant is zero
                        if (rowSwitched == false)
                            return 0;
                    }
                    // Continue setting the values of the lower triangle matrix using LU Decompisition
                    lowerTri[x, y] = (upperTri[x, y] / upperTri[y, y]);
                    // Upper triangle matrix conatins all zeros on the upper triangle
                    upperTri[x, y] = 0;
                    for (int innerY = (y + 1); innerY < dimension; innerY++)
                    {
                        // Sets the rest of the values of the upper triangle matrix (used for inverse)
                        upperTri[x, innerY] = (upperTri[x, innerY] - (upperTri[y, innerY] * lowerTri[x, y]));
                    }
                }
                // The diagonal of the upper triangle matrix is 1
                lowerTri[y, y] = 1;
            }

            // Calculates the final determinant by multiplying the diagonal of the new matrix
            for (int x = 0; x < dimension; x++)
            {
                finalDet = finalDet * upperTri[x, x];
            }

            // If finalDet contains 0.99999999 or more it will just round it up
            if (Math.Truncate(finalDet) + 0.99999999 < finalDet || Math.Truncate(finalDet) - 0.99999999 > finalDet)
                finalDet = Math.Round(finalDet);

            return finalDet;
        }

        public double solveInverse()
        {
            double[] z = new double [matrixSizeFunc()];
            z[0] = 1;

            for(int x = 1; x < matrixSizeFunc(); x++)
            {
                z[x] = 0 - (z[x-1] * lowerTri[x, 0]);
                MessageBox.Show(z[x].ToString());
            }
            return 0;
        }

        private void buttonRandomInt_Click_1(object sender, RoutedEventArgs e)
        {
            // Small function that adds random doubles to the numeric textboxes (mostly for speed testing)
            Random rnd = new Random();
            double randomMatrix ;
            for (int x = 0; x < (matrixSizeFunc()) * (matrixSizeFunc()); x++)
            {
                randomMatrix = (rnd.Next(-40, 40) + rnd.NextDouble());
                try
                {
                    matrixX[x].Text = randomMatrix.ToString();
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        private void buttonClear_Click_1(object sender, RoutedEventArgs e)
        {
            // Basic loop that clears all the numerical text boxes
            foreach (NumericTextBox item in matrixX)
            {
                try
                {
                    item.Text = "";
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        private void buttonOutputLower_Click_1(object sender, RoutedEventArgs e)
        {
            for(int y = 0; y < matrixSizeFunc(); y++)
            {
                for(int x = 0; x < matrixSizeFunc(); x++)
                {
                    try
                    {
                        richTextBoxOutput.Document.Blocks.Add(new Paragraph(new Run(solveDeterminant().ToString())));
                    }
                    catch(Exception)
                    {
                        break;
                    }
                }
            }
        }

        private void buttonOutputUpper_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }

    public class NumericTextBox : TextBox
    {
        public NumericTextBox()
            : base()
        {
            this.AddHandler(NumericTextBox.PreviewKeyDownEvent, new RoutedEventHandler(HandleHandledKeyDown), true);
        }

        public void HandleHandledKeyDown(object sender, RoutedEventArgs e)
        {
            // Handles if space was pressed
            KeyEventArgs ke = e as KeyEventArgs;
            if (ke.Key == Key.Space)
            {
                ke.Handled = true;
            }
        }

        bool allowSpace = false;

        [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int MessageBeep(int uType);
        
        // Restricts the entry of characters to digits (including hex), the negative sign,
        // the decimal point, and editing keystrokes (backspace).
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
            string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
            string groupSeparator = numberFormatInfo.NumberGroupSeparator;
            string negativeSign = numberFormatInfo.NegativeSign;

            string keyInput = e.Text;

            if (Char.IsDigit(e.Text, (e.Text.Length - 1)))
            {
                // Digits are OK
            }
            else if ((keyInput.Equals(decimalSeparator) && !this.Text.Contains(".")) ||
                    (keyInput.Equals(negativeSign) && !this.Text.Contains("-") && this.Text == "") )
            {
                // Decimal separator is OK
            }
            else if (e.Text == "\b")
            {
                // Backspace key is OK
            }
            //    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
            //    {
            //     // Let the edit control handle control and alt key combinations
            //    }
            else if (this.allowSpace && e.Text == " ")
            {

            }
            else
            {
                // Swallow this invalid key and beep
                e.Handled = true;
                MessageBeep(0);
            }
            /*if (!(this.Text.Contains(".")))
                allowDecimal = true;
            else
                allowDecimal = false;*/
        }

        public int IntValue
        {
            get
            {
                return Int32.Parse(this.Text);
            }
        }

        public decimal DecimalValue
        {
            get
            {
                return Decimal.Parse(this.Text);
            }
        }

        public bool AllowSpace
        {
            set
            {
                this.allowSpace = value;
            }

            get
            {
                return this.allowSpace;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Checks for space then removes it
            base.OnKeyDown(e);
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
    }
}
