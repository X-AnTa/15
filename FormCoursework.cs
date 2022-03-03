using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace coursework_15
{
    public partial class FormCoursework : Form
    {

        Game game;
        string result;
        int i = 0;
        int x, y, x0, y0;
        int k = 1;

        public FormCoursework()
        {
            InitializeComponent();
            game = new Game(4);
        }

        private void button0_Click(object sender, EventArgs e)
        {
            int position = Convert.ToInt32(((Button)sender).Tag);
            game.shift(position);
            refresh();
            if (game.check_num())
            {
                tableLayoutPanel.Enabled = false;
                menu_help.Enabled = false;
                label1.Visible = true;
            }
            k++;
        }

        private Button button(int position)
        {
            switch (position)
            {
                case 0: return button0;
                case 1: return button1;
                case 2: return button2;
                case 3: return button3;
                case 4: return button4;
                case 5: return button5;
                case 6: return button6;
                case 7: return button7;
                case 8: return button8;
                case 9: return button9;
                case 10: return button10;
                case 11: return button11;
                case 12: return button12;
                case 13: return button13;
                case 14: return button14;
                case 15: return button15;
                default:return null;

            }
        }

        private void menu_start_Click(object sender, EventArgs e)
        {
            start_game();
        }

        private void start_game()
        {
            label2.Visible = false;
            label1.Visible = false;
            tableLayoutPanel.Enabled = true;

            game.start();
            for (int i = 0; i < 200; i++)
                game.shift_random();
            refresh();

            tableLayoutPanel.Enabled = false;
            x0 = game.space_x;
            y0 = game.space_y;
            i = 0;
            k = 1;
            tableLayoutPanel.Enabled = true;
            menu_help.Enabled = true;
        }

        private void refresh()
        {
            for (int position = 0; position < 16; position++)
            {
                int n = game.get_num(position);
                button(position).Text = n.ToString();
                button(position).Visible = (n > 0);
            }
        }
        private void tableLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void FormCoursework15(object sender, EventArgs e)
        {
            start_game();
        }

        private void menu_help_Click(object sender, EventArgs e)
        {
            if (k >= 1)
            {
                menu_help.Enabled = false;
                tableLayoutPanel.Enabled = false;
                game.initGoalArrays();
                game.idaStar(out result);
                x0 = game.space_x;
                y0 = game.space_y;
                i = 0;
                tableLayoutPanel.Enabled = true;
                menu_help.Enabled = true;
            }

            shift_current(result, out x0, out y0);
            i++;
            k = 0;
            

        }

        private void shift_current(string res, out int x01, out int y01)
        {
            x = x0;
            y = y0;
            int p;
            try
            {
                char s = res[i];
                switch (s)
                {
                    case ('D'): x++; break;
                    case ('U'): x--; break;
                    case ('L'): y--; break;
                    case ('R'): y++; break;
                }
                p = game.coordstopos(x, y);

                switch (p)
                {
                    case 0: button0.PerformClick(); break;
                    case 1: button1.PerformClick(); break;
                    case 2: button2.PerformClick(); break;
                    case 3: button3.PerformClick(); break;
                    case 4: button4.PerformClick(); break;
                    case 5: button5.PerformClick(); break;
                    case 6: button6.PerformClick(); break;
                    case 7: button7.PerformClick(); break;
                    case 8: button8.PerformClick(); break;
                    case 9: button9.PerformClick(); break;
                    case 10: button10.PerformClick(); break;
                    case 11: button11.PerformClick(); break;
                    case 12: button12.PerformClick(); break;
                    case 13: button13.PerformClick(); break;
                    case 14: button14.PerformClick(); break;
                    case 15: button15.PerformClick(); break;
                } 
            }
            catch(Exception ex)
            {
                label2.Visible = true;
                tableLayoutPanel.Enabled = false;
                menu_help.Enabled = false;
            }

            x01 = x;
            y01 = y;
        }
    }
}
