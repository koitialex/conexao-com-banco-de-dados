using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace formulariosimples
{
    public partial class frmForumularioSimples : Form
    {

        MySqlConnection Conexao;
        string data_source = "datasource=localhost;username=root;password=;database=formulariosimples";

        private int? numero_usuario = null;

        public frmForumularioSimples()
        {
            InitializeComponent();

            //Configuração inicial do ListView para exibição dos dados dos clientes
            listView1.View = View.Details;           //Define a visualização como "Detalhes"
            listView1.LabelEdit = true;              //Permite editar os títulos das colunas
            listView1.AllowColumnReorder = true;    //Permite reordenar as colunas
            listView1.FullRowSelect = true;         //Seleciona a linha inteira ao clicar 
            listView1.GridLines = true;             //Exibe as linhas de grade no ListView


            //Definindo as colunas do ListView
            listView1.Columns.Add("Número de Cadastro", 200, HorizontalAlignment.Left); //Coluna de Número de Cadastro
            listView1.Columns.Add("Nome Completo", 200, HorizontalAlignment.Left); //Coluna de Nome Completo
            listView1.Columns.Add("Data de Nascimento", 200, HorizontalAlignment.Left); //Coluna de Data de Nascimento
            listView1.Columns.Add("Cidade", 200, HorizontalAlignment.Left); //Coluna de Cidade
            listView1.Columns.Add("Gênero", 200, HorizontalAlignment.Left); //Coluna de Gênero

            //Carrega os dados dos clientes na interface
            carregar_clientes();
        }


        private void carregar_clientes_com_query(string query)
        {
            try
            {
                //Cria a conexão com o banco de dados
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                //Executa a consulta SQL fornecida
                MySqlCommand cmd = new MySqlCommand(query, Conexao);

                //Se a consulta contém o parâmetro @q, adiciona o valor da caixa de pesquisa
                if (query.Contains("@q"))
                {
                    cmd.Parameters.AddWithValue("@q", "%" + txtBuscar.Text + "%");
                }

                //Executa o comando e obtém os resultados
                MySqlDataReader reader = cmd.ExecuteReader();

                //Limpa os itens existentes no ListView antes de adicionar novos
                listView1.Items.Clear();

                //Preenche o ListView com os dados dos clientes
                while (reader.Read())
                {
                    //Cria uma linha para cada cliente com os dados retornados da consulta
                    string[] row =
                    {
                        Convert.ToString(reader.GetInt32(0)), //Número de Cadastro
                        reader.GetString(1),
                        Convert.ToString(reader.GetDateTime(2)),
                        reader.GetString(3),
                        reader.GetString(4)
                    };



                    //Adiciona a linha ao ListView
                    listView1.Items.Add(new ListViewItem(row));
                }
            }

            catch (MySqlException ex)
            {
                //Trata erros relacionados ao MySQL
                MessageBox.Show("Erro " + ex.Number + " ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {

                //Trata outros tipos de erro
                MessageBox.Show("Ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                //Garante que a conexão com o banco será fechada, mesmo se ocorrer erro
                if (Conexao != null && Conexao.State == ConnectionState.Open)
                {
                    Conexao.Close();
                }
            }
        }

        private void carregar_clientes()
        {
            string query = "SELECT * FROM cadastro ORDER BY numero_cadastro DESC";
            carregar_clientes_com_query(query);
        }

        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            try
            {
                string nomeUsuario;
                int numeroCadastro;
                DateTime dataNascimento;
                string cidade;
                bool generoF;
                bool generoM;
                bool generoNB;

                //validação de campos obrigatórios
                if (string.IsNullOrWhiteSpace(txtNumeroCadastro.Text))
                {
                    MessageBox.Show("Por favor, preencha o número de cadastro.");
                    return; // Interrompe a execução do código caso o campo esteja 
                }

                if (string.IsNullOrWhiteSpace(txtNomeCompleto.Text))
                {
                    MessageBox.Show("Por favor, preencha o nome completo.");
                    return;
                }

                //validação de data de nascimento usando DataTIMEPicker
                dataNascimento = dateTimePicker1.Value.Date;

                //verifica se a data é posterior ou igual a data atual
                if (dataNascimento >= DateTime.Now.Date) //Compara com a data atual sem a hora
                {
                    MessageBox.Show("Verifique novamente a sua data de nascimento.");
                    return;
                }

                if (comboBoxCidade.SelectedItem == null)
                {
                    MessageBox.Show("Por favor, selecione a cidade.");
                    return;
                }

                if (!rbFeminino.Checked && !rbMasculino.Checked && !rbNaoBinario.Checked)
                {
                    MessageBox.Show("Por favor, selecione o gênero.");
                    return;
                }


                //Agora, caso todos os campos estejam preenchidos, a validação prossegue
                numeroCadastro = Convert.ToInt32(txtNumeroCadastro.Text);
                nomeUsuario = txtNomeCompleto.Text;
                cidade = comboBoxCidade.Text;
                generoF = rbFeminino.Checked;
                generoM = rbMasculino.Checked;
                generoNB = rbNaoBinario.Checked;

                //formatar a data para formato brasileiro
                string dataFormatada = dataNascimento.ToString("dd/MM/yyyy");

                //Determinar o gênero selecionado
                string generoSelecionado = "Não informado"; //Caso nenhum gênero seja seleciondado
                if (generoF)
                    generoSelecionado = "Feminino";
                else if (generoM)
                    generoSelecionado = "Masculino";
                else if (generoNB)
                    generoSelecionado = "Não Binário";

                //Exibir as informações em messageBox
                MessageBox.Show("Número cadastro: " + numeroCadastro);
                MessageBox.Show("Nome: " + nomeUsuario);
                MessageBox.Show("Data de Nascimento: " + dataNascimento);
                MessageBox.Show("Cidade: " + cidade);
                MessageBox.Show("Gênero: " + generoSelecionado);

                //Cria a conexão com o banco de dados
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                MessageBox.Show("Conexão feita com sucesso",
                               "Sucesso",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);

                //Comando SQL para inserir um novo cliente no banco de dados
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = Conexao
                };

                cmd.Prepare();


               if ( numero_usuario == null)
                {
                    //Insert CREAT
                    cmd.CommandText = "INSERT INTO cadastro(numero_cadastro, nome_completo, data_nascimento, cidade, genero) " +
                    "VALUES (@numero_cadastro, @nome_completo, @data_nascimento, @cidade, @genero)";

                    //Adiciona os parâmetros com os dados do formulário
                    cmd.Parameters.AddWithValue("@nome_completo", txtNomeCompleto.Text.Trim());
                    cmd.Parameters.AddWithValue("@numero_cadastro", txtNumeroCadastro.Text.Trim());
                    cmd.Parameters.AddWithValue("@data_nascimento", dataNascimento);
                    cmd.Parameters.AddWithValue("@cidade", cidade);
                    cmd.Parameters.AddWithValue("@genero", generoSelecionado);

                    //Executa o comando de inserção no banco
                    cmd.ExecuteNonQuery();

                    //Mensagem de sucesso
                    MessageBox.Show("Contato inserido com Sucesso: ",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    carregar_clientes();

                    

                    txtNumeroCadastro.Focus();
                }
                else
                {
                    //UPDATE
                    cmd.CommandText = $"UPDATE `cadastro` SET " +
                    $"nome_completo = @nome_completo, " +
                    $"numero_cadastro = @numero_cadastro, " +
                    $"cidade = @cidade, " +
                    $"genero = @genero " +
                    $"WHERE numero_cadastro = @numero_cadastro";

                    cmd.Parameters.AddWithValue("@nome_completo", txtNomeCompleto.Text.Trim());
                    cmd.Parameters.AddWithValue("@numero_cadastro", txtNumeroCadastro.Text.Trim());
                    cmd.Parameters.AddWithValue("@data_nascimento", dataNascimento);
                    cmd.Parameters.AddWithValue("@cidade", cidade);
                    cmd.Parameters.AddWithValue("@genero", generoSelecionado);
                    

                    //Executa o comando de alteração no banco
                    cmd.ExecuteNonQuery();

                    //Mensagem de sucesso para dados atualizados
                    MessageBox.Show($"Os dados com o código {numero_usuario} foram alterados com Sucesso!",
                                    "Sucesso",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                }

                numero_usuario = null;

                carregar_clientes();
            }
            catch (MySqlException ex)
            {
                //Trata erros relacionados ao MySQL
                MessageBox.Show("Erro " + ex.Number + " ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {

                //Trata outros tipos de erro
                MessageBox.Show("Ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                //Garante que a conexão com o banco será fechada, mesmo se ocorrer erro
                if (Conexao != null && Conexao.State == ConnectionState.Open)
                {
                    Conexao.Close();

                    //Teste de fechamento de banco
                    //MessageBox.Show("Conexão fechada com sucesso");
                }
            }

        }


        private void listView1_ItemSelectionChanged_1(object sender, ListViewItemSelectionChangedEventArgs e)
        {

            ListView.SelectedListViewItemCollection clientedaselecao = listView1.SelectedItems;

            foreach (ListViewItem item in clientedaselecao)
            {


                txtNumeroCadastro.Text = item.SubItems[0].Text;
                numero_usuario = Convert.ToInt32(item.SubItems[0].Text);

                //Exibe uma MessageBox com o código do cliente
                MessageBox.Show("Código do Cliente: " + numero_usuario.ToString(),
                                "Código Selecionado",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);


                txtNomeCompleto.Text = item.SubItems[1].Text;

                DateTime dataNascimento;

                if (DateTime.TryParse(item.SubItems[2].Text, out dataNascimento))
                {
                    dateTimePicker1.Value = dataNascimento;
                }


                // Preenche a cidade no ComboBox
                string cidade = item.SubItems[3].Text;
                if (comboBoxCidade.Items.Contains(cidade))
                {
                    comboBoxCidade.SelectedItem = cidade;
                }
                else
                {
                    // Caso a cidade não exista no ComboBox, podemos definir como nulo ou exibir uma mensagem
                    comboBoxCidade.SelectedItem = null;
                    MessageBox.Show("Cidade não encontrada no ComboBox.");
                }

                // Preenche o gênero no GroupBox com RadioButtons
                string genero = item.SubItems[4].Text;

                // Verifica o gênero e seleciona o RadioButton correspondente
                if (genero == "Masculino")
                {
                    rbMasculino.Checked = true;
                }
                else if (genero == "Feminino")
                {
                    rbFeminino.Checked = true;
                }
                else if (genero == "Não Binário")
                {
                    rbNaoBinario.Checked = true;
                }


            }

        }

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM cadastro WHERE nome_completo LIKE @q ORDER BY numero_cadastro DESC";
            carregar_clientes_com_query(query);
        }


        

        private void txtNumeroCadastro_Click(object sender, EventArgs e)
        {
            //Limpa o conteúdo do TextBox quando o usuário clicar nele
            if (txtNumeroCadastro.Text == "Número Cadastro")
            {
                txtNumeroCadastro.Text = "";
            }
            else
            {
                txtNumeroCadastro.Text = "Número Cadastro";
            }

        }

        private void txtNomeCompleto_Click(object sender, EventArgs e)
        {
            //Limpa o conteúdo do TextBox quando o usuário clicar nele
            if (txtNomeCompleto.Text == "Insira seu nome completo")
            {
                txtNomeCompleto.Text = "";
            }
            else
            {
                txtNomeCompleto.Text = "Nome Completo";
            }
     
        }

        private void btnApagar_Click(object sender, EventArgs e)
        {
            numero_usuario = null;
                // Limpar os campos de texto
                txtNumeroCadastro.Clear();
                txtNomeCompleto.Clear();

                // Limpar o ComboBox (deseleciona qualquer item selecionado)
                comboBoxCidade.SelectedItem = null;

            // Limpar o DateTimePicker (define a data para a data mínima possível)
            dateTimePicker1.Value = DateTime.Today;

                // Desmarcar os RadioButtons no GroupBox (gênero)
                rbMasculino.Checked = false;
                rbFeminino.Checked = false;
                rbNaoBinario.Checked = false;

                // Caso queira adicionar algum tipo de mensagem para informar que os campos foram limpos:
                MessageBox.Show("Campos limpos com sucesso!", "Limpeza", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
        }

        private void btnDeletar_Click(object sender, EventArgs e)
        {
            excluir_cliente();
        }
        private void excluir_cliente()
        {
            try
            {
                DialogResult opcaoDigitada = MessageBox.Show("Tem certeza que deseja excluir o registro de código: " + numero_usuario,
                    "Tem certeza?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (opcaoDigitada == DialogResult.Yes)
                {

                    Conexao = new MySqlConnection(data_source);

                    Conexao.Open();

                    MySqlCommand cmd = new MySqlCommand();

                    cmd.Connection = Conexao;

                    cmd.Prepare();

                    cmd.CommandText = "DELETE FROM cadastro WHERE numero_cadastro = @numero_cadastro";

                    cmd.Parameters.AddWithValue("@numero_cadastro", txtNumeroCadastro.Text.Trim());

                    cmd.ExecuteNonQuery();

                    //Excluir no Banco de Dados
                    MessageBox.Show("Os dados do cliente foram EXCLUÍDOS!",
                                     "Sucesso",
                                     MessageBoxButtons.OK,
                                     MessageBoxIcon.Information);

                    carregar_clientes();

                }

            }

            catch (MySqlException ex)
            {
                //Trata erros relacionados ao MySQL
                MessageBox.Show("Erro " + ex.Number + " ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {

                //Trata outros tipos de erro
                MessageBox.Show("Ocorreu: " + ex.Message,
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                //Garante que a conexão com o banco será fechada, mesmo se ocorrer erro
                if (Conexao != null && Conexao.State == ConnectionState.Open)
                {
                    Conexao.Close();

                }


            }
        }
    }
}



