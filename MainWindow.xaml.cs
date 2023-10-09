using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using Microsoft.Win32;
using System.IO;
using DatabaseApp_Paul_Zaldea;
using System.Collections.Generic;

namespace DatabaseApp_Paul_Zaldea
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();

            cmbBoxCurrency.ItemsSource = Enum.GetValues(typeof(Currencies));
            foreach (FieldInfo field in typeof(Constants).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                cmbBoxEndReason.Items.Add(field.Name);
            }

        }
        private string InitializeCredentials()
        {

            return $"Data Source={txtBoxServer.Text};Initial Catalog={txtBoxDatabase.Text};User ID={txtBoxUser.Text};Password={pwdBoxPassword.Password}";
        }



        private void btnTestCon_Click(object sender, RoutedEventArgs e)
        {


            /*  string server_name = "172.22.123.223,1433";
              string database_name = "RET_drReports";
              string user_name = "sa";
              string password = "geheim"; */


            try
            {
                SqlConnection connection = new SqlConnection(InitializeCredentials());

                connection.Open();
                MessageBox.Show("Working!");
                connection.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }



        private void btnSaveSQLScript_Click(object sender, RoutedEventArgs e)
        {
            string sqlScript = GenerateSQLScript();


            if (string.IsNullOrEmpty(sqlScript))
            {
                MessageBox.Show("No filters or updates selected. Nothing to save.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SQL Files (*.sql)|*.sql|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                string sqlScriptFilePath = saveFileDialog.FileName;

                File.WriteAllText(sqlScriptFilePath, sqlScript);

                MessageBox.Show("SQL script saved successfully!");
            }
        }



        private string GenerateSQLScript()
        {
            
            string sqlScript = "";
            if (!CheckChangeBoxes())
            {
                sqlScript = $"UPDATE {Constants.CardDataTableName} SET ";

               
                sqlScript += ChangeDataInDatabase();
            }
            if (!CheckFilterBoxes())
                {
                    string filterScript = FilterDataDatabase();
                    sqlScript += filterScript;
            }
            
            if (sqlScript == "")
            {
                MessageBox.Show("No checkboxes were checked!");
            }

            return sqlScript;
        }

        private string ChangeDataInDatabase()
        {
            var updateFields = new List<string>();

            if (checkboxCasinoIssued.IsChecked == true)
            {
                updateFields.Add(txtBoxCasinoIssued.ToSqlUpdate(Constants.CardDataTableName));
            }

            if (checkboxCasinoIdUsed.IsChecked == true)
            {
                updateFields.Add(txtBoxCasinoIdUsed.ToSqlUpdate(Constants.CardDataTableName));
            }

            if (checkboxSiteIssued.IsChecked == true)
            {
                updateFields.Add(txtBoxSiteIssuedd.ToSqlUpdate(Constants.CardDataTableName));
            }

            if (checkBoxTypeLVL.IsChecked == true)
            {
                updateFields.Add(cmbBoxTypeLevel.ToSqlUpdate(Constants.CardDataTableName));
            }

            if (checkBoxEndRsn.IsChecked == true)
            {
                updateFields.Add(cmbBoxEndReason.ToSqlUpdate(Constants.CardDataTableName));
            }

            if (checkboxCurrency.IsChecked == true)
            {
                updateFields.Add(cmbBoxCurrency.ToSqlUpdate(Constants.CardDataTableName));
            }

            if (checkboxBalCCC.IsChecked == true)
            {
                updateFields.Add(txtBoxBalCCC.ToSqlUpdate(Constants.CardDataTableName));
            }

            if (checkboxBalNCC.IsChecked == true)
            {
                updateFields.Add(txtBoxBalNCC.ToSqlUpdate(Constants.CardDataTableName));
            }

            if (checkboxBalPTS.IsChecked == true)
            {
                updateFields.Add(txtBoxBalPts.ToSqlUpdate(Constants.CardDataTableName));
            }

            if (checkboxDeposit.IsChecked == true)
            {
                updateFields.Add(txtBoxDeposit.ToSqlUpdate(Constants.CardDataTableName));
            }

            if (checkboxValidDays.IsChecked == true)
            {
                updateFields.Add(txtBoxValidDaydDays.ToSqlUpdate(Constants.CardDataTableName));
            }

            if (checkBoxCardLocked.IsChecked == true)
            {
                updateFields.Add(checkBoxCardLocked.ToSqlUpdate(Constants.CardDataTableName));
            }

            return string.Join(Environment.NewLine, updateFields);
        }

        private string FilterDataDatabase()
        {
            string filterScript = "";
            filterScript = "\nSelect * FROM SM_CardDataAct WHERE 1=1 ";


            if (checkBoxCardNR.IsChecked == true)
            {
                filterScript += $" AND CardSerNr IN ('{txtBoxCardNumber.Text.Trim()}')";
            }

            if (checkBoxTypeLvl.IsChecked == true)
            {
                filterScript += $" AND TypeLevel IN ('{txtBoxTypeLvl.Text.Trim()}')";
            }

            if (checkBoxSiteIssued.IsChecked == true)
            {
                filterScript += $" AND SiteIssued = '{txtBoxSiteIssued.Text.Trim()}'";
            }

            if (checkBoxCurrency.IsChecked == true)
            {
                string currencyFilter = txtBoxCurrency.Text.Trim();
                int currencyValue = (int)CurrencyConverter.ConvertToCurrency(currencyFilter);

                if (currencyValue != -1)
                {
                    filterScript += $" AND Currency = '{currencyValue}'";
                }
                else
                {
                    MessageBox.Show("Invalid currency code entered.");
                }
            }

            if (checkBoxRegedPlayers.IsChecked == true)
            {
                filterScript += " AND PlayerId > '0'";
            }

            if (checkBoxAssignedCards.IsChecked == true)
            {
                filterScript += " AND CasinoIssued <> '' AND SiteIssued <> '' AND IpAddrIssued > 0";
            }

            if (checkBoxRecordsNullCert.IsChecked == true)
            {
                filterScript += " AND Certificate = '0'";
            }

            
            return filterScript;
        }

            private void HandleTextBoxChange(TextBox textBox, string columnName)
        {
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                string value = textBox.Text;
                string cardNumber = txtBoxCardNumberChanges.Text;
               
                string updateQuery = $"UPDATE SM_CardDataAct SET {columnName} = '{value}'";
                ExecuteUpdateQuery(updateQuery);
            }
        }
        private bool CheckChangeBoxes()
        {
            if (!checkboxCasinoIssued.IsChecked == true && !checkboxCasinoIdUsed.IsChecked == true && !checkboxSiteIssued.IsChecked == true
                && !checkBoxTypeLVL.IsChecked == true && !checkBoxEndRsn.IsChecked == true && !checkboxCurrency.IsChecked == true
                && !checkboxBalCCC.IsChecked == true && !checkboxBalNCC.IsChecked == true && !checkboxBalPTS.IsChecked == true
                && !checkboxDeposit.IsChecked == true && !checkboxValidDays.IsChecked == true && !checkBoxCardLocked.IsChecked == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckFilterBoxes()
        {
            if (!checkBoxCardNR.IsChecked == true && !checkBoxTypeLvl.IsChecked == true && !checkBoxSiteIssued.IsChecked == true && !checkBoxCurrency.IsChecked == true
                && !checkBoxRegedPlayers.IsChecked == true && !checkBoxRecordsNullCert.IsChecked == true && !checkBoxAssignedCards.IsChecked==true) 
            {
                return true; 
            }
            return false;
        }


        private void HandleComboBoxInt(ComboBox comboBox, string columnName)
        {
            if (comboBox.SelectedItem != null)
            {
                string cardNumber = txtBoxCardNumberChanges.Text;
                string selectedItemText = comboBox.Text;

                if (int.TryParse(selectedItemText, out int selectedValue))
                {
                    
                    string updateQuery = $"UPDATE SM_CardDataAct SET {columnName} = @Value";

                    using (SqlConnection connection = new SqlConnection(InitializeCredentials()))
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
                    

                    
                    string updateQuery = $"UPDATE SM_CardDataAct SET {columnName} = '{(int)selectedCurrency}'";

                   
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
                        string updateQuery = $"UPDATE SM_CardDataAct SET {columnName} = '{selectedValue}'";
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
            
            string updateQuery = $"UPDATE SM_CardDataAct SET {columnName} = '{value}'";
            ExecuteUpdateQuery(updateQuery);
        }

        
        private void ExecuteUpdateQuery(string query)
        {
            
            
            using (SqlConnection connection = new SqlConnection(InitializeCredentials()))
            {
                connection.Open();
                SqlCommand updateCommand = new SqlCommand(query, connection);
                updateCommand.ExecuteNonQuery();
                connection.Close();
            }
        }

        
        private void ButtonApplyChanges_Click(object sender, RoutedEventArgs e)
        {
            if (checkboxCasinoIssued.IsChecked == true)
            {
                HandleTextBoxChange(txtBoxCasinoIssued, "CasinoIssued");
            }

            if (checkboxCasinoIdUsed.IsChecked == true)
            {
                HandleTextBoxChange(txtBoxCasinoIdUsed, "CasinoIdUsed");
            }

            if (checkboxSiteIssued.IsChecked == true)
            {
                HandleTextBoxChange(txtBoxSiteIssuedd, "SiteIssued");
            }

            if (checkBoxTypeLVL.IsChecked == true)
            {
                HandleComboBoxInt(cmbBoxTypeLevel, "TypeLevel");
            }

            if (checkBoxEndRsn.IsChecked == true)
            {
                HandleComboBoxEndReason(cmbBoxEndReason, "EndReason");
            }

            if (checkboxCurrency.IsChecked == true)
            {
                HandleComboBoxCurrency(cmbBoxCurrency, "Currency");
            }

            if (checkboxBalCCC.IsChecked == true)
            {
                HandleTextBoxChange(txtBoxBalCCC, "BalCCC");
            }

            if (checkboxBalNCC.IsChecked == true)
            {
                HandleTextBoxChange(txtBoxBalNCC, "BalNCC");
            }

            if (checkboxBalPTS.IsChecked == true)
            {
                HandleTextBoxChange(txtBoxBalPts, "BalPts");
            }

            if (checkboxDeposit.IsChecked == true)
            {
                HandleTextBoxChange(txtBoxDeposit, "Deposit");
            }

            if (checkboxValidDays.IsChecked == true)
            {
                HandleTextBoxChange(txtBoxValidDaydDays, "ValidDays");
            }

            if (checkBoxCardLocked.IsChecked == true)
            {
                HandleCheckBoxChange(checkBoxCardLocked, "CardLocked");
            }

            MessageBox.Show("Data changes applied successfully.");
        }        
    }
}

