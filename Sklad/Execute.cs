using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklad
{
    class Execute
    {
        static readonly string databaseName = @"Data Source=" + Environment.CurrentDirectory + @"\Sklad.db;"; //Определение расположение базы
        public SQLiteConnection connection = new SQLiteConnection(databaseName); //Создание подключения
        public void exe(string query) // метод для выполнения изменений в базе
        {
            connection.Open(); //Открыть соединение
            SQLiteCommand command = new SQLiteCommand(query, connection); // Команда
            command.ExecuteNonQuery(); //Выполнение команды
            connection.Close(); //Закрыть соединение
        }

        public void exe(string query, out DataTable dataTable) // метод для получения данных из бд
        {
            connection.Open(); //Открыть соединение
            SQLiteCommand command = new SQLiteCommand(query, connection); // Команда
            command.ExecuteNonQuery(); //Выполнение команды
            dataTable = new DataTable("List"); // Создание DataTable
            var sqlAdapter = new SQLiteDataAdapter(command); //Создание адаптера
            sqlAdapter.Fill(dataTable); // прикрипление DataTable к адаптеру
            connection.Close(); //Закрыть соединение            
        }
    }
}
