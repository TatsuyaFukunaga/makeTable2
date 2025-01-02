namespace makeTable
{
    using MySqlConnector;
    using System.Data;
    using System.Windows.Forms;
    //using MySql.Data.MySqlClient;

    public partial class Form1 : Form

    {
        private string[] column;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonDBConnect_Click(object sender, EventArgs e)
        {
            string connStr = "Server=localhost;User ID=root;Password=;Database=" + this.textBoxDBName.Text;
            MySqlConnection conn = new MySqlConnection(connStr);


            try
            {
                // 接続を開く
                conn.Open();

                // データを取得するテーブル
                DataTable tbl = new DataTable();

                // SQLを実行する
                MySqlDataAdapter dataAdp = new MySqlDataAdapter("show tables", conn);
                dataAdp.Fill(tbl);
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    this.listBoxTableName.Items.Add((tbl.Rows[i].ItemArray)[0].ToString());
                }
                conn.Close();
            }
            catch (MySqlException mse)
            {
                MessageBox.Show(mse.Message, "データ取得エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxNumOfColumn_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonTableConnect_Click(object sender, EventArgs e)
        {
            string connStr = "Server=localhost;User ID=root;Password=;Database=" + this.textBoxDBName.Text;
            MySqlConnection conn = new MySqlConnection(connStr);


            try
            {
                // 接続を開く
                conn.Open();

                // データを取得するテーブル
                DataTable tbl = new DataTable();

                // SQLを実行する
                string command = "select * from " + this.listBoxTableName.SelectedItem;
                MySqlDataAdapter dataAdp = new MySqlDataAdapter("select * from " + this.listBoxTableName.SelectedItem.ToString(), conn);
                dataAdp.Fill(tbl);
                this.column = new string[tbl.Columns.Count];
                for (int i = 0; i < tbl.Columns.Count; i++) { this.column[i] = tbl.Columns[i].ColumnName; };
                dataGridView1.DataSource = tbl;

                conn.Close();
            }
            catch (MySqlException mse)
            {
                MessageBox.Show(mse.Message, "データ取得エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void buttonDataAppend_Click(object sender, EventArgs e)
        {
            string connStr = "Server=localhost;User ID=root;Password=;Database=" + this.textBoxDBName.Text;
            MySqlConnection conn = new MySqlConnection(connStr);


            try
            {
                // 接続を開く
                conn.Open();

                // データを取得するテーブル
                DataTable tbl = new DataTable();

                // SQLを実行する
                string[] lines = this.richTextBoxDataAppend.Lines;
                string[][] tableData = new string[lines.Length][];
                char[] delimiterChars = { ' ', ',', '\t' };

                for (int i = 0; i < lines.Length; i++) tableData[i] = lines[i].Trim().Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                double number;
                for (int j = 0; j < tableData[0].Length; j++)
                {
                    if (!double.TryParse(tableData[0][j], out number))
                    {
                        for (int i = 0; i < tableData.Length; i++)
                        {
                            tableData[i][j] = "'" + tableData[i][j] + "'";
                        }
                    }
                }
                string command = "insert into " + this.listBoxTableName.SelectedItem + " (";
                for (int i = 0; i < this.column.Length - 1; i++) command = command + column[i] + ",";
                command = command + column[this.column.Length - 1] + ") values ";
                for (int i = 0; i < tableData.Length; i++)
                {
                    string dummy = "(";
                    for (int j = 0; j < tableData[i].Length - 1; j++)
                    {
                        dummy = dummy + tableData[i][j] + ",";
                    }
                    if (i == tableData.Length - 1) dummy = dummy + tableData[i][tableData[i].Length - 1] + ")";
                    else dummy = dummy + tableData[i][tableData[i].Length - 1] + "),";
                    command = command + dummy;
                }

                MySqlDataAdapter dataAdp = new MySqlDataAdapter(command, conn);
                dataAdp.Fill(tbl);
                dataGridView1.DataSource = tbl;

                conn.Close();
            }
            catch (MySqlException mse)
            {
                MessageBox.Show(mse.Message, "データ取得エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void buttonDropData_Click(object sender, EventArgs e)
        {
            string connStr = "Server=localhost;User ID=root;Password=;Database=" + this.textBoxDBName.Text;
            MySqlConnection conn = new MySqlConnection(connStr);
            string command = "delete from " + this.listBoxTableName.SelectedItem + " where ";
            for (int i = 0; i < this.dataGridView1.SelectedRows.Count; i++)
            {
                command = command + this.column[0] + "=" + this.dataGridView1.SelectedRows[i].Cells[0].Value.ToString() + " ";
                if (i != this.dataGridView1.SelectedRows.Count - 1) command = command + " or ";
            }

            try
            {
                // 接続を開く
                conn.Open();

                // データを取得するテーブル
                DataTable tbl = new DataTable();

                // SQLを実行する
                MySqlDataAdapter dataAdp = new MySqlDataAdapter(command, conn);
                dataAdp.Fill(tbl);
                conn.Close();
            }
            catch (MySqlException mse)
            {
                MessageBox.Show(mse.Message, "データ取得エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string getData(int rowIndex,int columnIndex)
        {
            if (this.dataGridView1 != null)
            {
                try
                {
                    double dummy;
                    string cellData = this.dataGridView1[columnIndex, rowIndex].Value.ToString();
                    if (double.TryParse(cellData, out dummy))
                    {
                        return cellData;
                    }
                    else return "'" + cellData + "'";
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.ToString());
                }
            }
            return "";
        }

        private void textBoxDBName_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connStr = "Server=localhost;User ID=root;Password=;Database=" + this.textBoxDBName.Text;
            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                // 接続を開く
                conn.Open();
                var sqlComm = conn.CreateCommand();
                // データを取得するテーブル
                DataTable tbl = new DataTable();
                MySqlDataAdapter dataAdp;

                string command = "";
                for (int i = 0; i < this.dataGridView1.RowCount-1; i++)
                {
                    command = "update " + this.listBoxTableName.SelectedItem + " set ";
                    for (int j = 1; j < this.column.Length; j++)
                    {
                        command = command + this.column[j] + "=" + this.getData(i,j);
                        if (j != this.column.Length - 1) command = command + " , ";

                    }
                    command = command + " where " + this.column[0] + "=" + this.getData(i,0);
                    // SQLを実行する
                    //conn.Open();
                    //dataAdp = new MySqlDataAdapter(command, conn);
                    //conn.Close();
                    sqlComm.CommandText=command;
                    sqlComm.ExecuteNonQuery();

                }


                //dataAdp.Fill(tbl);
                //conn.Close();
            }
            catch (MySqlException mse)
            {
                MessageBox.Show(mse.Message, "データ取得エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
