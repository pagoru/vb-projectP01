Imports System.Data.SqlClient

Public Class Form1

    Private connection As SqlConnection

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        connection = New SqlConnection()
        connection.ConnectionString = "Data Source=.\SQLEXPRESS;Initial Catalog=MAGATZEM;Trusted_Connection=True;"

        Try
            connection.Open()
        Catch ex As Exception
            MsgBox("No es pot connectar al servidor sql")
        End Try

        If connection.State = ConnectionState.Closed Then
            Close()
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim list As List(Of String) = GetEmpleados("idEmpleado")
        ListBox1.Items.Clear()

        For Each item As String In list
            ListBox1.Items.Add(item)
        Next

        ListBox1.SelectedIndex = 0
        LoadEmpleado()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim list As List(Of String) = GetEmpleados("Apellidos")
        ListBox1.Items.Clear()

        For Each item As String In list
            ListBox1.Items.Add(item)
        Next

        ListBox1.SelectedIndex = 0
        LoadEmpleado()
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        LoadEmpleado()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If ListBox1.Items.Count - 1 = ListBox1.SelectedIndex Then
            ListBox1.SelectedIndex = 0
        Else
            ListBox1.SelectedIndex += 1
        End If
        LoadEmpleado()
    End Sub

    Private Function GetEmpleados(orderBy As String)

        Dim com As SqlCommand = New SqlCommand()
        Dim read As SqlDataReader
        Dim list As List(Of String) = New List(Of String)

        com.Connection = connection
        com.CommandText = "SELECT idEmpleado, Nombre, Apellidos FROM Empleados ORDER BY " + orderBy

        read = com.ExecuteReader

        While read.Read()
            list.Add(read(0).ToString() + "- " + read(1).ToString() + " " + read(2).ToString())
        End While

        read.Close()

        Return list

    End Function

    Private Function GetSingleEmpleado(id As String)

        Dim com As SqlCommand = New SqlCommand()
        Dim read As SqlDataReader
        Dim res As String

        com.Connection = connection
        com.CommandText = "SELECT idEmpleado, Nombre, Apellidos FROM Empleados WHERE idEmpleado=" + id

        read = com.ExecuteReader

        read.Read()
        res = read(0).ToString() + "- " + read(1).ToString() + " " + read(2).ToString()

        read.Close()
        Return res

    End Function

    Private Function LoadEmpleado()

        Dim id As String = ListBox1.Items.Item(ListBox1.SelectedIndex).ToString().Split("-").ToList().First()
        Console.WriteLine(id)

        Dim com As SqlCommand = New SqlCommand()
        Dim read As SqlDataReader

        Dim jefeId As String
        Dim list As List(Of String) = GetEmpleados("idEmpleado")

        com.Connection = connection
        com.CommandText = "SELECT idEmpleado, Apellidos, Nombre, Cargo, FechaNacimiento, Jefe FROM Empleados WHERE idEmpleado=" + id

        read = com.ExecuteReader

        ComboBox1.Items.Clear()

        For Each item As String In list
            ComboBox1.Items.Add(item)
        Next

        read.Read()

        TextBox1.Text = read(0).ToString
        TextBox2.Text = read(1).ToString
        TextBox3.Text = read(2).ToString
        TextBox4.Text = read(3).ToString
        TextBox5.Text = read(4).ToString
        jefeId = read(5).ToString

        read.Close()

        If jefeId IsNot "" Then
            ComboBox1.SelectedItem = GetSingleEmpleado(jefeId)
        End If
    End Function
End Class
