using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace WPFDataManager
{
    // Main window for the data manager application
    public partial class MainWindow : Window
    {
        private WPFTestDBEntities dbContext;
        private ObservableCollection<Item> items;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize the database context to interact with the database
            dbContext = new WPFTestDBEntities();

            // Load items from the database, rounding prices to 2 decimal places
            items = new ObservableCollection<Item>(
                dbContext.Items.ToList().Select(item =>
                {
                    item.Price = Math.Round(item.Price, 2);
                    return item;
                })
            );

            // Bind the items to the data grid for display
            ItemsDataGrid.DataContext = items;
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if the name field is empty
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    throw new ArgumentException("Name cannot be empty.");
                }

                // Try to parse the quantity input
                if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
                {
                    throw new ArgumentException("Quantity must be a non-negative integer.");
                }

                // Try to parse the price input
                if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
                {
                    throw new ArgumentException("Price must be a non-negative decimal number.");
                }

                // Make sure the name isn't too long
                if (NameTextBox.Text.Length > 100)
                {
                    throw new ArgumentException("Name cannot exceed 100 characters.");
                }

                // Round the price to 2 decimal places
                price = Math.Round(price, 2);

                // Create a new Item object with the provided data
                var newItem = new Item
                {
                    Name = NameTextBox.Text,
                    Quantity = quantity,
                    Price = price,
                    Description = DescriptionTextBox.Text,
                    CreatedAt = DateTime.Now
                };

                // Add the new item to the database and save changes
                dbContext.Items.Add(newItem);
                dbContext.SaveChanges();

                // Add the new item to the observable collection to refresh the UI
                items.Add(newItem);

                // Notify the user that the item was added successfully
                MessageBox.Show("Item added successfully!");

                // Clear the input fields for the next entry
                NameTextBox.Clear();
                QuantityTextBox.Clear();
                PriceTextBox.Clear();
                DescriptionTextBox.Clear();
            }
            catch (Exception ex)
            {
                // Show an error message if something went wrong
                MessageBox.Show($"Error adding item: {ex.Message}");
            }
        }
    }
}
