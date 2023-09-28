using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using System;
using Microsoft.Win32;
using System.Data;
using System.IO;
using CheckBox = System.Windows.Controls.CheckBox;
using ComboBox = System.Windows.Controls.ComboBox;
using System.Reflection;

namespace DatabaseApp_Paul_Zaldea
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string connectionString;
        public string server_name;
        public string database_name;
        public string user_name;
        public string password;

        public MainWindow()
        {   
            
            InitializeComponent();
            
            cmbBoxCurrency.ItemsSource = Enum.GetValues(typeof(Currencies));
            foreach (FieldInfo field in typeof(Constants).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                cmbBoxEndReason.Items.Add(field.Name);
            }

        }
        private void InitializeCredentials()
        {
            server_name = txtBoxServer.Text;
            database_name = txtBoxDatabase.Text;
            user_name = txtBoxUser.Text;
            password = pwdBoxPassword.Password;
        }



        private void btnTestCon_Click(object sender, RoutedEventArgs e)
        {


          /*  string server_name = "172.22.123.223,1433";
            string database_name = "RET_drReports";
            string user_name = "sa";
            string password = "geheim"; */
            InitializeCredentials();
            connectionString = $"Data Source={server_name};Initial Catalog={database_name};User ID={user_name};Password={password}";

            try
            {
                SqlConnection connection = new SqlConnection(connectionString);

                connection.Open();
                MessageBox.Show("Working!");
                connection.Close();
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void btnApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            
            string cardNumberFilter = txtBoxCardNumber.Text.Trim();
            bool applyCardNumberFilter = checkBoxCardNR.IsChecked ?? false;

            string typeLevelFilter = txtBoxTypeLvl.Text.Trim();
            bool applyTypeLevelFilter = checkBoxTypeLvl.IsChecked ?? false;

            string siteIssuedFilter = txtBoxSiteIssued.Text.Trim();
            bool applySiteIssuedFilter = checkBoxSiteIssued.IsChecked ?? false;

            string currencyFilter = txtBoxCurrency.Text.Trim();
            bool applyCurrencyFilter = checkBoxCurrency.IsChecked ?? false;

            bool applyRegisteredPlayersFilter = checkBoxRegedPlayers.IsChecked ?? false;

            bool applyAssignedCardsFilter = checkBoxAssignedCards.IsChecked ?? false;

            bool applyRecordsNullCertFilter = checkBoxRecordsNullCert.IsChecked ?? false;

            
            string sqlQuery = "SELECT * FROM SM_CardDataAct WHERE 1=1 "; 

            if (applyCardNumberFilter)
            {
                sqlQuery += $"AND CardSerNr IN ({cardNumberFilter})";  
            }

            if (applyTypeLevelFilter)
            {
                sqlQuery += $"AND TypeLevel IN ({typeLevelFilter})";      
            }

            if (applySiteIssuedFilter)
            {
                sqlQuery += $"AND SiteIssued = '{siteIssuedFilter}'";   
            }

            if (applyCurrencyFilter)                                          
            {
                int currencyValue = (int)CurrencyConverter.ConvertToCurrency(currencyFilter);
                if (currencyValue != -1)
                {
                    sqlQuery += $" AND Currency = {currencyValue}";
                }
                else
                {
                   
                    MessageBox.Show("Invalid currency code entered.");
                }
            }

            if (applyRegisteredPlayersFilter)
            {
                sqlQuery += " AND PlayerId > 0";                    
            }

            if (applyAssignedCardsFilter)
            {
                sqlQuery += " AND CasinoIssued <> '' AND SiteIssued <> '' AND IpAddrIssued > 0";              
            }

            if (applyRecordsNullCertFilter)
            {
                sqlQuery += " AND Certificate = '0'";               
            }

            try
            {
                InitializeCredentials();
                DatabaseManager dbManager = new DatabaseManager(connectionString);


                DataTable filteredData = dbManager.ExecuteQuery(sqlQuery);



                MessageBox.Show("Filters applied successfully!");
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"An error occurred: {ex.Message}");
            
            }
        }

        private void btnSaveSQLScript_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SQL Files (*.sql)|*.sql|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                string sqlScriptFilePath = saveFileDialog.FileName;

               
                string sqlScriptContent = GenerateSQLScript();

                
                File.WriteAllText(sqlScriptFilePath, sqlScriptContent);

                MessageBox.Show("SQL script saved successfully!");
            }
        }

        private string GenerateSQLScript()
        {
            // Build your SQL script content here based on the filters
            string sqlScript = "SELECT * FROM SM_CardDataAct WHERE 1=1";

           
            if (checkBoxCardNR.IsChecked == true)                          
            {
                sqlScript += $" AND CardSerNr IN ({txtBoxCardNumber.Text.Trim()})";
            }

            if (checkBoxTypeLvl.IsChecked == true)                         
            {
                sqlScript += $" AND TypeLevel IN ({txtBoxTypeLvl.Text.Trim()})";
            }

            if (checkBoxSiteIssued.IsChecked == true)
            {
                sqlScript += $" AND SiteIssued = '{txtBoxSiteIssued.Text.Trim()}'"; 
            }

            if (checkBoxCurrency.IsChecked == true)            
            {
                string currencyFilter = txtBoxCurrency.Text.Trim();
                int currencyValue = (int)CurrencyConverter.ConvertToCurrency(currencyFilter);

                if (currencyValue != -1)
                {
                    sqlScript += $" AND Currency = {currencyValue}";
                }
                else
                {
                    
                    MessageBox.Show("Invalid currency code entered.");
                }
            }

            if (checkBoxRegedPlayers.IsChecked == true)
            {
                sqlScript += " AND PlayerId > 0";
            }

            if (checkBoxAssignedCards.IsChecked == true)
            {
                sqlScript += " AND CasinoIssued <> '' AND SiteIssued <> '' AND IpAddrIssued > 0";
            }

            if (checkBoxRecordsNullCert.IsChecked == true)
            {
                sqlScript += " AND Certificate = '0'";
            }

            return sqlScript;
        }

        private void HandleTextBoxChange(TextBox textBox, string columnName)
        {
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                string value = textBox.Text;
                string cardNumber = txtBoxCardNumberChanges.Text;
               
                string updateQuery = $"UPDATE SM_CardDataAct SET {columnName} = '{value}' WHERE CardSerNr = '{cardNumber}'";
                ExecuteUpdateQuery(updateQuery);
            }
        }

        
        private void HandleComboBoxInt(ComboBox comboBox, string columnName)
        {
            if (comboBox.SelectedItem != null)
            {
                string cardNumber = txtBoxCardNumberChanges.Text;
                string selectedItemText = comboBox.Text;

                if (int.TryParse(selectedItemText, out int selectedValue))
                {
                    
                    string updateQuery = $"UPDATE SM_CardDataAct SET {columnName} = @Value WHERE CardSerNr = @CardNumber";

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = new SqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Value", selectedValue); 
                            command.Parameters.AddWithValue("@CardNumber", cardNumber); 

                            int rowsAffected = command.ExecuteNonQuery();

                            
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Update successful");
                            }
                            else
                            {
                                MessageBox.Show("Update failed");
                            }
                        }
                    }
                }
                else
                {
                    
                    MessageBox.Show("Invalid selection in the ComboBox.");
                }
            }
        }
        private void HandleComboBoxCurrency(ComboBox comboBox, string columnName)
        {
            if (comboBox.SelectedItem != null)
            {
                string cardNumber = txtBoxCardNumberChanges.Text;
                string selectedItemText = comboBox.Text; 

                
                if (Enum.TryParse<Currencies>(selectedItemText, out Currencies selectedCurrency))
                {
                    

                    
                    string updateQuery = $"UPDATE SM_CardDataAct SET {columnName} = '{(int)selectedCurrency}' WHERE CardSerNr = '{cardNumber}'";

                   
                    ExecuteUpdateQuery(updateQuery);
                }
                else
                {
                    
                    MessageBox.Show("Invalid currency selection.");
                }
            }
        }
        private void HandleComboBoxEndReason(ComboBox comboBox, string columnName)
        {

            int selectedValue;
            if (cmbBoxEndReason.SelectedItem != null)
            {
                string cardNumber = txtBoxCardNumberChanges.Text;
                string selectedItem = comboBox.Text;
                FieldInfo[] fields = typeof(Constants).GetFields(BindingFlags.Public | BindingFlags.Static);
                for (int i = 0; i < fields.Length; i++)
                {
                    if (fields[i].Name == selectedItem)
                    {
                        selectedValue = (int)(uint)fields[i].GetValue(null);
                        string updateQuery = $"UPDATE SM_CardDataAct SET {columnName} = '{selectedValue}' WHERE CardSerNr = '{cardNumber}'";
                        ExecuteUpdateQuery(updateQuery);
                        break;
                    }
                }
            }
        }



        
        private void HandleCheckBoxChange(CheckBox checkBox, string columnName)
        {
            int value = checkBox.IsChecked == true ? 1 : 0;
            string cardNumber = txtBoxCardNumberChanges.Text;
            
            string updateQuery = $"UPDATE SM_CardDataAct SET {columnName} = '{value}' WHERE CardSerNr = '{cardNumber}'";
            ExecuteUpdateQuery(updateQuery);
        }

        
        private void ExecuteUpdateQuery(string query)
        {
            InitializeCredentials();
            connectionString = $"Data Source={server_name};Initial Catalog={database_name};User ID={user_name};Password={password}";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand updateCommand = new SqlCommand(query, connection);
                updateCommand.ExecuteNonQuery();
                connection.Close();
            }
        }

        
        private void ButtonApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBoxCardNumberChanges.Text))
            {
                
                HandleTextBoxChange(txtBoxCasinoIssued, "CasinoIssued");
                HandleTextBoxChange(txtBoxCasinoIdUsed, "CasinoIdUsed");
                HandleTextBoxChange(txtBoxSiteIssuedd, "SiteIssued");
                HandleComboBoxInt(cmbBoxTypeLevel, "TypeLevel");
                HandleComboBoxEndReason(cmbBoxEndReason, "EndReason");
                HandleComboBoxCurrency(cmbBoxCurrency, "Currency");
                HandleCheckBoxChange(checkBoxCardLocked, "CardLocked");
                HandleTextBoxChange(txtBoxBalCCC, "BalCCC");
                HandleTextBoxChange(txtBoxBalNCC, "BalNCC");
                HandleTextBoxChange(txtBoxBalPts, "BalPts");
                HandleTextBoxChange(txtBoxDeposit, "Deposit");
                HandleTextBoxChange(txtBoxValidDaydDays, "ValidDays");
                

                MessageBox.Show("Data changes applied successfully.");
            }
            else
            {
                MessageBox.Show("Please enter a Card Number.");
            }
        }


        private void txtBoxCardNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBoxCardNumber.Text = "";
        }



        private void txtBoxTypeLvl_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBoxTypeLvl.Text = "";
        }



        private void txtBoxSiteIssued_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBoxSiteIssued.Text = "";
        }



        private void txtBoxCurrency_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBoxCurrency.Text = "";
        }


        private void txtBoxServer_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBoxServer.Text = "";
        }




        private void txtBoxDatabase_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBoxDatabase.Text = "";
        }


     



        private void txtBoxUser_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBoxUser.Text = "";
        }
    }
}

