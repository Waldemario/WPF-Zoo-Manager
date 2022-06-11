using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Runtime.InteropServices;

namespace WpfZooManager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		SqlConnection sqlConnection;
		public MainWindow()
		{
			InitializeComponent();

			string connectionString = ConfigurationManager.ConnectionStrings["WpfZooManager.Properties.Settings.WaldoDBConnectionString"].ConnectionString;
			sqlConnection = new SqlConnection(connectionString);
			ShowZoos();
			ShowAnimals();


		}

		private void ShowZoos()
		{
			string query = "select * from Zoo";
			SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
			using (sqlDataAdapter)
			{
				DataTable zooTable = new DataTable();
				sqlDataAdapter.Fill(zooTable);
				listZoos.DisplayMemberPath = "Location";
				listZoos.SelectedValuePath = "Id";
				listZoos.ItemsSource = zooTable.DefaultView;
			}
		}

		private void ShowAssociatedAnimals()
		{
			try
			{
				string query = "select * from Animal a inner join ZooAnimal za on a.Id = za.AnimalId " +
				"where za.ZooId = @ZooId";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

				using (sqlDataAdapter)
				{
					sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
					DataTable animalTable = new DataTable();
					sqlDataAdapter.Fill(animalTable);

					listAssociatedAnimals.DisplayMemberPath = "Name";
					listAssociatedAnimals.SelectedValuePath = "Id";
					listAssociatedAnimals.ItemsSource = animalTable.DefaultView;
				}
			}
			catch (Exception e)
			{

				// MessageBox.Show(e.ToString());
			}
			
		}

		private void ShowAnimals()
		{
			try
			{
				string query = "select * from Animal";
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
				using (sqlDataAdapter)
				{
					DataTable animalTable = new DataTable();
					sqlDataAdapter.Fill(animalTable);
					listAnimals.DisplayMemberPath = "Name";
					listAnimals.SelectedValuePath = "Id";
					listAnimals.ItemsSource = animalTable.DefaultView;
				}
			}
			catch (Exception e)
			{

				//MessageBox.Show(e.ToString());
			}
			
		}

		private void listZoos_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ShowAssociatedAnimals();
			Zoos_UpdateTextBox();
		}
		
		 private void DeleteZoo_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string query = "delete from Zoo where Id = @ZooId";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
				sqlCommand.ExecuteScalar();
				
			}
			catch (Exception ex)
			{

				MessageBox.Show(ex.ToString());
			}
			finally {
				sqlConnection.Close();
				ShowZoos();
			}
			
		}

		private void AddZoo_Click(object sender, RoutedEventArgs e)
		{
			string query = "insert into dbo.Zoo values (@NewZoo)";
			SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
			sqlConnection.Open();
			sqlCommand.Parameters.AddWithValue("@NewZoo", TextBox1.Text);
			sqlCommand.ExecuteScalar();
			sqlConnection.Close();
			ShowZoos();
		}

		private void UpdateZoo_Click(object sender, RoutedEventArgs e) {
			try
			{
				string query = "update Zoo set Location = @UpdatedZoo where Id = @ZooId";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@UpdatedZoo", TextBox1.Text);
				sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
				sqlCommand.ExecuteScalar();
				
			}
			catch (Exception ex)
			{

				MessageBox.Show(ex.ToString());
			}
			sqlConnection.Close();
			ShowZoos();

		}

		private void RemoveAnimal_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string query = "delete from ZooAnimal where ZooId = @ZooId and AnimalId = @AnimalId";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@AnimalId", listAssociatedAnimals.SelectedValue);
				sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
				sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{

				MessageBox.Show(ex.ToString());
			}
			sqlConnection.Close();
			ShowAssociatedAnimals();

		}

		private void AddAnimaltoZoo_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string query = "insert into ZooAnimal values (@ZooId, @AnimalId)";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue);
				sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
				sqlCommand.ExecuteScalar();
			}
			catch (Exception ex)
			{

				MessageBox.Show(ex.ToString());
			}
			sqlConnection.Close();
			ShowAssociatedAnimals();
			
		}

		private void UpdateAnimal_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string query = "update Animal set Name = @UpdatedAnimal where Id = @AnimalId";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@UpdatedAnimal", TextBox1.Text);
				sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue);
				sqlCommand.ExecuteScalar();

			}
			catch (Exception ex)
			{

				MessageBox.Show(ex.ToString());
			}
			sqlConnection.Close();
			ShowAnimals();

		}

		private void AddAnimal_Click(object sender, RoutedEventArgs e)
		{
			string query = "insert into Animal values (@NewAnimal)";
			SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
			sqlConnection.Open();
			sqlCommand.Parameters.AddWithValue("@NewAnimal", TextBox1.Text);
			sqlCommand.ExecuteScalar();
			sqlConnection.Close();
			ShowAnimals();
		}

		private void DeleteAnimal_Click(object sender, RoutedEventArgs e) 
		{
			try
			{
				string query = "delete from Animal where Id = @AnimalId";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				sqlConnection.Open();
				sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue);
				sqlCommand.ExecuteScalar();

			}
			catch (Exception ex)
			{

				MessageBox.Show(ex.ToString());
			}
			finally
			{
				sqlConnection.Close();
				ShowAnimals();
				ShowAssociatedAnimals();
			}

		}

		private void Zoos_UpdateTextBox() {
			try
			{
				string query = "select location from Zoo where Id = @ZooId";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

				using (sqlDataAdapter)
				{
					sqlCommand.Parameters.AddWithValue("@ZooId", listZoos.SelectedValue);
					DataTable zooDataTable = new DataTable();
					sqlDataAdapter.Fill(zooDataTable);

					TextBox1.Text = zooDataTable.Rows[0]["Location"].ToString();
				}
			}
			catch (Exception e)
			{

				// MessageBox.Show(e.ToString());
			}
		}

		private void listAnimals_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Animals_UpdateTextBox();
		}

		private void Animals_UpdateTextBox()
		{
			try
			{
				string query = "select Name from Animal where Id = @AnimalId";
				SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
				SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

				using (sqlDataAdapter)
				{
					sqlCommand.Parameters.AddWithValue("@AnimalId", listAnimals.SelectedValue);
					DataTable zooDataTable = new DataTable();
					sqlDataAdapter.Fill(zooDataTable);

					TextBox1.Text = zooDataTable.Rows[0]["Name"].ToString();
				}
			}
			catch (Exception e)
			{

				// MessageBox.Show(e.ToString());
			}
		}
	}
}