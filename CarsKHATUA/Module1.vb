' Mova os Imports para o topo, fora do Module
Imports System.Data

Module Module1
    ' Declaração de variáveis públicas
    Public diretorio As String
    Public db As New ADODB.Connection ' Variável do banco
    Public rs As New ADODB.Recordset    ' Variável das tabelas
    Public sql As String               ' Query de leitura e Escrita 
    Public cont As Integer

    Sub Connecta_banco()
        Try
            ' Verifica se a conexão já está aberta para não dar erro de redundância
            If db.State = 1 Then db.Close()

            db = CreateObject("ADODB.Connection")
            ' Database ajustado para "produtos_db" e Driver 9.6 conforme suas imagens
            db.Open("Provider=SQLOLEDB;Data Source=LAB5-09;Initial Catalog=produtos_db;Integrated Security=SSPI;")

            ' MsgBox("Conexão OK", MsgBoxStyle.Information + MsgBoxStyle.OkOnly, "AVISO")
        Catch ex As Exception
            MsgBox("ERRO AO CONECTAR: " & ex.Message, MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "AVISO")
        End Try
    End Sub

    Sub Carregar_dados()
        Try
            ' 1. Busca os dados no banco
            sql = "SELECT id_produto, desc_produto, categoria_prod, marca_produto, qnt_dis_prod, preco_venda, preco_custo FROM tb_cadastro_prod ORDER BY desc_produto ASC"
            rs = db.Execute(sql)

            With Consultar.dgv_dados
                .Rows.Clear()
                cont = 0

                Do While Not rs.EOF
                    cont += 1

                    ' 2. Cálculos de Preço e Margem (Protegidos contra Nulos)
                    Dim v_custo As Double = Val(rs.Fields("preco_custo").Value & "")
                    Dim v_venda As Double = Val(rs.Fields("preco_venda").Value & "")

                    Dim v_margem As String = "0,00%"
                    If v_venda > 0 Then
                        Dim calculo As Double = ((v_venda - v_custo) / v_venda) * 100
                        v_margem = FormatNumber(calculo, 2) & "%"
                    End If

                    ' 3. Adicionando na Grade
                    ' IMPORTANTE: Para as colunas de EDITAR e EXCLUIR (8 e 9), 
                    ' usamos 'Nothing'. Assim o DataGridView mostra o ícone que você configurou no Design.
                    .Rows.Add(
                    rs.Fields("id_produto").Value,           ' Coluna 1: ID real do banco (útil para excluir/editar)
                    rs.Fields("desc_produto").Value & "",    ' Coluna 2: Descrição
                    rs.Fields("categoria_prod").Value & "",  ' Coluna 3: Categoria
                    rs.Fields("marca_produto").Value & "",   ' Coluna 4: Marca
                    Val(rs.Fields("qnt_dis_prod").Value & ""), ' Coluna 5: Qtd
                    FormatCurrency(v_venda),                 ' Coluna 6: Preço Venda
                    v_margem,                                ' Coluna 7: Margem %
                    Nothing,                                 ' Coluna 8: Botão Editar (Ícone do Design)
                    Nothing                                  ' Coluna 9: Botão Excluir (Ícone do Design)
                )

                    rs.MoveNext()
                Loop
            End With
        Catch ex As Exception
            MsgBox("Erro ao Carregar os dados: " & ex.Message, MsgBoxStyle.Critical, "ATENÇÃO")
        End Try
    End Sub

    Sub Carregar_tipo()
        Try
            ' Limpa o ComboBox real da tela
            Consultar.cmd_campo.Items.Clear()
            With Consultar.cmd_campo.Items
                .Add("DESCRIÇÃO")
                .Add("CATEGORIA")
                .Add("MARCA")
            End With
            Consultar.cmd_campo.SelectedIndex = 0
        Catch ex As Exception
            ' Se o erro persistir, a mensagem abaixo vai nos dizer o porquê
            MsgBox("Erro ao Carregar tipos: " & ex.Message)
        End Try
    End Sub

End Module