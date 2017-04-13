Imports System.Data.SqlClient
Imports System.Globalization

Public Class Form1

    Private connection As SqlConnection

    'Aquesta funció s'encarrega de
    'carregar el recurs de sql al 
    'formulari actual
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

    'Aquesta funcio permet
    'carregar y ordenar per
    'identificador als
    'empleats
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim list As List(Of String) = GetEmpleados("idEmpleado")
        ListBox1.Items.Clear()

        For Each item As String In list
            ListBox1.Items.Add(item)
        Next

        SelectIndex(0)
    End Sub
    'Aquesta funcio permet
    'carregar y ordenar per
    'cognom als
    'empleats
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim list As List(Of String) = GetEmpleados("Apellidos")
        ListBox1.Items.Clear()

        For Each item As String In list
            ListBox1.Items.Add(item)
        Next

        SelectIndex(0)
    End Sub

    'Aquesta funcio detecta
    'si es fa algun
    'canvi en la llista
    'de seleccio i llavors
    'carrega el trebalaldor
    'actual
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        LoadEmpleado()
    End Sub

    'Aquesta funcio detecta
    'el botó seguent
    'i selecciona el seguent
    'element de la llista
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If ListBox1.Items.Count > 0 Then
            If ListBox1.Items.Count - 1 = ListBox1.SelectedIndex Then
                SelectIndex(0)
            Else
                SelectIndex(ListBox1.SelectedIndex + 1)
            End If
        End If
    End Sub

    'Aquesta funcio detecta
    'si s'elimina la seleccio
    'actual i la elimina mitjançant
    'sql
    Private Async Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

        Dim id As String = ListBox1.Items.Item(ListBox1.SelectedIndex).ToString().Split("-").ToList().First()
        Dim com As SqlCommand = New SqlCommand()
        Dim read As SqlDataReader

        com.Connection = connection
        com.CommandText = "DELETE FROM Empleados WHERE idEmpleado=" + id

        read = Await com.ExecuteReaderAsync()
        read.Close()

        Dim list As List(Of String) = GetEmpleados("idEmpleado")
        ListBox1.Items.Clear()

        For Each item As String In list
            ListBox1.Items.Add(item)
        Next

        SelectIndex(0)

    End Sub

    'Aquesta funcio
    'selecciona dins de 
    'la llista el item
    'desitjat en funcio
    'del numero introduit
    Private Sub SelectIndex(i As Integer)
        If ListBox1.Items.Count > 0 Then
            ListBox1.SelectedIndex = i
        End If
    End Sub

    'Aquesta funcio reotrna 
    'els treballadors
    'en funcio de un string
    'que consisteix en un ordre
    'determinat i en funcio
    'de les opcions en els 
    'checkbox per si te jefe
    'o no
    Private Function GetEmpleados(orderBy As String)

        Dim com As SqlCommand = New SqlCommand()
        Dim read As SqlDataReader
        Dim list As List(Of String) = New List(Of String)

        Dim jefew As String = ""

        If RadioButton2.Checked Then
            jefew = "WHERE Jefe IS NOT NULL"
        ElseIf RadioButton3.Checked Then
            jefew = "WHERE Jefe IS NULL"
        End If

        com.Connection = connection
        com.CommandText = "SELECT idEmpleado, Nombre, Apellidos FROM Empleados " + jefew + " ORDER BY " + orderBy

        read = com.ExecuteReader

        While read.Read()
            list.Add(read(0).ToString() + "- " + read(1).ToString() + " " + read(2).ToString())
        End While

        read.Close()

        Return list

    End Function

    'Aquest metode
    'carrega al treballador
    'actual en els caixes de edició
    Private Sub LoadEmpleado()

        Dim id As String = ListBox1.Items.Item(ListBox1.SelectedIndex).ToString().Split("-").ToList().First()

        Dim com As SqlCommand = New SqlCommand()
        Dim read As SqlDataReader

        Dim jefeId As String
        Dim list As List(Of String) = GetEmpleados("idEmpleado")

        com.Connection = connection
        com.CommandText = "SELECT idEmpleado, Apellidos, Nombre, Cargo, FechaNacimiento, Jefe FROM Empleados WHERE idEmpleado=" + id

        read = com.ExecuteReader

        ComboBox1.Items.Clear()

        ComboBox1.Items.Add("")
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

        ComboBox1.SelectedIndex = 0
        If jefeId.Length > 0 Then
            For number As Integer = 0 To ComboBox1.Items.Count - 1
                Dim currentId As String = ComboBox1.Items.Item(number).ToString().Split("-").ToList().First()
                If currentId = jefeId Then
                    ComboBox1.SelectedIndex = number
                End If
            Next
        End If

    End Sub

    'Aquest metode 
    'actualitza la informació
    'actual del item seleccioant
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim com As SqlCommand = New SqlCommand()
        Dim read As SqlDataReader

        Dim currentSelection As Integer = ListBox1.SelectedIndex

        Dim JefeId As String = ComboBox1.SelectedText

        If JefeId.Length > 0 Then
            JefeId = ComboBox1.Items.Item(ComboBox1.SelectedIndex).ToString().Split("-").ToList().First()
        End If

        Dim edate As Date = DateTime.ParseExact(TextBox5.Text.ToString, "dd/MM/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)

        com.Connection = connection
        com.CommandText = "UPDATE Empleados SET Apellidos='" + TextBox2.Text + "', Nombre='" + TextBox3.Text + "', Cargo='" + TextBox4.Text + "', FechaNacimiento='" + edate.ToString("dd/MM/yy") + "', Jefe='" + JefeId + "' WHERE idEmpleado=" + TextBox1.Text

        read = com.ExecuteReader

        read.Close()


        Dim list As List(Of String) = GetEmpleados("idEmpleado")
        ListBox1.Items.Clear()

        For Each item As String In list
            ListBox1.Items.Add(item)
        Next

        SelectIndex(currentSelection)
    End Sub

    'Aquest metode 
    'insereix un nou
    'element en la base
    'de dades
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Dim com As SqlCommand = New SqlCommand()
        Dim read As SqlDataReader

        Dim currentSelection As Integer = ListBox1.SelectedIndex

        Dim JefeId As String = ComboBox1.SelectedText

        If JefeId.Length > 0 Then
            JefeId = ComboBox1.Items.Item(ComboBox1.SelectedIndex).ToString().Split("-").ToList().First()
        End If

        Dim edate As Date = DateTime.ParseExact(TextBox5.Text.ToString, "dd/MM/yy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)

        com.Connection = connection
        com.CommandText = "INSERT INTO Empleados (Apellidos, Nombre, Cargo, FechaNacimiento, Jefe) VALUES ('" + TextBox2.Text + "', '" + TextBox3.Text + "', '" + TextBox4.Text + "', '" + edate.ToString("dd/MM/yy") + "', '" + JefeId + "') "

        read = com.ExecuteReader

        read.Close()


        Dim list As List(Of String) = GetEmpleados("idEmpleado")
        ListBox1.Items.Clear()

        For Each item As String In list
            ListBox1.Items.Add(item)
        Next
    End Sub
End Class
