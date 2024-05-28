using System;
using System.Drawing;
using System.Windows.Forms;
namespace Gobang2
{
    public partial class Form1 : Form
    {
        private const int Size = 15;
        private const int ButtonSize = 50;
        private Button[,] buttons = new Button[Size, Size];
        private bool isPlayer1Turn = true; // 玩家1先手
        private Label currentPlayerLabel;
        private ListBox moveHistoryListBox;
        private Label blackWinsLabel;
        private Label whiteWinsLabel;
        private int moveCount = 0;
        private int blackWins = 0;
        private int whiteWins = 0;
        public Form1()
        {
            InitializeComponent();
            InitializeBoard();
            InitializeUI();
        }
        private void InitializeBoard()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    buttons[i, j] = new Button();
                    buttons[i, j].Size = new Size(ButtonSize, ButtonSize);
                    buttons[i, j].Location = new Point(i * ButtonSize, j * ButtonSize);
                    buttons[i, j].Name = $"button_{i}_{j}";
                    buttons[i, j].Click += Button_Click;
                    buttons[i, j].Paint += Button_Paint;
                    this.Controls.Add(buttons[i, j]);
                }
            }
        }

        private void InitializeUI()
        {
            currentPlayerLabel = new Label();
            currentPlayerLabel.AutoSize = true;
            currentPlayerLabel.Location = new Point(Size * ButtonSize + 20, 20);
            UpdateCurrentPlayerLabel();
            this.Controls.Add(currentPlayerLabel);

            moveHistoryListBox = new ListBox();
            moveHistoryListBox.Size = new Size(220, 200); // 調整 ListBox 寬度
            moveHistoryListBox.Location = new Point(Size * ButtonSize + 20, 50);
            this.Controls.Add(moveHistoryListBox);

            blackWinsLabel = CreateAndSetupLabel(new Point(Size * ButtonSize + 20, 260), $"黑子獲勝次數：{blackWins}");
            this.Controls.Add(blackWinsLabel);

            whiteWinsLabel = CreateAndSetupLabel(new Point(Size * ButtonSize + 20, 290), $"白子獲勝次數：{whiteWins}");
            this.Controls.Add(whiteWinsLabel);
        }

        private Label CreateAndSetupLabel(Point location, string text)
        {
            Label label = new Label();
            label.AutoSize = true;
            label.Location = location;
            label.Text = text;
            return label;
        }

        private void UpdateCurrentPlayerLabel()
        {
            currentPlayerLabel.Text = isPlayer1Turn ? "當前玩家：黑子" : "當前玩家：白子";
        }

        private void UpdateMoveHistory(string move)
        {
            moveHistoryListBox.Items.Add(move);
            moveHistoryListBox.TopIndex = moveHistoryListBox.Items.Count - 1; // 自動滾動到最後一項
        }

        private void UpdateWinLabels()
        {
            if (blackWinsLabel != null)
            {
                blackWinsLabel.Text = $"黑子獲勝次數：{blackWins}";
            }
            if (whiteWinsLabel != null)
            {
                whiteWinsLabel.Text = $"白子獲勝次數：{whiteWins}";
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null && clickedButton.Tag == null)
            {
                string moveSymbol = isPlayer1Turn ? "black" : "white";
                clickedButton.Tag = moveSymbol; // 使用 Tag 來保存顏色信息
                clickedButton.Enabled = false;

                moveCount++;
                UpdateMoveHistory($"Move {moveCount}: {(isPlayer1Turn ? "●" : "○")}");

                if (CheckForWin(clickedButton))
                {
                    string winner = isPlayer1Turn ? "黑子" : "白子";
                    MessageBox.Show($"恭喜！{winner}獲勝！");
                    UpdateMoveHistory($"恭喜！{winner}獲勝！");
                    if (isPlayer1Turn)
                    {
                        blackWins++;
                    }
                    else
                    {
                        whiteWins++;
                    }
                    UpdateWinLabels();
                    moveCount = 0; // 重置 moveCount
                    ResetBoard();
                }
                else
                {
                    // 如果沒有勝利，換下一位玩家
                    isPlayer1Turn = !isPlayer1Turn;
                    UpdateCurrentPlayerLabel();
                }
            }
        }

        private void Button_Paint(object sender, PaintEventArgs e)
        {
            Button button = sender as Button;
            if (button != null && button.Tag != null)
            {
                string color = button.Tag.ToString();
                if (color == "black")
                {
                    e.Graphics.FillEllipse(Brushes.Black, 5, 5, button.Width - 10, button.Height - 10);
                }
                else if (color == "white")
                {
                    e.Graphics.FillEllipse(Brushes.White, 5, 5, button.Width - 10, button.Height - 10);
                    e.Graphics.DrawEllipse(Pens.Black, 5, 5, button.Width - 10, button.Height - 10);
                }
            }
        }

        private bool CheckForWin(Button button)
        {
            int row = int.Parse(button.Name.Split('_')[1]);
            int col = int.Parse(button.Name.Split('_')[2]);
            string symbol = button.Tag.ToString();

            // 檢查水平方向
            int count = 1 + CountConsecutiveSymbols(row, col, 1, 0, symbol) + CountConsecutiveSymbols(row, col, -1, 0, symbol);
            if (count >= 5) return true;

            // 檢查垂直方向
            count = 1 + CountConsecutiveSymbols(row, col, 0, 1, symbol) + CountConsecutiveSymbols(row, col, 0, -1, symbol);
            if (count >= 5) return true;

            // 檢查斜線方向 (左上到右下)
            count = 1 + CountConsecutiveSymbols(row, col, 1, 1, symbol) + CountConsecutiveSymbols(row, col, -1, -1, symbol);
            if (count >= 5) return true;

            // 檢查斜線方向 (右上到左下)
            count = 1 + CountConsecutiveSymbols(row, col, 1, -1, symbol) + CountConsecutiveSymbols(row, col, -1, 1, symbol);
            if (count >= 5) return true;

            return false;
        }

        private int CountConsecutiveSymbols(int row, int col, int rowIncrement, int colIncrement, string symbol)
        {
            int count = 0;
            // Move to the next cell in the specified direction
            row += rowIncrement;
            col += colIncrement;

            while (row >= 0 && row < Size && col >= 0 && col < Size && buttons[row, col].Tag != null && buttons[row, col].Tag.ToString() == symbol)
            {
                count++;
                row += rowIncrement;
                col += colIncrement;
            }
            return count;
        }

        private void ResetBoard()
        {
            foreach (Button button in buttons)
            {
                button.Tag = null;
                button.Invalidate(); // 重新?制按?
                button.Enabled = true;
            }
            isPlayer1Turn = true; // 重置為玩家1先手
            moveHistoryListBox.Items.Clear();
            UpdateCurrentPlayerLabel();
        }
    }
}
