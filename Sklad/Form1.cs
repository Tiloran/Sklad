using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Collections.Generic;

namespace Sklad
{
        public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private List<string> idList = new List<string> {}; // List для сбора id изменных строк
        Execute execute = new Execute(); // метод выполнения команды
        private void update() //Метод обновления данных в dataGridView
        {
            string query = @"SELECT a.id, a.Название, a.Количество, a.Закупочная_цена, b.Категория  FROM [Товары] a, [Категория] b WHERE a.Категория=b.id;"; // Запрос
            DataTable dataTable;        // Создание DataTable   
            execute.exe(query, out dataTable); //Выполнение команды  
            dataGridView1.DataSource = dataTable.DefaultView; //Заполнение dataGridView 
            dataGridView1.Columns[0].Visible = false; //Скрытие 1 колонки id            
            if (dataGridView1.Rows.Count > 0) // Проверка на количество строк в dataGridView
            {
                dataGridView1.Rows[0].Cells[1].Selected = true; //Выбор 1 ячейки 1 столбца для устранения проблем с удалением
            }
        }        

        private void button5_Click(object sender, EventArgs e)
        {
            var mainmenu1 = new Dobavit(this); //Создание объекта для перехода но другую форму 
            mainmenu1.Show(); // открытие новой формы
            this.Hide(); // скрытие старой формы
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var mainmenu1 = new Indexacia(this); //Создание объекта для перехода но другую форму
            mainmenu1.Show(); // открытие новой формы
            this.Hide(); // скрытие старой формы
        }

        private void button7_Click(object sender, EventArgs e)
        {
            
            if (dataGridView1.Rows.Count > 0) // Проверка на количество строк в dataGridView
            {
                var adress = dataGridView1[0, dataGridView1.CurrentCellAddress.Y].Value.ToString(); // в выделеной строке определение значения ячейки id           
                string delete = string.Format(@"DELETE FROM Товары WHERE id = {0} ;", adress); // Запрос для удаления товара
                execute.exe(delete);    //Выполнение команды        
                update(); // Обновление данных в dataGridView
            }
            
        }       

        private void Form1_VisibleChanged(object sender, EventArgs e)
        {
            update(); // Обновление данных в dataGridView
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (idList.Count > 0) // Проверка на количество изменнных строк
            {
                string name, quantity, price, Category; // Переменные для хранения измененных данных
                for (int i=0; i<idList.Count; i++)
                {
                    search(idList[i], out name, out quantity, out price, out Category); //Метод для сбора значений строк по id
                    quantity = quantity.Replace(",", ".");
                    price = price.Replace(",", ".");
                    var change = @"SELECT id, Категория FROM Категория WHERE Категория='"+Category+"';"; //Узнаем id измененной категории
                    DataTable dataTable;
                    execute.exe(change, out dataTable);                    
                    change = string.Format(
                                    @"UPDATE Товары SET Название='{0}', Количество={1}, Закупочная_цена={2}, Категория={3} WHERE id={4};"
                                    , name, quantity, price, dataTable.Rows[0].Field<Int64>("id").ToString() , idList[i]);                    
                    execute.exe(change);    //Выполнение команды 
                }                      
                update(); // Обновление данных в dataGridView
                MessageBox.Show("Изменения в базу внесены.");
            }
            idList.Clear(); //Очистка List от списка изменнных строк
        }
        
        private void search(string id, out string name, out string quantity, out string price, out string Category) //Метод для сбора значений строк в DataGridView по id
        {
            name = null;
            quantity = null;
            price = null;
            Category = null;
            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Contains(id))
                        {
                    name = dataGridView1.Rows[i].Cells[1].Value.ToString();
                    quantity = dataGridView1.Rows[i].Cells[2].Value.ToString();
                    price = dataGridView1.Rows[i].Cells[3].Value.ToString();
                    Category = dataGridView1.Rows[i].Cells[4].Value.ToString();                
                        }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e) //Метод для записи id измененных строк
        {
            var adress = dataGridView1[0, dataGridView1.CurrentCellAddress.Y].Value.ToString(); // в измененной строке получаем значения ячейки id
            bool exist = false; //Переменная для проверки изменения одной и той же строки 2 раза
            for (int i=0; i<idList.Count; i++)
            {
                if (idList[i] == adress) //Проверка на случай изменения одной и той же строки 2 раза
                {
                    exist = true;
                    break;
                }                
            }
            if (exist==false)
            {
                idList.Add(adress); //Запись id в List для формирования списка строк для изменения
            }            
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) //Метод проверки правильности ввода данных в ячейки
        {            
            string headerText = dataGridView1.Columns[e.ColumnIndex].HeaderText;
            if (headerText.Equals("Название")) //Проверка столбика с названиями на ограничение количества знаков
            {
                if( e.FormattedValue.ToString().Length>40 && e.FormattedValue.ToString().Length<2)
                {
                    MessageBox.Show("Название не должно быть больше 40 символов и не меньше 2 символов.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    e.Cancel = true;
                }
            }
            if (headerText.Equals("Количество") || Equals("Закупочная_цена")) //Проверка столбика количества и цен 
                //на допустимость только дробных чисел в которых не больше 2 знаков после запятой
            {
                try
                {                    
                    double sad = Convert.ToDouble(e.FormattedValue.ToString());
                    if (sad != Math.Round(sad, 2))
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    MessageBox.Show("Можно вводить только числа в которых не больше 2 цифр после запятой.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    e.Cancel = true;
                }                
            }
            if (headerText.Equals("Категория")) //Проверка столбика категорий на допустимость значений
            {
                DataTable dataTable;
                string query = @"SELECT id, Категория FROM Категория";
                execute.exe(query,out dataTable);
                bool exist = false;
                string error = null;
                for(int i=0; i<dataTable.Rows.Count; i++)
                {
                    error += dataTable.Rows[i].Field<string>("Категория")+" ";
                    if (e.FormattedValue.ToString()==dataTable.Rows[i].Field<string>("Категория"))
                    {
                        exist = true;
                    }                    
                }
                if(exist == false)
                {
                    MessageBox.Show("Можно вводить только такие варианты ("+ error+")", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    e.Cancel = true;
                }                
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e) // Убирает дефолтные ошибки DataGridView
        {

        }
        
    }


    
}
