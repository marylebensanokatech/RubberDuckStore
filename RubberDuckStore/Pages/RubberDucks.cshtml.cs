using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
namespace RubberDuckStore.Pages
{
    //Page Model - the model connects the C# code to the HTML that defines the gui
    public class RubberDucksModel : PageModel
    {
        //Binding the components that are in our GUI to the getters and setters 
        //(the methods for setting the attributes) in the C# code
        [BindProperty]
        public int SelectedDuckId { get; set; }
        public List<SelectListItem> DuckList { get; set; }
        public Duck SelectedDuck { get; set; }
        //HTTP call for when the page is loaded
        public void OnGet()
        {
            //POpulate the list of ducks that the user can buy
            LoadDuckList();
        }

        //HTTP call for when the form is submitted on the page 
        public IActionResult OnPost()
        {
            //Reload/refresh the list of ducks
            LoadDuckList();
            //If they selected a duck
            if (SelectedDuckId != 0)
            {
                //Get the duck they selected
                SelectedDuck = GetDuckById(SelectedDuckId);
            }
            //Return the pages - so the web pages is displayed again in the browser
            return Page();
        }
        //Method that populates the list of ducks the user can buy
        private void LoadDuckList()
        {
            //Creating a new List
            DuckList = new List<SelectListItem>();
            //Connect to the database
            using (var connection = new SqliteConnection("Data Source=RubberDucks.db"))
            {
                //Open the connection
                connection.Open();
                //Create a SQL command
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name FROM Ducks";
                //Execute the SQL command
                using (var reader = command.ExecuteReader())
                {
                    //Use a loop to iterate through the resultset - the group of records 
                    //retrieved from the database
                    while (reader.Read())
                    {
                        //For each record in the database, we'll add the duck name and ID to the list
                        DuckList.Add(new SelectListItem
                        {
                            Value = reader.GetInt32(0).ToString(),
                            Text = reader.GetString(1)
                        });
                    }
                }
            }
        }

        //Retrieves a single record from the database based on the ID of the duck
        private Duck GetDuckById(int id)
        {
            //Create a new DB connection, then open the connection, create a SQL command and 
            //executing the command
            using (var connection = new SqliteConnection("Data Source=RubberDucks.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Ducks WHERE Id = @Id"; //@ sylmbol maps back to the
                //the HTML element on the page
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    //Returns a single record, so no loop is needed
                    if (reader.Read())
                    {
                        //Create and return a new Duck object that's populated with the values
                        //from the database record
                        return new Duck
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            ImageFileName = reader.GetString(4)
                        };
                    }
                }
            }
            return null;
        }
    }
    //Declaring a Duck class, this serves as a blueprint for creating Duck objects
    //which are the ducks that our store sells
    public class Duck
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageFileName { get; set; }
    }
}
    

