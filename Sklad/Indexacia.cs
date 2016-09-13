using System;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Sklad
{
    public partial class Indexacia : Form
    {
        private Form1 mn; //Создание объекта основной формы
        public Indexacia(Form1 bb)
        {
            InitializeComponent();
            mn = bb;
        }
        Execute execute = new Execute(); // метод выполнения команды
        private void button1_Click(object sender, EventArgs e)
        {
            Close(); // Закрытие формы
        }

        private void Indexacia_FormClosing(object sender, FormClosingEventArgs e)
        {
            mn.Show(); //Показ основной формы
        }

        private void button2_Click(object sender, EventArgs e)
        {                 
            string query = string.Format(@"UPDATE 'Товары' SET  Закупочная_цена = Закупочная_цена * {0} ;"
                           , numericUpDown1.Value+1); //Запрос индексации
            query = query.Replace(',', '.');    // замена запятой на точку для sql запросов         
            execute.exe(query); //Выполнение команды
        }
    }
}
