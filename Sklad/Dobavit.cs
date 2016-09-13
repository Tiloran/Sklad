using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace Sklad
{
    public partial class Dobavit : Form
    {
        
        private Form1 mn; //Создание объекта основной формы
        public Dobavit(Form1 bb)
        {
            InitializeComponent();
            mn = bb;
        }

        Execute execute = new Execute(); // метод выполнения команды

        private void button1_Click(object sender, EventArgs e)
        {
            Close(); // Закрытие формы
        }

        private void Dobavit_Load(object sender, EventArgs e)
        {
            string query = @"SELECT id, Категория  FROM [Категория] ;"; // Запрос
            DataTable dataTable;             // Создание DataTable
            execute.exe(query, out dataTable); //Выполнение команды           
            comboBox1.DataSource = dataTable; //Заполнение  comboBox  
            comboBox1.DisplayMember = "Категория"; // столбец для отображения
            comboBox1.ValueMember = "id"; //столбец со значениями
        }

        private void Dobavit_FormClosed(object sender, FormClosedEventArgs e)
        {
            mn.Show(); //Показ основной формы
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox1.TextLength>2 && textBox1.TextLength < 40) // проверка на ввод имени
            {                
                string quantity = numericUpDown1.Value.ToString(); //Строка со значением количества
                quantity = quantity.Replace(',', '.'); // замена запятой на точку для sql запросов
                string purchase_price = numericUpDown2.Value.ToString(); //Строка со значением закупочной цены
                purchase_price = purchase_price.Replace(',', '.'); // замена запятой на точку для sql запросов                
                string dobavit =string.Format(@"INSERT INTO 'Товары' ('Название', 'Количество', 'Закупочная_цена', 'Категория')  VALUES ( '{0}' , {1} , {2} , {3} )"
                           , textBox1.Text, quantity, purchase_price, comboBox1.SelectedValue); // Запрос для добавления нового товара
                execute.exe(dobavit); //Выполнение команды
                label5.Visible = false; //Скрытие надписи о пустом имени
            }
            else
            {
                label5.Visible = true; //Показ надписи о пустом имени
            }
            
        }
    }
}
