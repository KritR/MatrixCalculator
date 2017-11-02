using System;
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
       NumericTextBox[] matrixX = new NumericTextBox[10000];
        NumericTextBox[,] matrixXY = new NumericTextBox[100, 100];
        int initialXPos = 30, initialYPos = 30;
        bool changed = false;

        public MainWindow()
        {
            InitializeComponent();
            for (int x = 0; x < 10000; x++)
            {
                matrixX[x] = new NumericTextBox();
            }
        }

        private void comboBoxMatrixSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int loop = 0;

            // Removes all the other NumericTextBoxes on the screen for new maniuplation
            if (changed == true)
            {
                grid1.Children.Clear();
            }
            changed = true;

            for (int nY = 0; nY < comboBoxMatrixSize.SelectedIndex + 2; nY++)
            {
                //MessageBox.Show(nY.ToString(), "Not Nested", 0);
                for (int nX = 0; nX < comboBoxMatrixSize.SelectedIndex + 2; nX++)
                {
                    matrixXY[nX, nY] = matrixX[nX + loop];
                    //matrixX[nX + loop].Margin = new Thickness(initialXPos,initialYPos,0,0);
                    grid1.Children.Add(matrixX[nX + loop]);
                    Grid.SetColumn(matrixX[nX + loop], nX);
                    Grid.SetRow(matrixX[nX + loop], nY);
                    matrixX[nX + loop].Width = 30;
                    matrixX[nX + loop].Height = 20;
                    initialXPos = initialXPos + 70;
                }
                initialXPos = 30;
                loop = loop + comboBoxMatrixSize.SelectedIndex + 2;
                initialYPos = initialYPos + 44;
            }
            // Resets the initial location of the first textbox
            initialXPos = 12;
            initialYPos = 12;
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
        }

        public double solveDeterminant()
        {
            // Very compact version using recursive functions and 2 dimensional arrays but computer intensive
            int dimensionXY = comboBoxMatrixSize.SelectedIndex + 2;
            double[,] numMatrix = new double[dimensionXY, dimensionXY];
            for(int x = 0; x < (comboBoxMatrixSize.SelectedIndex + 2); x++)
            {
                for (int y = 0; y < (comboBoxMatrixSize.SelectedIndex + 2); y++)
                {
                    try
                    {
                        if (matrixXY[x, y].Text == null || matrixXY[x, y].Text == "" || matrixXY[x, y].Text == "-" || matrixXY[x, y].Text == "+" || matrixXY[x, y].Text == ".")
                            matrixXY[x,y].Text = "0";
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    numMatrix[y, x] = Convert.ToDouble(matrixXY[x, y].Text);
                }
            }
            return solveDet(numMatrix);
        }

        public double solveDet(double[,] matrix)
        {
            double finalDet = 1; // Final Determinant
            int dimension = matrix.GetUpperBound(0) + 1; // Dimensions of the Matrix
            double tempRow = 0; // Multiplier for the lower matrix
            double[,] upperTri = new double[dimension, dimension] ; // Upper triangular matrix values
            double[,] lowerTri = new double[dimension, dimension]; // Lower triangular matrix values

            // Assigns upperTri to the original matrix for later modifying
            for (int y = 0; y < dimension; y++)
            {
                for (int x = 0; x < dimension; x++)
                {
                    upperTri[x, y] = matrix[x, y];
                }
            }

            for (int y = 0; y < (dimension - 1); y++)
            {
                for (int x = (y + 1); x < dimension; x++)
                {
                    if (upperTri[y, y] == 0)
                    {
                        bool rowSwitched = false;
                        finalDet = finalDet * -1;
                        for (int z = (y + 1); z < dimension; z++)
                        {
                            if (upperTri[y, z] != 0)
                            {
                                rowSwitched = true;
                                for (int w = 0; w < dimension; w++)
                                {
                                    tempRow = upperTri[y, w];
                                    upperTri[y, w] = upperTri[y, z];
                                    upperTri[y, z] = tempRow;
                                }
                            }
                        }
                        if (rowSwitched == false)
                            return 0;
                    }
                    lowerTri[x, y] = (upperTri[x, y] / upperTri[y, y]);
                    upperTri[x, y] = 0;
                    for (int innerY = (y + 1); innerY < dimension; innerY++)
                    {
                        upperTri[x, innerY] = (upperTri[x, innerY] - (upperTri[y, innerY] * lowerTri[x, y]));
                    }
                }
                lowerTri[y, y] = 1;
            }

            for (int x = 0; x < dimension; x++)
            {
                finalDet = finalDet * upperTri[x, x];
            }
            return finalDet;
        }

        private void buttonRandomInt_Click_1(object sender, RoutedEventArgs e)
        {
            Random rnd = new Random();
            double randomMatrix ;
            for (int x = 0; x < (comboBoxMatrixSize.SelectedIndex + 2) * (comboBoxMatrixSize.SelectedIndex + 2); x++)
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
            KeyEventArgs ke = e as KeyEventArgs;
            if (ke.Key == Key.Space)
            {
                ke.Handled = true;
            }
            else if (ke.Key == Key.LeftCtrl || ke.Key == Key.RightCtrl)
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
            base.OnKeyDown(e);
            if (e.Key == Key.Space)
            {
                MessageBox.Show(e.Key.ToString());
                e.Handled = true;
            }
        }
    }
}
