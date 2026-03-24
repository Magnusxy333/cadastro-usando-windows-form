Public Class Form1
    ' Instanciamos o Label preventivamente para evitar a exceção NullReferenceException 
    ' Você precisa colocar a Label no Form1 Design e nomeá-la lbl_margem
    Private lbl_margem As New Label()

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Connecta_banco()
        ' Chamamos Carregar_tipo primeiro para preparar os combos
        Carregar_tipo()
        Carregar_dados()
    End Sub

    ' --- FUNÇÃO DE CÁLCULO DA MARGEM (VERSÃO SEGURA) ---
    Sub Calcular_Margem()
        Try
            Dim custo As Double = 0
            Dim venda As Double = 0

            ' Usamos TryParse para evitar erros se o campo estiver vazio ou com letras
            ' Substituímos o ponto por vírgula para o VB entender o cálculo decimal corretamente
            Dim textoCusto As String = preco_cust_text.Text.Replace(".", ",")
            Dim textoVenda As String = preco_venda_text.Text.Replace(".", ",")

            If Double.TryParse(textoCusto, custo) AndAlso Double.TryParse(textoVenda, venda) Then
                If venda > 0 Then
                    Dim margem As Double = ((venda - custo) / venda) * 100

                    ' lbl_margem deve ser o nome da sua Label no Design
                    lbl_margem.Text = FormatNumber(margem, 2) & "%"

                    If margem < 0 Then
                        lbl_margem.ForeColor = Color.Red
                    Else
                        lbl_margem.ForeColor = Color.Green
                    End If
                Else
                    lbl_margem.Text = "0,00%"
                    lbl_margem.ForeColor = Color.Black
                End If
            Else
                ' Se não conseguir converter (campo vazio), apenas reseta a label
                lbl_margem.Text = "0,00%"
            End If
        Catch ex As Exception
            ' Evita que o programa feche em caso de erro inesperado
            Debug.Print("Erro cálculo: " & ex.Message)
        End Try
    End Sub

    ' --- EVENTOS PARA CALCULAR ENQUANTO DIGITA ---
    Private Sub preco_cust_text_TextChanged(sender As Object, e As EventArgs) Handles preco_cust_text.TextChanged
        Calcular_Margem()
    End Sub

    ' --- BOTÃO GRAVAR ---
    Private Sub bnt_gravar_Click(sender As Object, e As EventArgs) Handles bnt_gravar.Click
        Try
            ' Validação básica: Descrição não pode ser vazia
            If String.IsNullOrWhiteSpace(desc_produto_text.Text) Then
                MsgBox("Por favor, digite a descrição do produto!", MsgBoxStyle.Exclamation)
                Exit Sub
            End If

            sql = $"select * from tb_cadastro_prod where desc_produto='{desc_produto_text.Text}'"
            rs = db.Execute(sql)

            If rs.EOF = True Then
                Dim data_mysql As String = data_lote_date.Value.ToString("yyyy-MM-dd")
                Dim caminho_foto As String = If(diretorio Is Nothing, "", diretorio.Replace("\", "\\"))

                ' SQL Corrigido com as 8 colunas e valores formatados para MySQL (Ponto em vez de vírgula)
                sql = "insert into tb_cadastro_prod (desc_produto, qnt_dis_prod, data_lote, preco_custo, preco_venda, marca_produto, categoria_prod, foto_prod) " &
                      "values (" &
                      "'" & desc_produto_text.Text & "', " &
                      "'" & qnt_dis_prod_text.Text & "', " &
                      "'" & data_mysql & "', " &
                      "'" & preco_cust_text.Text.Replace(",", ".") & "', " &
                      "'" & preco_venda_text.Text.Replace(",", ".") & "', " &
                      "'" & marca_produto_cmd.Text & "', " &
                      "'" & categoria_produto_cmd.Text & "', " &
                      "'" & caminho_foto & "')"

                db.Execute(sql)

                MsgBox("Dados gravados com sucesso!", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "AVISO")
                Carregar_dados()
            Else
                MsgBox("Produto já cadastrado!", MsgBoxStyle.Exclamation + MsgBoxStyle.OkOnly, "AVISO")
            End If
        Catch ex As Exception
            MsgBox("Erro ao Gravar: " & ex.Message, MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "AVISO")
        End Try
    End Sub

    ' --- SELECIONAR FOTO ---
    Private Sub foto_prod_pic_Click(sender As Object, e As EventArgs) Handles foto_prod_pic.Click
        Try
            With OpenFileDialog1
                .Title = "Selecione uma Foto"
                .Filter = "Imagens|*.jpg;*.png;*.jpeg"
                If .ShowDialog() = DialogResult.OK Then
                    diretorio = .FileName
                    foto_prod_pic.Load(diretorio)
                    foto_prod_pic.SizeMode = PictureBoxSizeMode.StretchImage
                End If
            End With
        Catch ex As Exception
            ' Apenas ignora se o usuário cancelar
        End Try
    End Sub

    ' --- EVENTOS ADICIONAIS ---
    Private Sub Label10_Click(sender As Object, e As EventArgs) Handles Label10.Click
        ' Ação da label se houver
    End Sub

End Class