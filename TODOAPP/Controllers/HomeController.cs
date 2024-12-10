using Microsoft.AspNetCore.Mvc;
using TODOAPP.Models;
using System.Collections.Generic;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Diagnostics;

namespace TODOAPP.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string connectionString = "Data Source=Hamza;Initial Catalog=TODOAPP;Integrated Security=True;TrustServerCertificate=True;";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<SQLDBModel> todoItems = GetTodoItems();
            return View(todoItems);
        }
        [Obsolete]
        public List<SQLDBModel> GetTodoItems()
        {
            List<SQLDBModel> todoItems = new List<SQLDBModel>();
            string query = "SELECT Id, Title, Date, IsCompleted FROM todoitems";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SQLDBModel todoItem = new SQLDBModel
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Title = reader["Title"].ToString(),
                                    Date = Convert.ToDateTime(reader["Date"]),
                                    IsCompleted = Convert.ToBoolean(reader["IsCompleted"])
                                };

                                todoItems.Add(todoItem);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return todoItems;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrUpdate(SQLDBModel todoItem)
        {
            if (todoItem.Id == 0 || todoItem.Id == null)  
            {
                AddTodoItem(todoItem); 
                return RedirectToAction(nameof(Index));
            }
            else  
            {
                UpdateTodoItem(todoItem);  
                return RedirectToAction(nameof(Index));
            }
            return View("Index", todoItem);
        }
        public void AddTodoItem(SQLDBModel todoItem)
        {
            string query = "INSERT INTO todoitems (Title, Date, IsCompleted) VALUES (@Title, @Date, @IsCompleted)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", todoItem.Title);
                    command.Parameters.AddWithValue("@Date", todoItem.Date);
                    command.Parameters.AddWithValue("@IsCompleted", todoItem.IsCompleted);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateTodoItem(SQLDBModel todoItem)
        {
            string query = "UPDATE todoitems SET Title = @Title, Date = @Date, IsCompleted = @IsCompleted WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", todoItem.Id);
                    command.Parameters.AddWithValue("@Title", todoItem.Title);
                    command.Parameters.AddWithValue("@Date", todoItem.Date);
                    command.Parameters.AddWithValue("@IsCompleted", todoItem.IsCompleted);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            DeleteTodoItem(id);  
            return RedirectToAction(nameof(Index));
        }
        public void DeleteTodoItem(int id)
        {
            string query = "DELETE FROM todoitems WHERE Id = @Id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public IActionResult Complete(int id)
        {
            var todoItem = GetTodoItems().FirstOrDefault(t => t.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }
            todoItem.IsCompleted = !todoItem.IsCompleted;
            UpdateTodoItem(todoItem); 

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
